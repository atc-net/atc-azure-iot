namespace Atc.Azure.IoT.Tests.Extensions;

public sealed class RestartPolicyExtensionsTests
{
    [Theory]
    [InlineData(RestartPolicy.Always, "always")]
    [InlineData(RestartPolicy.Never, "never")]
    [InlineData(RestartPolicy.OnFailure, "on-failure")]
    [InlineData(RestartPolicy.OnUnhealthy, "on-unhealthy")]
    public void ToManifestValue_ReturnsTheValueIoTEdgeUses(
        RestartPolicy restartPolicy,
        string expected)
        => restartPolicy.ToManifestValue().Should().Be(expected);

    [Fact]
    public void ToManifestValue_Unknown_ThrowsArgumentOutOfRangeException()
    {
        // Act
        var act = () => RestartPolicy.Unknown.ToManifestValue();

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>().WithMessage("A deployment manifest needs a restart policy*");
    }

    [Theory]
    [InlineData("always", RestartPolicy.Always)]
    [InlineData("never", RestartPolicy.Never)]
    [InlineData("on-failure", RestartPolicy.OnFailure)]
    [InlineData("on-unhealthy", RestartPolicy.OnUnhealthy)]
    [InlineData("On-Failure", RestartPolicy.OnFailure)]
    public void FromManifestValue_ValueIoTEdgeKnows_ReturnsTheRestartPolicy(
        string value,
        RestartPolicy expected)
        => RestartPolicyExtensions.FromManifestValue(value).Should().Be(expected);

    [Theory]
    [InlineData("unknown")]
    [InlineData("onfailure")]
    [InlineData("")]
    public void FromManifestValue_ValueIoTEdgeDoesNotKnow_ThrowsArgumentException(string value)
    {
        // Act
        var act = () => RestartPolicyExtensions.FromManifestValue(value);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage($"'{value}' is not a restart policy*");
    }
}