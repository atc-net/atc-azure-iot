namespace Atc.Azure.IoTEdge.TestMocks;

public static class MockTwinProperties
{
    public static TwinCollection Desired { get; set; } = new();

    public static TwinCollection Reported { get; set; } = new();
}