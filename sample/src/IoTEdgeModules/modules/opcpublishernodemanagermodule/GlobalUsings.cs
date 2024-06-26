global using System.Net;
global using System.Runtime.CompilerServices;
global using System.Runtime.Serialization;
global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using Atc.Azure.IoT.Options;
global using Atc.Azure.Iot.Sample.Modules.Contracts.OpcPublisherNodeManagerModule;
global using Atc.Azure.IoT.Services.IoTHub;
global using Atc.Azure.IoTEdge;
global using Atc.Azure.IoTEdge.DeviceEmulator.Extensions;
global using Atc.Azure.IoTEdge.DeviceEmulator.Factories;
global using Atc.Azure.IoTEdge.DeviceEmulator.Options;
global using Atc.Azure.IoTEdge.DeviceEmulator.Services;
global using Atc.Azure.IoTEdge.DeviceEmulator.Services.Docker;
global using Atc.Azure.IoTEdge.DeviceEmulator.Services.Environment;
global using Atc.Azure.IoTEdge.Extensions;
global using Atc.Azure.IoTEdge.Factories;
global using Atc.Azure.IoTEdge.TestMocks;
global using Atc.Azure.IoTEdge.Wrappers;
global using Atc.Helpers;
global using Atc.Serialization;
global using Microsoft.Azure.Devices.Client;
global using Microsoft.Azure.Devices.Common.Exceptions;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;