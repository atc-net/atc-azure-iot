namespace Atc.Azure.IoTEdge.Wrappers;

public interface IModuleClientWrapper : IDisposable
{
    /// <summary>
    /// The diagnostic sampling percentage.
    /// </summary>
    int DiagnosticSamplingPercentage { get; set; }

    /// <summary>
    /// Stores the timeout used in the operation retries. Note that this value is ignored for operations
    /// where a cancellation token is provided. For example, SendEventAsync(Message) will use this timeout, but
    /// SendEventAsync(Message, CancellationToken) will not. The latter operation will only be canceled by the
    /// provided cancellation token.
    /// </summary>
    uint OperationTimeoutInMilliseconds { get; set; }

    /// <summary>
    /// Stores custom product information that will be appended to the user agent string that is sent to IoT Hub.
    /// </summary>
    string ProductInfo { get; set; }

    /// <summary>Puts a received message back onto the module queue</summary>
    /// <param name="lockToken">The message lockToken.</param>
    /// <returns>The previously received message</returns>
    Task AbandonAsync(
        string lockToken);

    /// <summary>Puts a received message back onto the module queue</summary>
    /// <param name="lockToken">The message lockToken.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <exception cref="OperationCanceledException">Thrown when the operation has been canceled.</exception>
    /// <returns>The previously received message</returns>
    Task AbandonAsync(
        string lockToken,
        CancellationToken cancellationToken);

    /// <summary>Puts a received message back onto the module queue</summary>
    /// <param name="message">The message.</param>
    /// <returns>The lock identifier for the previously received message</returns>
    Task AbandonAsync(
        Message message);

    /// <summary>Puts a received message back onto the module queue</summary>
    /// <param name="message">The message.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <exception cref="OperationCanceledException">Thrown when the operation has been canceled.</exception>
    /// <returns>The lock identifier for the previously received message</returns>
    Task AbandonAsync(
        Message message,
        CancellationToken cancellationToken);

    /// <summary>
    /// Close the ModuleClient instance
    /// </summary>
    /// <returns>A Task representing the asynchronous operation of closing the ModuleClient.</returns>
    Task CloseAsync();

    /// <summary>Close the ModuleClient instance</summary>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <exception cref="OperationCanceledException">Thrown when the operation has been canceled.</exception>
    /// <returns>A Task representing the asynchronous operation of closing the ModuleClient.</returns>
    Task CloseAsync(
        CancellationToken cancellationToken);

    /// <summary>Deletes a received message from the module queue</summary>
    /// <param name="lockToken">The message lockToken.</param>
    /// <returns>The lock identifier for the previously received message</returns>
    Task CompleteAsync(
        string lockToken);

    /// <summary>Deletes a received message from the module queue</summary>
    /// <param name="lockToken">The message lockToken.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <exception cref="OperationCanceledException">Thrown when the operation has been canceled.</exception>
    /// <returns>The lock identifier for the previously received message</returns>
    Task CompleteAsync(
        string lockToken,
        CancellationToken cancellationToken);

    /// <summary>Deletes a received message from the module queue</summary>
    /// <param name="message">The message.</param>
    /// <returns>The previously received message</returns>
    Task CompleteAsync(
        Message message);

    /// <summary>Deletes a received message from the module queue</summary>
    /// <param name="message">The message.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <exception cref="OperationCanceledException">Thrown when the operation has been canceled.</exception>
    /// <returns>The previously received message</returns>
    Task CompleteAsync(
        Message message,
        CancellationToken cancellationToken);

    /// <summary>Retrieve a module twin object for the current module.</summary>
    /// <returns>The module twin object for the current module</returns>
    Task<Twin> GetTwinAsync();

    /// <summary>Retrieve a module twin object for the current module.</summary>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <exception cref="OperationCanceledException">Thrown when the operation has been canceled.</exception>
    /// <returns>The module twin object for the current module</returns>
    Task<Twin> GetTwinAsync(
        CancellationToken cancellationToken);

    /// <summary>
    /// Interactively invokes a method from an edge module to an edge device.
    /// Both the edge module and the edge device need to be connected to the same edge hub.
    /// </summary>
    /// <param name="deviceId">The unique identifier of the edge device to invoke the method on.</param>
    /// <param name="methodRequest">The details of the method to invoke.</param>
    /// <returns>The result of the method invocation.</returns>
    Task<MethodResponse> InvokeMethodAsync(
        string deviceId,
        MethodRequest methodRequest);

