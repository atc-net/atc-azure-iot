namespace Atc.Azure.IoTEdge.TestMocks;

/// <summary>
/// An <see cref="IModuleClientWrapper"/> that doesn't connect to anything, for running a module in the Standalone
/// environment and for tests.
/// </summary>
/// <remarks>
/// <para>
/// Each instance has its own twin, which starts with the properties in <see cref="MockTwinProperties"/>.
/// </para>
/// <para>
/// Tests can read what the module did (<see cref="GetSentMessages"/>, <see cref="RetryPolicy"/>, the twin through
/// <see cref="GetTwinAsync()"/>) and act as IoT Hub towards the module (<see cref="SimulateMethodCallAsync"/>,
/// <see cref="SimulateInputMessageAsync"/>, <see cref="SimulateDesiredPropertiesUpdateAsync"/>,
/// <see cref="SimulateConnectionStatusChange"/>). <see cref="SetMethodResponse"/> sets what
/// <c>InvokeMethodAsync</c> returns.
/// </para>
/// </remarks>
[SuppressMessage("Design", "CA1063:Implement IDisposable Correctly", Justification = "OK - By Design")]
[SuppressMessage("Usage", "CA1816:Dispose methods should call SuppressFinalize", Justification = "OK - By Design")]
[SuppressMessage("Performance", "CA1848:Use the LoggerMessage delegates", Justification = "OK - By Design")]
[SuppressMessage("Performance", "CA1873:Avoid potentially expensive logging", Justification = "OK - By Design")]
[SuppressMessage("Major Code Smell", "S3881:\"IDisposable\" should be implemented correctly", Justification = "OK - By Design")]
[SuppressMessage("Usage", "CA2254:Template should be a static expression", Justification = "OK - By Design")]
[SuppressMessage("Performance", "CA1851:Possible multiple enumeration", Justification = "OK - By Design")]
[SuppressMessage("Usage", "MA0040:Forward the CancellationToken parameter to methods that take one", Justification = "OK - By Design")]
public class MockModuleClientWrapper : IModuleClientWrapper
{
    /// <summary>
    /// The output that <c>SendEventAsync</c> and <c>SendEventBatchAsync</c> without an output name send to.
    /// </summary>
    public const string DefaultOutputName = "default";

    public int DiagnosticSamplingPercentage { get; set; }

    public uint OperationTimeoutInMilliseconds { get; set; }

    public string ProductInfo { get; set; } = string.Empty;

    /// <summary>
    /// Gets the retry policy the module set with <see cref="SetRetryPolicy"/>, if any.
    /// </summary>
    public IRetryPolicy? RetryPolicy { get; private set; }

    private readonly ILogger<MockModuleClientWrapper> logger;

    private readonly Dictionary<string, List<Message>> messageQueues = new(StringComparer.Ordinal);
    private readonly Dictionary<string, (MessageHandler Handler, object UserContext)> inputMessageHandlers = new(StringComparer.Ordinal);
    private readonly Dictionary<string, (MethodCallback Handler, object UserContext)> methodHandlers = new(StringComparer.Ordinal);
    private readonly Dictionary<string, MethodResponse> simulatedMethodResponses = new(StringComparer.Ordinal);

    private TwinCollection desiredProperties;
    private TwinCollection reportedProperties;

    private DesiredPropertyUpdateCallback? desiredPropertyUpdateCallback;
    private object? desiredPropertyUpdateUserContext;

    private ConnectionStatusChangesHandler? connectionStatusChangesHandler;

    private MessageHandler? defaultMessageHandler;
    private object? defaultMessageHandlerUserContext;

    private MethodCallback? defaultMethodHandler;
    private object? defaultMethodHandlerUserContext;

    public MockModuleClientWrapper(ILogger<MockModuleClientWrapper> logger)
    {
        this.logger = logger;
        desiredProperties = Copy(MockTwinProperties.Desired);
        reportedProperties = Copy(MockTwinProperties.Reported);
    }

    /// <summary>
    /// Gets the messages the module sent to an output, in the order it sent them.
    /// </summary>
    /// <param name="outputName">The output; <see cref="DefaultOutputName"/> for messages sent without an output name.</param>
    /// <returns>The messages; empty if none were sent to the output.</returns>
    public IReadOnlyList<Message> GetSentMessages(string outputName = DefaultOutputName)
    {
        lock (messageQueues)
        {
            return messageQueues.TryGetValue(outputName, out var messages)
                ? messages.ToList()
                : [];
        }
    }

