namespace Stations.Web.Models
{
    using System;

    /// <summary>
    /// Schedule item.
    /// </summary>
    public class ScheduleItem : Entity
    {
        public Guid ScheduleID { get; set; }

        public DateTime Time { get; set; }
    }
}