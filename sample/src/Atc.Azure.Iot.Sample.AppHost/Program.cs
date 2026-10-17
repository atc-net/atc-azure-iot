var builder = DistributedApplication.CreateBuilder(args);

// An IoT Hub connection string with registry read/write and service connect permissions, e.g. the iothubowner policy:
// dotnet user-secrets set "Parameters:iothub-connection-string" "HostName=<hub>.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=<key>"
var iotHubConnectionString = builder.AddParameter("iothub-connection-string", secret: true);

// The edge device and its module identities are created in the IoT Hub on start.
// The modules connect through a local edgeHub, which applies the routes from the deployment template.
var device = builder
    .AddIoTEdgeDevice("edge-device", iotHubConnectionString)
    .WithDeploymentManifest("../IoTEdgeModules/deployment.template.json")
    .WithEdgeHub();

// An OPC UA server with simulated nodes for OPC Publisher to read from.
// The telemetry counts against the IoT Hub daily quota, so the data sources only start from the dashboard.
// OPC Publisher waits for the OPC PLC, so starting the OPC PLC starts the data flow.
var opcPlc = builder
    .AddContainer("opc-plc", "iotedge/opc-plc", "2.15.11")
    .WithImageRegistry("mcr.microsoft.com")
    .WithArgs("--pn=50000", "--ph=opc-plc", "--autoaccept", "--ut")
    .WithEndpoint(port: 50000, targetPort: 50000, scheme: "tcp", name: "opcua")
    .WithExplicitStart();

// OPC Publisher and the node manager module share the published nodes file.
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

    if (!OperatingSystem.IsWindows())
    {
        // OPC Publisher runs as a non-root user in its container and rewrites the file.
        File.SetUnixFileMode(directory, (UnixFileMode)0b111_111_111);
        File.SetUnixFileMode(file, (UnixFileMode)0b110_110_110);
    }

    return directory;
}