// ReSharper disable once CheckNamespace
namespace Aspire.Hosting;

/// <summary>
/// Extension methods for running Aspire resources as IoT Edge modules on an emulated IoT Edge device.
/// </summary>
public static class IoTEdgeModuleBuilderExtensions
{
    private const string MethodNameArgument = "methodName";
    private const string PayloadArgument = "payload";
    private const string DesiredPropertiesArgument = "desiredProperties";

    /// <summary>
    /// Runs the resource as an IoT Edge module on the device. A module identity is created in IoT Hub, and the module gets
    /// the <c>EdgeHubConnectionString</c> (and <c>EdgeModuleCACertificateFile</c> when the device has an edgeHub)
    /// environment variables that <c>ModuleClient.CreateFromEnvironmentAsync</c> reads.
    /// The resource also gets dashboard commands to invoke direct methods, update desired properties and show the module twin.
    /// </summary>
    /// <typeparam name="T">The resource type, e.g. a project or a container.</typeparam>
    /// <param name="builder">The module resource builder.</param>
    /// <param name="device">The IoT Edge device the module runs on.</param>
    /// <param name="moduleId">The module id in IoT Hub. Defaults to the resource name.</param>
    /// <returns>The resource builder.</returns>
    public static IResourceBuilder<T> AsIoTEdgeModule<T>(
        this IResourceBuilder<T> builder,
        IResourceBuilder<IoTEdgeDeviceResource> device,
        string? moduleId = null)
        where T : IResourceWithEnvironment, IResourceWithWaitSupport
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(device);

        moduleId = string.IsNullOrWhiteSpace(moduleId)
            ? builder.Resource.Name
            : moduleId;

        var deviceResource = device.Resource;
        var module = new IoTEdgeModuleAnnotation(deviceResource, builder.Resource, moduleId);
        deviceResource.AddModule(module);

        builder
            .WithAnnotation(module)
            .WaitFor(device);

        // When the edgeHub is added after the module, WithEdgeHub adds the wait itself.
        if (deviceResource.EdgeHub is not null)
        {
            builder.WithAnnotation(new WaitAnnotation(deviceResource.EdgeHub, WaitType.WaitUntilHealthy));
        }

        if (builder.Resource is ContainerResource)
        {
            builder.WithAnnotation(
                new ContainerMountAnnotation(
                    deviceResource.CertificateDirectory,
                    IoTEdgeHubResource.ContainerCertificateDirectory,
                    ContainerMountType.BindMount,
                    isReadOnly: true));
        }

