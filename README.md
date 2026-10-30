# Introduction

IoT library, which contains common services for Azure IotHub, DeviceProvisioningService (DPS), Iot edge devices, modules, and device twins, a CLI tool, and samples for running IoT Edge modules locally with Aspire and for setting up the Microsoft OPC Publisher module.

> Upgrading from 2.x? See [Upgrading to 3.0](#upgrading-to-30).

# Table of Content

- [Introduction](#introduction)
- [Table of Content](#table-of-content)
- [Atc.Azure.IoT](#atcazureiot)
  - [IIoTHubService](#iiothubservice)
    - [Features](#features)
    - [Methods](#methods)
      - [Device Management](#device-management)
      - [Device Twin Management](#device-twin-management)
      - [Module Management](#module-management)
      - [IoT Edge Logs](#iot-edge-logs)
    - [Errors](#errors)
    - [Usage Example](#usage-example)
    - [Configuring IIoTHubService](#configuring-iiothubservice)
      - [Example Usage](#example-usage)
  - [IoTHubModuleService](#iothubmoduleservice)
    - [Features](#features-1)
    - [Method](#method)
      - [Direct Method Invocation](#direct-method-invocation)
      - [Errors and retries](#errors-and-retries)
    - [Usage Example](#usage-example-1)
  - [IDeviceProvisioningService](#ideviceprovisioningservice)
    - [Features](#features-2)
    - [Methods](#methods-1)
      - [Individual Enrollment Management](#individual-enrollment-management)
      - [Enrollment Errors](#enrollment-errors)
    - [Usage Example](#usage-example-2)
    - [Configuring IDeviceProvisioningService](#configuring-ideviceprovisioningservice)
      - [Example Usage](#example-usage-1)
  - [IDeviceTwinModuleExtractor](#idevicetwinmoduleextractor)
    - [Features](#features-3)
    - [Method](#method-1)
      - [Module Retrieval](#module-retrieval)
    - [Usage Example](#usage-example-3)
  - [IConfigurationContentProvider](#iconfigurationcontentprovider)
    - [Features](#features-4)
    - [Methods](#methods-2)
      - [Configuration Content Retrieval from File](#configuration-content-retrieval-from-file)
      - [Configuration Content Retrieval from Stream](#configuration-content-retrieval-from-stream)
    - [Usage Example](#usage-example-4)
- [CLI](#cli)
  - [Installation](#installation)
  - [Update](#update)
  - [Usage](#usage)
    - [Option --help](#option---help)
- [Atc.Azure.IoTEdge](#atcazureiotedge)
  - [Features](#features-5)
    - [Extensions](#extensions)
    - [Factories](#factories)
    - [Wrappers](#wrappers)
    - [Mocks](#mocks)
    - [TestMocks](#testmocks)
  - [Usage Examples](#usage-examples)
    - [Setup for Different Execution Modes](#setup-for-different-execution-modes)
    - [Testing a Module](#testing-a-module)
  - [Server Certificate Validation](#server-certificate-validation)
    - [Moving existing modules to Validate](#moving-existing-modules-to-validate)
- [Running IoT Edge modules locally](#running-iot-edge-modules-locally)
  - [Choosing an IoT Hub](#choosing-an-iot-hub)
  - [IoT Hub quota](#iot-hub-quota)
  - [Ports](#ports)
  - [Running the sample](#running-the-sample)
- [Sample Project](#sample-project)
  - [Atc.Azure.Iot.Sample.Modules.Contracts](#atcazureiotsamplemodulescontracts)
  - [IoT Edge Modules](#iot-edge-modules)
    - [SimulationModule](#simulationmodule)
    - [OpcPublisherNodeManager](#opcpublishernodemanager)
    - [OpcPublisher](#opcpublisher)
      - [Certificate](#certificate)
      - [Deployment](#deployment)
- [Upgrading to 3.0](#upgrading-to-30)
  - [Changes that need code changes](#changes-that-need-code-changes)
  - [Changes in behaviour](#changes-in-behaviour)
- [Requirements](#requirements)
- [How to contribute](#how-to-contribute)

# Atc.Azure.IoT

[![NuGet Version](https://img.shields.io/nuget/v/atc.azure.iot.svg?logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/atc.azure.iot)

The `Atc.Azure.IoT` package contains various services needed when working with Azure IoTHub, Device Provisioning Service (DPS) and IoT Edge module communication.

## IIoTHubService

The `IIoTHubService` is designed to facilitate communication with Azure IoT Hub, enabling management of devices and modules. This service is integral for applications needing comprehensive control over device registries, twin configurations, and direct method invocations.

### Features

- **Device Management**: Create, retrieve, update, and delete devices in the IoT Hub. It supports both normal and edge-enabled devices.
- **Device Registry Statistics**: Retrieve statistics of the device registry, providing insights into the current state and scale of your IoT infrastructure.
- **Device Twins**: Manage device twins, which represent device properties and states, allowing for detailed configuration and control of each device remotely.
- **Module Management**: For IoT Edge devices, manage individual modules, including creating, updating, and deleting module twins and invoking direct methods on them.
- **Connection Management**: Fetch connection strings for individual devices, facilitating secure communication channels.
- **Configuration and Updates**: Apply configuration changes and updates to devices and modules, ensuring devices operate with the latest settings and firmware.

### Methods

#### Device Management

- `CreateDevice(string deviceId, bool edgeEnabled, CancellationToken cancellationToken)`: Create a new device with the option to enable as an edge device.
- `DeleteDevice(string deviceId, CancellationToken cancellationToken)`: Delete a device from the IoT Hub. Returns `false` if the device doesn't exist.
- `GetDevice(string deviceId, CancellationToken cancellationToken)`: Retrieve a specific device using its device ID, or `null` if it doesn't exist.
- `GetDeviceConnectionString(string deviceId, CancellationToken cancellationToken)`: Get the connection string for a specific device, or `null` if it doesn't exist.
- `ApplyConfigurationContentOnDevice(string deviceId, ConfigurationContent manifestContent, CancellationToken cancellationToken)`: Apply a deployment manifest to a device, as a single-device deployment.
- `GetDeviceRegistryStatistics(CancellationToken cancellationToken)`: Get the number of devices in the IoT Hub, and how many are enabled and disabled.

#### Device Twin Management
- `GetDeviceTwin(string deviceId, CancellationToken cancellationToken)`: Fetch the device twin for a specific device, or `null` if the device doesn't exist.
- `UpdateDeviceTwin(string deviceId, Twin twin, CancellationToken cancellationToken)`: Update the device twin data. The twin's ETag must match the current twin, so read the twin, change it and pass it back.
- `GetDeviceTwins(bool onlyIncludeEdgeDevices, CancellationToken cancellationToken)`: Retrieve all device twins, with an option to filter for only edge devices.

#### Module Management

- `GetModulesOnIotEdgeDevice(string deviceId, CancellationToken cancellationToken)`: Retrieve the module identities on an IoT Edge device, including their keys.
- `GetModuleConnectionString(string deviceId, string moduleId, string? gatewayHostName, CancellationToken cancellationToken)`: Get the connection string for a module, or `null` if it doesn't exist. With `gatewayHostName`, the connection string connects through that edgeHub, as an IoT Edge module does.
- `GetModuleTwin(string deviceId, string moduleId, CancellationToken cancellationToken)`: Retrieve a specific module twin, or `null` if the module doesn't exist.
- `AddModuleToDevice(string deviceId, string moduleId, CancellationToken cancellationToken)`: Add a module identity to a device, and return it with the keys IoT Hub generated.
- `UpdateModule(Module moduleIdentity, CancellationToken cancellationToken)`: Update a module identity, e.g. its authentication, and return the updated module.
- `UpdateDesiredProperties(string deviceId, string moduleId, TwinCollection twinCollection, CancellationToken cancellationToken)`: Merge desired properties into a module twin. Properties that aren't given keep their value, and a property set to `null` is removed.
- `RemoveModuleFromDevice(string deviceId, string moduleId, CancellationToken cancellationToken)`: Remove a module from an IoT device. Returns `false` if the module doesn't exist.
- `AddNewModules(string deviceId, ConfigurationContent manifestContent, CancellationToken cancellationToken)`: Add a module identity to an IoT device for each module in a deployment manifest, except the system modules.
- `RestartModuleOnDevice(string deviceId, string moduleId, CancellationToken cancellationToken)`: Restart a specific module on a device.

#### IoT Edge Logs

- `UploadModuleLogs(string deviceId, UploadModuleLogsRequest request, IoTHubRequestOptions? requestOptions, CancellationToken cancellationToken)`: Ask edgeAgent to upload module logs to a blob storage container.
- `UploadSupportBundle(string deviceId, UploadSupportBundleRequest request, IoTHubRequestOptions? requestOptions, CancellationToken cancellationToken)`: Ask edgeAgent to upload a support bundle to a blob storage container.
- `GetTaskStatus(string deviceId, GetTaskStatusRequest request, IoTHubRequestOptions? requestOptions, CancellationToken cancellationToken)`: Get the status of an upload started by one of the two methods above.

### Errors

`null` (or `false` for the deletes) only means that the device, module or twin doesn't exist. Every other failure is thrown as the IoT Hub SDK's exception, for example `UnauthorizedException` for a wrong key or `ThrottlingException` when IoT Hub throttles.

A cancelled token throws `OperationCanceledException`. The IoT Hub SDK itself reports a cancelled request as an `IotHubException`, so a cancelled call would look like a failed one; the service rethrows it as `OperationCanceledException`, with the SDK's exception as `InnerException`.

The IoT Edge log methods are the exception: they return an IoT Hub error, such as an offline device, as the response's status code. A cancelled token still throws `OperationCanceledException`.

The SDK puts IoT Hub's error in the exception message as JSON inside JSON. `GetErrorMessage()` returns just IoT Hub's message and error code:

```csharp
catch (IotHubException ex)
{
    // e.g. "The specified SAS token has an invalid signature. (401002)"
    logger.LogError(ex, "Could not read the device: {Error}", ex.GetErrorMessage());
}
```

### Usage Example

Below is a sample usage of `IIoTHubService` to create a new device, unless it already exists:

```csharp
var iotHubService = serviceProvider.GetRequiredService<IIoTHubService>();

var device = await iotHubService.GetDevice("myDeviceId", cancellationToken)
             ?? await iotHubService.CreateDevice("myDeviceId", edgeEnabled: true, cancellationToken);

Console.WriteLine($"Device ID: {device.Id}");
```

### Configuring IIoTHubService

The `ConfigureIotHubServices` extension method from `ServiceCollectionExtensions` helps configure and register IoT Hub related services with your application's `IServiceCollection`. This setup is crucial for utilizing the IIoTHubService and related services effectively.

#### Example Usage

Here's how you can configure your services using this extension method:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    var iotHubOptions = new IotHubOptions
    {
        ConnectionString = "your_iot_hub_connection_string"
    };

    // Default: Uses DI's ILoggerFactory if registered, otherwise no logging
    services.ConfigureIotHubServices(iotHubOptions);

    // Explicitly disable logging
    services.ConfigureIotHubServices(iotHubOptions, _ => null);

    // Require logging (throws if ILoggerFactory is not registered)
    services.ConfigureIotHubServices(iotHubOptions, sp => sp.GetRequiredService<ILoggerFactory>());
}
```

The options can also be bound from configuration. `IotHubOptions.SectionName` is `IotHub`, so the connection string can come from `IotHub:ConnectionString` in `appsettings.json` or from the `IotHub__ConnectionString` environment variable:

```csharp
var iotHubOptions = configuration
    .GetRequiredSection(IotHubOptions.SectionName)
    .Get<IotHubOptions>()!;
```

`IotHubOptions.ToString()` redacts the shared access key, so the options can be logged safely.

## IoTHubModuleService

The `IIoTHubModuleService` is tailored for direct interactions with IoT devices and modules through Azure IoT Hub. It enables the invocation of direct methods on devices, providing a bridge for cloud-to-device communication. This service is essential for scenarios requiring immediate action or feedback from IoT devices, such as changing operational modes, updating settings, or retrieving immediate status reports.

### Features

- **Direct Method Invocation**: Send commands directly to a specific module on an IoT device and receive responses, which is crucial for real-time device management and operations.

### Method

#### Direct Method Invocation

- `CallMethod(string deviceId, string moduleId, MethodParameterModel parameters, IoTHubRequestOptions? requestOptions, CancellationToken cancellationToken)`: This method sends a direct command to a specified module on a device. The `MethodParameterModel` holds the method name and its JSON payload, and the operation returns a `MethodResultModel` with the module's status code and its JSON answer.

#### Errors and retries

The module's answer is returned whatever its status code, also when the module reports an error, for example 501 for a method it has no handler for.

A transient IoT Hub error, such as throttling, is retried as `IoTHubRequestOptions` says (by default 3 retries, 2 seconds apart). Any other IoT Hub error, or a transient one that remains after the retries, is thrown, for example `DeviceNotFoundException` with the error code `DeviceNotOnline` when the device is offline. `IoTHubRequestOptions.Timeout` sets how long to wait for the module's answer; `TimeSpan.Zero` uses IoT Hub's default of 30 seconds.

A cancelled token throws `OperationCanceledException` and is not retried. The IoT Hub SDK reports a cancelled request as an `IotHubException`, which becomes the `InnerException`.

### Usage Example

Below is an example demonstrating how to invoke a direct method on an IoT module using `IoTHubModuleService`:

```csharp
var iotHubModuleService = serviceProvider.GetRequiredService<IIoTHubModuleService>();
var parameters = new MethodParameterModel(
    Name: "Restart",
    JsonPayload: "{}"); // Additional parameters for the method could be specified here.

try
{
    var result = await iotHubModuleService.CallMethod("device123", "module456", parameters, cancellationToken: cancellationToken);
    if (result.Status == 200)
    {
        Console.WriteLine("Method executed successfully. Response: " + result.JsonPayload);
    }
    else
    {
        Console.WriteLine("The module answered with status " + result.Status + ": " + result.JsonPayload);
    }
}
catch (DeviceNotFoundException ex) when (ex.Code == ErrorCode.DeviceNotOnline)
{
    Console.WriteLine("The device is offline.");
}
```

## IDeviceProvisioningService

The `IDeviceProvisioningService` is designed to manage device enrollments within Azure's Device Provisioning Service (DPS). It supports operations for TPM and non-TPM devices, streamlining the enrollment and provisioning process. This service is crucial for managing individual device enrollment records and executing enrollment operations, including TPM-based enrollments with specific properties and tags.

### Features

- **Enrollment Management**: Manage the lifecycle of device enrollments in DPS, including creation, retrieval, and deletion.
- **TPM and Non-TPM Support**: Facilitates provisioning for devices with or without TPM (Trusted Platform Module), catering to a wide range of security requirements.
- **Bulk Operations**: Retrieve all individual enrollments, aiding in large-scale management and oversight.

### Methods

#### Individual Enrollment Management

- `GetIndividualEnrollment(string registrationId, CancellationToken cancellationToken)`: Retrieves a specific enrollment using the registration ID, or `null` if it doesn't exist.
- `GetIndividualEnrollments(CancellationToken cancellationToken)`: Fetches all registered individual enrollments.
- `CreateIndividualTpmEnrollment(string endorsementKey, string registrationId, string deviceId, Dictionary<string, string>? tags, Dictionary<string, string>? desiredProperties, CancellationToken cancellationToken)`: Creates or updates a TPM enrollment for an IoT Edge device with specified parameters, and returns it.
- `DeleteIndividualEnrollment(string registrationId, CancellationToken cancellationToken)`: Deletes an individual enrollment based on the registration ID. Returns `false` if it doesn't exist.

#### Enrollment Errors

`null` (or `false` for the delete) only means that the enrollment doesn't exist. Every other failure is thrown as the DPS SDK's exception, usually a `ProvisioningServiceClientHttpException` whose `StatusCode` and `Body` say what DPS rejected.

A cancelled token throws `OperationCanceledException`. The DPS SDK reports a cancelled request as a `ProvisioningServiceClientException`, which becomes the `InnerException`.

### Usage Example

Below is an example demonstrating how to create a TPM-based enrollment using `DeviceProvisioningService`:

```csharp
var deviceProvisioningService = serviceProvider.GetRequiredService<IDeviceProvisioningService>();
var tags = new Dictionary<string, string>
{
    { "location", "datacenter1" }
};
var desiredProperties = new Dictionary<string, string>
{
    { "configVersion", "v1" }
};

try
{
    var enrollment = await deviceProvisioningService.CreateIndividualTpmEnrollment(
        "TPM_ENDORSEMENT_KEY_HERE",
        "registration123",
        "device123",
        tags,
        desiredProperties,
        cancellationToken);

    Console.WriteLine($"Enrollment created: {enrollment.RegistrationId}");
}
catch (ProvisioningServiceClientHttpException ex)
{
    Console.WriteLine($"DPS rejected the enrollment ({ex.StatusCode}): {ex.Body}");
}
```

### Configuring IDeviceProvisioningService

The `ConfigureDeviceProvisioningServices` extension method simplifies the integration of Azure DPS related services into your service collection, facilitating easy management of device enrollments.

#### Example Usage

To set up your services, you can add the following code to your `ConfigureServices` method:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    var deviceProvisioningServiceOptions = new DeviceProvisioningServiceOptions
    {
        ConnectionString = "your_device_provisioning_service_connection_string"
    };

    // Default: Uses DI's ILoggerFactory if registered, otherwise no logging
    services.ConfigureDeviceProvisioningServices(deviceProvisioningServiceOptions);

    // Explicitly disable logging
    services.ConfigureDeviceProvisioningServices(deviceProvisioningServiceOptions, _ => null);

    // Require logging (throws if ILoggerFactory is not registered)
    services.ConfigureDeviceProvisioningServices(deviceProvisioningServiceOptions, sp => sp.GetRequiredService<ILoggerFactory>());
}
```

As with `IotHubOptions`, the options can be bound from the `DeviceProvisioningServiceOptions.SectionName` section (`DeviceProvisioningService`), and `ToString()` redacts the shared access key.

## IDeviceTwinModuleExtractor

The `IDeviceTwinModuleExtractor` is designed to aid in extracting module information from the Edge Agent Twin in Azure IoT solutions. This interface provides targeted functionality for retrieving specific modules from the complex structure of an Edge Agent twin, simplifying the access to module-specific details crucial for operations and monitoring.

### Features

- **Module Extraction**: Extract and retrieve detailed information about a specific module from the Edge Agent twin, enhancing module management and oversight.

### Method

#### Module Retrieval

- `GetModuleFromEdgeAgentTwin(Twin twin, string moduleId)`: Extracts a module from the Edge Agent twin using the module identifier. This method is essential for operations needing detailed information about individual modules managed by the Edge Agent.

### Usage Example

Below is an example demonstrating how to use the `DeviceTwinModuleExtractor` to extract a module's information from an Edge Agent twin:

```csharp
var deviceTwinModuleExtractor = serviceProvider.GetRequiredService<IDeviceTwinModuleExtractor>();
var edgeAgentTwin = // Assume this is fetched or provided somewhere in your application.

var moduleId = "module123";
var module = await deviceTwinModuleExtractor.GetModuleFromEdgeAgentTwin(edgeAgentTwin, moduleId);

if (module != null)
{
    Console.WriteLine($"Module Found: {module.Name}");
}
else
{
    Console.WriteLine("Module not found.");
}
```

## IConfigurationContentProvider

The `IConfigurationContentProvider` is designed to assist in retrieving configuration content from deployment manifests in Azure IoT solutions. This interface provides methods for obtaining configuration content from both file and stream sources, ensuring flexibility and reliability in handling deployment manifests.

### Features

- **Configuration Content Retrieval**: Extract configuration content from deployment manifests, facilitating efficient deployment and management of IoT modules.

### Methods

#### Configuration Content Retrieval from File

- `GetConfigurationContent(FileInfo deploymentManifestFileInfo, CancellationToken cancellationToken)`: Retrieves configuration content from a deployment manifest file. Throws `FileNotFoundException` if the file doesn't exist, and `InvalidDataException` if it is empty or isn't a valid JSON object.

#### Configuration Content Retrieval from Stream

- `GetConfigurationContent(Stream deploymentManifestFileStream, CancellationToken cancellationToken)`: Retrieves configuration content from a deployment manifest stream, and leaves the stream open. Throws `InvalidDataException` if it is empty or isn't a valid JSON object.

### Usage Example

Below is an example demonstrating how to use the `ConfigurationContentProvider` to retrieve configuration content from a deployment manifest file:

```csharp
var configurationContentProvider = serviceProvider.GetRequiredService<IConfigurationContentProvider>();
var deploymentManifestFile = new FileInfo("path/to/deploymentManifest.json");

try
{
    var configurationContent = await configurationContentProvider.GetConfigurationContent(deploymentManifestFile, cancellationToken);
    Console.WriteLine($"Read {configurationContent.ModulesContent.Count} modules from the deployment manifest.");
}
catch (Exception ex) when (ex is FileNotFoundException or InvalidDataException)
{
    Console.WriteLine($"Failed to read the deployment manifest: {ex.Message}");
}
```

# CLI

[![NuGet Version](https://img.shields.io/nuget/v/atc-azure-iot.svg?logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/atc-azure-iot)

The `Atc.Azure.IoT.CLI` tool is available through a cross platform command line application.

## Installation

The tool can be installed as a .NET global tool by the following command

```powershell
dotnet tool install --global atc-azure-iot
```

or by following the instructions [here](https://www.nuget.org/packages/atc-azure-iot/) to install a specific version of the tool.

A successful installation will output something like

```powershell
The tool can be invoked by the following command: atc-azure-iot
Tool 'atc-azure-iot' (version 'x.y.z') was successfully installed.`
```

## Update

The tool can be updated by the following command

```powershell
dotnet tool update --global atc-azure-iot
```

## Usage

Since the tool is published as a .NET Tool, it can be launched from anywhere using any shell or command-line interface by calling **atc-azure-iot**. The help information is displayed when providing the `--help` argument to **atc-azure-iot**

### Option <span style="color:yellow">--help</span>

```powershell
atc-azure-iot --help

USAGE:
    atc-azure-iot.exe [OPTIONS] <COMMAND>

EXAMPLES:
    atc-azure-iot.exe iothub statistics -c <connection-string>
    atc-azure-iot.exe iothub device create -c <connection-string> -d <device-id> --edge-enabled
    atc-azure-iot.exe iothub device get -c <connection-string> -d <device-id>
    atc-azure-iot.exe iothub device delete -c <connection-string> -d <device-id>
    atc-azure-iot.exe dps enrollment individual all -c <connection-string>

OPTIONS:
    -h, --help       Prints help information
    -v, --version    Prints version information

COMMANDS:
    dps       Operations related to Device Provisioning Service
    iothub    Operations related to IoT Hub
```

# Atc.Azure.IoTEdge

[![NuGet Version](https://img.shields.io/nuget/v/atc.azure.iotedge.svg?logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/atc.azure.iotedge)

The `Atc.Azure.IoTEdge` package is specifically designed to enhance Azure IoT Edge module projects, providing critical functionalities for both development and testing environments. This library simplifies the creation, configuration, and testing of IoT Edge modules, enabling developers to efficiently manage device interactions and module behaviors.

## Features

### Extensions
- **HostBuilderContextExtensions**: Determines the execution mode of the IoT Edge module, facilitating different behaviors and configurations based on the deployment environment (Standalone or Regular). In `Standalone` mode a module can run without IoT Hub, on the `MockModuleClientWrapper`.

- **ServiceCollectionExtensions**: Assists in integrating IoT Edge module functionalities by registering dependencies like `ModuleClientWrapper` which abstracts and simplifies interactions with Azure IoT Hub. `AddModuleClientWrapper` takes the transport settings, or a function that creates them from the service provider when the module client is created (for example to get a logger).

### Factories
- **IMethodResponseFactory/MethodResponseFactory**: Supports creating method responses for device modules, allowing customization of serialization settings to fit different data handling requirements.

- **TransportSettingsFactory**: Provides the MQTT and AMQP transport settings a module connects to edgeHub with: `BuildMqttTransportSettings` (TCP, port 8883), `BuildMqttWebSocketTransportSettings` (WebSockets, port 443), `BuildAmqpTransportSettings` (TCP, port 5671) and `BuildAmqpWebSocketTransportSettings` (WebSockets, port 443). Use a WebSocket method where only port 443 is open. Each method takes a `ServerCertificateValidation`; see [Server Certificate Validation](#server-certificate-validation).

### Wrappers
- **IModuleClientWrapper/ModuleClientWrapper**: Wraps the Azure IoT `ModuleClient`, providing a managed way to interact with IoT Hub. It handles operations like sending device-to-cloud messages, responding to direct method calls, and updating device twins.

### Mocks
- **MockModuleClientWrapper**: An `IModuleClientWrapper` that doesn't connect to anything, for running a module in the Standalone environment and for tests. Each mock has its own twin. Tests can read what the module did (`GetSentMessages`, `RetryPolicy`, the twin) and act as IoT Hub towards it (`SimulateMethodCallAsync`, `SimulateInputMessageAsync`, `SimulateDesiredPropertiesUpdateAsync`, `SimulateConnectionStatusChange`); `SetMethodResponse` sets what `InvokeMethodAsync` returns. Twin updates are merged as IoT Hub does: a property set to `null` is removed.

### TestMocks
- **MockTwinProperties**: The desired and reported properties a new `MockModuleClientWrapper` starts with, for example to run a module in the Standalone environment with some desired properties.

## Usage Examples

### Setup for Different Execution Modes
Here is how you can use `Atc.Azure.IoTEdge` in an IoT Edge module, considering the standalone and regular execution modes:

```csharp
using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddLogging(builder =>
        {
            builder.AddSimpleConsole(options => options.TimestampFormat = LoggingConstants.TimestampFormat);
            builder.SetMinimumLevel(LogLevel.Trace);
        });

        if (hostContext.IsStandaloneMode())
        {
            services.AddSingleton<IModuleClientWrapper, MockModuleClientWrapper>();
        }
        else
        {
            // On an IoT Edge device, or emulated by Atc.Aspire.Hosting.Azure.IoTEdge
            services.AddModuleClientWrapper(TransportSettingsFactory.BuildMqttTransportSettings(ServerCertificateValidation.Validate));
        }

        services.AddSingleton<IMethodResponseFactory, MethodResponseFactory>();
        services.AddHostedService<MyEdgeModuleService>();

        services.Configure<HostOptions>(options =>
        {
            options.ShutdownTimeout = TimeSpan.FromSeconds(10);
        });
    })
    .UseConsoleLifetime()
    .Build();

await host.RunAsync();
```

### Testing a Module

`MockModuleClientWrapper` lets a test act as IoT Hub towards a module:

```csharp
var moduleClient = new MockModuleClientWrapper(NullLogger<MockModuleClientWrapper>.Instance);
var service = new MyEdgeModuleService(moduleClient, new MethodResponseFactory());
await service.StartAsync(cancellationToken);

// IoT Hub calls a direct method on the module.
var response = await moduleClient.SimulateMethodCallAsync("SetInterval", """{"seconds":5}""");
response.Status.Should().Be(200);

// The module sent telemetry to its "telemetry" output.
moduleClient.GetSentMessages("telemetry").Should().NotBeEmpty();
```

## Server Certificate Validation

When a module connects to edgeHub (or directly to IoT Hub), the server shows a TLS certificate. `ServerCertificateValidation` says how the module checks it:

| Setting | Checks | If the certificate is wrong | Use it when |
|---|---|---|---|
| `AcceptAny` | nothing | it connects anyway | you can't validate yet; the first step of an upgrade |
| `Audit` | the host name and the chain to a trusted root | it still connects, and logs a warning saying what would have failed | trying out validation in production without risk |
| `Validate` | the host name and the chain to a trusted root | the connection is refused | the goal |

`Validate` is the device SDK's own validation. For edgeHub on an IoT Edge device, the trusted root is the device's edge CA, which the SDK gets from IoT Edge. Revocation isn't checked, unless you turn on `CertificateRevocationCheck` in the transport settings, so it doesn't need internet access.

`Audit` needs a logger for its warnings. Create the transport settings from the service provider to get one:

```csharp
services.AddModuleClientWrapper(serviceProvider =>
    TransportSettingsFactory.BuildMqttTransportSettings(
        ServerCertificateValidation.Audit,
        serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("ModuleClient")));
```

### Moving existing modules to `Validate`

1. Choose `AcceptAny` explicitly. Nothing changes.
2. Switch one device to `Audit`, and let it run through normal operation, including reconnects, for a few days.
3. Read the logs for "Server certificate validation would have failed".
4. No warnings: switch to `Validate`, first on that device, then everywhere. Warnings: fix the cause (see below) and repeat step 3.

Before switching to `Validate`, check that the device clock is right, that the device host name hasn't changed since IoT Edge was set up (edgeHub's certificate must contain `IOTEDGE_GATEWAYHOSTNAME`), that the edge CA hasn't expired, and that no firewall inspects the TLS traffic.

| Warning | Meaning | Fix |
|---|---|---|
| `RemoteCertificateNameMismatch` | the module connected by a name that isn't on the certificate | check the host name / `GatewayHostName` |
| `UntrustedRoot` | signed by a CA the module doesn't trust | a TLS-inspecting firewall, or the edge CA is missing |
| `PartialChain` | an intermediate certificate is missing and couldn't be downloaded | allow the download, or install the intermediate |
| `NotTimeValid` | the certificate isn't valid yet, or has expired | fix the clock, or renew the certificate |

# Running IoT Edge modules locally

IoT Edge modules built on `Atc.Azure.IoTEdge` can run on a developer machine against a real Azure IoT Hub, without an IoT Edge device. This uses the [Aspire](https://aspire.dev) hosting integration [Atc.Aspire.Hosting.Azure.IoTEdge](https://www.nuget.org/packages/Atc.Aspire.Hosting.Azure.IoTEdge) from [atc-aspire](https://github.com/atc-net/atc-aspire).

- The AppHost creates an edge-enabled device and the module identities in the IoT Hub, or reuses them if they exist.
- A local edgeHub container applies the routes from the deployment manifest, as on a real device.
- The modules run as .NET projects or containers, and connect to the local edgeHub with `ModuleClient.CreateFromEnvironmentAsync`, so module code needs no changes.
- A module whose launch profile sets the environment to `Standalone` (see `IsStandaloneMode` above) runs in `Development` instead, so it connects instead of using the `MockModuleClientWrapper`.

```csharp
var iotHub = builder.AddParameter("iothub-connection-string", secret: true);

var device = builder
    .AddIoTEdgeDevice("edge-device", iotHub)
    .WithDeploymentManifest("../IoTEdgeModules/deployment.template.json")
    .WithEdgeHub(hub => hub.WithDataVolume());

builder
    .AddProject<Projects.SimulationModule>("simulationmodule")
    .AsIoTEdgeModule(device);
```

See the [package README](https://github.com/atc-net/atc-aspire/blob/main/src/Atc.Aspire.Hosting.Azure.IoTEdge/README.md) for the API and how it works.

## Choosing an IoT Hub

- Use your own IoT Hub on the **Free (F1)** or **Standard (S1)** tier. The **Basic** tiers don't support device twins, direct methods or IoT Edge, so they can't be used.
- Don't use an IoT Hub that deployed services also use. Their cloud services react to devices, so your local device and its events would reach them.
- The device id defaults to `<name>-<machine name>`, so developers who share an IoT Hub each get their own device.
- Never use the device id of a real IoT Edge device. The AppHost applies a single-device deployment with only the system modules at each start, and a real IoT Edge runtime would remove its modules to follow it; its edgeHub would also compete with the real device for the connection. The device fails to start instead when the `$edgeAgent` twin shows that a real runtime uses the device.

## IoT Hub quota

- F1 allows 8,000 messages a day, metered in 0.5 KB chunks, so roughly 4 MB a day in total. Telemetry, twin updates and direct methods all count.
- S1 allows 400,000 messages a day, metered in 4 KB chunks. Use F1 for quick checks and S1 for longer test runs.
- The daily quota resets at 00:00 UTC.
- To save quota, the sample's data sources (`opc-plc` and `simulationmodule`) use `WithExplicitStart()`, so they don't start automatically with the AppHost. Start them from the dashboard when you need data, and stop them, or the whole AppHost, when you're not testing.
- Don't stop the edgeHub container to save quota. It stores messages and sends them to IoT Hub when it starts again. With `WithDataVolume()`, as in the sample, the stored messages also survive an AppHost restart.

## Ports

The edgeHub container publishes host ports 8883 (MQTT), 5671 (AMQP) and 443 (MQTT and AMQP over WebSockets), because the IoT device SDK always connects on these ports. Modules running on the host connect through `localhost`, so these ports must be free. Port 443 is only needed by modules that connect over WebSockets.

The sample's modules connect over MQTT on TCP, so the sample passes `httpsPort: null` and leaves port 443 free:

```csharp
.WithEdgeHub(hub => hub.WithDataVolume(), httpsPort: null);
```

## Running the sample

The [sample AppHost](sample/src/Atc.Azure.Iot.Sample.AppHost/Program.cs) runs an OPC PLC simulator, Microsoft OPC Publisher, the simulation module and the OPC Publisher node manager module on one emulated device with a local edgeHub.

- [OPC PLC](https://github.com/Azure-Samples/iot-edge-opc-plc) is an OPC UA server with simulated nodes. Its README lists the command-line options the sample passes.
- [OPC Publisher](https://github.com/Azure/Industrial-IoT/tree/main/docs/opc-publisher) reads the nodes listed in `pn.json` from OPC UA servers and sends their values as telemetry. Its documentation lists the command-line options.

1. Install Docker, the [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0) and the [Aspire CLI](https://aspire.dev). The AppHost uses the dashboard and orchestrator bundled with the CLI.
2. Set an IoT Hub connection string with registry read/write and service connect permissions (e.g. the `iothubowner` policy):
   ```powershell
   cd sample/src/Atc.Azure.Iot.Sample.AppHost
   dotnet user-secrets set "Parameters:iothub-connection-string" "HostName=<hub>.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=<key>"
   ```
3. Run the AppHost and open the dashboard link it prints:
   ```powershell
   dotnet run --project sample/src/Atc.Azure.Iot.Sample.AppHost
   ```
4. The device `edge-device-<machine name>` appears in the IoT Hub with its modules.
5. Start `opc-plc` and `simulationmodule` from the dashboard; with `WithExplicitStart()` they don't start on their own. OPC Publisher waits for `opc-plc`. The routes in the [deployment template](sample/src/IoTEdgeModules/deployment.template.json) send their telemetry to `$upstream`. Watch it with e.g. `az iot hub monitor-events -n <hub> -d edge-device-<machine name>`.
6. Use the module commands in the dashboard to call direct methods, e.g. `GetEndpoints` with payload `{ "pageIndex": 0 }` on `opcpublishernodemanagermodule`, or to change desired properties.
7. Stop the data sources, or the AppHost, when you're done.

# Sample Project

## Atc.Azure.Iot.Sample.Modules.Contracts

This sample library demonstrates how to share contracts between IoT Edge modules and, for example, a WebAPI, which uses the [IoTHubModuleService](/src/Atc.Azure.IoT/Services/IoTHub/IoTHubModuleService.cs) to invoke direct methods. By leveraging a shared common library, it is ensured that all components are properly linked at compile time, maintaining integrity across all references. The modules located in the [sample modules](/sample/src/IoTEdgeModules/modules/) folder all make use of this library, showcasing its practical application in real scenarios.

## IoT Edge Modules

In the [sample modules](/sample/src/IoTEdgeModules/modules/) folder, a few IoT Edge modules are included.

### SimulationModule

The module relies on Quartz.Net for scheduling tasks. The module features a [`PulsJob`](/sample/src/IoTEdgeModules/modules/simulationmodule/Jobs/PulsJob.cs), which is responsible for emitting temperature telemetry at 5-second intervals. This telemetry simulates temperature readings by following a sinusoidal curve.

### OpcPublisherNodeManager

This module manages the published nodes file (`pn.json`) of the Microsoft OpcPublisher module through direct methods. The file is read from `opc/opcpublisher` under the working directory, or from the directory in the `OpcPublisherNodeConfigurationDirectory` environment variable.

Direct Methods supported:

- GetEndpoints
- GetEndpointWithNodes
- GetEndpointsWithEmptyOpcNodesList
- AddEndpoint
- AddNodeToEndpoint
- AddNodesToEndpoint
- UpdateNodeOnEndpoint
- UpdateNodesOnEndpoint
- RemoveNodeFromEndpoint
- RemoveNodesFromEndpoint
- RemoveAllNodesFromEndpoint
- RemoveEndpoint

### OpcPublisher

For guidance on how to configure the OpcPublisher from Microsoft, refer to this [example](/sample/src/IoTEdgeModules/deployment.template.json) located in the sample folder. This file provides a detailed template for setting up your deployment configuration.

The latest release of the OpcPublisher can be accessed [here](https://github.com/Azure/Industrial-IoT/releases).

#### Certificate

If the OpcPublisher container is not provided with a certificate, it will generate a new self-signed certificate upon each startup. This approach may not be suitable in scenarios where a consuming party needs to trust the certificate for OpcPublisher to establish a connection with an OPC-UA server.

To address this, a certificate can be specifically created for the OpcPublisher module and included in the module's `createOptions`/`Cmd-parameters` as illustrated in the deployment template:

```json
    "--ApplicationName=opcpublisher",
    "--ApplicationCertificateSubjectName=O=myorganization,CN=opcpublisher"
```

In the sample folder, there are two methods provided for generating this certificate. You can either use the [Generate OpcPublisher Certificate.bat](/sample/src/IoTEdgeModules/Generate%20OpcPublisher%20Certificate.bat) script or the C# program [Atc.Azure.Iot.Certificate.Sample](sample/src/Atc.Azure.Iot.Certificate.Sample/Program.cs).

Make sure to correctly set the organisation and CN name for the certificate generation to align with those specified in the deployment manifest.

#### Deployment

Once you are ready to deploy the OpcPublisher module, there are several requirements that need to be met on your IoTEdge device.

1. Create the following folders on your IoTEdge device and ensure the Binds in the [deployment manifest](/sample/src/IoTEdgeModules/deployment.template.json) match these folders. This setup ensures that the Container Mounts are correctly mapped to the file system on your IoTEdge device.
 > ```json
   > "/opc/opcpublisher"
   > "/opc/pki/own/certs"
   > "/opc/pki/own/private"
   > "/opc/pki/trusted/certs"
 > ```
2. Ensure the iotedge user has 777 chmod permissions on the opc folder
3. Copy the generated certificate files to the proper folders on your IoTEdge device
 > ```json
   > Empty file "pn.json" to "/opc/opcpublisher"
   > "opcpublisher.der" to "/opc/pki/own/certs"
   > "opcpublisher.pfx" to "/opc/pki/own/private"
   > "opcpublisher.der" to "/opc/pki/trusted/certs"
 > ```
4. Change ownership and ensure 777 permissions on the files
 > ```json
   > iotedge on "/opc/opcpublisher/pn.json"
   > aziotcs on "/opc/pki/own/certs/opcpublisher.der"
   > aziotks on "/opc/pki/own/private/opcpublisher.pfx"
   > aziotcs on "/opc/pki/trusted/certs/opcpublisher.der"
 > ```

# Upgrading to 3.0

3.0 changes how failures and cancellation are reported, how IoT Edge modules connect, and removes the Docker device emulator. Go through this list when you upgrade from 2.x.

## Changes that need code changes

**Results instead of tuples.** These methods return the result and throw on failure:

| Method | 2.x | 3.0 |
|---|---|---|
| `IIoTHubService.CreateDevice` | `(bool Succeeded, Device? Device)` | `Device` |
| `IIoTHubService.UpdateDeviceTwin` | `(bool Succeeded, Twin? UpdatedTwin)` | `Twin` |
| `IIoTHubService.AddModuleToDevice`, `UpdateModule` | `(bool Succeeded, Module? Module)` | `Module` |
| `IIoTHubService.UpdateDesiredProperties` | `Twin?` | `Twin` |
| `IIoTHubService.GetDeviceRegistryStatistics` | `RegistryStatistics?` | `RegistryStatistics` |
| `IIoTHubService.ApplyConfigurationContentOnDevice`, `AddNewModules` | `(bool Succeeded, string? ErrorMessage)` | `Task` |
| `IIoTHubService.GetModulesOnIotEdgeDevice` | `IEnumerable<Module>` | `IReadOnlyCollection<Module>` |
| `IDeviceProvisioningService.CreateIndividualTpmEnrollment` | `(IndividualEnrollment? Enrollment, string? ErrorMessage)` | `IndividualEnrollment` |
| `IDeviceProvisioningService.GetIndividualEnrollments` | `IEnumerable<IndividualEnrollment>` | `IReadOnlyCollection<IndividualEnrollment>` |
| `IConfigurationContentProvider.GetConfigurationContent` | `(ConfigurationContent? ConfigurationContent, string? ErrorMessage)` | `ConfigurationContent` |

```csharp
// 2.x
var (succeeded, errorMessage) = await iotHubService.ApplyConfigurationContentOnDevice(deviceId, content, cancellationToken);
if (!succeeded)
{
    logger.LogError(errorMessage);
}

// 3.0
try
{
    await iotHubService.ApplyConfigurationContentOnDevice(deviceId, content, cancellationToken);
}
catch (IotHubException ex)
{
    logger.LogError(ex, "Could not apply the deployment");
}
```

`ErrorResponse`, which held a parsed DPS error, is removed; the DPS exception's `StatusCode` and `Body` hold DPS's answer.

**Transport settings.** Every `TransportSettingsFactory` method takes a `ServerCertificateValidation` (see [Server Certificate Validation](#server-certificate-validation)), and `BuildMqttTransportSettings` connects over TCP (port 8883) instead of WebSockets (port 443). To keep the 2.x behaviour exactly:

```csharp
// 2.x
services.AddModuleClientWrapper(TransportSettingsFactory.BuildMqttTransportSettings());

// 3.0, the same behaviour
services.AddModuleClientWrapper(
    TransportSettingsFactory.BuildMqttWebSocketTransportSettings(ServerCertificateValidation.AcceptAny));
```

Then move to TCP and to `Validate` one device at a time.

**The Docker device emulator is removed:** the `Atc.Azure.IoTEdge.DeviceEmulator` package, `IotEdgeExecutionMode.Emulator` and `IsEmulatorMode()`. Use the Standalone environment with `MockModuleClientWrapper` to run a module without IoT Hub.

## Changes in behaviour

**Failures throw.** In 2.x, `IoTHubService`, `DeviceProvisioningService` and `ConfigurationContentProvider` logged a failure and returned `null`, `false` or an empty list, so a wrong key or a throttled hub looked like a missing device. In 3.0, `null` (or `false` for `DeleteDevice`, `RemoveModuleFromDevice` and `DeleteIndividualEnrollment`) only means that it doesn't exist, and every other failure throws the SDK's exception. The code compiles unchanged, so check code that treats `null` or `false` as "failed": a delete that returns `false` now means "already gone". `GetDeviceTwins` no longer returns a partial list when a page fails.

**Cancellation throws `OperationCanceledException`.** Both SDKs report a cancelled request as their own exception (`IotHubException`, `ProvisioningServiceClientException`). In 3.0 a cancelled token throws `OperationCanceledException`, with the SDK's exception as `InnerException`.

**Unchanged on purpose:** `UploadModuleLogs`, `UploadSupportBundle` and `GetTaskStatus` still return an IoT Hub error, such as an offline device, as the response's status code. A cancelled token throws.

**Other changes you may notice:**

- `UpdateDesiredProperties` sends one patch with only the desired properties (any twin version), instead of reading and resending the whole twin.
- `EdgeAgentDesiredProperties.SystemModuleVersion` defaults to 1.6. It is only used when no system modules are given.
- Building a deployment throws for a module whose `RestartPolicy` or `Status` is `Unknown`, instead of writing `"unknown"`. `RestartPolicy` gains `OnFailure` and `OnUnhealthy`; use `RestartPolicyExtensions.FromManifestValue("on-failure")` to read them.
- `IsStandaloneMode()` ignores the case of the environment name, as `IsEnvironment` does.
- `GetConfigurationContent(Stream)` leaves the stream open.
- `MethodResponseFactory` writes compact JSON (no indentation) by default, because a direct method response is limited to 128 KB. Pass your own `JsonSerializerOptions` to change it.
- `MockModuleClientWrapper`: each mock has its own twin, which starts with `MockTwinProperties` (changing `MockTwinProperties` later doesn't change an existing mock); `UpdateReportedPropertiesAsync` merges the patch instead of replacing the reported properties, and no longer calls the desired property callback.
- `ConfigureIotHubServices` also registers `IConfigurationContentProvider`.
- The IoT Hub log event `Failure` (10000) and the DPS failure events are removed, because the caller gets the exception; the connection string events moved to 10110/10111.

# Requirements

* [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
* Docker and the [Aspire CLI](https://aspire.dev), for the Aspire sample

# How to contribute

[Contribution Guidelines](https://atc-net.github.io/introduction/about-atc#how-to-contribute)

[Coding Guidelines](https://atc-net.github.io/introduction/about-atc#coding-guidelines)