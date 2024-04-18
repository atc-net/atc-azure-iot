// ReSharper disable InvertIf
namespace OpcPublisherNodeManagerModule;

/// <summary>
/// OpcPublisherNodeManagerModuleService method handlers.
/// </summary>
public sealed partial class OpcPublisherNodeManagerModuleService
{
    private const int MaxNodesForPageSize = 250;

    private const int SyncLockTimeoutInMs = 2000;

    /// <summary>
    /// Semaphore to protect the node configuration file.
    /// </summary>
    private static readonly SemaphoreSlim OpcPublisherNodeConfigurationFileSemaphore = new(1, 1);

    private static string OpcPublisherNodeConfigurationFilename { get; } = $"{Directory.GetCurrentDirectory()}/opc/opcpublisher/pn.json";

    private static string OpcPublisherNodeConfigurationFilenameTemp { get; } = $"{Directory.GetCurrentDirectory()}/opc/opcpublisher/pn.tmp";

    /// <summary>
    /// Max allowed payload of an IoTHub direct method call response.
    /// </summary>
    private static int MaxResponsePayloadLength => (128 * 1024) - 256;

    private static int MaxUriLength => 256;

    private static readonly int PageSizeForEndpointList = MaxResponsePayloadLength / MaxUriLength;

    private async Task<MethodResponse> GetEndpoints(
        MethodRequest methodRequest,
        object userContext)
    {
        LogMethodCalled(OpcPublisherNodeManagerModuleConstants.DirectMethodGetEndpoints, OpcPublisherNodeManagerModuleConstants.ModuleId);

        if (string.IsNullOrEmpty(methodRequest.DataAsJson))
        {
            LogMethodRequestEmpty(OpcPublisherNodeManagerModuleConstants.DirectMethodGetEndpoints, OpcPublisherNodeManagerModuleConstants.ModuleId);
            return methodResponseFactory.Create(HttpStatusCode.BadRequest, "No data in request.");
        }

        await OpcPublisherNodeConfigurationFileSemaphore.WaitAsync(SyncLockTimeoutInMs, CancellationToken.None);

        try
        {
            var request = JsonSerializer.Deserialize<GetEndpointsRequest>(methodRequest.Data, jsonSerializerOptions)!;
            var (succeeded, configurationFileEntries, errorMessage) = await ReadConfig(CancellationToken.None);

            if (!succeeded)
            {
                return methodResponseFactory.Create(HttpStatusCode.InternalServerError, errorMessage!);
            }

            var response = CreateGetEndpointsResponse(configurationFileEntries!, request);
            return methodResponseFactory.Create(HttpStatusCode.OK, response);
        }
        catch (SerializationException ex)
        {
            LogMethodRequestDeserializationError(OpcPublisherNodeManagerModuleConstants.DirectMethodGetEndpoints, OpcPublisherNodeManagerModuleConstants.ModuleId, ex.Message);
            return methodResponseFactory.Create(HttpStatusCode.InternalServerError, $"Exception ({ex.Message}) while deserializing message payload");
        }
        catch (Exception ex)
        {
            LogMethodRequestError(OpcPublisherNodeManagerModuleConstants.DirectMethodGetEndpoints, OpcPublisherNodeManagerModuleConstants.ModuleId, ex.Message);
            return methodResponseFactory.Create(HttpStatusCode.InternalServerError, $"Could not process request - Exception ({ex.Message}).");
        }
        finally
        {
            OpcPublisherNodeConfigurationFileSemaphore.Release();
        }
    }

    private async Task<MethodResponse> GetEndpointWithNodes(
        MethodRequest methodRequest,
        object userContext)
    {
        LogMethodCalled(OpcPublisherNodeManagerModuleConstants.DirectMethodGetEndpointWithNodes, OpcPublisherNodeManagerModuleConstants.ModuleId);

        if (string.IsNullOrEmpty(methodRequest.DataAsJson))
        {
            LogMethodRequestEmpty(OpcPublisherNodeManagerModuleConstants.DirectMethodGetEndpointWithNodes, OpcPublisherNodeManagerModuleConstants.ModuleId);
            return methodResponseFactory.Create(HttpStatusCode.BadRequest, "No data in request.");
        }

        GetEndpointWithNodesRequest request;

        try
        {
            request = JsonSerializer.Deserialize<GetEndpointWithNodesRequest>(methodRequest.Data, jsonSerializerOptions)!;

            if (!IsEndpointUrlValid(request.EndpointUrl))
            {
                var statusMessage = $"Error while parsing EndpointUrl '{request.EndpointUrl}'";
                LogMethodRequestUriParsingError(request.EndpointUrl, OpcPublisherNodeManagerModuleConstants.DirectMethodGetEndpointWithNodes, OpcPublisherNodeManagerModuleConstants.ModuleId, statusMessage);
                return methodResponseFactory.Create(HttpStatusCode.BadRequest, statusMessage);
            }
        }
        catch (JsonException e)
        {
            LogMethodRequestDeserializationError(OpcPublisherNodeManagerModuleConstants.DirectMethodGetEndpointWithNodes, OpcPublisherNodeManagerModuleConstants.ModuleId, e.Message);
            return methodResponseFactory.Create(HttpStatusCode.InternalServerError, "Request deserialization error");
        }

        await OpcPublisherNodeConfigurationFileSemaphore.WaitAsync(SyncLockTimeoutInMs, CancellationToken.None);

