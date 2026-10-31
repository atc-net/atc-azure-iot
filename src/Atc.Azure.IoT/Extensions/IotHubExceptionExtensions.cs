namespace Atc.Azure.IoT.Extensions;

public static class IotHubExceptionExtensions
{
    /// <summary>
    /// Gets IoT Hub's error message from the exception, without the JSON the IoT Hub SDK wraps it in.
    /// </summary>
    /// <remarks>
    /// The SDK puts IoT Hub's answer in the message as JSON inside JSON, for example
    /// <c>{"Message":"{\"errorCode\":401002,\"message\":\"The specified SAS token has an invalid signature.\",...}","ExceptionMessage":""}</c>.
    /// This method returns <c>The specified SAS token has an invalid signature. (401002)</c> for it, and the message
    /// unchanged when it isn't in that format.
    /// </remarks>
    /// <param name="ex">The IoT Hub exception.</param>
    /// <returns>IoT Hub's message, followed by its error code when there is one.</returns>
    public static string GetErrorMessage(this IotHubException ex)
    {
        ArgumentNullException.ThrowIfNull(ex);

        return TryReadIoTHubError(ex.Message, out var message, out var errorCode)
            ? errorCode is null ? message : $"{message} ({errorCode})"
            : ex.Message;
    }

    /// <summary>
    /// Describes the exception as a JSON object with its message and its error code, for example to return it as the
    /// payload of a direct method response.
    /// </summary>
    /// <param name="ex">The IoT Hub exception.</param>
    /// <returns>The JSON object, for example <c>{"message":"...","errorCode":"DeviceNotOnline"}</c>.</returns>
    public static string ToJsonPayload(this IotHubException ex)
    {
        ArgumentNullException.ThrowIfNull(ex);

        return JsonSerializer.Serialize(new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            ["message"] = ex.InnerException is null ? ex.GetErrorMessage() : ex.GetLastInnerMessage(),
            ["errorCode"] = ex.Code.ToString(),
        });
    }

    private static bool TryReadIoTHubError(
        string text,
        [NotNullWhen(true)] out string? message,
        out string? errorCode)
    {
        message = null;
        errorCode = null;

        if (!TryParseObject(text, out var outer))
        {
            return false;
        }

        // IoT Hub's own error object, possibly wrapped in the SDK's { "Message": "<IoT Hub's JSON>" }.
        var error = TryGetString(outer, "Message") is { } inner && TryParseObject(inner, out var innerObject)
            ? innerObject
            : outer;

        message = TryGetString(error, "message") ?? TryGetString(error, "Message");
        if (string.IsNullOrWhiteSpace(message))
        {
            message = null;
            return false;
        }

        if (error.TryGetPropertyValue("errorCode", out var code) && code is not null)
        {
            errorCode = code.ToString();
        }

        return true;
    }

    private static bool TryParseObject(
        string text,
        [NotNullWhen(true)] out JsonObject? jsonObject)
    {
        jsonObject = null;

        if (!text.TrimStart().StartsWith('{'))
        {
            return false;
        }

        try
        {
            jsonObject = JsonNode.Parse(text) as JsonObject;
        }
        catch (JsonException)
        {
            return false;
        }

        return jsonObject is not null;
    }

    private static string? TryGetString(
        JsonObject jsonObject,
        string propertyName)
        => jsonObject.TryGetPropertyValue(propertyName, out var value) &&
           value is JsonValue jsonValue &&
           jsonValue.TryGetValue<string>(out var text)
            ? text
            : null;
}