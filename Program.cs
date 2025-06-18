using NetCord;
using NetCord.Gateway;
using NetCord.Logging;
using Chii;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;

GatewayClient client = new(new BotToken(AppConfig.Settings.BotToken), new GatewayClientConfiguration
{
    Logger = new ConsoleLogger(),
});

await client.StartAsync();

//await client.Rest.SendMessageAsync(AppConfig.Settings.RoomId, $"<{AppConfig.Settings.ToNotify}>");

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddControllers();
builder.Services.AddSingleton<FeedNotificationService>();

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = 
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

await PubSubResubscribe.SubscribeAsync();

var timer = new PeriodicTimer(TimeSpan.FromDays(2));

_ = Task.Run(async () =>
{
    while (await timer.WaitForNextTickAsync())
    {
        await PubSubResubscribe.SubscribeAsync();
    }
});

var checker = new YoutubeVideoChecker(AppConfig.Settings.YTApiKey);

// Listening to the feed
string current_id = "";
var feedService = app.Services.GetRequiredService<FeedNotificationService>();
feedService.OnNotificationReceived += async (sender, args) =>
{
    Console.WriteLine("🔔 New Atom feed received!");
    //Console.WriteLine($"XML: {args.XmlPayload}");

    // Example: parse feed title
    var doc = System.Xml.Linq.XDocument.Parse(args.XmlPayload);
    var entries = doc.Descendants("{http://www.w3.org/2005/Atom}entry");
    foreach (var entry in entries)
    {
        var vid_id = entry.Element("{http://www.youtube.com/xml/schemas/2015}videoId")?.Value;
        if (vid_id != current_id) {
            current_id = vid_id;
        } else {
            continue;
        }

        var vid_type = await checker.GetVideoTypeAsync(vid_id);
	if (vid_type == YoutubeVideoType.LivestreamReplay || vid_type == YoutubeVideoType.Upload) {
            continue;
        }
	Console.WriteLine($"Video Type: {vid_type}");

        var title = entry.Element("{http://www.w3.org/2005/Atom}title")?.Value;
        Console.WriteLine($"Entry Title: {title}");

        string url = $"https://www.youtube.com/watch?v={vid_id}";
        Console.WriteLine($"Entry Id: {url}");

        await client.Rest.SendMessageAsync(AppConfig.Settings.RoomId, $"<{AppConfig.Settings.ToNotify}> {title}\n{url}");
    }
};

app.MapControllers();

app.Run();
