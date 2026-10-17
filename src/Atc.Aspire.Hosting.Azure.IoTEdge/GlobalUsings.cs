global using System.Net.Security;
global using System.Net.Sockets;
global using System.Text.Json;
global using System.Text.RegularExpressions;
global using Aspire.Hosting.ApplicationModel;
global using Atc.Aspire.Hosting.Azure.IoTEdge;
global using Atc.Azure.IoT.Models;
global using Atc.Azure.IoT.Options;
global using Atc.Azure.IoT.Services.IoTHub;
global using Atc.Azure.IoTEdge.DeviceEmulator;
global using Atc.Azure.IoTEdge.DeviceEmulator.Certificates;
global using Atc.Azure.IoTEdge.DeviceEmulator.Provisioning;
global using Microsoft.Azure.Devices;
global using Microsoft.Azure.Devices.Shared;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Diagnostics.HealthChecks;
global using Microsoft.Extensions.Logging;

global using DeploymentManifest = Atc.Azure.IoTEdge.DeviceEmulator.Manifest.DeploymentManifest;