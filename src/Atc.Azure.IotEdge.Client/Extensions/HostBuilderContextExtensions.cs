namespace Atc.Azure.IotEdge.Client.Extensions;

public static class HostBuilderContextExtensions
{
    public static bool IsStandaloneMode(
        this HostBuilderContext hostBuilderContext)
        => hostBuilderContext.HostingEnvironment.EnvironmentName.Equals(
            IotEdgeExecutionMode.Standalone.ToString(),
            StringComparison.Ordinal);
}