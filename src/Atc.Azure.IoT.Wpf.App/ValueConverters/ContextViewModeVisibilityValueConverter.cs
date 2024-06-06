namespace Atc.Azure.IoT.Wpf.App.ValueConverters;

public class ContextViewModeVisibilityValueConverter : IValueConverter
{
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is not ContextViewMode currentContextViewMode ||
            parameter is not ContextViewMode matchContextViewMode)
        {
            return Visibility.Collapsed;
        }

        return currentContextViewMode == matchContextViewMode
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture) => throw new NotImplementedException();
}