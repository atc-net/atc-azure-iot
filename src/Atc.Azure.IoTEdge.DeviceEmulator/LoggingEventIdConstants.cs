namespace Atc.Azure.IoTEdge.DeviceEmulator;

internal static class LoggingEventIdConstants
{
    public const int DockerClientCreated = 100;

    public const int DockerImageDownloadStarted = 110;
    public const int DockerImageDownloadSucceeded = 111;
    public const int DockerImageDownloadStatus = 112;
    public const int DockerImageDownloadFailed = 113;

    public const int DockerContainerCreationStarted = 120;
    public const int DockerContainerCreationSucceeded = 121;
    public const int DockerContainerCreationFailed = 122;

    public const int DockerContainerStarting = 130;
    public const int DockerContainerStartSucceeded = 131;
    public const int DockerContainerStartFailed = 132;

    public const int DockerContainerStopMissingContainerId = 140;
    public const int DockerContainerStopping = 141;
    public const int DockerContainerStopSucceeded = 142;

    public const int DockerContainerRemovalStarting = 150;
    public const int DockerContainerRemovalSucceeded = 151;

    public const int DockerContainerStopOrRemovalFailed = 160;

    public const int DockerProcessCommandFailure = 170;

    public const int IotEdgeEmulatorStarting = 180;
    public const int IotEdgeEmulatorGetTemplateContentSucceeded = 181;
    public const int IotEdgeEmulatorGetTemplateContentFailed = 182;
    public const int IotEdgeEmulatorCheck = 183;
    public const int IotEdgeEmulatorWaiting = 184;
    public const int IotEdgeEmulatorReady = 185;
    public const int IotEdgeEmulatorFetchingVariables = 186;
    public const int IotEdgeEmulatorEnsuringProperVariables = 187;
    public const int IotEdgeEmulatorStopping = 188;

    public const int IotHubProvisionIotEdgeDeviceSucceeded = 190;
    public const int IotHubProvisionIotEdgeDeviceFailed = 191;

    public const int IotHubProvisionIotEdgeDeviceMissingHostName = 192;
    public const int IotHubProvisionIotEdgeDeviceInvalidManifest = 193;
    public const int IotHubTransformTemplateToManifestSucceeded = 194;
    public const int IotHubTransformTemplateToManifestFailed = 195;

    public const int IotHubAddModulesToIotEdgeDeviceSucceeded = 200;
    public const int IotHubAddModulesToIotEdgeDeviceFailed = 201;

    public const int IotHubApplyConfigurationContentToIotEdgeDeviceSucceeded = 210;
    public const int IotHubApplyConfigurationContentToIotEdgeDeviceFailed = 211;

    public const int IotHubRemoveModulesFromIotEdgeDeviceSucceeded = 220;
    public const int IotHubRemoveModulesFromIotEdgeDeviceFailed = 221;

    public const int EnvironmentInjectedVariable = 240;

    public const int EnvironmentPlatformNotImplemented = 250;
    public const int EnvironmentPlatformNotSupported = 251;
}