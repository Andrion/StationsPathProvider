namespace Stations.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net.Http;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using HtmlAgilityPack;
    using Microsoft.AspNet.SignalR;
    using Models;
    using Newtonsoft.Json;
    using Fizzler;
    using Fizzler.Systems.HtmlAgilityPack;
    using Group = Models.Group;

    /// <summary>
    /// Home controller.
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// Index action.
        /// </summary>
        /// <returns>Action result.</returns>
        public async Task<ActionResult> Index()
        {
            using (DataContext dataContext = new DataContext())
            {
                IEnumerable<String> transports = dataContext.Stations.Select(q => q.Name).ToList();

                return this.View(transports);
            }
        }

        public ActionResult ParseStations()
        {
            Task operationTask = Task.Run(() =>
            {
                this.DoParseStations();
            });

            return this.View("Progress");
        }

        public ActionResult ImportTransportTypes()
        {
            String[] transportTypes = new[]
            {
                "Автобусы",
                "Троллейбусы",
                "Маршрутные такси"
            };

            using (DataContext dataContext = new DataContext())
            {
                foreach (String typeName in transportTypes)
                {
                    TransportType type = new TransportType()
                    {
                        Name = typeName
                    };

                    dataContext.TransportTypes.Add(type);
                    dataContext.SaveChanges();
                }
            }

            return this.Content("Transport types imported");
        }

        private async void DoParseStations()
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync("http://proezd.by/js/localdata.js");

            String data = await response.Content.ReadAsStringAsync();

            Int32 start = data.IndexOf('[');
            Int32 length = data.LastIndexOf(']') - start + 1;

            data = data.Substring(start, length);

            Int32 handledStationCount = 0;

            List<String> stations = JsonConvert.DeserializeObject<List<String>>(data);

            IHubContext progressHubContext = GlobalHost.ConnectionManager.GetHubContext<ProgressHub>();

            using (DataContext dataContext = new DataContext())
            {
                foreach (String stationName in stations)
                {
                    String url = String.Format("http://proezd.by/stop?poisk_up={0}", stationName);

                    String stationHtml = await (await client.GetAsync(url)).Content.ReadAsStringAsync();

                    HtmlDocument html = new HtmlDocument();
                    html.LoadHtml(stationHtml);
                    HtmlNode document = html.DocumentNode;

                    HtmlNode listNode = document.QuerySelector(".spisok_na_vivod_33");

                    dataContext.SaveChanges();

                    foreach (HtmlNode infoNode in listNode.QuerySelectorAll(".vivod_33 a"))
                    {
                        String hrefAttr = infoNode.GetAttributeValue("href", String.Empty);
                        String name = infoNode.InnerText;

                        String description = infoNode
                            .QuerySelector(".opisianie_oostanovok").InnerText
                            .Replace("(", String.Empty)
                            .Replace(")", String.Empty);

                        String stationCodeStr = hrefAttr.Replace("?id=", String.Empty);
                        Int32 code = 0;
                        if (!Int32.TryParse(stationCodeStr, out code))
                        {
                            continue;
                        }

                        Station station = new Station()
                        {
                            Code = code,
                            Name = name,
                        };

                        dataContext.Stations.Add(station);
                        dataContext.SaveChanges();
                    }

                    handledStationCount++;

                    Info info = new Info()
                    {
                        Progress =
                            ((handledStationCount == stations.Count) ? 1 : ((Single) handledStationCount/stations.Count))*
                                100,
                        Operation = stationName
                    };

                    progressHubContext.Clients.All.onProgress(info);
                }
            }
        }

        public async Task ParseSchedule()
        {
            using (HttpClient client = new HttpClient())
            using (DataContext dataContext = new DataContext())
            {
                IEnumerable<Station> stations = dataContext.Stations.ToList();

                foreach (Station station in stations)
                {
                    String data = await client.GetStringAsync(String.Format("http://proezd.by/stop?id={0}", station.Code));

                    HtmlDocument htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(data);

                    HtmlNode document = htmlDocument.DocumentNode;

                    TransportType busType = dataContext.TransportTypes.FirstOrDefault(
                            q => q.Name.StartsWith("Автобус", StringComparison.InvariantCultureIgnoreCase));
                    if (busType != null)
                    {
                        HtmlNode busesNode = document.QuerySelector(".vivod_avtobusov_po_namber_ouan");

                        IEnumerable<HtmlNode> schRows = busesNode.QuerySelectorAll(".st_hr");

                        foreach (HtmlNode schRow in schRows)
                        {
                            HtmlNode transportInfoNode = schRow.QuerySelector(".naprovleniu_v_avtobusov_v_spiske_marshrut a");

                            String numbStr = transportInfoNode.GetAttributeValue("href", String.Empty);

                            String number = numbStr.Substring(numbStr.IndexOf('_') + 1);
                            String name = transportInfoNode.InnerText;

                            Transport transport = dataContext.Transports.FirstOrDefault(q => q.Name == number);
                            if (transport == null)
                            {
                                transport = new Transport()
                                {
                                    Name = number,
                                    TypeID = busType.ID
                                };
                                dataContext.SaveChanges();
                            }
                        }
                    }
                }
            }
        }
    }
}
