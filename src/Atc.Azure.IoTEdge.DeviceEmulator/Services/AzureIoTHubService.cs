// ReSharper disable ConvertIfStatementToReturnStatement
// ReSharper disable InvertIf
namespace Atc.Azure.IoTEdge.DeviceEmulator.Services;

/// <summary>
/// The main AzureIoTHubService - Handles call execution.
/// </summary>
public partial class AzureIoTHubService : IAzureIoTHubService
{
    private readonly JsonSerializerOptions jsonSerializerOptions;
    private readonly IRegistryManagerWrapper registryManagerWrapper;

    public AzureIoTHubService(
        ILogger logger,
        IRegistryManagerWrapper registryManagerWrapper)
    {
        this.logger = logger;
        this.registryManagerWrapper = registryManagerWrapper;
        jsonSerializerOptions = JsonSerializerOptionsFactory.Create();
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
        string iotHubConnectionString,
        string deviceId,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(emulationManifest);
        ArgumentNullException.ThrowIfNull(iotHubConnectionString);
        ArgumentNullException.ThrowIfNull(deviceId);

        if (!iotHubConnectionString.Contains("HostName=", StringComparison.Ordinal))
        {
            const string message = "Could not find HostName in IoTHubConnectionString";
            LogIotHubProvisionIotEdgeDeviceMissingHostName(message);
            throw new ArgumentException(message, nameof(iotHubConnectionString));
        }

        var manifestContent = GetConfigurationContentFromManifest(emulationManifest);
        if (manifestContent is null)
        {
            var message = $"Could not deserialize the emulationManifest: '{emulationManifest}'";
            LogIotHubProvisionIotEdgeDeviceInvalidManifest(message);
            throw new SerializationException(message);
        }

        return InvokeProvisionIotEdgeDevice(manifestContent, iotHubConnectionString, deviceId, cancellationToken);
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
        => Newtonsoft.Json.JsonConvert.DeserializeObject<ConfigurationContent>(emulationManifest);

    public Task<bool> RemoveIotEdgeDevice(
        string deviceId,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(deviceId);

        return InvokeRemoveIotEdgeDevice(deviceId, cancellationToken);
    }

    private async Task<(bool Succeeded, string DeviceConnectionString)> InvokeProvisionIotEdgeDevice(
        ConfigurationContent manifestContent,
        string iotHubConnectionString,
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

        var addNewModulesSucceeded = await AddNewModules(deviceId, manifestContent, cancellationToken);
        if (!addNewModulesSucceeded)
        {
            return (false, string.Empty);
        }

        var applyConfigurationSucceeded = await ApplyConfigurationContentOnDevice(manifestContent, deviceId, cancellationToken);
        if (!applyConfigurationSucceeded)
        {
            return (false, string.Empty);
        }

        return (true, $"HostName={GetIotHubHostName(iotHubConnectionString)};DeviceId={deviceId};SharedAccessKey={sasKey}");
    }

    private async Task<bool> InvokeRemoveIotEdgeDevice(
        string deviceId,
        CancellationToken cancellationToken)
    {
        try
        {
            await registryManagerWrapper.RemoveDeviceAsync(deviceId, cancellationToken);
            LogIotHubRemoveIotEdgeDeviceSucceeded(deviceId);
        }
        catch (Exception ex)
        {
            LogIotHubRemoveIotEdgeDeviceFailed(deviceId, ex.Message);
            return false;
        }

        return true;
    }

    private static string GetIotHubHostName(
        string iotHubConnectionString)
        => iotHubConnectionString
            .Split(";")
            .Single(e => e.Contains("HostName=", StringComparison.Ordinal))
            .Replace("HostName=", string.Empty, StringComparison.Ordinal);

    private async Task<string> EnsureDeviceRegistrationAndGetDeviceSasKey(
        string deviceId,
        CancellationToken cancellationToken)
    {
        try
        {
            var device = await registryManagerWrapper.GetDeviceAsync(deviceId, cancellationToken) ??
                         await registryManagerWrapper.AddDeviceAsync(
                             new Device(deviceId)
                             {
                                 Capabilities = new DeviceCapabilities { IotEdge = true },
                             },
                             cancellationToken);

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
            foreach (var module in await registryManagerWrapper.GetModulesOnDeviceAsync(deviceId, cancellationToken))
            {
                if (!module.Id.StartsWith('$'))
                {
                    await registryManagerWrapper.RemoveModuleAsync(module, cancellationToken);
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

    private async Task<bool> AddNewModules(
        string deviceId,
        ConfigurationContent manifestContent,
        CancellationToken cancellationToken)
    {
        try
        {
            foreach (var module in manifestContent.ModulesContent.Keys)
            {
                if (!module.StartsWith("$", StringComparison.Ordinal))
                {
                    await registryManagerWrapper.AddModuleAsync(new Module(deviceId, module), cancellationToken);
                }
            }

            LogIotHubAddModulesToIotEdgeDeviceSucceeded(deviceId);
        }
        catch (Exception ex)
        {
            LogIotHubAddModulesToIotEdgeDeviceFailed(deviceId, ex.Message);
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
            await registryManagerWrapper.ApplyConfigurationContentOnDeviceAsync(deviceId, manifestContent, cancellationToken);
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