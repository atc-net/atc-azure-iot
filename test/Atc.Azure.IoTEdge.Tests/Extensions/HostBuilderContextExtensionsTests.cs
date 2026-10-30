namespace Atc.Azure.IoTEdge.Tests.Extensions;

public sealed class HostBuilderContextExtensionsTests
{
    [Fact]
    public void IsStandaloneMode_EnvironmentIsStandalone_ReturnsTrue()
        => CreateContext("Standalone").IsStandaloneMode().Should().BeTrue();

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