        try
        {
            var (succeeded, configurationFileEntries, errorMessage) = await ReadConfig(CancellationToken.None);

            if (!succeeded)
            {
                return methodResponseFactory.Create(HttpStatusCode.InternalServerError, errorMessage!);
            }

            var response = CreateGetEndpointWithNodesResponse(configurationFileEntries!, request);
            LogMethodCallCompleted(OpcPublisherNodeManagerModuleConstants.DirectMethodGetEndpointWithNodes, OpcPublisherNodeManagerModuleConstants.ModuleId);
            return methodResponseFactory.Create(HttpStatusCode.OK, response);
        }
        finally
        {
            OpcPublisherNodeConfigurationFileSemaphore.Release();
        }
    }

    private async Task<MethodResponse> GetEndpointsWithEmptyOpcNodesList(
        MethodRequest methodRequest,
        object userContext)
    {
        LogMethodCalled(OpcPublisherNodeManagerModuleConstants.DirectMethodGetEndpointsWithEmptyOpcNodesList, OpcPublisherNodeManagerModuleConstants.ModuleId);

        await OpcPublisherNodeConfigurationFileSemaphore.WaitAsync(SyncLockTimeoutInMs, CancellationToken.None);

        try
        {
            var (succeeded, configurationFileEntries, errorMessage) = await ReadConfig(CancellationToken.None);
            if (!succeeded)
            {
                return methodResponseFactory.Create(HttpStatusCode.InternalServerError, errorMessage!);
            }

            var response = CreateGetEndpointsWithEmptyOpcNodesListResponse(configurationFileEntries!);
            LogMethodCallCompleted(OpcPublisherNodeManagerModuleConstants.DirectMethodGetEndpointsWithEmptyOpcNodesList, OpcPublisherNodeManagerModuleConstants.ModuleId);
            return methodResponseFactory.Create(HttpStatusCode.OK, response);
        }
        finally
        {
            OpcPublisherNodeConfigurationFileSemaphore.Release();
        }
    }

    private async Task<MethodResponse> AddEndpoint(
        MethodRequest methodRequest,
        object userContext)
    {
        LogMethodCalled(OpcPublisherNodeManagerModuleConstants.DirectMethodAddEndpoint, OpcPublisherNodeManagerModuleConstants.ModuleId);

        if (string.IsNullOrEmpty(methodRequest.DataAsJson))
        {
            LogMethodRequestEmpty(OpcPublisherNodeManagerModuleConstants.DirectMethodAddEndpoint, OpcPublisherNodeManagerModuleConstants.ModuleId);
            return methodResponseFactory.Create(HttpStatusCode.BadRequest, "No data in request.");
        }

        AddEndpointRequest? request = null;
        Uri? endpointUri;

        try
        {
            request = JsonSerializer.Deserialize<AddEndpointRequest>(methodRequest.Data, jsonSerializerOptions)!;
            endpointUri = new Uri(request.EndpointUrl);

            if (!IsEndpointUrlValid(request.EndpointUrl))
            {
                var statusMessage = $"Error while parsing EndpointUrl '{request.EndpointUrl}'";
                LogMethodRequestUriParsingError(request.EndpointUrl, OpcPublisherNodeManagerModuleConstants.DirectMethodGetEndpointWithNodes, OpcPublisherNodeManagerModuleConstants.ModuleId, statusMessage);
                return methodResponseFactory.Create(HttpStatusCode.BadRequest, statusMessage);
            }
        }
        catch (UriFormatException ex)
        {
            LogMethodRequestUriFormatExceptionError(request!.EndpointUrl, ex.Message);
            return methodResponseFactory.Create(HttpStatusCode.InternalServerError, $"Exception ({ex.Message}) while parsing EndpointUrl '{request.EndpointUrl}'");
        }
        catch (JsonException e)
        {
            LogMethodRequestDeserializationError(OpcPublisherNodeManagerModuleConstants.DirectMethodAddEndpoint, OpcPublisherNodeManagerModuleConstants.ModuleId, e.Message);
            return methodResponseFactory.Create(HttpStatusCode.InternalServerError, "Request deserialization error");
        }

        await OpcPublisherNodeConfigurationFileSemaphore.WaitAsync(SyncLockTimeoutInMs, CancellationToken.None);

        try
        {
            var (succeeded, configurationFileEntries, errorMessage) = await ReadConfig(CancellationToken.None);
            if (!succeeded)
            {
                return methodResponseFactory.Create(HttpStatusCode.InternalServerError, errorMessage!);
            }

            var configurationFileEntry = configurationFileEntries!.Find(x => x.EndpointUrl!.OriginalString.Equals(endpointUri.OriginalString, StringComparison.OrdinalIgnoreCase));
            if (configurationFileEntry != null)
            {
                return methodResponseFactory.Create(HttpStatusCode.Conflict, "Endpoint is already added.");
            }

            configurationFileEntry = new ConfigurationFileEntry(endpointUri)
            {
                UseSecurity = false,
                OpcNodes = [],
            };

            if (!string.IsNullOrEmpty(request.UserName) && !string.IsNullOrEmpty(request.Password))
            {
                configurationFileEntry.UseSecurity = true;
                configurationFileEntry.OpcAuthenticationMode = nameof(OpcAuthenticationMode.UsernamePassword);
                configurationFileEntry.OpcAuthenticationUsername = request.UserName;
                configurationFileEntry.OpcAuthenticationPassword = request.Password;
            }

            configurationFileEntries.Add(configurationFileEntry);

            var (writeSucceeded, writeErrorMessage) = await WriteConfig(configurationFileEntries, CancellationToken.None);
            return writeSucceeded
                ? methodResponseFactory.Create(HttpStatusCode.OK, "Endpoint added.")
                : methodResponseFactory.Create(HttpStatusCode.InternalServerError, writeErrorMessage!);
        }
        finally
        {
            OpcPublisherNodeConfigurationFileSemaphore.Release();
        }
    }

