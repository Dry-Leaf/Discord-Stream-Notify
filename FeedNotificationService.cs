namespace Chii;

public class FeedNotificationEventArgs : EventArgs
{
    public string XmlPayload { get; }

    public FeedNotificationEventArgs(string xmlPayload)
    {
        XmlPayload = xmlPayload;
    }
}

public class FeedNotificationService
{
    public event EventHandler<FeedNotificationEventArgs>? OnNotificationReceived;

    public void HandleNotification(string xml)
    {
        OnNotificationReceived?.Invoke(this, new FeedNotificationEventArgs(xml));
    }
}