using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System.Threading.Tasks;

namespace Chii;

public class YoutubeVideoChecker
{
    private readonly YouTubeService _youtubeService;

    public YoutubeVideoChecker(string apiKey)
    {
        _youtubeService = new YouTubeService(new BaseClientService.Initializer()
        {
            ApiKey = apiKey,
            ApplicationName = "MyYoutubeCheckerApp"
        });
    }

    public async Task<YoutubeVideoType> GetVideoTypeAsync(string videoId)
    {
        var request = _youtubeService.Videos.List("snippet,liveStreamingDetails");
        request.Id = videoId;

        var response = await request.ExecuteAsync();

        if (response.Items.Count == 0)
        {
            //throw new Exception($"Video with ID '{videoId}' not found");
            return YoutubeVideoType.Upload;
        }

        var video = response.Items[0];

        // If 'liveStreamingDetails' is not null, it is or was a livestream.
        if (video.LiveStreamingDetails != null)
        {
            if (video.LiveStreamingDetails.ActualEndTimeDateTimeOffset != null) {
                return YoutubeVideoType.LivestreamReplay;
            }
            if (video.LiveStreamingDetails.ActualStartTimeDateTimeOffset != null) {
                return YoutubeVideoType.LivestreamInProgress;
            }
            return YoutubeVideoType.LivestreamScheduled;
        }

        // Otherwise, it's a normal uploaded video.
        return YoutubeVideoType.Upload;
    }
}

public enum YoutubeVideoType
{
    Upload,
    LivestreamScheduled,
    LivestreamInProgress,
    LivestreamReplay
}
