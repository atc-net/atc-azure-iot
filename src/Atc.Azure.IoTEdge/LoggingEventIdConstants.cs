namespace Atc.Azure.IoTEdge;

public static class LoggingEventIdConstants
{
    public const int ModuleStarted = 10_000;
    public const int ModuleStopping = 10_010;
    public const int ModuleStopped = 10_020;

    public const int ModuleClientStarted = 11_000;
    public const int ModuleClientStopped = 11_010;

    public const int FailedToAcquireModuleClient = 11_050;

    public const int ConnectionStatusChange = 12_000;

    public const int MethodCalled = 13_000;
    public const int MethodCallCompleted = 13_010;

    public const int MethodRequestEmpty = 14_000;
    public const int MethodRequestDeserializationError = 14_010;
    public const int MethodRequestUriParsingError = 14_020;
    public const int MethodRequestUriFormatExceptionError = 14_030;
    public const int MethodRequestError = 14_040;

    public const int DesiredPropertyChangedInvoked = 15_000;
    public const int DesiredPropertyChangedHandled = 15_010;
    public const int GetDesiredPropertiesFailed = 15_100;
    public const int GetReportedPropertiesFailed = 15_110;
    public const int ReportedPropertiesUpdated = 15_120;
}