global using System.Diagnostics.CodeAnalysis;
global using System.Net;
global using System.Runtime.CompilerServices;
global using System.Runtime.Serialization;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using Atc.Azure.IoT.Exceptions;
global using Atc.Azure.IoT.Extensions;
global using Atc.Azure.IoT.Extensions.Internal;
global using Atc.Azure.IoT.Extractors;
global using Atc.Azure.IoT.Models;
global using Atc.Azure.IoT.Models.Internal;
global using Atc.Azure.IoT.Options;
global using Atc.Azure.IoT.Serialization.JsonConverters;
global using Atc.Azure.IoT.Services.DeviceProvisioning;
global using Atc.Azure.IoT.Services.IoTHub;
global using Atc.Serialization;
global using Microsoft.AspNetCore.Http;
global using Microsoft.Azure.Devices;
global using Microsoft.Azure.Devices.Common.Exceptions;
global using Microsoft.Azure.Devices.Provisioning.Service;
global using Microsoft.Azure.Devices.Shared;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.Extensions.Logging;