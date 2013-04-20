namespace Stations.Web.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Transport model.
    /// </summary>
    public class Transport
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }

        public String Number { get; set; }

        public Guid TypeID { get; set; }

        public String Name { get; set; }

        public Guid StartID { get; set; }

        public Guid EndID { get; set; }
    }
}