    /// <summary>
    /// Sets the response <c>InvokeMethodAsync</c> returns for a method the module calls on another device or module.
    /// </summary>
    /// <param name="methodName">The method name.</param>
    /// <param name="response">The response.</param>
    public void SetMethodResponse(
        string methodName,
        MethodResponse response)
        => simulatedMethodResponses[methodName] = response;

    /// <summary>
    /// Calls a direct method on the module, as IoT Hub does: the handler set for the method, or else the default
    /// handler, gets the request.
    /// </summary>
    /// <param name="methodName">The method name.</param>
    /// <param name="jsonPayload">The method's JSON payload, if any.</param>
    /// <returns>The module's response, or status 501 when the module has no handler for the method, as from IoT Hub.</returns>
    public Task<MethodResponse> SimulateMethodCallAsync(
        string methodName,
        string? jsonPayload = null)
    {
        var request = jsonPayload is null
            ? new MethodRequest(methodName)
            : new MethodRequest(methodName, Encoding.UTF8.GetBytes(jsonPayload));

        if (methodHandlers.TryGetValue(methodName, out var methodHandler))
        {
            return methodHandler.Handler(request, methodHandler.UserContext);
        }

        if (defaultMethodHandler is not null)
        {
            return defaultMethodHandler(request, defaultMethodHandlerUserContext!);
        }

        logger.LogWarning($"No handler for method {methodName}");
        return Task.FromResult(new MethodResponse((int)HttpStatusCode.NotImplemented));
    }

    /// <summary>
    /// Sends a message to one of the module's inputs, as edgeHub does when a route delivers it: the handler set for the
    /// input, or else the default message handler, gets it.
    /// </summary>
    /// <param name="inputName">The input name.</param>
    /// <param name="message">The message.</param>
    /// <returns>The handler's response.</returns>
    /// <exception cref="InvalidOperationException">The module has no handler for the input and no default handler.</exception>
    public Task<MessageResponse> SimulateInputMessageAsync(
        string inputName,
        Message message)
    {
        if (inputMessageHandlers.TryGetValue(inputName, out var inputHandler))
        {
            return inputHandler.Handler(message, inputHandler.UserContext);
        }

        if (defaultMessageHandler is not null)
        {
            return defaultMessageHandler(message, defaultMessageHandlerUserContext!);
        }

        throw new InvalidOperationException($"The module has no message handler for input {inputName}");
    }

    /// <summary>
    /// Updates the twin's desired properties, as IoT Hub does: the patch is merged into them (a property set to
    /// <see langword="null"/> is removed), and the module's desired property callback, if it set one, gets the patch.
    /// </summary>
    /// <param name="desiredPropertiesPatch">The desired properties that changed.</param>
    /// <returns>A task that completes when the callback has handled them.</returns>
    public async Task SimulateDesiredPropertiesUpdateAsync(
        TwinCollection desiredPropertiesPatch)
    {
        ArgumentNullException.ThrowIfNull(desiredPropertiesPatch);

        desiredProperties = Merge(desiredProperties, desiredPropertiesPatch);

        if (desiredPropertyUpdateCallback is not null)
        {
            await desiredPropertyUpdateCallback(desiredPropertiesPatch, desiredPropertyUpdateUserContext!);
        }
    }

    /// <summary>
    /// Reports a change of the connection status to the module's handler, if it set one.
    /// </summary>
    /// <param name="status">The new connection status.</param>
    /// <param name="reason">The reason for the change.</param>
    public void SimulateConnectionStatusChange(
        ConnectionStatus status,
        ConnectionStatusChangeReason reason)
        => connectionStatusChangesHandler?.Invoke(status, reason);

    public Task AbandonAsync(
        string lockToken)
        => Task.CompletedTask;

    public Task AbandonAsync(
        string lockToken,
        CancellationToken cancellationToken)
        => Task.CompletedTask;

    public Task AbandonAsync(
        Message message)
        => Task.CompletedTask;

    public Task AbandonAsync(
        Message message,
        CancellationToken cancellationToken)
        => Task.CompletedTask;

    public Task CloseAsync()
    {
        logger.LogInformation("Closed ModuleClient");
        return Task.CompletedTask;
    }

    public Task CloseAsync(
        CancellationToken cancellationToken)
        => CloseAsync();

    public Task CompleteAsync(
        string lockToken)
        => Task.CompletedTask;

    public Task CompleteAsync(
        string lockToken,
        CancellationToken cancellationToken)
        => CompleteAsync(lockToken);