    /// <summary>
    /// Interactively invokes a method from an edge module to an edge device.
    /// Both the edge module and the edge device need to be connected to the same edge hub.
    /// </summary>
    /// <param name="deviceId">The unique identifier of the edge device to invoke the method on.</param>
    /// <param name="methodRequest">The details of the method to invoke.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <exception cref="OperationCanceledException">Thrown when the operation has been canceled.</exception>
    /// <returns>The result of the method invocation.</returns>
    Task<MethodResponse> InvokeMethodAsync(
        string deviceId,
        MethodRequest methodRequest,
        CancellationToken cancellationToken);

    /// <summary>
    /// Interactively invokes a method from an edge module to a different edge module.
    /// Both of the edge modules need to be connected to the same edge hub.
    /// </summary>
    /// <param name="deviceId">The unique identifier of the device.</param>
    /// <param name="moduleId">The unique identifier of the edge module to invoke the method on.</param>
    /// <param name="methodRequest">The details of the method to invoke.</param>
    /// <exception cref="OperationCanceledException">Thrown when the operation has been canceled.</exception>
    /// <returns>The result of the method invocation.</returns>
    Task<MethodResponse> InvokeMethodAsync(
        string deviceId,
        string moduleId,
        MethodRequest methodRequest);

    /// <summary>
    /// Interactively invokes a method from an edge module to a different edge module.
    /// Both of the edge modules need to be connected to the same edge hub.
    /// </summary>
    /// <param name="deviceId">The unique identifier of the device.</param>
    /// <param name="moduleId">The unique identifier of the edge module to invoke the method on.</param>
    /// <param name="methodRequest">The details of the method to invoke.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <exception cref="OperationCanceledException">Thrown when the operation has been canceled.</exception>
    /// <returns>The result of the method invocation.</returns>
    Task<MethodResponse> InvokeMethodAsync(
        string deviceId,
        string moduleId,
        MethodRequest methodRequest,
        CancellationToken cancellationToken);

    /// <summary>
    /// Explicitly open the ModuleClient instance.
    /// </summary>
    /// <returns>A Task representing the asynchronous operation of opening the ModuleClient.</returns>
    Task OpenAsync();

    /// <summary>Explicitly open the ModuleClient instance.</summary>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <exception cref="OperationCanceledException">Thrown when the operation has been canceled.</exception>
    /// <returns>A Task representing the asynchronous operation of opening the ModuleClient.</returns>
    Task OpenAsync(
        CancellationToken cancellationToken);

    /// <summary>Sends an event to IoT hub</summary>
    /// <param name="message">The message.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required parameter is null.</exception>
    /// <exception cref="TimeoutException">Thrown if the service does not respond to the request within the timeout specified for the operation.
    /// The timeout values are largely transport protocol specific. Check the corresponding transport settings to see if they can be configured.
    /// The operation timeout for the client can be set using <see cref="ModuleClient.OperationTimeoutInMilliseconds" />.</exception>
    /// <exception cref="Microsoft.Azure.Devices.Client.Exceptions.IotHubCommunicationException">Thrown if the client encounters a transient retryable exception. </exception>
    /// <exception cref="System.Net.Sockets.SocketException">Thrown if a socket error occurs.</exception>
    /// <exception cref="System.Net.WebSockets.WebSocketException">Thrown if an error occurs when performing an operation on a WebSocket connection.</exception>
    /// <exception cref="IOException">Thrown if an I/O error occurs.</exception>
    /// <exception cref="DotNetty.Transport.Channels.ClosedChannelException">Thrown if the MQTT transport layer closes unexpectedly.</exception>
    /// <exception cref="Microsoft.Azure.Devices.Client.Exceptions.IotHubException">Thrown if an error occurs when communicating with IoT Hub service.
    /// If <see cref="Microsoft.Azure.Devices.Client.Exceptions.IotHubException.IsTransient" /> is set to <c>true</c> then it is a transient exception.
    /// If <see cref="Microsoft.Azure.Devices.Client.Exceptions.IotHubException.IsTransient" /> is set to <c>false</c> then it is a non-transient exception.</exception>
    /// <remarks>
    /// In case of a transient issue, retrying the operation should work. In case of a non-transient issue, inspect the error details and take steps accordingly.
    /// Please note that the list of exceptions is not exhaustive.
    /// </remarks>
    /// <returns>The message containing the event</returns>
    Task SendEventAsync(
        Message message);

