namespace Stations.Web.Extensions
{
    using System;
    using System.Web;

    /// <summary>
    /// Request extension.
    /// </summary>
    public static class RequestExtensions
    {
        /// <summary>
        /// Gets the name of the controller.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Controller name.</returns>
        public static String GetControllerName(this HttpRequestBase request)
        {
            return request.RequestContext.RouteData.GetRequiredString("controller");
        }

        /// <summary>
        /// Gets the name of the action.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Action name.</returns>
        public static String GetActionName(this HttpRequestBase request)
        {
            return request.RequestContext.RouteData.GetRequiredString("action");
        }
    }
}