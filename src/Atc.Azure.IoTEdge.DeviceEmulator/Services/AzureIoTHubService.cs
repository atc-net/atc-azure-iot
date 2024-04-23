// ReSharper disable ConvertIfStatementToReturnStatement
// ReSharper disable InvertIf
namespace Atc.Azure.IoTEdge.DeviceEmulator.Services;

/// <summary>
/// The main AzureIoTHubService - Handles call execution.
/// </summary>
public partial class AzureIoTHubService : IAzureIoTHubService
{
    private readonly JsonSerializerOptions jsonSerializerOptions;
    private readonly IIoTHubService iotHubService;

    public AzureIoTHubService(
        ILoggerFactory loggerFactory,
        IIoTHubService iotHubService)
    {
        logger = loggerFactory.CreateLogger<AzureIoTHubService>();
        jsonSerializerOptions = JsonSerializerOptionsFactory.Create();
        this.iotHubService = iotHubService;
    }

    public (bool Succeeded, string EmulationManifest) TransformTemplateToEmulationManifest(
        string templateContent)
    {
        ArgumentNullException.ThrowIfNull(templateContent);

        JsonNode? rootNode;

        try
        {
            rootNode = JsonNode.Parse(templateContent);

            var modulesContent = rootNode!["modulesContent"];
            var edgeAgentDesired = modulesContent![EdgeAgentConstants.ModuleId]!["properties.desired"];
            var edgeAgentModulesSection = edgeAgentDesired!["modules"];

            foreach (var customModule in edgeAgentModulesSection!.AsObject().AsEnumerable())
            {
                var settings = customModule.Value!["settings"];
                settings!["image"] = "wardsco/sleep:latest";
            }

            LogIotHubTransformTemplateToManifestSucceeded();
        }
        catch (JsonException ex)
        {
            LogIotHubTransformTemplateToManifestFailed(ex.Message);
            return (false, ex.Message);
        }

        var emulationManifest = rootNode
            .ToJsonString(jsonSerializerOptions)
            .Replace("\\u0022", "\\\"", StringComparison.Ordinal);

        return (true, emulationManifest);
    }

    public Task<(bool Succeeded, string DeviceConnectionString)> ProvisionIotEdgeDevice(
        string emulationManifest,
        string deviceId,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(emulationManifest);
        ArgumentNullException.ThrowIfNull(deviceId);

        var manifestContent = GetConfigurationContentFromManifest(emulationManifest);
        if (manifestContent is null)
        {
            var message = $"Could not deserialize the emulationManifest: '{emulationManifest}'";
            LogIotHubProvisionIotEdgeDeviceInvalidManifest(message);
            throw new SerializationException(message);
        }

        return InvokeProvisionIotEdgeDevice(manifestContent, deviceId, cancellationToken);
    }

    /// <summary>
    /// Gets the ConfigurationContent from the template manifest
    /// </summary>
    /// <param name="emulationManifest">The emulation manifest</param>
    /// <returns>The emulation manifest as a <see cref="ConfigurationContent"/></returns>
    /// <remarks>
    /// We utilize Newtonsoft.Json here, because System.Text.Json does not work!
    /// When de-serializing the edgeAgent - properties.desired, instead of "object-array", the serializer returns:
    /// ValueKind = Object : " instead of {
    /// and " instead of } (for the end).
    /// </remarks>
    public static ConfigurationContent GetConfigurationContentFromManifest(
        string emulationManifest)
        => Newtonsoft.Json.JsonConvert.DeserializeObject<ConfigurationContent>(emulationManifest)!;

    private async Task<(bool Succeeded, string DeviceConnectionString)> InvokeProvisionIotEdgeDevice(
        ConfigurationContent manifestContent,
        string deviceId,
        CancellationToken cancellationToken)
    {
        var sasKey = await EnsureDeviceRegistrationAndGetDeviceSasKey(deviceId, cancellationToken);
        if (string.IsNullOrEmpty(sasKey))
        {
            return (false, string.Empty);
        }

        var removeOldModulesSucceeded = await RemoveOldModules(deviceId, cancellationToken);
        if (!removeOldModulesSucceeded)
        {
            return (false, string.Empty);
        }

        var (addNewModulesSucceeded, _) = await iotHubService.AddNewModules(deviceId, manifestContent, cancellationToken);
        if (!addNewModulesSucceeded)
        {
            return (false, string.Empty);
        }

        var applyConfigurationSucceeded = await ApplyConfigurationContentOnDevice(manifestContent, deviceId, cancellationToken);
        if (!applyConfigurationSucceeded)
        {
            return (false, string.Empty);
        }

        var deviceConnectionString = await iotHubService.GetDeviceConnectionString(deviceId, cancellationToken);
        if (string.IsNullOrEmpty(deviceConnectionString))
        {
            return (false, string.Empty);
        }

        return (true, deviceConnectionString);
    }

    private async Task<string?> EnsureDeviceRegistrationAndGetDeviceSasKey(
        string deviceId,
        CancellationToken cancellationToken)
    {
        try
        {
            var device = await iotHubService.GetDevice(deviceId, cancellationToken);
            if (device is null)
            {
                var (succeeded, createdDevice) = await iotHubService.CreateDevice(deviceId, edgeEnabled: true, cancellationToken);
                if (succeeded)
                {
                    device = createdDevice;
                }
            }

            if (device is null)
            {
                LogIotHubProvisionIotEdgeDeviceFailed(deviceId, "Failed to create device");
                return null;
            }

            LogIotHubProvisionIotEdgeDeviceSucceeded(deviceId);
            return device.Authentication.SymmetricKey.PrimaryKey;
        }
        catch (Exception ex)
        {
            LogIotHubProvisionIotEdgeDeviceFailed(deviceId, ex.Message);
            return string.Empty;
        }
    }

    private async Task<bool> RemoveOldModules(
        string deviceId,
        CancellationToken cancellationToken)
    {
        try
        {
            foreach (var module in await iotHubService.GetModulesOnIotEdgeDevice(deviceId, cancellationToken))
            {
                if (!module.Id.StartsWith('$'))
                {
                    await iotHubService.RemoveModuleFromDevice(deviceId, module.Id, cancellationToken);
                }
            }

            LogIotHubRemoveModulesFromIotEdgeDeviceSucceeded(deviceId);
        }
        catch (Exception ex)
        {
            LogIotHubRemoveModulesFromIotEdgeDeviceFailed(deviceId, ex.Message);
            return false;
        }

        return true;
    }

    private async Task<bool> ApplyConfigurationContentOnDevice(
        ConfigurationContent manifestContent,
        string deviceId,
        CancellationToken cancellationToken)
    {
        try
        {
            await iotHubService.ApplyConfigurationContentOnDevice(deviceId, manifestContent, cancellationToken);
            LogIotHubApplyConfigurationContentToIotEdgeDeviceSucceeded(deviceId);
        }
        catch (Exception ex)
        {
            LogIotHubApplyConfigurationContentToIotEdgeDeviceFailed(deviceId, ex.Message);
            return false;
        }

        return true;
    }
}