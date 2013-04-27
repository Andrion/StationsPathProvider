namespace Stations.Web.Controllers
{
    using System.Web.Mvc;

    /// <summary>
    /// Admin controller.
    /// </summary>
    public class AdminController : BaseController
    {
        public ActionResult MapGStations()
        {
            return this.View();
        }

        public ActionResult MAPYStations()
        {
            return this.View();
        }

        public ActionResult TestMap()
        {
            return this.View();
        }
    }
}
