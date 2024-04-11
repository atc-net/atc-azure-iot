namespace Atc.Azure.IoTEdge.TestMocks;

[SuppressMessage("Design", "CA1063:Implement IDisposable Correctly", Justification = "OK - By Design")]
[SuppressMessage("Usage", "CA1816:Dispose methods should call SuppressFinalize", Justification = "OK - By Design")]
[SuppressMessage("Performance", "CA1848:Use the LoggerMessage delegates", Justification = "OK - By Design")]
[SuppressMessage("Major Code Smell", "S3881:\"IDisposable\" should be implemented correctly", Justification = "OK - By Design")]
[SuppressMessage("Usage", "CA2254:Template should be a static expression", Justification = "OK - By Design")]
[SuppressMessage("Performance", "CA1851:Possible multiple enumeration", Justification = "OK - By Design")]
[SuppressMessage("Usage", "MA0040:Forward the CancellationToken parameter to methods that take one", Justification = "OK - By Design")]
[SuppressMessage("CodeQuality", "IDE0052:Remove unread private member", Justification = "OK - By Design")]
[SuppressMessage("Critical Code Smell", "S4487:Unread \"private\" fields should be removed", Justification = "OK - By Design")]
public class MockModuleClientWrapper : IModuleClientWrapper
{
    public int DiagnosticSamplingPercentage { get; set; }

    public uint OperationTimeoutInMilliseconds { get; set; }

    public string ProductInfo { get; set; } = string.Empty;

    private readonly ILogger<MockModuleClientWrapper> logger;

    private readonly Dictionary<string, List<Message>> messageQueues = new(StringComparer.Ordinal);
    private readonly Dictionary<string, (MessageHandler, object)> inputMessageHandlers = new(StringComparer.Ordinal);
    private readonly Dictionary<string, MethodCallback> methodMessageHandlers = new(StringComparer.Ordinal);
    private readonly Dictionary<string, MethodResponse> simulatedMethodResponses = new(StringComparer.Ordinal);

    private DesiredPropertyUpdateCallback? desiredPropertyUpdateCallback;
    private object? desiredPropertyUpdateUserContext;

    private ConnectionStatusChangesHandler? connectionStatusChangesHandler;

    private MessageHandler? defaultMessageHandler;
    private object? defaultMessageHandlerUserContext;

    private MethodCallback? defaultMethodHandler;
    private object? defaultMethodHandlerUserContext;

    private IRetryPolicy? policy;

    public MockModuleClientWrapper(
        ILogger<MockModuleClientWrapper> logger)
    {
        this.logger = logger;
    }

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
                Desired = MockTwinProperties.Desired,
                Reported = MockTwinProperties.Reported,
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
        => SendEventAsync("default", message);

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
    {
        lock (messageQueues)
        {
            foreach (var message in messages)
            {
                SendEventToQueue("default", message);
            }
        }

        logger.LogInformation($"Batch of {messages.Count()} messages sent to default output.");
        return Task.CompletedTask;
    }

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

    public void SetConnectionStatusChangesHandler(
        ConnectionStatusChangesHandler statusChangesHandler)
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
        methodMessageHandlers[methodName] = methodHandler;
        logger.LogInformation($"Method Handler Set for {methodName}");
        return Task.CompletedTask;
    }

    public Task SetMethodHandlerAsync(
        string methodName,
        MethodCallback methodHandler,
        object userContext,
        CancellationToken cancellationToken)
        => SetMethodHandlerAsync(methodName, methodHandler, userContext);

    public void SetRetryPolicy(
        IRetryPolicy retryPolicy)
    {
        policy = retryPolicy;
        logger.LogInformation($"Retry policy set: {retryPolicy}");
    }

    public async Task UpdateReportedPropertiesAsync(
        TwinCollection reportedProperties)
    {
        MockTwinProperties.Reported = reportedProperties;

        if (desiredPropertyUpdateCallback is not null)
        {
            await desiredPropertyUpdateCallback.Invoke(MockTwinProperties.Desired, desiredPropertyUpdateUserContext);
        }
    }

    public Task UpdateReportedPropertiesAsync(
        TwinCollection reportedProperties,
        CancellationToken cancellationToken)
        => UpdateReportedPropertiesAsync(reportedProperties);

    public void Dispose()
    {
    }

    private void SendEventToQueue(
        string outputName,
        Message message)
    {
        if (!messageQueues.TryGetValue(outputName, out var value))
        {
            value = new List<Message>();
            messageQueues[outputName] = value;
        }

        value.Add(message);
    }
}