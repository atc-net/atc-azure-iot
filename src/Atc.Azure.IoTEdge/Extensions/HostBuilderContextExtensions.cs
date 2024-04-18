namespace Atc.Azure.IoTEdge.Extensions;

public static class HostBuilderContextExtensions
{
    public static bool IsStandaloneMode(
        this HostBuilderContext hostBuilderContext)
        => hostBuilderContext.HostingEnvironment.EnvironmentName.Equals(
            IotEdgeExecutionMode.Standalone.ToString(),
            StringComparison.Ordinal);

    public static bool IsEmulatorMode(
        this HostBuilderContext hostBuilderContext)
        => hostBuilderContext.HostingEnvironment.EnvironmentName.Equals(
            IotEdgeExecutionMode.Emulator.ToString(),
            StringComparison.Ordinal);
}