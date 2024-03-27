namespace Atc.Azure.IotEdge.Client.TestMocks;

public static class MockTwinProperties
{
    public static TwinCollection Desired { get; set; } = new();

    public static TwinCollection Reported { get; set; } = new();
}