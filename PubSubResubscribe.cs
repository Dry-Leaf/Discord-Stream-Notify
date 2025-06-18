using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Chii;

public class PubSubResubscribe
{
    private static readonly HttpClient http = new HttpClient();

    public static async Task SubscribeAsync()
    {
        var values = new Dictionary<string, string>
        {
            { "hub.callback", AppConfig.Settings.CallbackUrl },
            { "hub.topic", $"https://www.youtube.com/xml/feeds/videos.xml?channel_id={AppConfig.Settings.ChannelId}" },
            { "hub.mode", "subscribe" },
            { "hub.verify", "async" }
        };

        var content = new FormUrlEncodedContent(values);

        var response = await http.PostAsync("https://pubsubhubbub.appspot.com/subscribe", content);
        response.EnsureSuccessStatusCode();
    }
}
