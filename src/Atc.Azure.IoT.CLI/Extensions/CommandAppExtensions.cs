namespace Atc.Azure.IoT.CLI.Extensions;

public static class CommandAppExtensions
{
    public static void ConfigureCommands(
        this CommandApp app)
    {
        ArgumentNullException.ThrowIfNull(app);

        //   dps
        //      enrollment
        //        get
        //        list
        //        create
        //          tpm
        //        delete
        //      enrollment-group
        //   iothub
        //     ??

        app.Configure(config =>
        {
            //ConfigureTestConnectionCommand(config);
            config.AddBranch("dps", ConfigureDpsCommands());

            config.AddBranch("iothub", ConfigureIotHubCommands());
        });
    }

    //private static void ConfigureTestConnectionCommand(
    //    IConfigurator config)
    //    => config.AddCommand<TestConnectionCommand>("testconnection")
    //        .WithDescription("Tests if a connection can be made to a given server.")
    //        .WithExample($"testconnection -s {SampleOpcUaServerUrl}");

    private static Action<IConfigurator<CommandSettings>> ConfigureDpsCommands()
        => node =>
        {
            node.SetDescription("Operations related to Device Provisioning Service.");
            ConfigureNodeReadCommands(node);
            ConfigureNodeWriteCommands(node);
        };

    private static Action<IConfigurator<CommandSettings>> ConfigureIotHubCommands()
        => method =>
        {
            method.SetDescription("Operations related to IoT Hub.");

            method.AddCommand<ExecuteMethodCommand>("execute")
                .WithDescription("Used to execute a given method.");
        };

    private static void ConfigureNodeReadCommands(
        IConfigurator<CommandSettings> node)
        => node.AddBranch("read", read =>
        {
            read.SetDescription("Operations related to reading nodes.");
            read.AddCommand<NodeReadObjectCommand>("object")
                .WithDescription("Reads a given node object.")
                .WithExample($"node read object -s {SampleOpcUaServerUrl} -n \"ns=2;s=Demo.Dynamic.Scalar\"");

            read.AddBranch("variable", variable =>
            {
                variable.SetDescription("Reads one or more node variable(s).");
                variable.AddCommand<NodeReadVariableSingleCommand>("single")
                    .WithDescription("Reads a single node variable.")
                    .WithExample($"node read variable single -s {SampleOpcUaServerUrl} -n \"ns=2;s=Demo.Dynamic.Scalar.Float\"");

                variable.AddCommand<NodeReadVariableMultiCommand>("multi")
                    .WithDescription("Reads a list of node variables.")
                    .WithExample($"node read variable multi -s {SampleOpcUaServerUrl} -n \"ns=2;s=Demo.Dynamic.Scalar.Float\" -n \"ns=2;s=Demo.Dynamic.Scalar.Int32\"");
            });
        });

    private static void ConfigureNodeWriteCommands(
        IConfigurator<CommandSettings> node)
        => node.AddBranch("write", write =>
        {
            write.SetDescription("Operations related to writing nodes.");

            write.AddBranch("variable", variable =>
            {
                variable.SetDescription("Writes a value to one or more node variable(s).");
                variable.AddCommand<NodeWriteVariableSingleCommand>("single")
                    .WithDescription("Write a value to a single node variable.")
                    .WithExample($"node write variable single -s {SampleOpcUaServerUrl} -n \"ns=2;s=Demo.Dynamic.Scalar.Float\" -d float --value \"100.5\"");
            });
        });
}