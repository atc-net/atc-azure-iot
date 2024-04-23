namespace Atc.Azure.IoTEdge.DeviceEmulator.Services.File;

public sealed class FileService : IFileService
{
    public async Task<string> ReadTemplateContent(
        string filePath,
        CancellationToken cancellationToken)
    {
        if (!System.IO.File.Exists(filePath))
        {
            throw new FileNotFoundException($"File in path '{filePath}' does not exist!");
        }

        var content = await FileHelper.ReadAllTextAsync(new FileInfo(filePath), cancellationToken);
        if (!content.IsFormatJson())
        {
            throw new FormatException("Invalid json for template!");
        }

        return content;
    }
}