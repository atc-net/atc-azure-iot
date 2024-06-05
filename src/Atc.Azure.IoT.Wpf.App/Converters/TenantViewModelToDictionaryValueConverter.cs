namespace Atc.Azure.IoT.Wpf.App.Converters;

public sealed class TenantViewModelToDictionaryValueConverter : IValueConverter
{
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => value is ObservableCollectionEx<TenantViewModel> collection
            ? collection.ToDictionary(x => x.TenantId.ToString(), x => x.DisplayName)
            : Binding.DoNothing;

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException();
}
