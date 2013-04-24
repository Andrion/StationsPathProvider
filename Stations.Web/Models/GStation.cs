namespace Stations.Web.Models
{
    using System;

    /// <summary>
    /// Google station model.
    /// </summary>
    public class GStation : Entity
    {
        public String Name { get; set; }

        public Double Lat { get; set; }

        public Double Lng { get; set; }
    }
}