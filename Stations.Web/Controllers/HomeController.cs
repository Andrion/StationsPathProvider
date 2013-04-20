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
                IEnumerable<String> transports = dataContext.Stations.Select(q => q.FullName).ToList();

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

        public ActionResult ParseIFStations()
        {
            Task operationTask = Task.Run(() =>
            {
                this.DoParseIFStations();
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

        private async Task DoParseStations()
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

                    Group group = new Group()
                    {
                        Name = stationName
                    };

                    dataContext.Groups.Add(group);
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
                            GroupID = group.ID,
                            Code = code,
                            Name = description,
                            FullName = name
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

        private async Task DoParseIFStations()
        {
            HttpClient client = new HttpClient();
            IHubContext progressHubContext = GlobalHost.ConnectionManager.GetHubContext<ProgressHub>();

            Int32 handledStationCount = 0;

            Int32 start = 90001;
            Int32 end = 90657;

            using (DataContext dataContext = new DataContext())
            {
                for (Int32 i = start; i <= end; i++)
                {
                    String url = String.Format("http://maps.interfax.by/details/feed.view?city_id=8&type=1&object_id={0}", i);

                    String htmlContent = await client.GetStringAsync(url);

                    HtmlDocument html = new HtmlDocument();
                    html.LoadHtml(htmlContent);

                    HtmlNode document = html.DocumentNode;

                    handledStationCount++;

                    if (document.QuerySelector(".info-panel") != null)
                    {
                        continue;
                    }

                    HtmlNode infoBlock = document.QuerySelector(".nav-block");

                    String stationName = infoBlock.QuerySelector("h2").InnerText;

                    String stationCode = infoBlock.QuerySelector("p").InnerText;
                    stationCode = Regex.Match(stationCode, @"№(?<code>\w+)").Groups["code"].Value;

                    Int32 code = 0;
                    Int32.TryParse(stationCode, out code);

                    String coordStr = document.QuerySelector("a.transport").GetAttributeValue("href", String.Empty);
                    Match latLngMatch = Regex.Match(coordStr, @"transport_b=(?<lng>\d+\.\d+);(?<lat>\d+\.\d+)&");

                    Double lat = 0;
                    Double lng = 0;

                    Double.TryParse(latLngMatch.Groups["lat"].Value.Replace('.', ','), out lat);
                    Double.TryParse(latLngMatch.Groups["lng"].Value.Replace('.', ','), out lng);
                    
                    IFStation station = new IFStation()
                    {
                        Code = code,
                        IFID = i,
                        Lat = lat,
                        Lng = lng,
                        Name = stationName
                    };

                    dataContext.IFStations.Add(station);
                    dataContext.SaveChanges();

                    Info info = new Info()
                    {
                        Progress = ((handledStationCount + start == end) ? 1 : ((Single) handledStationCount / (end - start)))*100,
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

                            Transport transport = dataContext.Transports.FirstOrDefault(q => q.Number == number);
                            if (transport == null)
                            {
                                transport = new Transport()
                                {
                                    Name = name,
                                    Number = number,
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