    private async Task<MethodResponse> AddNodeToEndpoint(
        MethodRequest methodRequest,
        object userContext)
    {
        LogMethodCalled(OpcPublisherNodeManagerModuleConstants.DirectMethodAddNodeToEndpoint, OpcPublisherNodeManagerModuleConstants.ModuleId);

        if (string.IsNullOrEmpty(methodRequest.DataAsJson))
        {
            LogMethodRequestEmpty(OpcPublisherNodeManagerModuleConstants.DirectMethodAddNodeToEndpoint, OpcPublisherNodeManagerModuleConstants.ModuleId);
            return methodResponseFactory.Create(HttpStatusCode.BadRequest, "No data in request.");
        }

        AddNodeToEndpointRequest? request = null;
        Uri? endpointUri;

        try
        {
            request = JsonSerializer.Deserialize<AddNodeToEndpointRequest>(methodRequest.Data, jsonSerializerOptions)!;
            endpointUri = new Uri(request.EndpointUrl);
        }
        catch (UriFormatException ex)
        {
            LogMethodRequestUriFormatExceptionError(request!.EndpointUrl, ex.Message);
            return methodResponseFactory.Create(HttpStatusCode.InternalServerError, $"Exception ({ex.Message}) while parsing EndpointUrl '{request.EndpointUrl}'");
        }
        catch (SerializationException ex)
        {
            LogMethodRequestDeserializationError(OpcPublisherNodeManagerModuleConstants.DirectMethodAddNodeToEndpoint, OpcPublisherNodeManagerModuleConstants.ModuleId, ex.Message);
            return methodResponseFactory.Create(HttpStatusCode.InternalServerError, $"Exception ({ex.Message}) while deserializing message payload");
        }

        await OpcPublisherNodeConfigurationFileSemaphore.WaitAsync(SyncLockTimeoutInMs, CancellationToken.None);

        try
        {
            var (succeeded, configurationFileEntries, errorMessage) = await ReadConfig(CancellationToken.None);
            if (!succeeded)
            {
                return methodResponseFactory.Create(HttpStatusCode.InternalServerError, errorMessage!);
            }

            var configurationFileEntry = configurationFileEntries!.Find(x => x.EndpointUrl!.OriginalString.Equals(endpointUri.OriginalString, StringComparison.OrdinalIgnoreCase));
            if (configurationFileEntry is null)
            {
                return methodResponseFactory.Create(HttpStatusCode.NotFound, "Endpoint was not found.");
            }

            configurationFileEntry.OpcNodes ??= [];

            var opcNodeOnEndpoint = configurationFileEntry.OpcNodes.Find(x => x.Id.Equals(request.OpcNode.Id, StringComparison.OrdinalIgnoreCase));
            if (opcNodeOnEndpoint != null)
            {
                return methodResponseFactory.Create(HttpStatusCode.Conflict, "Node already present on endpoint - please use update instead.");
            }

            configurationFileEntry.OpcNodes.Add(request.OpcNode);

            var (writeSucceeded, writeErrorMessage) = await WriteConfig(configurationFileEntries, CancellationToken.None);
            return writeSucceeded
                ? methodResponseFactory.Create(HttpStatusCode.OK, "Node added to endpoint.")
                : methodResponseFactory.Create(HttpStatusCode.InternalServerError, writeErrorMessage!);
        }
        finally
        {
            OpcPublisherNodeConfigurationFileSemaphore.Release();
        }
    }

    private async Task<MethodResponse> AddNodesToEndpoint(
        MethodRequest methodRequest,
        object userContext)
    {
        LogMethodCalled(OpcPublisherNodeManagerModuleConstants.DirectMethodAddNodesToEndpoint, OpcPublisherNodeManagerModuleConstants.ModuleId);

        if (string.IsNullOrEmpty(methodRequest.DataAsJson))
        {
            LogMethodRequestEmpty(OpcPublisherNodeManagerModuleConstants.DirectMethodAddNodesToEndpoint, OpcPublisherNodeManagerModuleConstants.ModuleId);
            return methodResponseFactory.Create(HttpStatusCode.BadRequest, "No data in request.");
        }

        AddNodesToEndpointRequest? request = null;
        Uri? endpointUri;

        try
        {
            request = JsonSerializer.Deserialize<AddNodesToEndpointRequest>(methodRequest.Data, jsonSerializerOptions)!;
            endpointUri = new Uri(request.EndpointUrl);
        }
        catch (UriFormatException ex)
        {
            LogMethodRequestUriFormatExceptionError(request!.EndpointUrl, ex.Message);
            return methodResponseFactory.Create(HttpStatusCode.InternalServerError, $"Exception ({ex.Message}) while parsing EndpointUrl '{request.EndpointUrl}'");
        }
        catch (SerializationException ex)
        {
            LogMethodRequestDeserializationError(OpcPublisherNodeManagerModuleConstants.DirectMethodAddNodesToEndpoint, OpcPublisherNodeManagerModuleConstants.ModuleId, ex.Message);
            return methodResponseFactory.Create(HttpStatusCode.InternalServerError, $"Exception ({ex.Message}) while deserializing message payload");
        }

        await OpcPublisherNodeConfigurationFileSemaphore.WaitAsync(SyncLockTimeoutInMs, CancellationToken.None);

