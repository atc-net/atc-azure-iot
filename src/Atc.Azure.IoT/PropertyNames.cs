namespace Atc.Azure.IoT;

public static class PropertyNames
{
    public const string ModuleEnvironment = "env";
    public const string Modules = "modules";
    public const string ModuleSettings = "settings";
    public const string ModuleSettingsImage = "image";
    public const string ModuleStartupOrder = "startupOrder";
    public const string Runtime = "runtime";
    public const string SystemModules = "systemModules";

    public static class Manifest
    {
        public const string Address = "address";
        public const string CreateOptions = "createOptions";
        public const string Docker = "docker";
        public const string EdgeAgent = "edgeAgent";
        public const string EdgeHub = "edgeHub";
        public const string Image = "image";
        public const string LoggingOptions = "loggingOptions";
        public const string MinDockerVersion = "minDockerVersion";
        public const string ModulesContent = "modulesContent";
        public const string Password = "password";
        public const string Priority = "priority";
        public const string PropertiesDesired = "properties.desired";
        public const string PropertiesDesiredModules = "properties.desired.modules";
        public const string PropertiesDesiredRoutes = "properties.desired.routes";
        public const string RegistryCredentials = "registryCredentials";
        public const string RestartPolicy = "restartPolicy";
        public const string Route = "route";
        public const string Routes = "routes";
        public const string SchemaVersion = "schemaVersion";
        public const string Status = "status";
        public const string StoreAndForwardConfiguration = "storeAndForwardConfiguration";
        public const string TimeToLiveSecs = "timeToLiveSecs";
        public const string Username = "username";
        public const string Type = "type";
        public const string Value = "value";
        public const string Version = "version";
    }

    public static class DeviceTwin
    {
        public const string Platform = "platform";
        public const string PlatformOs = "os";
        public const string PlatformArchitecture = "architecture";

        public const string LastDesiredStatus = "lastDesiredStatus";
        public const string LastDesiredStatusCode = "code";
        public const string LastDesiredStatusDescription = "description";

        public const string ModuleExitCode = "exitCode";
        public const string ModuleStatusDescription = "statusDescription";
        public const string ModuleRuntimeStatus = "runtimeStatus";
        public const string ModuleLastExitTimeUtc = "lastExitTimeUtc";
        public const string ModuleLastStartTimeUtc = "lastStartTimeUtc";
        public const string ModuleLastRestartTimeUtc = "lastRestartTimeUtc";
        public const string ModuleRestartCount = "restartCount";
        public const string ModuleEnvironmentUpstreamProtocol = "upstreamProtocol";
        public const string ModuleEnvironmentUpstreamProtocolValue = "value";
    }
}