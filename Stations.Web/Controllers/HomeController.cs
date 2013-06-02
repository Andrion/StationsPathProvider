namespace Stations.Web.Controllers
{
    using System.Web.Mvc;

    /// <summary>
    /// Home controller.
    /// </summary>
    public class HomeController : BaseController
    {
        /// <summary>
        /// Index action.
        /// </summary>
        /// <returns>Action result.</returns>
        public ActionResult Index()
        {
            return this.View();
        }
    }
}