    /// <summary>Sends an event to IoT hub</summary>
    /// <param name="message">The message.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required parameter is null.</exception>
    /// <exception cref="OperationCanceledException">Thrown if the service does not respond to the request before the expiration of the passed <see cref="CancellationToken" />.
    /// If a cancellation token is not supplied to the operation call, a cancellation token with an expiration time of 4 minutes is used.
    /// </exception>
    /// <exception cref="Microsoft.Azure.Devices.Client.Exceptions.IotHubCommunicationException">Thrown if the client encounters a transient retryable exception. </exception>
    /// <exception cref="System.Net.Sockets.SocketException">Thrown if a socket error occurs.</exception>
    /// <exception cref="System.Net.WebSockets.WebSocketException">Thrown if an error occurs when performing an operation on a WebSocket connection.</exception>
    /// <exception cref="IOException">Thrown if an I/O error occurs.</exception>
    /// <exception cref="DotNetty.Transport.Channels.ClosedChannelException">Thrown if the MQTT transport layer closes unexpectedly.</exception>
    /// <exception cref="Microsoft.Azure.Devices.Client.Exceptions.IotHubException">Thrown if an error occurs when communicating with IoT Hub service.
    /// If <see cref="Microsoft.Azure.Devices.Client.Exceptions.IotHubException.IsTransient" /> is set to <c>true</c> then it is a transient exception.
    /// If <see cref="Microsoft.Azure.Devices.Client.Exceptions.IotHubException.IsTransient" /> is set to <c>false</c> then it is a non-transient exception.</exception>
    /// <remarks>
    /// In case of a transient issue, retrying the operation should work. In case of a non-transient issue, inspect the error details and take steps accordingly.
    /// Please note that the list of exceptions is not exhaustive.
    /// </remarks>
    /// <returns>The message containing the event</returns>
    Task SendEventAsync(
        Message message,
        CancellationToken cancellationToken);

    /// <summary>Sends an event to IoT hub.</summary>
    /// <param name="outputName">The output target for sending the given message</param>
    /// <param name="message">The message to send</param>
    /// <exception cref="ArgumentNullException">Thrown when a required parameter is null.</exception>
    /// <exception cref="TimeoutException">Thrown if the service does not respond to the request within the timeout specified for the operation.
    /// The timeout values are largely transport protocol specific. Check the corresponding transport settings to see if they can be configured.
    /// The operation timeout for the client can be set using <see cref="ModuleClient.OperationTimeoutInMilliseconds" />.</exception>
    /// <exception cref="Microsoft.Azure.Devices.Client.Exceptions.IotHubCommunicationException">Thrown if the client encounters a transient retryable exception. </exception>
    /// <exception cref="System.Net.Sockets.SocketException">Thrown if a socket error occurs.</exception>
    /// <exception cref="System.Net.WebSockets.WebSocketException">Thrown if an error occurs when performing an operation on a WebSocket connection.</exception>
    /// <exception cref="IOException">Thrown if an I/O error occurs.</exception>
    /// <exception cref="DotNetty.Transport.Channels.ClosedChannelException">Thrown if the MQTT transport layer closes unexpectedly.</exception>
    /// <exception cref="Microsoft.Azure.Devices.Client.Exceptions.IotHubException">Thrown if an error occurs when communicating with IoT Hub service.
    /// If <see cref="Microsoft.Azure.Devices.Client.Exceptions.IotHubException.IsTransient" /> is set to <c>true</c> then it is a transient exception.
    /// If <see cref="Microsoft.Azure.Devices.Client.Exceptions.IotHubException.IsTransient" /> is set to <c>false</c> then it is a non-transient exception.</exception>
    /// <remarks>
    /// In case of a transient issue, retrying the operation should work. In case of a non-transient issue, inspect the error details and take steps accordingly.
    /// Please note that the above list is not exhaustive.
    /// </remarks>
    /// <returns>The message containing the event</returns>
    Task SendEventAsync(
        string outputName,
        Message message);

