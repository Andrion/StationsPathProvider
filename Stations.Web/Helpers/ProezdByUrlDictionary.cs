using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Stations.Web.Helpers
{
    public static class ProezdByUrlDictionary
    {
        public static String BusRoutesList = @"http://proezd.by/transport?vt=a";

        public static String TrolleybusRoutesList = @"http://proezd.by/transport?vt=t";
        
        /// <summary>
        /// Format string
        /// {0} - bus number
        /// </summary>
        public static String BusStationsListDirectionUp = @"http://proezd.by/transport?vt=a&t=avtobus_{0}&n=napravlenie_up";
    }
}