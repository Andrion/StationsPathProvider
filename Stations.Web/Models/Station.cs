namespace Stations.Web.Models
{
    using System;

    /// <summary>
    /// Station model.
    /// </summary>
    public class Station : Entity
    {
        public Int32 Code { get; set; }

        public String Name { get; set; }

        public Double Lat { get; set; }

        public Double Lng { get; set; }
    }
}