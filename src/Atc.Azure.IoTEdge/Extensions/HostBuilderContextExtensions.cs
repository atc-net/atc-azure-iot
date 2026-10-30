namespace Atc.Azure.IoTEdge.Extensions;

public static class HostBuilderContextExtensions
{
    /// <summary>
    /// Determines whether the module runs in the Standalone environment (<c>DOTNET_ENVIRONMENT=Standalone</c>), where it
    /// uses <see cref="TestMocks.MockModuleClientWrapper"/> instead of connecting to edgeHub. The case is ignored, as for
    /// <see cref="HostEnvironmentEnvExtensions.IsEnvironment"/>.
    /// </summary>
    /// <param name="hostBuilderContext">The host builder context.</param>
    /// <returns><see langword="true"/> if the environment is Standalone; otherwise, <see langword="false"/>.</returns>
    public static bool IsStandaloneMode(this HostBuilderContext hostBuilderContext)
    {
        ArgumentNullException.ThrowIfNull(hostBuilderContext);

        return hostBuilderContext.HostingEnvironment.IsEnvironment(nameof(IotEdgeExecutionMode.Standalone));
    }
}