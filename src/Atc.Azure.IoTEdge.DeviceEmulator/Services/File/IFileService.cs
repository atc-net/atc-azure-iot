namespace Atc.Azure.IoTEdge.DeviceEmulator.Services.File;

public interface IFileService
{
    Task<string> ReadTemplateContent(
        string filePath,
        CancellationToken cancellationToken);
}