        try
        {
            var (succeeded, configurationFileEntries, errorMessage) = await ReadConfig(CancellationToken.None);
            if (!succeeded)
            {
                return methodResponseFactory.Create(HttpStatusCode.InternalServerError, errorMessage!);
            }

            var configurationFileEntry = configurationFileEntries!.Find(x => x.EndpointUrl!.OriginalString.Equals(endpointUri.OriginalString, StringComparison.OrdinalIgnoreCase));
            if (configurationFileEntry is null)
            {
                return methodResponseFactory.Create(HttpStatusCode.NotFound, "Endpoint was not found.");
            }

            configurationFileEntry.OpcNodes ??= [];

            foreach (var opcNode in request.Nodes)
            {
                if (!configurationFileEntry.OpcNodes.Exists(x => x.Id.Equals(opcNode.Id, StringComparison.Ordinal)))
                {
                    configurationFileEntry.OpcNodes.Add(opcNode);
                }
            }

            var (writeSucceeded, writeErrorMessage) = await WriteConfig(configurationFileEntries, CancellationToken.None);
            return writeSucceeded
                ? methodResponseFactory.Create(HttpStatusCode.OK, "Nodes added to endpoint.")
                : methodResponseFactory.Create(HttpStatusCode.InternalServerError, writeErrorMessage!);
        }
        finally
        {
            OpcPublisherNodeConfigurationFileSemaphore.Release();
        }
    }

    private async Task<MethodResponse> UpdateNodeOnEndpoint(
        MethodRequest methodRequest,
        object userContext)
    {
        LogMethodCalled(OpcPublisherNodeManagerModuleConstants.DirectMethodUpdateNodeOnEndpoint, OpcPublisherNodeManagerModuleConstants.ModuleId);

        if (string.IsNullOrEmpty(methodRequest.DataAsJson))
        {
            LogMethodRequestEmpty(OpcPublisherNodeManagerModuleConstants.DirectMethodUpdateNodeOnEndpoint, OpcPublisherNodeManagerModuleConstants.ModuleId);
            return methodResponseFactory.Create(HttpStatusCode.BadRequest, "No data in request.");
        }

        UpdateNodeOnEndpointRequest? request = null;
        Uri? endpointUri;

        try
        {
            request = JsonSerializer.Deserialize<UpdateNodeOnEndpointRequest>(methodRequest.Data, jsonSerializerOptions)!;
            endpointUri = new Uri(request.EndpointUrl);
        }
        catch (UriFormatException ex)
        {
            LogMethodRequestUriFormatExceptionError(request!.EndpointUrl, ex.Message);
            return methodResponseFactory.Create(HttpStatusCode.InternalServerError, $"Exception ({ex.Message}) while parsing EndpointUrl '{request.EndpointUrl}'");
        }
        catch (SerializationException ex)
        {
            LogMethodRequestDeserializationError(OpcPublisherNodeManagerModuleConstants.DirectMethodUpdateNodeOnEndpoint, OpcPublisherNodeManagerModuleConstants.ModuleId, ex.Message);
            return methodResponseFactory.Create(HttpStatusCode.InternalServerError, $"Exception ({ex.Message}) while deserializing message payload");
        }

        await OpcPublisherNodeConfigurationFileSemaphore.WaitAsync(SyncLockTimeoutInMs, CancellationToken.None);

        try
        {
            var (succeeded, configurationFileEntries, errorMessage) = await ReadConfig(CancellationToken.None);
            if (!succeeded)
            {
                return methodResponseFactory.Create(HttpStatusCode.InternalServerError, errorMessage!);
            }

            var configurationFileEntry = configurationFileEntries!.Find(x => x.EndpointUrl!.OriginalString.Equals(endpointUri.OriginalString, StringComparison.OrdinalIgnoreCase));
            if (configurationFileEntry is null)
            {
                return methodResponseFactory.Create(HttpStatusCode.NotFound, "Endpoint was not found.");
            }

            configurationFileEntry.OpcNodes ??= [];

            var opcNodeOnEndpoint = configurationFileEntry.OpcNodes.Find(x => x.Id.Equals(request.Node.NodeId, StringComparison.OrdinalIgnoreCase));
            if (opcNodeOnEndpoint is null)
            {
                return methodResponseFactory.Create(HttpStatusCode.NotFound, "Node was not found.");
            }

            var updated = false;
            if (opcNodeOnEndpoint.OpcPublishingInterval != request.Node.OpcPublishingInterval)
            {
                opcNodeOnEndpoint.OpcPublishingInterval = request.Node.OpcPublishingInterval;
                updated = true;
            }

            if (opcNodeOnEndpoint.OpcSamplingInterval != request.Node.OpcSamplingInterval)
            {
                opcNodeOnEndpoint.OpcSamplingInterval = request.Node.OpcSamplingInterval;
                updated = true;
            }

            if (!updated)
            {
                return methodResponseFactory.Create(HttpStatusCode.BadRequest, "Node was not updated.");
            }

            var (writeSucceeded, writeErrorMessage) = await WriteConfig(configurationFileEntries, CancellationToken.None);
            return writeSucceeded
                ? methodResponseFactory.Create(HttpStatusCode.OK, "Node updated.")
                : methodResponseFactory.Create(HttpStatusCode.InternalServerError, writeErrorMessage!);
        }
        finally
        {
            OpcPublisherNodeConfigurationFileSemaphore.Release();
        }
    }

