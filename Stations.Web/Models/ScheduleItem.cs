namespace Stations.Web.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Schedule item.
    /// </summary>
    public class ScheduleItem
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }

        public Guid StationID { get; set; }

        public DateTime Time { get; set; }
    }
}