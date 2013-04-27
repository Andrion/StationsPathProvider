namespace Stations.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Models;

    /// <summary>
    /// Stations controller.
    /// </summary>
    public class StationsController : BaseController
    {
        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Get()
        {
            using (DataContext dataContext = new DataContext())
            {
                IEnumerable<Object> stations = dataContext.Stations.OrderBy(q => q.Name)
                    .Select(q => new
                    {
                        Station = q,
                        Handled = dataContext.HandledStations.FirstOrDefault(h => h.StationID == q.ID)
                    })
                    .ToList();

                return this.Json(stations, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Set station info.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Set(Guid id, Double lat, Double lng)
        {
            using (DataContext dataContext = new DataContext())
            {
                Station station = dataContext.Stations.Find(id);
                if (station == null)
                {
                    return this.Json(new { success = false, message = "Station not found`" });
                }

                HandledStation handledStation = dataContext.HandledStations.FirstOrDefault(q => q.StationID == id);
                if (handledStation == null)
                {
                    handledStation = new HandledStation()
                    {
                        StationID = id,
                        UpdateTime = DateTime.Now
                    };

                    dataContext.HandledStations.Add(handledStation);
                }

                station.Lat = lat;
                station.Lng = lng;

                dataContext.SaveChanges();

                return this.Json(new { success = true });
            }
        }
    }
}
