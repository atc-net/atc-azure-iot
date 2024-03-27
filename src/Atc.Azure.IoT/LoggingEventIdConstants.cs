namespace Atc.Azure.IoT;

internal static class LoggingEventIdConstants
{
    internal static class IoTHubService
    {
        public const int Failure = 10_000;

        public const int RetrievingIotDevice = 10_100;
        public const int IotDeviceNotFound = 10_101;
        public const int RetrieveIotDeviceSucceeded = 10_102;

        public const int RetrievingIotEdgeDeviceTwins = 10_200;
        public const int RetrieveIotEdgeDeviceTwinsSucceeded = 10_201;

        public const int RetrievingIotDeviceTwin = 10_300;
        public const int IotDeviceTwinNotFound = 10_301;
        public const int RetrieveIotDeviceTwinSucceeded = 10_302;

        public const int UpdatingIotDeviceTwin = 10_400;
        public const int IotDeviceTwinNotUpdated = 10_401;
        public const int UpdateIotDeviceTwinSucceeded = 10_402;

        public const int RetrievingIotEdgeDeviceTwinModules = 10_500;
        public const int IotEdgeDeviceTwinModulesNotFound = 10_501;
        public const int RetrieveIotEdgeDeviceTwinModulesSucceeded = 10_502;
        public const int UpdatingModuleTwinDesiredProperties = 10_503;
        public const int UpdateModuleTwinDesiredPropertiesSucceeded = 10_504;
        public const int ModuleTwinDesiredPropertiesNotUpdated = 10_505;

        public const int RetrievingIotEdgeDeviceTwinModule = 10_600;
        public const int IotEdgeDeviceTwinModuleNotFound = 10_601;
        public const int RetrieveIotEdgeDeviceTwinModuleSucceeded = 10_602;

        public const int IotDeviceModuleRemoved = 10_700;

        public const int AddedModulesToIotEdgeDevice = 10_800;
        public const int AppliedConfigurationContent = 10_810;

        public const int IotDeviceDeleted = 10_900;
    }

    internal static class IoTHubModuleService
    {
        public const int MethodCallFailedDeviceNotFound = 11_000;
        public const int MethodCallFailed = 11_001;
    }

    internal static class DeviceProvisioningManager
    {
        public const int IndividualTpmEnrollmentFailed = 12_000;
        public const int IndividualTpmEnrollmentBadRequest = 12_010;
        public const int IndividualTpmEnrollmentConflict = 12_020;
        public const int IndividualEnrollmentNotFound = 12_030;
        public const int DeleteIndividualEnrollmentNotFound = 12_040;
    }
}