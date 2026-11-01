# IoT Hub event viewer

A terminal app that shows the IoT Hub events the sample AppHost's event bridge forwards to the local Event Hubs: device lifecycle events, twin change events and telemetry. It shows what a consumer of those Event Hubs receives, including the properties that consumers read.

## Opening it

The sample AppHost runs the viewer as the `event-viewer` resource with a terminal (`WithTerminal()`):

- In the dashboard, select `event-viewer`; its page shows the terminal.
- Or from a shell: `aspire terminal attach event-viewer`, and `Ctrl+B D` to detach.

## Screens

**List**: every event, newest first, with the time IoT Hub received it, the Event Hub, the device, the module and what happened (the `opType` of lifecycle and twin change events, the start of the body for telemetry). The title shows the number of events per Event Hub.

**Details**: the selected event's

- application properties, e.g. `opType`, `deviceId` and `moduleId`, which consumers usually read;
- system properties, e.g. `iothub-message-source` and `iothub-connection-device-id`, as IoT Hub set them;
- body, formatted when it is JSON.

| Key | List | Details |
| --- | --- | --- |
| ↑ ↓ | Select an event | Scroll |
| PgUp PgDn | | Scroll a page |
| Enter | Show the details | |
| Esc | | Back to the list |

## How it works

- The AppHost references a consumer group named `event-viewer` on each Event Hub, and the viewer reads every Event Hub it gets a connection string for, from its first event on. So it also shows events forwarded before it started, and an Event Hub added to the AppHost shows up without changes here.
- It keeps the latest 1000 events in memory. After a read error, it shows the error and reads that Event Hub again from its first event.
- It reads with the [Aspire Event Hubs client](https://www.nuget.org/packages/Aspire.Azure.Messaging.EventHubs) and draws with [Spectre.Console](https://spectreconsole.net).