    private async Task<MethodResponse> UpdateNodesOnEndpoint(
        MethodRequest methodRequest,
        object userContext)
    {
        LogMethodCalled(OpcPublisherNodeManagerModuleConstants.DirectMethodUpdateNodesOnEndpoint, OpcPublisherNodeManagerModuleConstants.ModuleId);

        if (string.IsNullOrEmpty(methodRequest.DataAsJson))
        {
            LogMethodRequestEmpty(OpcPublisherNodeManagerModuleConstants.DirectMethodUpdateNodesOnEndpoint, OpcPublisherNodeManagerModuleConstants.ModuleId);
            return methodResponseFactory.Create(HttpStatusCode.BadRequest, "No data in request.");
        }

        UpdateNodesOnEndpointRequest? request = null;
        Uri? endpointUri;

        try
        {
            request = JsonSerializer.Deserialize<UpdateNodesOnEndpointRequest>(methodRequest.Data, jsonSerializerOptions)!;
            endpointUri = new Uri(request.EndpointUrl);
        }
        catch (UriFormatException ex)
        {
            LogMethodRequestUriFormatExceptionError(request!.EndpointUrl, ex.Message);
            return methodResponseFactory.Create(HttpStatusCode.InternalServerError, $"Exception ({ex.Message}) while parsing EndpointUrl '{request.EndpointUrl}'");
        }
        catch (SerializationException ex)
        {
            LogMethodRequestDeserializationError(OpcPublisherNodeManagerModuleConstants.DirectMethodUpdateNodesOnEndpoint, OpcPublisherNodeManagerModuleConstants.ModuleId, ex.Message);
            return methodResponseFactory.Create(HttpStatusCode.InternalServerError, $"Exception ({ex.Message}) while deserializing message payload");
        }

        await OpcPublisherNodeConfigurationFileSemaphore.WaitAsync(SyncLockTimeoutInMs, CancellationToken.None);

        try
        {
            var (succeeded, configurationFileEntries, errorMessage) = await ReadConfig(CancellationToken.None);
            if (!succeeded)
            {
                return methodResponseFactory.Create(HttpStatusCode.InternalServerError, errorMessage!);
            }

            var configurationFileEntry = configurationFileEntries!.Find(x => x.EndpointUrl!.OriginalString.Equals(endpointUri.OriginalString, StringComparison.OrdinalIgnoreCase));
            if (configurationFileEntry is null)
            {
                return methodResponseFactory.Create(HttpStatusCode.NotFound, "Endpoint was not found.");
            }

            configurationFileEntry.OpcNodes ??= [];
            var updated = false;

            foreach (var endpointToUpdate in request.Nodes)
            {
                var opcNodeOnEndpoint = configurationFileEntry.OpcNodes.Find(x => x.Id.Equals(endpointToUpdate.NodeId, StringComparison.OrdinalIgnoreCase));
                if (opcNodeOnEndpoint is null)
                {
                    continue;
                }

                if (opcNodeOnEndpoint.OpcPublishingInterval.HasValue && opcNodeOnEndpoint.OpcPublishingInterval.Value != endpointToUpdate.OpcPublishingInterval)
                {
                    opcNodeOnEndpoint.OpcPublishingInterval = opcNodeOnEndpoint.OpcPublishingInterval.Value;
                    updated = true;
                }

                if (opcNodeOnEndpoint.OpcSamplingInterval.HasValue && opcNodeOnEndpoint.OpcSamplingInterval.Value != endpointToUpdate.OpcSamplingInterval)
                {
                    opcNodeOnEndpoint.OpcSamplingInterval = opcNodeOnEndpoint.OpcSamplingInterval.Value;
                    updated = true;
                }
            }

            if (!updated)
            {
                return methodResponseFactory.Create(HttpStatusCode.BadRequest, "Nodes were not updated.");
            }

            var (writeSucceeded, writeErrorMessage) = await WriteConfig(configurationFileEntries, CancellationToken.None);
            return writeSucceeded
                ? methodResponseFactory.Create(HttpStatusCode.OK, "Nodes updated.")
                : methodResponseFactory.Create(HttpStatusCode.InternalServerError, writeErrorMessage!);
        }
        finally
        {
            OpcPublisherNodeConfigurationFileSemaphore.Release();
        }
    }

    private async Task<MethodResponse> RemoveNodeFromEndpoint(
        MethodRequest methodRequest,
        object userContext)
    {
        LogMethodCalled(OpcPublisherNodeManagerModuleConstants.DirectMethodRemoveNodeFromEndpoint, OpcPublisherNodeManagerModuleConstants.ModuleId);

        if (string.IsNullOrEmpty(methodRequest.DataAsJson))
        {
            LogMethodRequestEmpty(OpcPublisherNodeManagerModuleConstants.DirectMethodRemoveNodeFromEndpoint, OpcPublisherNodeManagerModuleConstants.ModuleId);
            return methodResponseFactory.Create(HttpStatusCode.BadRequest, "No data in request.");
        }

        RemoveNodeFromEndpointRequest? request = null;
        Uri? endpointUri;

        try
        {
            request = JsonSerializer.Deserialize<RemoveNodeFromEndpointRequest>(methodRequest.Data, jsonSerializerOptions);
            endpointUri = new Uri(request!.EndpointUrl);
        }
        catch (UriFormatException ex)
        {
            LogMethodRequestUriFormatExceptionError(request!.EndpointUrl, ex.Message);
            return methodResponseFactory.Create(HttpStatusCode.InternalServerError, $"Exception ({ex.Message}) while parsing EndpointUrl '{request.EndpointUrl}'");
        }
        catch (SerializationException ex)
        {
            LogMethodRequestDeserializationError(OpcPublisherNodeManagerModuleConstants.DirectMethodRemoveNodeFromEndpoint, OpcPublisherNodeManagerModuleConstants.ModuleId, ex.Message);
            return methodResponseFactory.Create(HttpStatusCode.InternalServerError, $"Exception ({ex.Message}) while deserializing message payload");
        }

