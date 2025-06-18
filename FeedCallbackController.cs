using Microsoft.AspNetCore.Mvc;

namespace Chii;

[ApiController]
[Route("api/feeds")]
public class FeedCallbackController : ControllerBase
{
    private readonly FeedNotificationService _feedService;

    public FeedCallbackController(FeedNotificationService feedService)
    {
        _feedService = feedService;
    }

    [HttpPost("callback")]
    public async Task<IActionResult> ReceiveNotification()
    {
	Console.WriteLine("Notification Received");
        using var reader = new StreamReader(Request.Body);
        var xml = await reader.ReadToEndAsync();

        // Raise the event
        _feedService.HandleNotification(xml);

        return Ok();
    }

    [HttpGet("callback")]
    public IActionResult VerifySubscription([FromQuery(Name = "hub.challenge")] string challenge)
    {
        Console.WriteLine("Verification Request Received");
        return Content(challenge, "text/plain");
    }
}
