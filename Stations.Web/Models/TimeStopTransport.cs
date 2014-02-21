using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Stations.Web.Models
{
    public class TimeStopTransport
    {
        public Int32 Hour { get; set; }
        public Int32 Minute { get; set; }
        public Boolean IsWeekend { get; set; }
    }
}