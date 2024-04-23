namespace Atc.Azure.IoT;

public static class DeviceTwinPropertyNames
{
    public const string Runtime = "runtime";

    public const string Platform = "platform";
    public const string PlatformOs = "os";
    public const string PlatformArchitecture = "architecture";

    public const string LastDesiredStatus = "lastDesiredStatus";
    public const string LastDesiredStatusCode = "code";
    public const string LastDesiredStatusDescription = "description";

    public const string SystemModules = "systemModules";
    public const string Modules = "modules";
    public const string ModuleExitCode = "exitCode";
    public const string ModuleStatusDescription = "statusDescription";
    public const string ModuleRuntimeStatus = "runtimeStatus";
    public const string ModuleLastExitTimeUtc = "lastExitTimeUtc";
    public const string ModuleLastStartTimeUtc = "lastStartTimeUtc";
    public const string ModuleLastRestartTimeUtc = "lastRestartTimeUtc";
    public const string ModuleRestartCount = "restartCount";
    public const string ModuleStartupOrder = "startupOrder";
    public const string ModuleSettings = "settings";
    public const string ModuleSettingsImage = "image";
    public const string ModuleEnvironment = "env";
    public const string ModuleEnvironmentUpstreamProtocol = "upstreamProtocol";
    public const string ModuleEnvironmentUpstreamProtocolValue = "value";
}