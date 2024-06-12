namespace Atc.Azure.IoT.Tests.Models;

public sealed class SerializationTests
{
    [Fact]
    public void CanSerializeAndDeserialize_IotDevice()
    {
        // Arrange
        const string json = """
                            {
                              "deviceId": "test",
                              "etag": "AAAAAAAAAAE=",
                              "deviceEtag": "MTQ1NzQ5NDU4",
                              "status": "enabled",
                              "statusUpdateTime": "0001-01-01T00:00:00Z",
                              "connectionState": "Disconnected",
                              "lastActivityTime": "0001-01-01T00:00:00Z",
                              "cloudToDeviceMessageCount": 0,
                              "authenticationType": "sas",
                              "x509Thumbprint": {
                                "primaryThumbprint": null,
                                "secondaryThumbprint": null
                              },
                              "modelId": "",
                              "version": 2,
                              "properties": {
                                "desired": {
                                  "$metadata": {
                                    "$lastUpdated": "2024-05-28T11:04:38.4422737Z"
                                  },
                                  "$version": 1
                                },
                                "reported": {
                                  "$metadata": {
                                    "$lastUpdated": "2024-05-28T11:04:38.4422737Z"
                                  },
                                  "$version": 1
                                }
                              },
                              "capabilities": {
                                "iotEdge": false
                              }
                            }
                            """;


        var expected = new IotDevice
        {
            DeviceId = "test",
            Etag = "AAAAAAAAAAE=",
            DeviceEtag = "MTQ1NzQ5NDU4",
            Status = IotDeviceStatus.Enabled,
            StatusUpdateTime = null,
            ConnectionState = IotDeviceConnectionState.Disconnected,
            LastActivityTime = null,
            AuthenticationType = IotDeviceAuthenticationType.Sas,
            Capabilities = new IoTDeviceCapabilities
            {
                IotEdge = false,
            },
        };

        // Act
        var actual = JsonSerializer.Deserialize<IotDevice>(json, JsonSerializerOptionsFactory.Create(new JsonSerializerFactorySettings
        {
            UseConverterDatetimeOffsetMinToNull = true,
        }));

        // Assert
        actual
            .Should()
            .BeEquivalentTo(expected);
    }
}