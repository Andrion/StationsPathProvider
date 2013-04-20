using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Stations.Web.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class IFStation
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }

        public Int32 Code { get; set; }

        public Int32 IFID { get; set; }

        public String Name { get; set; }

        public Double Lat { get; set; }

        public Double Lng { get; set; }
    }
}