namespace Stations.Web.Controllers
{
    using System.Web.Mvc;

    /// <summary>
    /// Home controller.
    /// </summary>
    public class HomeController : BaseController
    {
        /// <summary>
        /// Index action.
        /// </summary>
        /// <returns>Action result.</returns>
        public ActionResult Index()
        {
            return this.View();
        }

        public async Task<ActionResult> ParseTime()
        {
            Task task = Task.Run(() =>
            {
                this.DoParseTime();
            });

            return this.View("Progress");
        }


        private async Task DoParseTime()
        {
            Int32 handledRouteCount = 0;
            IHubContext progressHubContext = GlobalHost.ConnectionManager.GetHubContext<ProgressHub>();

            using (HttpClient client = new HttpClient())
            using (DataContext dataContext = new DataContext())
            {
                // Get all transports
                List<Transport> transports = dataContext.Transports.ToList();
                List<Station> stations = dataContext.Stations.ToList<Station>();

                // Get href attributes for bus
                List<String> hrefs = new List<String>();

                foreach (Transport transport in transports)
                {
                    foreach (Station station in stations)
                    {
                        String timeUrl = String.Format(ProezdByUrlDictionary.BusUpSheduleUrl, station.Code, transport.Name);
                        String pageContent = await client.GetStringAsync(timeUrl);

                        List<TimeStopTransport> dateTimes = getTime(pageContent);

                        if (dateTimes.Count > 0)
                        {
                            Schedule scheduleWorkDays = new Schedule
                            {
                                TransportID = transport.ID,
                                StationID = station.ID,
                                IsWeekend = false
                            };

                            Schedule scheduleWeekend = new Schedule
                            {
                                TransportID = transport.ID,
                                StationID = station.ID,
                                IsWeekend = true
                            };

                            dataContext.Schedules.Add(scheduleWorkDays);
                            dataContext.Schedules.Add(scheduleWeekend);
                            dataContext.SaveChanges();

                            foreach (TimeStopTransport item in dateTimes)
                            {
                                ScheduleItem scheduleItem = new ScheduleItem
                                {
                                    ScheduleID = item.IsWeekend ? scheduleWeekend.ID : scheduleWorkDays.ID,
                                    Time = new DateTime(2013, 5, 5, item.Hour, item.Minute, 0)
                                };
                                dataContext.ScheduleItems.Add(scheduleItem);
                                dataContext.SaveChanges();
                            }
                        }
                    }

                    #region Handle progress

                    handledRouteCount++;

                    Info info = new Info()
                    {
                        Progress =
                            ((handledRouteCount == transports.Count)
                                 ? 1
                                 : ((Single)handledRouteCount / transports.Count)) *
                            100,
                        Operation = transport.Name
                    };

                    progressHubContext.Clients.All.onProgress(info);

                    #endregion
                }
            }
        }

        private List<TimeStopTransport> getTime(String content)
        {
            List<TimeStopTransport> dateTimes = new List<TimeStopTransport>();

            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(content);

            HtmlNode document = htmlDocument.DocumentNode;
            List<HtmlNode> transportStopedTimeWorkedDay = document.QuerySelectorAll(".budni_vremia").ToList();
            List<HtmlNode> transportStopedTimeWeekend = document.QuerySelectorAll(".vihodnoi_vremia").ToList();

            if (transportStopedTimeWorkedDay.Count > 0)
            {
                List<HtmlNode> times = transportStopedTimeWorkedDay[0].ChildNodes.ToList();

                Int32 hour = 0;
                Int32 minute = 0;

                foreach (HtmlNode item in times)
                {
                    if (item.Attributes.Count>0 && item.Attributes["class"].Value.ToLower().Equals("h"))
                    {
                        if(Int32.TryParse(item.InnerText, out hour))
                        {
                            continue;
                        }
                    }

                    if (item.Attributes.Count > 0 && item.Attributes["class"].Value.ToLower().Equals("m"))
                    {
                        String maybeM = item.InnerText[item.InnerText.Length - 1] == ',' ? item.InnerText.Remove(item.InnerText.Length - 1) : item.InnerText;
                        if (Int32.TryParse(maybeM, out minute))
                        {
                            dateTimes.Add(new TimeStopTransport { Hour = hour, Minute = minute, IsWeekend = false });
                        }
                    }
                }
            }

            if (transportStopedTimeWeekend.Count > 0)
            {
                List<HtmlNode> times = transportStopedTimeWeekend[0].ChildNodes.ToList();

                Int32 hour = 0;
                Int32 minute = 0;

                foreach (HtmlNode item in times)
                {
                    if (item.Attributes.Count > 0 && item.Attributes["class"].Value.ToLower().Equals("h"))
                    {
                        if (Int32.TryParse(item.InnerText, out hour))
                        {
                            continue;
                        }
                    }

                    if (item.Attributes.Count > 0 && item.Attributes["class"].Value.ToLower().Equals("m"))
                    {
                        String maybeM = item.InnerText[item.InnerText.Length - 1] == ',' ? item.InnerText.Remove(item.InnerText.Length - 1) : item.InnerText;
                        if (Int32.TryParse(maybeM, out minute))
                        {
                            dateTimes.Add(new TimeStopTransport { Hour = hour, Minute = minute, IsWeekend = true });
                        }
                    }
                }
            }

            return dateTimes;
        }
    }
}
