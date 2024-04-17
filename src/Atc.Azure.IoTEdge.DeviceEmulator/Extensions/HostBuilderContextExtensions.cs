namespace Atc.Azure.IoTEdge.DeviceEmulator.Extensions;

public static class HostBuilderContextExtensions
{
    public static bool IsStandaloneMode(
        this HostBuilderContext hostBuilderContext)
    {
        ArgumentNullException.ThrowIfNull(hostBuilderContext);

        return DeviceEmulationConstants.StandaloneMode.Equals(
            hostBuilderContext.HostingEnvironment.EnvironmentName,
            StringComparison.Ordinal);
    }

    public static bool IsEmulatorMode(
        this HostBuilderContext hostBuilderContext)
    {
        ArgumentNullException.ThrowIfNull(hostBuilderContext);

        return DeviceEmulationConstants.EmulatorMode.Equals(
            hostBuilderContext.HostingEnvironment.EnvironmentName,
            StringComparison.Ordinal);
    }
}