        await OpcPublisherNodeConfigurationFileSemaphore.WaitAsync(SyncLockTimeoutInMs, CancellationToken.None);

        try
        {
            var (succeeded, configurationFileEntries, errorMessage) = await ReadConfig(CancellationToken.None);
            if (!succeeded)
            {
                return methodResponseFactory.Create(HttpStatusCode.InternalServerError, errorMessage!);
            }

            var configurationFileEntry = configurationFileEntries!.Find(x => x.EndpointUrl!.OriginalString.Equals(endpointUri.OriginalString, StringComparison.OrdinalIgnoreCase));
            if (configurationFileEntry is null)
            {
                return methodResponseFactory.Create(HttpStatusCode.NotFound, "Endpoint was not found.");
            }

            configurationFileEntry.OpcNodes ??= [];

            var opcNodeOnEndpoint = configurationFileEntry.OpcNodes.Find(x => x.Id.Equals(request.NodeId, StringComparison.OrdinalIgnoreCase));

            if (opcNodeOnEndpoint is null)
            {
                return methodResponseFactory.Create(HttpStatusCode.BadRequest, "Node is not present on endpoint - cant remove.");
            }

            configurationFileEntry.OpcNodes.Remove(opcNodeOnEndpoint);

            var (writeSucceeded, writeErrorMessage) = await WriteConfig(configurationFileEntries, CancellationToken.None);
            return writeSucceeded
                ? methodResponseFactory.Create(HttpStatusCode.OK, "Node removed from endpoint.")
                : methodResponseFactory.Create(HttpStatusCode.InternalServerError, writeErrorMessage!);
        }
        finally
        {
            OpcPublisherNodeConfigurationFileSemaphore.Release();
        }
    }

    private async Task<MethodResponse> RemoveNodesFromEndpoint(
        MethodRequest methodRequest,
        object userContext)
    {
        LogMethodCalled(OpcPublisherNodeManagerModuleConstants.DirectMethodRemoveNodesFromEndpoint, OpcPublisherNodeManagerModuleConstants.ModuleId);

        if (string.IsNullOrEmpty(methodRequest.DataAsJson))
        {
            LogMethodRequestEmpty(OpcPublisherNodeManagerModuleConstants.DirectMethodRemoveNodesFromEndpoint, OpcPublisherNodeManagerModuleConstants.ModuleId);
            return methodResponseFactory.Create(HttpStatusCode.BadRequest, "No data in request.");
        }

        RemoveNodesFromEndpointRequest? request = null;
        Uri? endpointUri;

        try
        {
            request = JsonSerializer.Deserialize<RemoveNodesFromEndpointRequest>(methodRequest.Data, jsonSerializerOptions);
            endpointUri = new Uri(request!.EndpointUrl);
        }
        catch (UriFormatException ex)
        {
            LogMethodRequestUriFormatExceptionError(request!.EndpointUrl, ex.Message);
            return methodResponseFactory.Create(HttpStatusCode.InternalServerError, $"Exception ({ex.Message}) while parsing EndpointUrl '{request.EndpointUrl}'");
        }
        catch (SerializationException ex)
        {
            LogMethodRequestDeserializationError(OpcPublisherNodeManagerModuleConstants.DirectMethodRemoveNodesFromEndpoint, OpcPublisherNodeManagerModuleConstants.ModuleId, ex.Message);
            return methodResponseFactory.Create(HttpStatusCode.InternalServerError, $"Exception ({ex.Message}) while deserializing message payload");
        }

        await OpcPublisherNodeConfigurationFileSemaphore.WaitAsync(SyncLockTimeoutInMs, CancellationToken.None);

        try
        {
            var (succeeded, configurationFileEntries, errorMessage) = await ReadConfig(CancellationToken.None);
            if (!succeeded)
            {
                return methodResponseFactory.Create(HttpStatusCode.InternalServerError, errorMessage!);
            }

            var configurationFileEntry = configurationFileEntries!.Find(x => x.EndpointUrl!.OriginalString.Equals(endpointUri.OriginalString, StringComparison.OrdinalIgnoreCase));
            if (configurationFileEntry is null)
            {
                return methodResponseFactory.Create(HttpStatusCode.NotFound, "Endpoint was not found.");
            }

            if (configurationFileEntry.OpcNodes is null)
            {
                return methodResponseFactory.Create(HttpStatusCode.NotFound, "Endpoint with nodes was not found.");
            }

            foreach (var nodeToRemove in request.Nodes)
            {
                var opcNodeOnEndpoint = configurationFileEntry.OpcNodes.Find(x => x.Id.Equals(nodeToRemove.NodeId, StringComparison.OrdinalIgnoreCase));

                if (opcNodeOnEndpoint is null)
                {
                    continue;
                }

                configurationFileEntry.OpcNodes.Remove(opcNodeOnEndpoint);
            }

            var (writeSucceeded, writeErrorMessage) = await WriteConfig(configurationFileEntries, CancellationToken.None);
            return writeSucceeded
                ? methodResponseFactory.Create(HttpStatusCode.OK, "Nodes removed from endpoint.")
                : methodResponseFactory.Create(HttpStatusCode.InternalServerError, writeErrorMessage!);
        }
        finally
        {
            OpcPublisherNodeConfigurationFileSemaphore.Release();
        }
    }

