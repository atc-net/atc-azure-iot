// ReSharper disable InvertIf
namespace Atc.Azure.IoT.Wpf.App.ValueConverters;

public class IotEdgeModuleStateToVisibilityVisibleValueConverter : IValueConverter
{
    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is null)
        {
            return Visibility.Collapsed;
        }

        if (!Enum<IotEdgeModuleState>.TryParse(value.ToString()!, false, out var state))
        {
            return Visibility.Collapsed;
        }

        if (parameter is not null)
        {
            switch (state)
            {
                case IotEdgeModuleState.Running when
                    parameter.ToString()!.Contains("Running", StringComparison.OrdinalIgnoreCase):
                case IotEdgeModuleState.Failed when
                    parameter.ToString()!.Contains("Failed", StringComparison.OrdinalIgnoreCase):
                case IotEdgeModuleState.Unknown when
                    parameter.ToString()!.Contains("Unknown", StringComparison.OrdinalIgnoreCase):
                    return Visibility.Visible;
                default:
                    return Visibility.Collapsed;
            }
        }

        return state switch
        {
            IotEdgeModuleState.Running => Visibility.Visible,
            IotEdgeModuleState.Failed => Visibility.Visible,
            _ => Visibility.Collapsed,
        };
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotImplementedException();
}