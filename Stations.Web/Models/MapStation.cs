namespace Stations.Web.Models
{
    using System;

    /// <summary>
    /// Map station model.
    /// </summary>
    public class MapStation : Entity
    {
        public String Name { get; set; }

        public Double Lat { get; set; }

        public Double Lng { get; set; }
    }
}