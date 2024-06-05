global using System.ComponentModel;
global using System.Globalization;
global using System.IdentityModel.Tokens.Jwt;
global using System.IO;
global using System.Reflection;
global using System.Security.Claims;
global using System.Windows;
global using System.Windows.Data;
global using System.Windows.Input;
global using System.Windows.Media.Imaging;
global using Atc.Azure.IoT.Wpf.App.Authentication;
global using Atc.Azure.IoT.Wpf.App.Dialogs;
global using Atc.Azure.IoT.Wpf.App.Extensions;
global using Atc.Azure.IoT.Wpf.App.Models;
global using Atc.Azure.IoT.Wpf.App.UserControls;
global using Atc.Wpf;
global using Atc.Wpf.Collections;
global using Atc.Wpf.Command;
global using Atc.Wpf.Mvvm;
global using Azure.Core;
global using Azure.Identity;
global using Azure.ResourceManager;
global using Azure.ResourceManager.DeviceProvisioningServices;
global using Azure.ResourceManager.IotHub;
global using Azure.ResourceManager.Resources;
global using Azure.ResourceManager.Resources.Models;
global using ControlzEx.Theming;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Identity.Client;