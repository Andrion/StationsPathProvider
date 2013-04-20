namespace Stations.Web.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Station model.
    /// </summary>
    public class Station
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }

        public Guid GroupID { get; set; }

        public Int32 Code { get; set; }

        public String Name { get; set; }

        public String FullName { get; set; }
    }
}