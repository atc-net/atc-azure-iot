namespace Atc.Azure.IoTEdge.DeviceEmulator.Provisioning;

/// <summary>
/// The identities of a provisioned IoT Edge device and its modules.
/// </summary>
/// <param name="IotHubHostName">The IoT Hub host name, e.g. myhub.azure-devices.net.</param>
/// <param name="DeviceId">The edge device id.</param>
/// <param name="ModuleKeys">The primary symmetric key for each module identity, including $edgeHub.</param>
public sealed record IoTEdgeDeviceIdentity(
    string IotHubHostName,
    string DeviceId,
    IReadOnlyDictionary<string, string> ModuleKeys)
{
    /// <summary>
    /// Builds the connection string for the $edgeHub module, which edgeHub uses to connect to IoT Hub.
    /// </summary>
    /// <returns>The $edgeHub module connection string.</returns>
    public string GetEdgeHubConnectionString()
        => GetModuleConnectionString(EdgeHubConstants.ModuleId, gatewayHostName: null);

    /// <summary>
    /// Builds a module connection string.
    /// </summary>
    /// <param name="moduleId">The module id.</param>
    /// <param name="gatewayHostName">
    /// The host name of the edgeHub the module connects through, or <see langword="null"/> to connect directly to IoT Hub.
    /// </param>
    /// <returns>The module connection string.</returns>
    /// <exception cref="KeyNotFoundException">The module was not provisioned.</exception>
    public string GetModuleConnectionString(
        string moduleId,
        string? gatewayHostName)
    {
        ArgumentException.ThrowIfNullOrEmpty(moduleId);

        if (!ModuleKeys.TryGetValue(moduleId, out var key))
        {
            throw new KeyNotFoundException($"Module '{moduleId}' is not provisioned on device '{DeviceId}'.");
        }

        return string.IsNullOrEmpty(gatewayHostName)
            ? $"HostName={IotHubHostName};DeviceId={DeviceId};ModuleId={moduleId};SharedAccessKey={key}"
            : $"HostName={IotHubHostName};GatewayHostName={gatewayHostName};DeviceId={DeviceId};ModuleId={moduleId};SharedAccessKey={key}";
    }
}