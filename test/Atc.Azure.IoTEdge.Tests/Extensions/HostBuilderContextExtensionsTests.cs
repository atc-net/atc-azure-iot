namespace Atc.Azure.IoTEdge.Tests.Extensions;

public sealed class HostBuilderContextExtensionsTests
{
    [Theory]
    [InlineData("Standalone")]
    [InlineData("standalone")]
    public void IsStandaloneMode_EnvironmentIsStandaloneInAnyCase_ReturnsTrue(string environmentName)
        => CreateContext(environmentName).IsStandaloneMode().Should().BeTrue();

    [Theory]
    [InlineData("Production")]
    [InlineData("Development")]
    [InlineData("Regular")]
    public void IsStandaloneMode_OtherEnvironment_ReturnsFalse(string environmentName)
        => CreateContext(environmentName).IsStandaloneMode().Should().BeFalse();

    private static HostBuilderContext CreateContext(string environmentName)
    {
        var hostEnvironment = Substitute.For<IHostEnvironment>();
        hostEnvironment.EnvironmentName.Returns(environmentName);

        return new HostBuilderContext(new Dictionary<object, object>())
        {
            HostingEnvironment = hostEnvironment,
        };
    }
}