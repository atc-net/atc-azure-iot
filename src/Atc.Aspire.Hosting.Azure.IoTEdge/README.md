# Atc.Aspire.Hosting.Azure.IoTEdge

An [Aspire](https://aspire.dev) hosting integration for running Azure IoT Edge modules locally against a real Azure IoT Hub.

On start, the AppHost creates an edge-enabled device and a module identity for every module in the IoT Hub, or reuses them if they already exist. The modules then connect in one of two ways:

- **Through a local edgeHub** (`WithEdgeHub`): an `azureiotedge-hub:1.6` container runs next to the modules, like on a real IoT Edge device. Routes from your deployment manifest apply, so module outputs, inputs and `$upstream` work as in production.
- **Directly to IoT Hub** (no `WithEdgeHub`): each module gets its own connection string to IoT Hub. Telemetry, direct methods and twins work, but routes and module inputs do not.

Modules can be .NET projects running on the host or containers. They receive the `EdgeHubConnectionString` environment variable, which `ModuleClient.CreateFromEnvironmentAsync` reads, so module code needs no changes.

## Usage

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// An IoT Hub connection string with registry read/write and service connect permissions, e.g. iothubowner.
var iotHub = builder.AddParameter("iothub-connection-string", secret: true);

var device = builder
    .AddIoTEdgeDevice("edge-device", iotHub)
    .WithDeploymentManifest("../IoTEdgeModules/deployment.template.json")
    .WithEdgeHub();

builder
    .AddProject<Projects.SimulationModule>("simulationmodule")
    .AsIoTEdgeModule(device);

builder
    .AddContainer("opcpublisher", "iotedge/opc-publisher", "2.9.18")
    .WithImageRegistry("mcr.microsoft.com")
    .AsIoTEdgeModule(device);

await builder.Build().RunAsync();
```

Set the connection string with user secrets:

```powershell
dotnet user-secrets set "Parameters:iothub-connection-string" "HostName=<hub>.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=<key>"
```

## API

| Method | Description |
| --- | --- |
| `AddIoTEdgeDevice(name, connectionString, deviceId?)` | Adds the emulated device. The device id defaults to `<name>-<machine name>`, so developers sharing an IoT Hub get their own device. |
| `WithDeviceId(deviceId)` | Sets the device id in IoT Hub. |
| `WithDeploymentManifest(path)` | Applies the edgeHub routes and the module twin desired properties from a deployment manifest or template. |
| `WithEdgeHub(configure?, mqttPort, amqpPort, httpsPort)` | Runs a local edgeHub container with generated test certificates. |
| `WithUpstreamProtocol(protocol)` | Sets the protocol edgeHub uses to reach IoT Hub (`Amqp`, `AmqpWs`, `Mqtt`, `MqttWs`). |
| `AsIoTEdgeModule(device, moduleId?)` | Runs a project or container as a module. The module id defaults to the resource name. |

## Dashboard commands

- **Device**: delete the device and its module identities from IoT Hub.
- **Module**: invoke a direct method, update the twin's desired properties, and show the module twin.

## How the edgeHub mode works

- edgeHub runs with `configSource=local`, so it takes routes from environment variables instead of from edgeAgent. It authenticates to IoT Hub with the `$edgeHub` module identity of the device.
- A test CA chain (device CA, edgeHub CA, server certificate) is generated per device in `atc-aspire-iotedge` under the temp folder. Its SANs cover the container name, `localhost` and `host.docker.internal`. Modules get `EdgeModuleCACertificateFile` pointing at the device CA. On macOS this is skipped for modules running on the host, so they must accept the edgeHub certificate themselves (the `TransportSettingsFactory` in `Atc.Azure.IoTEdge` does).
- edgeHub publishes ports 8883 (MQTT), 5671 (AMQP) and 443 (WebSockets) on the host, because the IoT device SDK always connects on these fixed ports. Modules running on the host connect to `localhost`, and container modules connect to the edgeHub container by name. If port 443 is taken, pass `httpsPort: null` and use MQTT or AMQP over TCP in your modules.
- Modules wait until edgeHub accepts TLS connections before they start.

## Limitations

- Cloud-to-device messages are not supported for modules, by IoT Hub itself.
- edgeAgent is not emulated, so the `$edgeAgent` direct methods (restart module, get logs) do not work, and the module images in the deployment manifest are not used. Aspire decides what runs.
- With direct connections (no edgeHub), `SendEventAsync` to an output goes straight to IoT Hub, and module inputs receive nothing.