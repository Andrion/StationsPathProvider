namespace Stations.Web.Models
{
    using System;

    /// <summary>
    /// Mapping Station - Google Station.
    /// </summary>
    public class MapGStation
    {
        public Guid StationID { get; set; }

        public Guid GStationID { get; set; }
    }
}