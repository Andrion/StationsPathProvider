namespace Stations.Web.Models
{
    using System;

    /// <summary>
    /// Direction station model.
    /// </summary>
    public class DirectionStation : Entity
    {
        public Guid DirectionID { get; set; }

        public Guid StationID { get; set; }

        public Int32 Order { get; set; }
    }
}