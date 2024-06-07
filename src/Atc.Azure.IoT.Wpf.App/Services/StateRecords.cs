namespace Atc.Azure.IoT.Wpf.App.Services;

#pragma warning disable MA0048 // File name must match type name
#pragma warning disable SA1402 // File may only contain a single type
#pragma warning disable SA1649 // File name should match first type name

public abstract record ResourceStateBase(
    SubscriptionResource Subscription,
    ResourceGroupResource ResourceGroup);

public record IotHubServiceState(
    SubscriptionResource Subscription,
    ResourceGroupResource ResourceGroup,
    IotHubDescriptionResource Resource)
    : ResourceStateBase(
        Subscription,
        ResourceGroup);

public record DeviceProvisioningServiceState(
    SubscriptionResource Subscription,
    ResourceGroupResource ResourceGroup,
    DeviceProvisioningServiceResource Resource)
    : ResourceStateBase(
        Subscription,
        ResourceGroup);

#pragma warning restore SA1649 // File name should match first type name
#pragma warning restore SA1402 // File may only contain a single type
#pragma warning restore MA0048 // File name must match type name