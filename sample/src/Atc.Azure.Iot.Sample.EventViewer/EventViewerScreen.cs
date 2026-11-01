namespace Atc.Azure.Iot.Sample.EventViewer;

/// <summary>
/// Shows the events in the terminal: a live list, newest first, and the details of the selected event.
/// ↑↓ select an event, Enter shows its properties and body, Esc goes back; in the details, ↑↓ and PgUp/PgDn scroll.
/// </summary>
public sealed class EventViewerScreen(EventFeed feed)
{
    // The lines around the list: the title, the table's borders and header, and the error line.
    private const int ListChromeLines = 6;

    // The lines around the details: the title and the bottom margin.
    private const int DetailsChromeLines = 3;

    private static readonly TimeSpan RefreshInterval = TimeSpan.FromMilliseconds(200);
    private static readonly JsonSerializerOptions IndentedJson = new() { WriteIndented = true };

    // The selected event, or null while the top row is selected: the selection then moves to each new event as it arrives.
    private ForwardedEvent? selected;

    // The event whose details are shown, or null while the list is shown.
    private ForwardedEvent? details;

    // How many lines the details are scrolled down.
    private int detailsScroll;

    public Task RunAsync(CancellationToken cancellationToken)
        => AnsiConsole.Live(new Text(string.Empty))
            .AutoClear(enabled: false)
            .Overflow(VerticalOverflow.Crop)
            .StartAsync(async context =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var snapshot = feed.GetSnapshot();
                    HandleKeys(snapshot.Events);

                    context.UpdateTarget(details is null
                        ? RenderList(snapshot.Events, snapshot.Counts, snapshot.Errors)
                        : RenderDetails(details));

                    try
                    {
                        await Task.Delay(RefreshInterval, cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                        return;
                    }
                }
            });

    private static int Height => Math.Max(Console.WindowHeight, 10);

    private static int Width => Math.Max(Console.WindowWidth, 40);

    private void HandleKeys(IReadOnlyList<ForwardedEvent> events)
    {
        // Without a terminal, e.g. when the AppHost runs the viewer without WithTerminal, there are no keys to read.
        if (Console.IsInputRedirected)
        {
            return;
        }

        while (Console.KeyAvailable)
        {
            var key = Console.ReadKey(intercept: true).Key;
            if (details is null)
            {
                HandleListKey(key, events);
            }
            else
            {
                HandleDetailsKey(key);
            }
        }
    }

    private void HandleListKey(
        ConsoleKey key,
        IReadOnlyList<ForwardedEvent> events)
    {
        if (events.Count == 0)
        {
            return;
        }

        var index = GetSelectedIndex(events);
        switch (key)
        {
            case ConsoleKey.UpArrow:
                selected = index <= 1 ? null : events[index - 1];
                break;
            case ConsoleKey.DownArrow:
                selected = events[Math.Min(index + 1, events.Count - 1)];
                break;
            case ConsoleKey.Enter:
                details = events[index];
                detailsScroll = 0;
                break;
        }
    }

    private void HandleDetailsKey(ConsoleKey key)
    {
        var page = Height - DetailsChromeLines;
        switch (key)
        {
            case ConsoleKey.Escape:
                details = null;
                break;
            case ConsoleKey.UpArrow:
                detailsScroll = Math.Max(detailsScroll - 1, 0);
                break;
            case ConsoleKey.DownArrow:
                detailsScroll++;
                break;
            case ConsoleKey.PageUp:
                detailsScroll = Math.Max(detailsScroll - page, 0);
                break;
            case ConsoleKey.PageDown:
                detailsScroll += page;
                break;
        }
    }

    // An event that dropped out of the list selects the newest again.
    private int GetSelectedIndex(IReadOnlyList<ForwardedEvent> events)
    {
        if (selected is null)
        {
            return 0;
        }

        for (var i = 0; i < events.Count; i++)
        {
            if (ReferenceEquals(events[i], selected))
            {
                return i;
            }
        }

        selected = null;
        return 0;
    }

    private IRenderable RenderList(
        IReadOnlyList<ForwardedEvent> events,
        IReadOnlyList<KeyValuePair<string, int>> counts,
        IReadOnlyList<string> errors)
    {
        var title = new Markup(
            $"[bold]IoT Hub events[/]   {Markup.Escape(string.Join(" · ", counts.Select(x => $"{x.Key} {x.Value}")))}" +
            "        [dim]↑↓ select   Enter details[/]");

        var table = new Table()
            .Border(TableBorder.Rounded)
            .Expand()
            .AddColumn("Time")
            .AddColumn("Event Hub")
            .AddColumn("Device")
            .AddColumn("Module")
            .AddColumn("Event");

        var index = GetSelectedIndex(events);
        var visibleRows = Math.Max(Height - ListChromeLines, 1);
        var first = Math.Max(index - visibleRows + 1, 0);

        foreach (var forwardedEvent in events.Skip(first).Take(visibleRows))
        {
            var style = ReferenceEquals(forwardedEvent, events.Count > 0 ? events[index] : null)
                ? new Style(decoration: Decoration.Invert)
                : Style.Plain;

            table.AddRow(
                new Text(forwardedEvent.Time.ToLocalTime().ToString("HH:mm:ss", CultureInfo.InvariantCulture), style),
                new Text(forwardedEvent.EventHubName, style),
                new Text(forwardedEvent.DeviceId ?? string.Empty, style),
                new Text(forwardedEvent.ModuleId ?? string.Empty, style),
                new Text(forwardedEvent.Summary, style));
        }

        // Each reader adds its Event Hub to the counts, so no counts means no Event Hubs to read.
        if (counts.Count == 0)
        {
            table.AddRow(new Markup("[dim]No Event Hubs to read. Run the viewer from the sample AppHost, which passes their connection strings.[/]"));
        }
        else if (events.Count == 0)
        {
            table.AddRow(new Markup("[dim]No events yet. Start the IoT Hub event bridge, or a module that sends telemetry.[/]"));
        }

        IRenderable footer = errors.Count == 0
            ? new Text(string.Empty)
            : new Markup($"[red]{Markup.Escape(string.Join("   ", errors))}[/]");

        return new Rows(title, table, footer);
    }

    private IRenderable RenderDetails(ForwardedEvent forwardedEvent)
    {
        // The properties come first: they are short, and a twin's body can take many screens.
        var lines = new List<IRenderable>();
        AddSection(lines, "Application properties", FormatProperties(forwardedEvent.ApplicationProperties));
        AddSection(lines, "System properties", FormatProperties(forwardedEvent.SystemProperties));
        AddSection(lines, "Body", FormatBody(forwardedEvent.Body));

        var page = Height - DetailsChromeLines;
        detailsScroll = Math.Min(detailsScroll, Math.Max(lines.Count - page, 0));

        var device = forwardedEvent.ModuleId is null
            ? forwardedEvent.DeviceId
            : $"{forwardedEvent.DeviceId} / {forwardedEvent.ModuleId}";

        var title = new Markup(
            $"[bold]{Markup.Escape(forwardedEvent.EventHubName)}[/] · {Markup.Escape(device ?? "unknown device")} · " +
            $"{forwardedEvent.Time.ToLocalTime().ToString("HH:mm:ss", CultureInfo.InvariantCulture)}" +
            "        [dim]↑↓ PgUp PgDn scroll   Esc back[/]");

        List<IRenderable> visible = [title, .. lines.Skip(detailsScroll).Take(page)];
        return new Rows(visible);
    }

    private static void AddSection(
        List<IRenderable> lines,
        string heading,
        IEnumerable<string> content)
    {
        lines.Add(new Text(string.Empty));
        lines.Add(new Markup($"[bold underline]{heading}[/]"));
        lines.AddRange(content.SelectMany(Wrap).Select(x => new Text(x)));
    }

    private static IEnumerable<string> FormatBody(string body)
    {
        if (body.Length == 0)
        {
            return ["(empty)"];
        }

        try
        {
            using var document = JsonDocument.Parse(body);
            return JsonSerializer.Serialize(document.RootElement, IndentedJson).ReplaceLineEndings("\n").Split('\n');
        }
        catch (JsonException)
        {
            return body.ReplaceLineEndings("\n").Split('\n');
        }
    }

    private static IEnumerable<string> FormatProperties(IReadOnlyList<KeyValuePair<string, string>> properties)
    {
        if (properties.Count == 0)
        {
            return ["(none)"];
        }

        var width = properties.Max(x => x.Key.Length) + 2;
        return properties.Select(x => x.Key.PadRight(width) + x.Value.ReplaceLineEndings(" "));
    }

    // Each line is cut to the terminal width, so a long line takes several lines instead of being wrapped by the terminal,
    // which keeps the scrolling exact.
    private static IEnumerable<string> Wrap(string line)
    {
        var width = Width - 1;
        if (line.Length <= width)
        {
            yield return line;
            yield break;
        }

        for (var start = 0; start < line.Length; start += width)
        {
            yield return line.Substring(start, Math.Min(width, line.Length - start));
        }
    }
}