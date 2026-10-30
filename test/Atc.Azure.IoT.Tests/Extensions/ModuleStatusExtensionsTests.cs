namespace Atc.Azure.IoT.Tests.Extensions;

public sealed class ModuleStatusExtensionsTests
{
    [Theory]
    [InlineData(ModuleStatus.Running, "running")]
    [InlineData(ModuleStatus.Stopped, "stopped")]
    public void ToManifestValue_ReturnsTheValueIoTEdgeUses(
        ModuleStatus moduleStatus,
        string expected)
        => moduleStatus.ToManifestValue().Should().Be(expected);

    [Fact]
    public void ToManifestValue_Unknown_ThrowsArgumentOutOfRangeException()
    {
        // Act
        var act = () => ModuleStatus.Unknown.ToManifestValue();

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>().WithMessage("A deployment manifest needs a module status*");
    }

    [Theory]
    [InlineData("running", ModuleStatus.Running)]
    [InlineData("stopped", ModuleStatus.Stopped)]
    [InlineData("Running", ModuleStatus.Running)]
    public void FromManifestValue_ValueIoTEdgeKnows_ReturnsTheModuleStatus(
        string value,
        ModuleStatus expected)
        => ModuleStatusExtensions.FromManifestValue(value).Should().Be(expected);

    [Theory]
    [InlineData("unknown")]
    [InlineData("paused")]
    public void FromManifestValue_ValueIoTEdgeDoesNotKnow_ThrowsArgumentException(string value)
    {
        // Act
        var act = () => ModuleStatusExtensions.FromManifestValue(value);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage($"'{value}' is not a module status*");
    }
}