namespace Atc.Azure.IoT.Wpf.App.Models;

public sealed class TenantViewModel : ViewModelBase
{
    private string displayName = string.Empty;
    private Guid tenantId;
    private bool isCurrent;

    public string DisplayName
    {
        get => displayName;
        set
        {
            if (value == displayName)
            {
                return;
            }

            displayName = value;
            RaisePropertyChanged();
        }
    }

    public Guid TenantId
    {
        get => tenantId;
        set
        {
            if (value.Equals(tenantId))
            {
                return;
            }

            tenantId = value;
            RaisePropertyChanged();
        }
    }

    public bool IsCurrent
    {
        get => isCurrent;
        set
        {
            if (value == isCurrent)
            {
                return;
            }

            isCurrent = value;
            RaisePropertyChanged();
        }
    }

    public override string ToString()
        => $"{nameof(DisplayName)}: {DisplayName}, {nameof(TenantId)}: {TenantId}, {nameof(IsCurrent)}: {IsCurrent}";
}