    /// <summary>Sends an event to IoT hub.</summary>
    /// <param name="outputName">The output target for sending the given message</param>
    /// <param name="message">The message to send</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required parameter is null.</exception>
    /// <exception cref="OperationCanceledException">Thrown if the service does not respond to the request before the expiration of the passed <see cref="CancellationToken" />.
    /// If a cancellation token is not supplied to the operation call, a cancellation token with an expiration time of 4 minutes is used.
    /// </exception>
    /// <exception cref="Microsoft.Azure.Devices.Client.Exceptions.IotHubCommunicationException">Thrown if the client encounters a transient retryable exception. </exception>
    /// <exception cref="System.Net.Sockets.SocketException">Thrown if a socket error occurs.</exception>
    /// <exception cref="System.Net.WebSockets.WebSocketException">Thrown if an error occurs when performing an operation on a WebSocket connection.</exception>
    /// <exception cref="IOException">Thrown if an I/O error occurs.</exception>
    /// <exception cref="DotNetty.Transport.Channels.ClosedChannelException">Thrown if the MQTT transport layer closes unexpectedly.</exception>
    /// <exception cref="Microsoft.Azure.Devices.Client.Exceptions.IotHubException">Thrown if an error occurs when communicating with IoT Hub service.
    /// If <see cref="Microsoft.Azure.Devices.Client.Exceptions.IotHubException.IsTransient" /> is set to <c>true</c> then it is a transient exception.
    /// If <see cref="Microsoft.Azure.Devices.Client.Exceptions.IotHubException.IsTransient" /> is set to <c>false</c> then it is a non-transient exception.</exception>
    /// <remarks>
    /// In case of a transient issue, retrying the operation should work. In case of a non-transient issue, inspect the error details and take steps accordingly.
    /// Please note that the above list is not exhaustive.
    /// </remarks>
    /// <returns>The message containing the event</returns>
    Task SendEventAsync(
        string outputName,
        Message message,
        CancellationToken cancellationToken);

    /// <summary>
    /// Sends a batch of events to IoT hub. Use AMQP or HTTPs for a true batch operation. MQTT will just send the messages one after the other.
    /// For more information on IoT Edge module routing <see href="https://docs.microsoft.com/en-us/azure/iot-edge/module-composition?view=iotedge-2018-06#declare-routes" />
    /// </summary>
    /// <param name="messages">The messages.</param>
    /// <returns>The task containing the event</returns>
    Task SendEventBatchAsync(
        IEnumerable<Message> messages);

    /// <summary>
    /// Sends a batch of events to IoT hub. Use AMQP or HTTPs for a true batch operation. MQTT will just send the messages one after the other.
    /// For more information on IoT Edge module routing <see href="https://docs.microsoft.com/en-us/azure/iot-edge/module-composition?view=iotedge-2018-06#declare-routes" />
    /// Sends a batch of events to IoT hub. Requires AMQP or AMQP over WebSockets.
    /// </summary>
    /// <param name="messages">An IEnumerable set of Message objects.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <exception cref="OperationCanceledException">Thrown when the operation has been canceled.</exception>
    /// <returns>The task containing the event</returns>
    Task SendEventBatchAsync(
        IEnumerable<Message> messages,
        CancellationToken cancellationToken);

    /// <summary>
    /// Sends a batch of events to IoT hub. Use AMQP or HTTPs for a true batch operation. MQTT will just send the messages one after the other.
    /// For more information on IoT Edge module routing <see href="https://docs.microsoft.com/en-us/azure/iot-edge/module-composition?view=iotedge-2018-06#declare-routes" />
    /// </summary>
    /// <param name="outputName">The output target for sending the given message</param>
    /// <param name="messages">A list of one or more messages to send</param>
    /// <exception cref="OperationCanceledException">Thrown when the operation has been canceled.</exception>
    /// <returns>The task containing the event</returns>
    Task SendEventBatchAsync(
        string outputName,
        IEnumerable<Message> messages);

    /// <summary>
    /// Sends a batch of events to IoT hub. Use AMQP or HTTPs for a true batch operation. MQTT will just send the messages one after the other.
    /// For more information on IoT Edge module routing <see href="https://docs.microsoft.com/en-us/azure/iot-edge/module-composition?view=iotedge-2018-06#declare-routes" />
    /// </summary>
    /// <param name="outputName">The output target for sending the given message</param>
    /// <param name="messages">A list of one or more messages to send</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <exception cref="OperationCanceledException">Thrown when the operation has been canceled.</exception>
    /// <returns>The task containing the event</returns>
    Task SendEventBatchAsync(
        string outputName,
        IEnumerable<Message> messages,
        CancellationToken cancellationToken);

