namespace Stations.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Models;

    /// <summary>
    /// Admin controller.
    /// </summary>
    public class AdminController : BaseController
    {
        #region Actions

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

        public ActionResult Replacements()
        {
            return this.View();
        }

        public ActionResult GetReplacements()
        {
            using (DataContext dataContext = new DataContext())
            {
                IEnumerable<Replacement> replacements = dataContext.Replacements.ToList();

                return this.Json(replacements, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult SetReplacement(String value, String replace)
        {
            using (DataContext dataContext = new DataContext())
            {
                Replacement replacement = dataContext.Replacements.FirstOrDefault(q => q.Value == value);
                if (replacement == null)
                {
                    replacement = new Replacement();
                    dataContext.Replacements.Add(replacement);
                }

                replacement.Value = value;
                replacement.ReplaceValue = replace;

                dataContext.SaveChanges();

                return this.Json(new { success = true });
            }
        }

        #endregion Actions
    }
}
