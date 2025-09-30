namespace Atc.Azure.IoT.Extensions;

public static class ErrorCodeExtensions
{
    public static int ToHttpStatusCode(this ErrorCode errorCode)
    {
        if (errorCode == ErrorCode.InvalidErrorCode)
        {
            return StatusCodes.Status500InternalServerError;
        }

        var statusCode = (int)errorCode;
        while (statusCode >= 1000)
        {
            statusCode /= 10;
        }

        return statusCode;
    }
}