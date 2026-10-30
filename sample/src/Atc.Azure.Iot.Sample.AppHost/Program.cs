var builder = DistributedApplication.CreateBuilder(args);

// An IoT Hub connection string with registry read/write and service connect permissions, e.g. the iothubowner policy:
// dotnet user-secrets set "Parameters:iothub-connection-string" "HostName=<hub>.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=<key>"
var iotHubConnectionString = builder.AddParameter("iothub-connection-string", secret: true);

// The edge device and its module identities are created in the IoT Hub on start.
// The modules connect through a local edgeHub, which applies the routes from the deployment template.
// Its data volume keeps the messages it queues while IoT Hub is unreachable across AppHost restarts.
// The modules connect over MQTT on TCP, so edgeHub doesn't publish port 443 for WebSockets.
var device = builder
    .AddIoTEdgeDevice("edge-device", iotHubConnectionString)
    .WithDeploymentManifest("../IoTEdgeModules/deployment.template.json")
    .WithEdgeHub(hub => hub.WithDataVolume(), httpsPort: null);

// An OPC UA server with simulated nodes for OPC Publisher to read from.
// Options: https://github.com/Azure-Samples/iot-edge-opc-plc
// The telemetry counts against the IoT Hub daily quota, so the data sources only start from the dashboard.
// OPC Publisher waits for the OPC PLC, so starting the OPC PLC starts the data flow.
var opcPlc = builder
    .AddContainer("opc-plc", "iotedge/opc-plc", "2.15.11")
    .WithImageRegistry("mcr.microsoft.com")
    .WithArgs("--pn=50000", "--ph=opc-plc", "--autoaccept", "--ut")
    .WithEndpoint(port: 50000, targetPort: 50000, scheme: "tcp", name: "opcua")
    .WithExplicitStart();

// OPC Publisher and the node manager module share the published nodes file.
// OPC Publisher options: https://github.com/Azure/Industrial-IoT/tree/main/docs/opc-publisher
var publishedNodesDirectory = PreparePublishedNodesDirectory(builder.AppHostDirectory);

builder
    .AddContainer("opcpublisher", "iotedge/opc-publisher", "2.9.18")
    .WithImageRegistry("mcr.microsoft.com")
    .WithBindMount(publishedNodesDirectory, "/app/opc/opcpublisher")
    .WithArgs(
        "--pf=/app/opc/opcpublisher/pn.json",
        "--cf",
        "--aa",
        "--mm=PubSub",
        "--me=Json",
        "--ht=MqttOverTcp",
        "--pki=/home/app/pki",
        "--an=opcpublisher")
    .AsIoTEdgeModule(device)
    .WaitFor(opcPlc);

builder
    .AddProject<Projects.SimulationModule>("simulationmodule")
    .AsIoTEdgeModule(device)
    .WithExplicitStart();

builder
    .AddProject<Projects.OpcPublisherNodeManagerModule>("opcpublishernodemanagermodule")
    .AsIoTEdgeModule(device)
    .WithEnvironment("OpcPublisherNodeConfigurationDirectory", publishedNodesDirectory);

await builder.Build().RunAsync();

static string PreparePublishedNodesDirectory(string appHostDirectory)
{
    var directory = Path.Combine(appHostDirectory, ".opcpublisher");
    var file = Path.Combine(directory, "pn.json");

    Directory.CreateDirectory(directory);
    if (!File.Exists(file))
    {
        File.Copy(Path.Combine(appHostDirectory, "pn.json"), file);
    }

    // OPC Publisher rewrites the file, but runs as a different, non-root user in its container,
    // and a bind mount keeps the permissions from the host. So everyone may read and write the file,
    // and open and write the directory. Windows has no Unix permissions, and Docker Desktop lets containers write there.
    if (!OperatingSystem.IsWindows())
    {
        const UnixFileMode everyoneReadWrite =
            UnixFileMode.UserRead | UnixFileMode.UserWrite |
            UnixFileMode.GroupRead | UnixFileMode.GroupWrite |
            UnixFileMode.OtherRead | UnixFileMode.OtherWrite;
        const UnixFileMode everyoneExecute = UnixFileMode.UserExecute | UnixFileMode.GroupExecute | UnixFileMode.OtherExecute;

        File.SetUnixFileMode(directory, everyoneReadWrite | everyoneExecute);
        File.SetUnixFileMode(file, everyoneReadWrite);
    }

    return directory;
}