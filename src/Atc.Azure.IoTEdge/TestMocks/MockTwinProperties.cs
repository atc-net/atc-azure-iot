namespace Atc.Azure.IoTEdge.TestMocks;

/// <summary>
/// The twin properties a new <see cref="MockModuleClientWrapper"/> starts with, for example to run a module in the
/// Standalone environment with some desired properties. Each mock copies them when it is created, and changes only its
/// own twin.
/// </summary>
public static class MockTwinProperties
{
    /// <summary>
    /// Gets or sets the desired properties a new mock starts with.
    /// </summary>
    public static TwinCollection Desired { get; set; } = new();

    /// <summary>
    /// Gets or sets the reported properties a new mock starts with.
    /// </summary>
    public static TwinCollection Reported { get; set; } = new();
}