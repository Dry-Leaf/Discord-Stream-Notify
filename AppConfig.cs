using Microsoft.Extensions.Configuration;

namespace Chii
{
    public class Config
    {
        public required string ChannelId { get; set; }
        public required string CallbackUrl { get; set; }
        public required string YTApiKey { get; set; }
        public required string BotToken { get; set; }
        public required string ToNotify { get; set; }
        public required ulong RoomId { get; set; }
    }

    public static class AppConfig
    {
        public static Config Settings { get; private set; }

        static AppConfig()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            Settings = configuration.Get<Config>();
        }
    }
}


