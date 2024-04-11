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
            node.AddBranch("enrollment", ConfigureDpsEnrollmentCommands);
        };

    private static Action<IConfigurator<CommandSettings>> ConfigureIotHubCommands()
        => node =>
        {
            node.SetDescription("Operations related to IoT Hub.");

            node.AddCommand<IotHubStatisticsCommand>("statistics")
                .WithDescription("Retrieve the statistics of the device registry in the IoT Hub.")
                .WithExample("iothub statistics");

            ConfigureIotHubDeviceCommands(node);
        };

    private static void ConfigureDpsEnrollmentCommands(
        IConfigurator<CommandSettings> node)
    {
        node.AddBranch("individual", individual =>
        {
            individual.SetDescription("Operations related to individual enrollments on Device Provisioning Service.");

            individual.AddCommand<DpsEnrollmentÍndividualGetAllCommand>("all")
                .WithDescription("Retrieves all individual enrollments.")
                .WithExample("dps enrollment individual list"); // TODO: Fill out example

            individual.AddBranch("create", create =>
            {
                create.SetDescription("Operations related to creating individual enrollments.");
                create.AddCommand<DpsEnrollmentÍndividualCreateTpmCommand>("tpm")
                    .WithDescription("Reads a single node variable.")
                    .WithExample("dps enrollment individual create tpm"); // TODO: Fill out example
            });

            individual.AddCommand<DpsEnrollmentÍndividualDeleteCommand>("delete")
                .WithDescription("Retrieves an individual enrollment.")
                .WithExample("dps enrollment individual get"); // TODO: Fill out example

            individual.AddCommand<DpsEnrollmentÍndividualGetCommand>("get")
                .WithDescription("Retrieves an individual enrollment.")
                .WithExample("dps enrollment individual get"); // TODO: Fill out example
        });
    }

    private static void ConfigureIotHubDeviceCommands(
        IConfigurator<CommandSettings> node)
    {
        node.AddBranch("device", device =>
        {
            device.SetDescription("Operations related to devices on the iot hub.");

            device.AddCommand<IotHubDeviceGetCommand>("get")
                .WithDescription("Retrieve a device from the device registry in the IoT Hub.")
                .WithExample("iothub device get"); // TODO: Fill out example

            device.AddCommand<IotHubDeviceDeleteCommand>("delete")
                .WithDescription("Delete a device from the device registry in the IoT Hub.")
                .WithExample("iothub device delete"); // TODO: Fill out example

            device.AddBranch("twin", twin =>
            {
                twin.SetDescription("Operations related to twins in the IoT Hub.");

                twin.AddCommand<IotHubDeviceTwinGetAllCommand>("all")
                    .WithDescription("Retrieve all device twins in the IoT Hub.")
                    .WithExample("iothub device twin all");  // TODO: Fill out example

                twin.AddCommand<IotHubDeviceTwinGetCommand>("get")
                    .WithDescription("Retrieve a device twin in the IoT Hub.")
                    .WithExample("iothub device twin get");  // TODO: Fill out example

                twin.AddCommand<IotHubDeviceTwinUpdateCommand>("update")
                    .WithDescription("Update a device twin in the IoT Hub.")
                    .WithExample("iothub device twin update");  // TODO: Fill out example
            });

            device.AddBranch("module", module =>
            {
                module.SetDescription("Operations related to device modules in the IoT Hub.");

                module.AddCommand<IotHubDeviceModuleGetAllCommand>("all")
                    .WithDescription("Retrieve all modules on a device.")
                    .WithExample("iothub device module all");  // TODO: Fill out example

                module.AddBranch("twin", twin =>
                {
                    twin.AddCommand<IotHubDeviceModuleGetTwinCommand>("get")
                        .WithDescription("Retrieve a module twin on a device.")
                        .WithExample("iothub device module twin get");  // TODO: Fill out example
                });

                module.AddCommand<IotHubDeviceModuleRemoveCommand>("remove")
                    .WithDescription("Remove a module on a device.")
                    .WithExample("iothub device module remove");  // TODO: Fill out example

                module.AddCommand<IotHubDeviceModuleRestartCommand>("restart")
                    .WithDescription("Restart a module on a device.")
                    .WithExample("iothub device module restart");  // TODO: Fill out example
            });
        });
    }
}