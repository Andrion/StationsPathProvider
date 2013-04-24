namespace Stations.Web.Models
{
    using System;

    /// <summary>
    /// Transport model.
    /// </summary>
    public class Transport : Entity
    {
        public Guid TypeID { get; set; }

        public String Name { get; set; }
    }
}