    /// <summary>
    /// Sets a new delegate for the connection status changed callback. If a delegate is already associated,
    /// it will be replaced with the new delegate. Note that this callback will never be called if the client is configured to use HTTP as that protocol is stateless
    /// </summary>
    /// <param name="statusChangesHandler">The name of the method to associate with the delegate.</param>
    void SetConnectionStatusChangesHandler(
        ConnectionStatusChangesHandler statusChangesHandler);

    /// <summary>
    /// Set a callback that will be called whenever the client receives a state update (desired or reported) from the service.
    /// Set callback value to null to clear.
    /// </summary>
    /// <remarks>
    /// This has the side-effect of subscribing to the PATCH topic on the service.
    /// </remarks>
    /// <param name="callback">Callback to call after the state update has been received and applied</param>
    /// <param name="userContext">Context object that will be passed into callback</param>
    /// <returns>A Task representing the asynchronous operation of setting the desired property update.</returns>
    Task SetDesiredPropertyUpdateCallbackAsync(
        DesiredPropertyUpdateCallback callback,
        object userContext);

    /// <summary>
    /// Set a callback that will be called whenever the client receives a state update (desired or reported) from the service.
    /// Set callback value to null to clear.
    /// </summary>
    /// <remarks>
    /// This has the side-effect of subscribing to the PATCH topic on the service.
    /// </remarks>
    /// <param name="callback">Callback to call after the state update has been received and applied</param>
    /// <param name="userContext">Context object that will be passed into callback</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <exception cref="OperationCanceledException">Thrown when the operation has been canceled.</exception>
    /// <returns>A Task representing the asynchronous operation of setting the desired property update.</returns>
    Task SetDesiredPropertyUpdateCallbackAsync(
        DesiredPropertyUpdateCallback callback,
        object userContext,
        CancellationToken cancellationToken);

    /// <summary>
    /// Sets a new delegate for the particular input. If a delegate is already associated with
    /// the input, it will be replaced with the new delegate.
    /// </summary>
    /// <param name="inputName">The name of the input to associate with the delegate.</param>
    /// <param name="messageHandler">The delegate to be used when a message is sent to the particular inputName.</param>
    /// <param name="userContext">generic parameter to be interpreted by the client code.</param>
    /// <exception cref="OperationCanceledException">Thrown when the operation has been canceled.</exception>
    /// <returns>The task containing the event</returns>
    Task SetInputMessageHandlerAsync(
        string inputName,
        MessageHandler messageHandler,
        object userContext);

    /// <summary>
    /// Sets a new delegate for the particular input. If a delegate is already associated with
    /// the input, it will be replaced with the new delegate.
    /// </summary>
    /// <param name="inputName">The name of the input to associate with the delegate.</param>
    /// <param name="messageHandler">The delegate to be used when a message is sent to the particular inputName.</param>
    /// <param name="userContext">generic parameter to be interpreted by the client code.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <exception cref="OperationCanceledException">Thrown when the operation has been canceled.</exception>
    /// <returns>The task containing the event</returns>
    Task SetInputMessageHandlerAsync(
        string inputName,
        MessageHandler messageHandler,
        object userContext,
        CancellationToken cancellationToken);

    /// <summary>
    /// Sets a new default delegate which applies to all endpoints. If a delegate is already associated with
    /// the input, it will be called, else the default delegate will be called. If a default delegate was set previously,
    /// it will be overwritten.
    /// </summary>
    /// <param name="messageHandler">The delegate to be called when a message is sent to any input.</param>
    /// <param name="userContext">generic parameter to be interpreted by the client code.</param>
    /// <exception cref="OperationCanceledException">Thrown when the operation has been canceled.</exception>
    /// <returns>The task containing the event</returns>
    Task SetMessageHandlerAsync(
        MessageHandler messageHandler,
        object userContext);

