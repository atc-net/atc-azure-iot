namespace Atc.Azure.IoTEdge.DeviceEmulator.Services.Environment;

public interface ISystemEnvironmentService
{
    FileInfo GetCommandFilePath();

    void SetEnvironmentVariables(
        IDictionary<string, string> variables);
}