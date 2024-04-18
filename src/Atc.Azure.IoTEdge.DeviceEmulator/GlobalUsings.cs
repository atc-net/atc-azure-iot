global using System.Diagnostics.CodeAnalysis;
global using System.Net;
global using System.Runtime.CompilerServices;
global using System.Runtime.InteropServices;
global using System.Runtime.Serialization;
global using System.Text.Json;
global using System.Text.Json.Nodes;
global using Atc.Azure.IoT;
global using Atc.Azure.IoT.Services.IoTHub;
global using Atc.Azure.IoTEdge.DeviceEmulator.Services;
global using Atc.Azure.IoTEdge.DeviceEmulator.Services.Docker;
global using Atc.Azure.IoTEdge.DeviceEmulator.Services.Environment;
global using Atc.Azure.IoTEdge.DeviceEmulator.Services.File;
global using Atc.Helpers;
global using Atc.Serialization;
global using Docker.DotNet;
global using Docker.DotNet.Models;
global using Microsoft.Azure.Devices;
global using Microsoft.Azure.Devices.Client;
global using Microsoft.Azure.Devices.Client.Transport.Mqtt;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;