    private async Task<MethodResponse> RemoveAllNodesFromEndpoint(
        MethodRequest methodRequest,
        object userContext)
    {
        LogMethodCalled(OpcPublisherNodeManagerModuleConstants.DirectMethodRemoveAllNodesFromEndpoint, OpcPublisherNodeManagerModuleConstants.ModuleId);

        if (string.IsNullOrEmpty(methodRequest.DataAsJson))
        {
            LogMethodRequestEmpty(OpcPublisherNodeManagerModuleConstants.DirectMethodRemoveAllNodesFromEndpoint, OpcPublisherNodeManagerModuleConstants.ModuleId);
            return methodResponseFactory.Create(HttpStatusCode.BadRequest, "No data in request.");
        }

        RemoveAllNodesFromEndpointRequest? request = null;
        Uri? endpointUri;

        try
        {
            request = JsonSerializer.Deserialize<RemoveAllNodesFromEndpointRequest>(methodRequest.Data, jsonSerializerOptions);
            endpointUri = new Uri(request!.EndpointUrl);
        }
        catch (UriFormatException ex)
        {
            LogMethodRequestUriFormatExceptionError(request!.EndpointUrl, ex.Message);
            return methodResponseFactory.Create(HttpStatusCode.InternalServerError, $"Exception ({ex.Message}) while parsing EndpointUrl '{request.EndpointUrl}'");
        }
        catch (SerializationException ex)
        {
            LogMethodRequestDeserializationError(OpcPublisherNodeManagerModuleConstants.DirectMethodRemoveAllNodesFromEndpoint, OpcPublisherNodeManagerModuleConstants.ModuleId, ex.Message);
            return methodResponseFactory.Create(HttpStatusCode.InternalServerError, $"Exception ({ex.Message}) while deserializing message payload");
        }

        await OpcPublisherNodeConfigurationFileSemaphore.WaitAsync(SyncLockTimeoutInMs, CancellationToken.None);

        try
        {
            var (succeeded, configurationFileEntries, errorMessage) = await ReadConfig(CancellationToken.None);
            if (!succeeded)
            {
                return methodResponseFactory.Create(HttpStatusCode.InternalServerError, errorMessage!);
            }

            var configurationFileEntry = configurationFileEntries!.Find(x => x.EndpointUrl!.OriginalString.Equals(endpointUri.OriginalString, StringComparison.OrdinalIgnoreCase));
            if (configurationFileEntry is null)
            {
                return methodResponseFactory.Create(HttpStatusCode.NotFound, "Endpoint was not found.");
            }

            configurationFileEntry.OpcNodes = [];

            var (writeSucceeded, writeErrorMessage) = await WriteConfig(configurationFileEntries, CancellationToken.None);
            return writeSucceeded
                ? methodResponseFactory.Create(HttpStatusCode.OK, "Nodes removed from endpoint.")
                : methodResponseFactory.Create(HttpStatusCode.InternalServerError, writeErrorMessage!);
        }
        finally
        {
            OpcPublisherNodeConfigurationFileSemaphore.Release();
        }
    }

    private async Task<MethodResponse> RemoveEndpoint(
        MethodRequest methodRequest,
        object userContext)
    {
        LogMethodCalled(OpcPublisherNodeManagerModuleConstants.DirectMethodRemoveEndpoint, OpcPublisherNodeManagerModuleConstants.ModuleId);

        if (string.IsNullOrEmpty(methodRequest.DataAsJson))
        {
            LogMethodRequestEmpty(OpcPublisherNodeManagerModuleConstants.DirectMethodRemoveEndpoint, OpcPublisherNodeManagerModuleConstants.ModuleId);
            return methodResponseFactory.Create(HttpStatusCode.BadRequest, "No data in request.");
        }

        RemoveEndpointRequest? request = null;
        Uri? endpointUri;

        try
        {
            request = JsonSerializer.Deserialize<RemoveEndpointRequest>(methodRequest.Data, jsonSerializerOptions);
            endpointUri = new Uri(request!.EndpointUrl);
        }
        catch (UriFormatException ex)
        {
            LogMethodRequestUriFormatExceptionError(request!.EndpointUrl, ex.Message);
            return methodResponseFactory.Create(HttpStatusCode.InternalServerError, $"Exception ({ex.Message}) while parsing EndpointUrl '{request.EndpointUrl}'");
        }
        catch (SerializationException ex)
        {
            LogMethodRequestDeserializationError(OpcPublisherNodeManagerModuleConstants.DirectMethodRemoveEndpoint, OpcPublisherNodeManagerModuleConstants.ModuleId, ex.Message);
            return methodResponseFactory.Create(HttpStatusCode.InternalServerError, $"Exception ({ex.Message}) while deserializing message payload");
        }

        await OpcPublisherNodeConfigurationFileSemaphore.WaitAsync(SyncLockTimeoutInMs, CancellationToken.None);

