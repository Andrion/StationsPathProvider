namespace Stations.Web.Models
{
    using System;

    /// <summary>
    /// Direction model.
    /// </summary>
    public class Direction : Entity
    {
        public String Name { get; set; }

        public Guid TransportID { get; set; }

        public Guid StartID { get; set; }

        public Guid EndID { get; set; }
    }
}