        return builder
            .WithEnvironment(context => ConfigureModuleEnvironment(context, module))
            .WithModuleCommands(module);
    }

    private static async Task ConfigureModuleEnvironment(
        EnvironmentCallbackContext context,
        IoTEdgeModuleAnnotation module)
    {
        if (!context.ExecutionContext.IsRunMode)
        {
            return;
        }

        var device = module.Device;
        var identity = await device.WaitForProvisioningAsync(context.CancellationToken);
        var isContainer = module.ModuleResource is ContainerResource;

        var gatewayHostName = device.EdgeHub switch
        {
            null => null,
            _ when isContainer => device.EdgeHub.ContainerHostName,
            _ => "localhost",
        };

        var environment = context.EnvironmentVariables;
        environment[EdgeHubEnvironmentVariableNames.EdgeHubConnectionString] = identity.GetModuleConnectionString(module.ModuleId, gatewayHostName);
        environment["IOTEDGE_IOTHUBHOSTNAME"] = identity.IotHubHostName;
        environment["IOTEDGE_DEVICEID"] = identity.DeviceId;
        environment["IOTEDGE_MODULEID"] = module.ModuleId;

        if (device.EdgeHub is null)
        {
            return;
        }

        environment["IOTEDGE_GATEWAYHOSTNAME"] = gatewayHostName!;

        if (isContainer)
        {
            environment[EdgeHubEnvironmentVariableNames.EdgeModuleCaCertificateFile] =
                $"{IoTEdgeHubResource.ContainerCertificateDirectory}/{EdgeHubCertificateFiles.DeviceCaCertificateFileName}";
        }
        else if (!OperatingSystem.IsMacOS())
        {
            // The SDK installs this CA into the current user's root store, which macOS does not allow without a prompt.
            // On macOS, a module running on the host must accept the edgeHub test certificate itself.
            environment[EdgeHubEnvironmentVariableNames.EdgeModuleCaCertificateFile] = device.EdgeHub.Certificates.DeviceCaCertificateFile;
        }
    }

    private static IResourceBuilder<T> WithModuleCommands<T>(
        this IResourceBuilder<T> builder,
        IoTEdgeModuleAnnotation module)
        where T : IResource
    {
        var device = module.Device;

        ResourceCommandState UpdateState(UpdateCommandStateContext context)
            => device.IoTHubService is null || context.ResourceSnapshot.State?.Text != KnownResourceStates.Running
                ? ResourceCommandState.Disabled
                : ResourceCommandState.Enabled;

        return builder
            .WithCommand(
                name: "iotedge-invoke-method",
                displayName: "Invoke direct method",
                executeCommand: context => InvokeDirectMethod(context, module),
                commandOptions: new CommandOptions
                {
                    Description = $"Invokes a direct method on module '{module.ModuleId}' through IoT Hub.",
                    IconName = "Play",
                    UpdateState = UpdateState,
                    Arguments =
                    [
                        new InteractionInput
                        {
                            Name = MethodNameArgument,
                            Label = "Method name",
                            InputType = InputType.Text,
                            Required = true,
                        },
                        new InteractionInput
                        {
                            Name = PayloadArgument,
                            Label = "JSON payload",
                            InputType = InputType.Text,
                            Placeholder = "{}",
                        },
                    ],
                })
            .WithCommand(
                name: "iotedge-update-desired-properties",
                displayName: "Update desired properties",
                executeCommand: context => UpdateDesiredProperties(context, module),
                commandOptions: new CommandOptions
                {
                    Description = $"Merges a JSON object into the desired properties of the '{module.ModuleId}' module twin.",
                    IconName = "Edit",
                    UpdateState = context => device.IoTHubService is null
                        ? ResourceCommandState.Disabled
                        : ResourceCommandState.Enabled,
                    Arguments =
                    [
                        new InteractionInput
                        {
                            Name = DesiredPropertiesArgument,
                            Label = "Desired properties (JSON object)",
                            InputType = InputType.Text,
                            Required = true,
                            Placeholder = "{ \"setting\": \"value\" }",
                        },
                    ],
                })
            .WithCommand(
                name: "iotedge-show-module-twin",
                displayName: "Show module twin",
                executeCommand: context => ShowModuleTwin(context, module),
                commandOptions: new CommandOptions
                {
                    Description = $"Shows the '{module.ModuleId}' module twin from IoT Hub.",
                    IconName = "DocumentText",
                    UpdateState = context => device.IoTHubService is null
                        ? ResourceCommandState.Disabled
                        : ResourceCommandState.Enabled,
                });
    }

    private static async Task<ExecuteCommandResult> InvokeDirectMethod(
        ExecuteCommandContext context,
        IoTEdgeModuleAnnotation module)
    {
        var moduleService = module.Device.IoTHubModuleService;
        if (moduleService is null)
        {
            return CommandResults.Failure("The device has not been provisioned.");
        }

        var methodName = context.Arguments.GetString(MethodNameArgument);
        if (string.IsNullOrWhiteSpace(methodName))
        {
            return CommandResults.Failure("A method name is required.");
        }

        var payload = context.Arguments.GetString(PayloadArgument);
        if (string.IsNullOrWhiteSpace(payload))
        {
            payload = "{}";
        }

        if (!IsValidJson(payload))
        {
            return CommandResults.Failure("The payload is not valid JSON.");
        }

        var result = await moduleService.CallMethod(
            module.Device.DeviceId,
            module.ModuleId,
            new MethodParameterModel(methodName, payload),
            requestOptions: null,
            context.CancellationToken);

        var message = $"'{methodName}' on '{module.ModuleId}' returned status {result.Status}.";
        return result.Status is >= 200 and < 300
            ? CommandResults.Success(message, result.JsonPayload, CommandResultFormat.Json)
            : CommandResults.Failure(message, result.JsonPayload, CommandResultFormat.Json);
    }

    private static async Task<ExecuteCommandResult> UpdateDesiredProperties(
        ExecuteCommandContext context,
        IoTEdgeModuleAnnotation module)
    {
        var iotHubService = module.Device.IoTHubService;
        if (iotHubService is null)
        {
            return CommandResults.Failure("The device has not been provisioned.");
        }

        var json = context.Arguments.GetString(DesiredPropertiesArgument);
        if (string.IsNullOrWhiteSpace(json) || !IsJsonObject(json))
        {
            return CommandResults.Failure("The desired properties must be a JSON object.");
        }

        var twin = await iotHubService.UpdateDesiredProperties(
            module.Device.DeviceId,
            module.ModuleId,
            new TwinCollection(json),
            context.CancellationToken);

        return twin is null
            ? CommandResults.Failure($"Could not update the desired properties of '{module.ModuleId}'.")
            : CommandResults.Success($"Updated the desired properties of '{module.ModuleId}'.", twin.Properties.Desired.ToJson(), CommandResultFormat.Json);
    }

    private static async Task<ExecuteCommandResult> ShowModuleTwin(
        ExecuteCommandContext context,
        IoTEdgeModuleAnnotation module)
    {
        var iotHubService = module.Device.IoTHubService;
        if (iotHubService is null)
        {
            return CommandResults.Failure("The device has not been provisioned.");
        }

        var twin = await iotHubService.GetModuleTwin(module.Device.DeviceId, module.ModuleId, context.CancellationToken);
        return twin is null
            ? CommandResults.Failure($"Could not get the twin of '{module.ModuleId}'.")
            : CommandResults.Success($"Module twin of '{module.ModuleId}'.", twin.ToJson(), CommandResultFormat.Json);
    }

    private static bool IsValidJson(string json)
    {
        try
        {
            using var document = JsonDocument.Parse(json);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private static bool IsJsonObject(string json)
    {
        try
        {
            using var document = JsonDocument.Parse(json);
            return document.RootElement.ValueKind == JsonValueKind.Object;
        }
        catch (JsonException)
        {
            return false;
        }
    }
}