        try
        {
            var (succeeded, configurationFileEntries, errorMessage) = await ReadConfig(CancellationToken.None);
            if (!succeeded)
            {
                return methodResponseFactory.Create(HttpStatusCode.InternalServerError, errorMessage!);
            }

            var configurationFileEntry = configurationFileEntries!.Find(x => x.EndpointUrl!.OriginalString.Equals(endpointUri.OriginalString, StringComparison.OrdinalIgnoreCase));
            if (configurationFileEntry is null)
            {
                return methodResponseFactory.Create(HttpStatusCode.NotFound, "Endpoint was not found.");
            }

            configurationFileEntries.Remove(configurationFileEntry);

            var (writeSucceeded, writeErrorMessage) = await WriteConfig(configurationFileEntries, CancellationToken.None);
            return writeSucceeded
                ? methodResponseFactory.Create(HttpStatusCode.OK, "Endpoint removed.")
                : methodResponseFactory.Create(HttpStatusCode.InternalServerError, writeErrorMessage!);
        }
        finally
        {
            OpcPublisherNodeConfigurationFileSemaphore.Release();
        }
    }

    private static GetEndpointsResponse CreateGetEndpointsResponse(
        IEnumerable<ConfigurationFileEntry> configurationFileEntries,
        GetEndpointsRequest request)
    {
        var endpointUrls = configurationFileEntries
            .Select(x => x.EndpointUrl!.OriginalString)
            .ToList();

        var elementsToSkip = request.PageIndex * PageSizeForEndpointList;

        var endpointUrlsToSend = endpointUrls
            .Skip(elementsToSkip)
            .Take(PageSizeForEndpointList)
            .ToList();

        var takenElements = elementsToSkip + endpointUrlsToSend.Count;

        return new GetEndpointsResponse(
            endpointUrlsToSend.Select(e => e).ToList(),
            endpointUrls.Count > takenElements);
    }

    private static GetEndpointWithNodesResponse CreateGetEndpointWithNodesResponse(
        IEnumerable<ConfigurationFileEntry> configurationFileEntries,
        GetEndpointWithNodesRequest request)
    {
        var endpoint = configurationFileEntries.Single(x => x.EndpointUrl!.OriginalString.Equals(request.EndpointUrl, StringComparison.OrdinalIgnoreCase));
        var elementsToSkip = request.PageIndex * MaxNodesForPageSize;
        var opcNodesToSend = endpoint.OpcNodes?.Skip(elementsToSkip).Take(MaxNodesForPageSize).ToList();
        var takenElements = elementsToSkip + opcNodesToSend?.Count;

        var response = new GetEndpointWithNodesResponse(
            new Endpoint(request.EndpointUrl, opcNodesToSend ?? []),
            endpoint.OpcNodes?.Count > takenElements);

        return response;
    }

    private static GetEndpointsWithEmptyOpcNodesListResponse CreateGetEndpointsWithEmptyOpcNodesListResponse(
        IEnumerable<ConfigurationFileEntry> configurationFileEntries)
    {
        var endpointList = new List<string>();

        foreach (var configurationFileEntry in configurationFileEntries)
        {
            if (configurationFileEntry.EndpointUrl is null)
            {
                continue;
            }

            if (configurationFileEntry.OpcNodes is null ||
                configurationFileEntry.OpcNodes.Count == 0)
            {
                endpointList.Add(configurationFileEntry.EndpointUrl.AbsoluteUri);
            }
        }

        return new GetEndpointsWithEmptyOpcNodesListResponse(endpointList);
    }

    private async Task<(bool Succeeded, List<ConfigurationFileEntry>? ConfigurationFileEntries, string? ErrorMessage)> ReadConfig(
        CancellationToken cancellationToken)
    {
        List<ConfigurationFileEntry>? configurationFileEntries = null;

        try
        {
            // If the file exists, read it, if not just continue.
            if (File.Exists(OpcPublisherNodeConfigurationFilename))
            {
                var json = await File.ReadAllTextAsync(OpcPublisherNodeConfigurationFilename, cancellationToken);
                configurationFileEntries = JsonSerializer.Deserialize<List<ConfigurationFileEntry>>(json, fileSerializerOptions);
            }
            else
            {
                LogConfigurationFileNotFound(OpcPublisherNodeConfigurationFilename);
                return (false, configurationFileEntries, $"The node configuration file '{OpcPublisherNodeConfigurationFilename}' does not exist.");
            }
        }
        catch (IOException ex)
        {
            var errorMessage = ex.GetLastInnerMessage();
            LogConfigurationFileLoadError(errorMessage);

            return (false, configurationFileEntries, $"Loading of the node configuration file failed. Check the file exist and have correct syntax: '{errorMessage}'.");
        }

        return (true, configurationFileEntries, null);
    }

    private async Task<(bool Succeeded, string? ErrorMessage)> WriteConfig(
        List<ConfigurationFileEntry>? configurationFileEntries,
        CancellationToken cancellationToken)
    {
        try
        {
            var jsonOutput = JsonSerializer.Serialize(configurationFileEntries, fileSerializerOptions);
            await File.WriteAllTextAsync(
                OpcPublisherNodeConfigurationFilenameTemp,
                jsonOutput,
                Encoding.UTF8,
                cancellationToken);

            File.Copy(OpcPublisherNodeConfigurationFilenameTemp, OpcPublisherNodeConfigurationFilename, overwrite: true);
            File.Delete(OpcPublisherNodeConfigurationFilenameTemp);
        }
        catch (IOException ex)
        {
            var errorMessage = ex.GetLastInnerMessage();
            LogConfigurationFileWriteError(errorMessage);
            return (false, $"Writing to the node configuration file failed. Check the file exist and have correct syntax: '{errorMessage}'");
        }

        return (true, null);
    }

    private static bool IsEndpointUrlValid(
        string endpointUrl)
        => Uri.TryCreate(endpointUrl, UriKind.Absolute, out _);
}