    public Task CompleteAsync(
        Message message)
        => Task.CompletedTask;

    public Task CompleteAsync(
        Message message,
        CancellationToken cancellationToken)
        => CompleteAsync(message);

    public Task<Twin> GetTwinAsync()
    {
        var twin = new Twin
        {
            DeviceId = "MyTestDevice",
            Properties =
            {
                Desired = desiredProperties,
                Reported = reportedProperties,
            },
        };

        return Task.FromResult(twin);
    }

    public Task<Twin> GetTwinAsync(
        CancellationToken cancellationToken)
        => GetTwinAsync();

    public Task<MethodResponse> InvokeMethodAsync(
        string deviceId,
        MethodRequest methodRequest)
        => InvokeMethodAsync(
            deviceId,
            moduleId: null,
            methodRequest,
            CancellationToken.None);

    public Task<MethodResponse> InvokeMethodAsync(
        string deviceId,
        MethodRequest methodRequest,
        CancellationToken cancellationToken)
        => InvokeMethodAsync(
            deviceId,
            moduleId: null,
            methodRequest,
            cancellationToken);

    public Task<MethodResponse> InvokeMethodAsync(
        string deviceId,
        string moduleId,
        MethodRequest methodRequest)
        => InvokeMethodAsync(
            deviceId,
            moduleId,
            methodRequest,
            CancellationToken.None);

    public Task<MethodResponse> InvokeMethodAsync(
        string deviceId,
        string? moduleId,
        MethodRequest methodRequest,
        CancellationToken cancellationToken)
    {
        if (simulatedMethodResponses.TryGetValue(methodRequest.Name, out var response))
        {
            logger.LogInformation($"Invoked method {methodRequest.Name} on device {deviceId} module {moduleId}");
            return Task.FromResult(response);
        }

        logger.LogError($"No simulated response found for method {methodRequest.Name} on device {deviceId} module {moduleId}");
        throw new InvalidOperationException($"No simulated response for method {methodRequest.Name}");
    }

    public Task OpenAsync()
    {
        logger.LogInformation("Opened ModuleClient");
        return Task.CompletedTask;
    }

    public Task OpenAsync(
        CancellationToken cancellationToken)
        => OpenAsync();

    public Task SendEventAsync(
        Message message)
        => SendEventAsync(DefaultOutputName, message);

    public Task SendEventAsync(
        Message message,
        CancellationToken cancellationToken)
        => SendEventAsync(message);

    public Task SendEventAsync(
        string outputName,
        Message message)
    {
        lock (messageQueues)
        {
            SendEventToQueue(outputName, message);
        }

        logger.LogInformation($"Message Sent to {outputName}");
        return Task.CompletedTask;
    }

    public Task SendEventAsync(
        string outputName,
        Message message,
        CancellationToken cancellationToken)
        => SendEventAsync(outputName, message);

    public Task SendEventBatchAsync(
        IEnumerable<Message> messages)
        => SendEventBatchAsync(DefaultOutputName, messages);

    public Task SendEventBatchAsync(
        IEnumerable<Message> messages,
        CancellationToken cancellationToken)
        => SendEventBatchAsync(messages);

    public Task SendEventBatchAsync(
        string outputName,
        IEnumerable<Message> messages)
    {
        lock (messageQueues)
        {
            foreach (var message in messages)
            {
                SendEventToQueue(outputName, message);
            }
        }

        logger.LogInformation($"Batch of {messages.Count()} messages sent to {outputName}.");
        return Task.CompletedTask;
    }

    public Task SendEventBatchAsync(
        string outputName,
        IEnumerable<Message> messages,
        CancellationToken cancellationToken)
        => SendEventBatchAsync(outputName, messages);

    public void SetConnectionStatusChangesHandler(ConnectionStatusChangesHandler statusChangesHandler)
    {
        connectionStatusChangesHandler = statusChangesHandler;
        logger.LogInformation("Connection status changes handler set.");
    }

    public Task SetDesiredPropertyUpdateCallbackAsync(
        DesiredPropertyUpdateCallback callback,
        object userContext)
    {
        desiredPropertyUpdateCallback = callback;
        desiredPropertyUpdateUserContext = userContext;
        return Task.CompletedTask;
    }

    public Task SetDesiredPropertyUpdateCallbackAsync(
        DesiredPropertyUpdateCallback callback,
        object userContext,
        CancellationToken cancellationToken)
        => SetDesiredPropertyUpdateCallbackAsync(callback, userContext);

