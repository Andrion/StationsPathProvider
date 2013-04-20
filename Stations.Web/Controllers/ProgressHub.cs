namespace Stations.Web.Controllers
{
    using Microsoft.AspNet.SignalR;
    using Microsoft.AspNet.SignalR.Hubs;

    /// <summary>
    /// Progress hub.
    /// </summary>
    [HubName("info")]
    public class ProgressHub : Hub
    {
    }
}