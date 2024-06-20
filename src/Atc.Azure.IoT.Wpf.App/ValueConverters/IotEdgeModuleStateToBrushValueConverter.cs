namespace Atc.Azure.IoT.Wpf.App.ValueConverters;

public class IotEdgeModuleStateToBrushValueConverter : IValueConverter
{
    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => value switch
        {
            IotEdgeModuleState.Running => Brushes.Green,
            IotEdgeModuleState.Failed => Brushes.Red,
            _ => Brushes.Goldenrod,
        };

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotImplementedException();
}