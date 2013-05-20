using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Stations.Web.Models
{
    public class HandledStation : Entity
    {
        public Guid StationID { get; set; }

        public DateTime UpdateTime { get; set; }
    }
}