namespace Atc.Azure.IoT.Tests.Models;

public sealed class SerializationTests
{
    [Fact]
    public void DeserializeAndMapIotDevice()
    {
        // Arrange
        const string json = """
                            {
                              "deviceId": "Connect-Edge-Tst",
                              "etag": "AAAAAAAAAAc=",
                              "deviceEtag": "Njc2NzA5MjU3",
                              "status": "enabled",
                              "statusUpdateTime": "0001-01-01T00:00:00Z",
                              "connectionState": "Disconnected",
                              "lastActivityTime": "2023-08-24T12:39:49.5076305Z",
                              "cloudToDeviceMessageCount": 0,
                              "authenticationType": "sas",
                              "x509Thumbprint": {
                                "primaryThumbprint": null,
                                "secondaryThumbprint": null
                              },
                              "modelId": "",
                              "version": 8,
                              "tags": {
                                "test": "test"
                              },
                              "properties": {
                                "desired": {
                                  "$metadata": {
                                    "$lastUpdated": "2023-10-12T13:38:52.3108419Z",
                                    "$lastUpdatedVersion": 4
                                  },
                                  "$version": 4
                                },
                                "reported": {
                                  "$metadata": {
                                    "$lastUpdated": "2023-08-17T08:24:40.736671Z"
                                  },
                                  "$version": 1
                                }
                              },
                              "capabilities": {
                                "iotEdge": true
                              },
                              "deviceScope": "ms-azure-iot-edge://Connect-Edge-Tst-638278574807366710"
                            }
                            """;

        var expected = new IotDevice
        {
            DeviceId = "Connect-Edge-Tst",
            Etag = "Njc2NzA5MjU3",
            Status = IotDeviceStatus.Enabled,
            StatusUpdateTime = null,
            ConnectionState = IotDeviceConnectionState.Disconnected,
            LastActivityTime = DateTimeOffset.Parse("2023-08-24T12:39:49.5076305Z", GlobalizationConstants.EnglishCultureInfo),
            AuthenticationMechanism = new IotDeviceAuthenticationMechanism
            {
                AuthenticationType = IotDeviceAuthenticationType.Sas,
            },
            IotEdge = true,
        };

        // Act
        var deserializeActual = JsonSerializer.Deserialize<SerializableDevice>(
            json,
            JsonSerializerOptionsFactory.Create(new JsonSerializerFactorySettings
            {
                UseConverterDatetimeOffsetMinToNull = true,
            }));

        var actual = deserializeActual!.ToIotDevice(json);

        // Assert
        actual
            .Should()
            .BeEquivalentTo(expected);
    }
}