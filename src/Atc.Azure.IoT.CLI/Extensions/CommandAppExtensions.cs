namespace Atc.Azure.IoT.CLI.Extensions;

public static class CommandAppExtensions
{
    public static void ConfigureCommands(
        this CommandApp app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app.Configure(config =>
        {
            config.AddBranch("dps", ConfigureDpsCommands());
            config.AddBranch("iothub", ConfigureIotHubCommands());
        });
    }

    private static Action<IConfigurator<CommandSettings>> ConfigureDpsCommands()
        => node =>
        {
            node.SetDescription("Operations related to Device Provisioning Service.");
            node.AddBranch("enrollment", ConfigureDpsEnrollmentIndividualCommands);
        };

    private static Action<IConfigurator<CommandSettings>> ConfigureIotHubCommands()
        => node =>
        {
            node.SetDescription("Operations related to IoT Hub.");

            node.AddCommand<IotHubStatisticsCommand>("statistics")
                .WithDescription("Retrieve the statistics of the device registry in the IoT Hub.")
                .WithExample("iothub statistics -c <connection-string>");

            ConfigureIotHubDeviceCommands(node);
        };

    private static void ConfigureDpsEnrollmentIndividualCommands(
        IConfigurator<CommandSettings> node)
    {
        node.AddBranch("individual", individual =>
        {
            individual.SetDescription("Operations related to individual enrollments on Device Provisioning Service.");

            individual.AddCommand<DpsEnrollmentIndividualGetAllCommand>("all")
                .WithDescription("Retrieves all individual enrollments.")
                .WithExample("dps enrollment individual all -c <connection-string>");

            individual.AddBranch("create", create =>
            {
                create.SetDescription("Operations related to creating individual enrollments.");
                create.AddCommand<DpsEnrollmentIndividualCreateTpmCommand>("tpm")
                    .WithDescription("Reads a single node variable.")
                    .WithExample("dps enrollment individual create tpm -c <connection-string> -r <registration-id> --endorsement-key <endorsement-key> --device-id <device-id> --tags tag1=value1,tag2=value2");
            });

            individual.AddCommand<DpsEnrollmentIndividualDeleteCommand>("delete")
                .WithDescription("Deletes an individual enrollment.")
                .WithExample("dps enrollment individual delete -c <connection-string> -r <registration-id>");

            individual.AddCommand<DpsEnrollmentIndividualGetCommand>("get")
                .WithDescription("Retrieves an individual enrollment.")
                .WithExample("dps enrollment individual get -c <connection-string> -r <registration-id>");
        });
    }

    private static void ConfigureIotHubDeviceCommands(
        IConfigurator<CommandSettings> node)
    {
        node.AddBranch("device", device =>
        {
            device.SetDescription("Operations related to devices on the iot hub.");

            device.AddCommand<IotHubDeviceCreateCommand>("create")
                .WithDescription("Create a device in the device registry in the IoT Hub.")
                .WithExample("iothub device create -c <connection-string> -d <device-id> --edge-enabled");

            device.AddCommand<IotHubDeviceGetCommand>("get")
                .WithDescription("Retrieve a device from the device registry in the IoT Hub.")
                .WithExample("iothub device get -c <connection-string> -d <device-id>");

            device.AddBranch("connection-string", connectionString =>
            {
                connectionString.SetDescription("Operations related to device connection-string");

                connectionString.AddCommand<IotHubDeviceConnectionStringGetCommand>("get")
                    .WithDescription("Retrieve a device connection-string from the device registry in the IoT Hub.")
                    .WithExample("iothub device connection-string get -c <connection-string> -d <device-id>");
            });

            device.AddCommand<IotHubDeviceDeleteCommand>("delete")
                .WithDescription("Delete a device from the device registry in the IoT Hub.")
                .WithExample("iothub device delete -c <connection-string> -d <device-id>");

            device.AddBranch("twin", twin =>
            {
                twin.SetDescription("Operations related to twins in the IoT Hub.");

                twin.AddCommand<IotHubDeviceTwinGetAllCommand>("all")
                    .WithDescription("Retrieve all device twins in the IoT Hub.")
                    .WithExample("iothub device twin all -c <connection-string> --edge-devices-only");

                twin.AddCommand<IotHubDeviceTwinGetCommand>("get")
                    .WithDescription("Retrieve a device twin in the IoT Hub.")
                    .WithExample("iothub device twin get -c <connection-string> -d <device-id>");

                twin.AddCommand<IotHubDeviceTwinUpdateCommand>("update")
                    .WithDescription("Update a device twin in the IoT Hub.")
                    .WithExample("iothub device twin update -c <connection-string> -d <device-id> --tags tag1=value1,tag2=value2");
            });

            device.AddBranch("module", module =>
            {
                module.SetDescription("Operations related to device modules in the IoT Hub.");

                module.AddCommand<IotHubDeviceModuleGetAllCommand>("all")
                    .WithDescription("Retrieve all modules on a device.")
                    .WithExample("iothub device module all -c <connection-string> -d <device-id>");

                module.AddBranch("twin", twin =>
                {
                    twin.AddCommand<IotHubDeviceModuleGetTwinCommand>("get")
                        .WithDescription("Retrieve a module twin on a device.")
                        .WithExample("iothub device module twin get -c <connection-string> -d <device-id> -m <module-id>");
                });

                module.AddCommand<IotHubDeviceModuleRemoveCommand>("remove")
                    .WithDescription("Remove a module on a device.")
                    .WithExample("iothub device module remove -c <connection-string> -d <device-id> -m <module-id>");

                module.AddCommand<IotHubDeviceModuleRestartCommand>("restart")
                    .WithDescription("Restart a module on a device.")
                    .WithExample("iothub device module restart -c <connection-string> -d <device-id> -m <module-id>");
            });
        });
    }
}