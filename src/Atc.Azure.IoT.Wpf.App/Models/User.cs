namespace Atc.Azure.IoT.Wpf.App.Models;

public sealed class User
{
    public string DisplayName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public override string ToString()
        => $"{nameof(DisplayName)}: {DisplayName}, {nameof(Email)}: {Email}";
}