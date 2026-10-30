namespace Atc.Azure.IoT;

internal static class LoggingEventIdConstants
{
    internal static class IoTHubService
    {
        public const int RetrievingRegistryStatistics = 10_050;
        public const int RetrievingIotDevice = 10_100;
        public const int IotDeviceNotFound = 10_101;
        public const int RetrieveIotDeviceSucceeded = 10_102;

        public const int RetrievingDeviceConnectionString = 10_110;
        public const int RetrieveIotEdgeDeviceConnectionStringSucceeded = 10_111;

        public const int RetrievingIotDeviceTwins = 10_200;
        public const int RetrieveIotDeviceTwinsSucceeded = 10_201;

        public const int RetrievingIotDeviceTwin = 10_300;
        public const int IotDeviceTwinNotFound = 10_301;
        public const int RetrieveIotDeviceTwinSucceeded = 10_302;

        public const int UpdatingIotDeviceTwin = 10_400;
        public const int UpdateIotDeviceTwinSucceeded = 10_402;

        public const int RetrievingIotEdgeDeviceModules = 10_500;
        public const int IotEdgeDeviceModulesNotFound = 10_501;
        public const int RetrieveIotEdgeDeviceModulesSucceeded = 10_502;
        public const int UpdatingModuleTwinDesiredProperties = 10_503;
        public const int UpdateModuleTwinDesiredPropertiesSucceeded = 10_504;

        public const int RetrievingIotEdgeDeviceTwinModule = 10_600;
        public const int IotEdgeDeviceTwinModuleNotFound = 10_601;
        public const int RetrieveIotEdgeDeviceTwinModuleSucceeded = 10_602;

        public const int RetrievingModuleConnectionString = 10_610;
        public const int IotEdgeDeviceModuleNotFound = 10_611;
        public const int RetrieveModuleConnectionStringSucceeded = 10_612;

        public const int IotDeviceModuleRemoved = 10_700;
        public const int IotDeviceModuleNotFoundOnRemove = 10_701;

        public const int AddedModulesToIotEdgeDevice = 10_800;
        public const int AppliedConfigurationContent = 10_810;

        public const int IotDeviceDeleted = 10_900;
        public const int IotDeviceNotFoundOnDelete = 10_901;
    }

    internal static class IoTHubModuleService
    {
        public const int MethodCallTransientError = 11_002;
    }

    internal static class DeviceProvisioningManager
    {
        public const int IndividualEnrollmentNotFound = 12_030;
        public const int DeleteIndividualEnrollmentNotFound = 12_040;
    }
}