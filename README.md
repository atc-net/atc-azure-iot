# Introduction

IoT library, which contains common services for Azure IotHub, DeviceProvisioningService (DPS), Iot edge devices, modules, and device twins, and examples for setting up Microsoft OPC publisher module and related projects.

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
    - [Usage Example](#usage-example-1)
  - [IDeviceProvisioningService](#ideviceprovisioningservice)
    - [Features](#features-2)
    - [Methods](#methods-1)
      - [Individual Enrollment Management](#individual-enrollment-management)
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
- [Sample Project](#sample-project)
  - [Atc.Azure.Iot.Sample.Modules.Contracts](#atcazureiotsamplemodulescontracts)
  - [IoT Edge Modules](#iot-edge-modules)
    - [SimulationModule](#simulationmodule)
    - [OpcPublisherNodeManager](#opcpublishernodemanager)
    - [OpcPublisher](#opcpublisher)
      - [Certificate](#certificate)
      - [Deployment](#deployment)
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

- `CallMethod(string deviceId, string moduleId, MethodParameterModel parameters, CancellationToken cancellationToken)`: This method sends a direct command to a specified module on a device. The `MethodParameterModel` allows for detailed specification of the command, and the operation returns a `MethodResultModel` that includes the status of the call and any resultant data in JSON format.

### Usage Example

Below is an example demonstrating how to invoke a direct method on an IoT module using `IoTHubModuleService`:

```csharp
var iotHubModuleService = serviceProvider.GetRequiredService<IIoTHubModuleService>();
var parameters = new MethodParameterModel
{
    MethodName = "Restart",
    Payload = "{}" // Additional parameters for the method could be specified here.
};

var result = await iotHubModuleService.CallMethod("device123", "module456", parameters, CancellationToken.None);
if (result.Status == 200)
{
    Console.WriteLine("Method executed successfully. Response: " + result.JsonPayload);
}
else
{
    Console.WriteLine("Method execution failed. Status: " + result.Status);
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

- `GetIndividualEnrollment(string registrationId, CancellationToken cancellationToken)`: Retrieves a specific enrollment using the registration ID.
- `GetIndividualEnrollments(CancellationToken cancellationToken)`: Fetches all registered individual enrollments.
- `CreateIndividualTpmEnrollment(string endorsementKey, string registrationId, string deviceId, Dictionary<string, string>? tags, Dictionary<string, string>? desiredProperties, CancellationToken cancellationToken)`: Creates or updates a TPM enrollment with specified parameters.
- `DeleteIndividualEnrollment(string registrationId, CancellationToken cancellationToken)`: Deletes an individual enrollment based on the registration ID.

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

var result = await deviceProvisioningService.CreateIndividualTpmEnrollment(
    "TPM_ENDORSEMENT_KEY_HERE",
    "registration123",
    "device123",
    tags,
    desiredProperties,
    CancellationToken.None
);

if (result.Enrollment != null)
{
    Console.WriteLine($"Enrollment created: {result.Enrollment.RegistrationId}");
}
else
{
    Console.WriteLine($"Failed to create enrollment: {result.ErrorMessage}");
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

- `GetConfigurationContent(FileInfo deploymentManifestFileInfo, CancellationToken cancellationToken)`: Retrieves configuration content from a deployment manifest file.

#### Configuration Content Retrieval from Stream

- `GetConfigurationContent(Stream deploymentManifestFileStream, CancellationToken cancellationToken)`: Retrieves configuration content from a deployment manifest stream.

### Usage Example

Below is an example demonstrating how to use the `ConfigurationContentProvider` to retrieve configuration content from a deployment manifest file:

```csharp
var configurationContentProvider = serviceProvider.GetRequiredService<IConfigurationContentProvider>();
var deploymentManifestFile = new FileInfo("path/to/deploymentManifest.json");
var cancellationToken = new CancellationToken();

var (configurationContent, errorMessage) = await configurationContentProvider.GetConfigurationContent(deploymentManifestFile, cancellationToken);

if (configurationContent != null)
{
    Console.WriteLine("Configuration content retrieved successfully.");
}
else
{
    Console.WriteLine($"Failed to retrieve configuration content: {errorMessage}");
}

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
- **HostBuilderContextExtensions**: Determines the execution mode of the IoT Edge module, facilitating different behaviors and configurations based on the deployment environment (Standalone or Regular).

- **ServiceCollectionExtensions**: Assists in integrating IoT Edge module functionalities by registering dependencies like `ModuleClientWrapper` which abstracts and simplifies interactions with Azure IoT Hub.

### Factories
- **IMethodResponseFactory/MethodResponseFactory**: Supports creating method responses for device modules, allowing customization of serialization settings to fit different data handling requirements.

- **TransportSettingsFactory**: Provides AMQP and MQTT transport settings, essential for connecting IoT modules to Azure IoT Hub.

### Wrappers
- **IModuleClientWrapper/ModuleClientWrapper**: Wraps the Azure IoT `ModuleClient`, providing a managed way to interact with IoT Hub. It handles operations like sending device-to-cloud messages, responding to direct method calls, and updating device twins.

### Mocks
- **IMockModuleClientWrapper/MockModuleClientWrapper**: Offers a mock implementation of `IModuleClientWrapper`, useful for module testing without requiring an actual IoT Hub connection. This enables comprehensive unit and integration testing by simulating various IoT operations.

### TestMocks
- **MockTwinProperties**: Allows testing modules with predefined desired and reported properties, simplifying state management and behavior testing without interacting with a live IoT Hub.

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
            // Regular operational mode
            services.AddModuleClientWrapper(TransportSettingsFactory.BuildMqttTransportSettings());
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

# Sample Project

## Atc.Azure.Iot.Sample.Modules.Contracts

This sample library demonstrates how to share contracts between IoT Edge modules and, for example, a WebAPI, which uses the [IoTHubModuleService](/src/Atc.Azure.IoT/Services/IoTHub/IoTHubModuleService.cs) to invoke direct methods. By leveraging a shared common library, it is ensured that all components are properly linked at compile time, maintaining integrity across all references. The modules located in the [sample modules](/sample/src/IoTEdgeModules/modules/) folder all make use of this library, showcasing its practical application in real scenarios.

## IoT Edge Modules

In the [sample modules](/sample/src/IoTEdgeModules/modules/) folder, a few IoT Edge modules are included.

### SimulationModule

The module relies on Quartz.Net for scheduling tasks. The module features a [`PulsJob`](/sample/src/IoTEdgeModules/modules/simulationmodule/Jobs/PulsJob.cs), which is responsible for emitting temperature telemetry at 5-second intervals. This telemetry simulates temperature readings by following a sinusoidal curve.

### OpcPublisherNodeManager

This module is designed to manage the configuration file for the Microsoft OpcPublisher module, providing methods to manipulate it.

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

# Requirements

* [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)

# How to contribute

[Contribution Guidelines](https://atc-net.github.io/introduction/about-atc#how-to-contribute)

[Coding Guidelines](https://atc-net.github.io/introduction/about-atc#coding-guidelines)
