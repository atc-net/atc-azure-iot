namespace Atc.Azure.IoT.Wpf.App.ValueConverters;

public class SvgImageOperatingSystemValueConverter : IValueConverter
{
    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        var operatingSystem = value?.ToString()?.ToUpperInvariant() ?? string.Empty;
        return operatingSystem switch
        {
            "LINUX" => "/Atc.Azure.IoT.Wpf.App;component/Resources/Images/32/linux.svg",
            "WINDOWS" => "/Atc.Azure.IoT.Wpf.App;component/Resources/Images/32/windows.svg",
            _ => null,
        };
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotImplementedException();
}