    public Task SetInputMessageHandlerAsync(
        string inputName,
        MessageHandler messageHandler,
        object userContext)
    {
        inputMessageHandlers[inputName] = (messageHandler, userContext);
        logger.LogInformation($"Message Handler Set for {inputName}");
        return Task.CompletedTask;
    }

    public Task SetInputMessageHandlerAsync(
        string inputName,
        MessageHandler messageHandler,
        object userContext,
        CancellationToken cancellationToken)
        => SetInputMessageHandlerAsync(inputName, messageHandler, userContext);

    public Task SetMessageHandlerAsync(
        MessageHandler messageHandler,
        object userContext)
    {
        defaultMessageHandler = messageHandler;
        defaultMessageHandlerUserContext = userContext;
        logger.LogInformation("Default message handler set.");
        return Task.CompletedTask;
    }

    public Task SetMessageHandlerAsync(
        MessageHandler messageHandler,
        object userContext,
        CancellationToken cancellationToken)
        => SetMessageHandlerAsync(messageHandler, userContext);

    public Task SetMethodDefaultHandlerAsync(
        MethodCallback methodHandler,
        object userContext)
    {
        defaultMethodHandler = methodHandler;
        defaultMethodHandlerUserContext = userContext;
        logger.LogInformation("Default method handler set.");
        return Task.CompletedTask;
    }

    public Task SetMethodDefaultHandlerAsync(
        MethodCallback methodHandler,
        object userContext,
        CancellationToken cancellationToken)
        => SetMethodDefaultHandlerAsync(methodHandler, userContext);

    public Task SetMethodHandlerAsync(
        string methodName,
        MethodCallback methodHandler,
        object userContext)
    {
        methodHandlers[methodName] = (methodHandler, userContext);
        logger.LogInformation($"Method Handler Set for {methodName}");
        return Task.CompletedTask;
    }

    public Task SetMethodHandlerAsync(
        string methodName,
        MethodCallback methodHandler,
        object userContext,
        CancellationToken cancellationToken)
        => SetMethodHandlerAsync(methodName, methodHandler, userContext);

    public void SetRetryPolicy(IRetryPolicy retryPolicy)
    {
        RetryPolicy = retryPolicy;
        logger.LogInformation($"Retry policy set: {retryPolicy}");
    }

    /// <summary>
    /// Updates the twin's reported properties, as IoT Hub does: the patch is merged into them, and a property set to
    /// <see langword="null"/> is removed.
    /// </summary>
    /// <param name="reportedProperties">The reported properties that changed.</param>
    /// <returns>A completed task.</returns>
    public Task UpdateReportedPropertiesAsync(
        TwinCollection reportedProperties)
    {
        ArgumentNullException.ThrowIfNull(reportedProperties);

        this.reportedProperties = Merge(this.reportedProperties, reportedProperties);
        return Task.CompletedTask;
    }

    public Task UpdateReportedPropertiesAsync(
        TwinCollection reportedProperties,
        CancellationToken cancellationToken)
        => UpdateReportedPropertiesAsync(reportedProperties);

    public void Dispose()
    {
    }

    private static TwinCollection Copy(TwinCollection twinCollection)
        => new(twinCollection.ToJson());

    private static TwinCollection Merge(
        TwinCollection current,
        TwinCollection patch)
    {
        var merged = Newtonsoft.Json.Linq.JObject.Parse(current.ToJson());
        ApplyPatch(merged, Newtonsoft.Json.Linq.JObject.Parse(patch.ToJson()));

        return new TwinCollection(merged.ToString(Newtonsoft.Json.Formatting.None));
    }

    private static void ApplyPatch(
        Newtonsoft.Json.Linq.JObject target,
        Newtonsoft.Json.Linq.JObject patch)
    {
        foreach (var property in patch.Properties())
        {
            if (property.Value.Type == Newtonsoft.Json.Linq.JTokenType.Null)
            {
                target.Remove(property.Name);
            }
            else if (property.Value is Newtonsoft.Json.Linq.JObject patchObject &&
                     target[property.Name] is Newtonsoft.Json.Linq.JObject targetObject)
            {
                ApplyPatch(targetObject, patchObject);
            }
            else
            {
                target[property.Name] = property.Value.DeepClone();
            }
        }
    }

    private void SendEventToQueue(
        string outputName,
        Message message)
    {
        if (!messageQueues.TryGetValue(outputName, out var value))
        {
            value = [];
            messageQueues[outputName] = value;
        }

        value.Add(message);
    }
}