namespace Atc.Azure.IoT.Extensions;

public static class IotHubExceptionExtensions
{
    public static string ToJsonPayload(this IotHubException ex)
        => $"{{\"message\":\"{ex.GetLastInnerMessage()}\", \"errorCode\": \"{ex.Code}\"}}";
}