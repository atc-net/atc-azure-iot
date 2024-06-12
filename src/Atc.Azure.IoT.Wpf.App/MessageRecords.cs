namespace Atc.Azure.IoT.Wpf.App;

#pragma warning disable MA0048 // File name must match type name
#pragma warning disable SA1402 // File may only contain a single type
#pragma warning disable SA1649 // File name should match first type name

public record AuthenticatedUserMessage(
    string UserName,
    string TenantName);

public record SubscriptionsCollectionStateMessage(
    CollectionActionType CollectionActionType);

public record IsBusyMessage(
    bool IsBusy);

public record SelectedIoTHubSubscriptionMessage(
    IoTHubSubscriptionViewModel? IoTHubSubscriptionViewModel);

#pragma warning restore SA1649 // File name should match first type name
#pragma warning restore SA1402 // File may only contain a single type
#pragma warning restore MA0048 // File name must match type name