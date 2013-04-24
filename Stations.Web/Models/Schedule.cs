namespace Stations.Web.Models
{
    using System;

    /// <summary>
    /// Schedule model.
    /// </summary>
    public class Schedule : Entity
    {
        public Guid TransportID { get; set; }

        public Guid StationID { get; set; }

        public Boolean IsWeekend { get; set; }
    }
}