namespace Atc.Azure.IoT.Services.DeviceProvisioning;

public record ErrorResponse(
    int ErrorCode,
    string TrackingId,
    string Message,
    DateTime TimestampUtc);