    /// <summary>
    /// Sets a new default delegate which applies to all endpoints. If a delegate is already associated with
    /// the input, it will be called, else the default delegate will be called. If a default delegate was set previously,
    /// it will be overwritten.
    /// </summary>
    /// <param name="messageHandler">The delegate to be called when a message is sent to any input.</param>
    /// <param name="userContext">generic parameter to be interpreted by the client code.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <exception cref="OperationCanceledException">Thrown when the operation has been canceled.</exception>
    /// <returns>The task containing the event</returns>
    Task SetMessageHandlerAsync(
        MessageHandler messageHandler,
        object userContext,
        CancellationToken cancellationToken);

    /// <summary>
    /// Sets a new delegate that is called for a method that doesn't have a delegate registered for its name.
    /// If a default delegate is already registered it will replace with the new delegate.
    /// A method handler can be unset by passing a null MethodCallback.
    /// </summary>
    /// <param name="methodHandler">The delegate to be used when a method is called by the cloud service and there is no delegate registered for that method name.</param>
    /// <param name="userContext">Generic parameter to be interpreted by the client code.</param>
    /// <returns>A Task representing the asynchronous operation of setting the method default handler.</returns>
    Task SetMethodDefaultHandlerAsync(
        MethodCallback methodHandler,
        object userContext);

    /// <summary>
    /// Sets a new delegate that is called for a method that doesn't have a delegate registered for its name.
    /// If a default delegate is already registered it will replace with the new delegate.
    /// A method handler can be unset by passing a null MethodCallback.
    /// </summary>
    /// <param name="methodHandler">The delegate to be used when a method is called by the cloud service and there is no delegate registered for that method name.</param>
    /// <param name="userContext">Generic parameter to be interpreted by the client code.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <exception cref="OperationCanceledException">Thrown when the operation has been canceled.</exception>
    /// <returns>A Task representing the asynchronous operation of setting the method default handler.</returns>
    Task SetMethodDefaultHandlerAsync(
        MethodCallback methodHandler,
        object userContext,
        CancellationToken cancellationToken);

    /// <summary>
    /// Sets a new delegate for the named method. If a delegate is already associated with
    /// the named method, it will be replaced with the new delegate.
    /// A method handler can be unset by passing a null MethodCallback.
    /// </summary>
    /// <param name="methodName">The name of the method to associate with the delegate.</param>
    /// <param name="methodHandler">The delegate to be used when a method with the given name is called by the cloud service.</param>
    /// <param name="userContext">generic parameter to be interpreted by the client code.</param>
    /// <returns>A Task representing the asynchronous operation of setting the method default handler.</returns>
    Task SetMethodHandlerAsync(
        string methodName,
        MethodCallback methodHandler,
        object userContext);

    /// <summary>
    /// Sets a new delegate for the named method. If a delegate is already associated with
    /// the named method, it will be replaced with the new delegate.
    /// A method handler can be unset by passing a null MethodCallback.
    /// </summary>
    /// <param name="methodName">The name of the method to associate with the delegate.</param>
    /// <param name="methodHandler">The delegate to be used when a method with the given name is called by the cloud service.</param>
    /// <param name="userContext">generic parameter to be interpreted by the client code.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <exception cref="OperationCanceledException">Thrown when the operation has been canceled.</exception>
    /// <returns>A Task representing the asynchronous operation of setting the method default handler.</returns>
    Task SetMethodHandlerAsync(
        string methodName,
        MethodCallback methodHandler,
        object userContext,
        CancellationToken cancellationToken);

    /// <summary>
    /// Sets the retry policy used in the operation retries.
    /// The change will take effect after any in-progress operations.
    /// </summary>
    /// <param name="retryPolicy">The retry policy.
    /// The default is new ExponentialBackOff(int.MaxValue, TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(10), TimeSpan.FromMilliseconds(100));
    /// </param>
    void SetRetryPolicy(
        IRetryPolicy retryPolicy);

    /// <summary>Push reported property changes up to the service.</summary>
    /// <param name="reportedProperties">Reported properties to push</param>
    /// <returns>A Task representing the asynchronous operation of updating the reported properties.</returns>
    Task UpdateReportedPropertiesAsync(
        TwinCollection reportedProperties);

    /// <summary>Push reported property changes up to the service.</summary>
    /// <param name="reportedProperties">Reported properties to push</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <exception cref="OperationCanceledException">Thrown when the operation has been canceled.</exception>
    /// <returns>A Task representing the asynchronous operation of updating the reported properties.</returns>
    Task UpdateReportedPropertiesAsync(
        TwinCollection reportedProperties,
        CancellationToken cancellationToken);
}