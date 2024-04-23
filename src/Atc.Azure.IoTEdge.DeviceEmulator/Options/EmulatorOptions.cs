namespace Atc.Azure.IoTEdge.DeviceEmulator.Options;

public sealed class EmulatorOptions
{
    public string TemplateFilePath { get; set; } = string.Empty;

    public string IotHubConnectionString { get; set; } = string.Empty;
}