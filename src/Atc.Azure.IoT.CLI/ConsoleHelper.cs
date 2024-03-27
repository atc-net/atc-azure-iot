namespace Atc.Azure.IoT.CLI;

public static class ConsoleHelper
{
    public static void WriteHeader()
        => Console.Spectre.Helpers.ConsoleHelper.WriteHeader("Azure IoT CLI");
}