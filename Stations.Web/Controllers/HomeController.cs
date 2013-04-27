﻿namespace Stations.Web.Controllers
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
    using Stations.Web.Helpers;

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

        public ActionResult ParseBusTransports()
        {
            Task operationTask = Task.Run(() =>
            {
                this.DoParseRoutes(ProezdByUrlDictionary.BusRoutesList, "Автобусы", "transport?vt=a&t=avtobus_");
            });

            return this.View("Progress");
        }

        public ActionResult ParseTrolleybusTransports()
        {
            Task operationTask = Task.Run(() =>
            {
                this.DoParseRoutes(ProezdByUrlDictionary.TrolleybusRoutesList, "Троллейбусы", "transport?vt=t&t=trolleybus_");
            });

            return this.View("Progress");
        }

        private async Task DoParseRoutes(String routesListUrl, String transportTypeName, String hrefBegin)
        {
            Int32 handledRouteCount = 0;
            IHubContext progressHubContext = GlobalHost.ConnectionManager.GetHubContext<ProgressHub>();

            using (HttpClient client = new HttpClient())
            using (DataContext dataContext = new DataContext())
            {
                Guid busTypeId = dataContext.TransportTypes.First(x => x.Name.Equals(transportTypeName)).ID;
                String pageContent = await client.GetStringAsync(routesListUrl);

                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(pageContent);

                HtmlNode document = htmlDocument.DocumentNode;
                List<HtmlNode> busRouteLinks = document.QuerySelectorAll(".naprovlenie_odin_variant a").ToList();
                List<String> busRouteLinksHref = busRouteLinks.Select(busRoute => busRoute.GetAttributeValue("href", String.Empty)).ToList();

                // Extract route number
                List<String> routes = busRouteLinksHref.Select(x =>
                    {
                        String temString = x.Remove(0, hrefBegin.Length);
                        temString = temString.Split('&')[0];

                        return temString;
                    }).Distinct().ToList();

                foreach (String route in routes)
                {
                    Transport transport = new Transport
                    {
                        Name = route,
                        TypeID = busTypeId
                    };
                    dataContext.Transports.Add(transport);
                    dataContext.SaveChanges();

                    handledRouteCount++;

                    Info info = new Info()
                    {
                        Progress =
                            ((handledRouteCount == routes.Count) ? 1 : ((Single)handledRouteCount / routes.Count)) *
                                100,
                        Operation = route
                    };

                    progressHubContext.Clients.All.onProgress(info);
                }
            }
        }

        public ActionResult ParseDirections()
        {
            Task task = Task.Run(() =>
                {
                    this.DoParseDirections();
                });

            return this.View("Progress");
        }

        private async Task DoParseDirections()
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
                String pageContent = await client.GetStringAsync(ProezdByUrlDictionary.BusRoutesList);
                hrefs.AddRange(ExtractDirectionStationsPageUrl(pageContent));

                pageContent = await client.GetStringAsync(ProezdByUrlDictionary.TrolleybusRoutesList);
                hrefs.AddRange(ExtractDirectionStationsPageUrl(pageContent));

                foreach (Transport transport in transports)
                {
                    // Get transport type
                    TransportType transportType = dataContext.TransportTypes.First(x => x.ID.Equals(transport.TypeID));

                    switch (transportType.Name)
                    {
                        case ("Автобусы"):
                            {
                                #region Up direction

                                String upDirectionUrl = String.Format(ProezdByUrlDictionary.BusUpDirectionUrl, transport.Name);
                                if (hrefs.Contains(upDirectionUrl))
                                {
                                    pageContent = await client.GetStringAsync(upDirectionUrl);
                                    List<Int32> directionStationsCodes = ExtractDirectionStationsCodes(pageContent);

                                    // Create direction
                                    Direction direction = new Direction
                                    {
                                        Name = transport.Name,
                                        TransportID = transport.ID,
                                        StartID = stations.AsParallel().First(x => x.Code.Equals(directionStationsCodes.First())).ID,
                                        EndID = stations.AsParallel().First(x => x.Code.Equals(directionStationsCodes.Last())).ID
                                    };
                                    dataContext.Directions.Add(direction);
                                    dataContext.SaveChanges();

                                    // Create direction stations
                                    for (Int32 i = 0; i < directionStationsCodes.Count; i++)
                                    {
                                        dataContext.DirectionStations.Add(new DirectionStation
                                        {
                                            DirectionID = direction.ID,
                                            StationID = stations.AsParallel().First(x => x.Code.Equals(directionStationsCodes[i])).ID,
                                            Order = i
                                        });
                                    }
                                    dataContext.SaveChanges();
                                }

                                #endregion

                                #region Down direction

                                String downDirectionUrl = String.Format(ProezdByUrlDictionary.BusDownDirectionUrl, transport.Name);
                                if (hrefs.Contains(downDirectionUrl))
                                {
                                    pageContent = await client.GetStringAsync(downDirectionUrl);
                                    List<Int32> directionStationsCodes = ExtractDirectionStationsCodes(pageContent);

                                    // Create direction
                                    Direction direction = new Direction
                                    {
                                        Name = transport.Name,
                                        TransportID = transport.ID,
                                        StartID = stations.AsParallel().First(x => x.Code.Equals(directionStationsCodes.First())).ID,
                                        EndID = stations.AsParallel().First(x => x.Code.Equals(directionStationsCodes.Last())).ID
                                    };
                                    dataContext.Directions.Add(direction);
                                    dataContext.SaveChanges();

                                    // Create direction stations
                                    for (Int32 i = 0; i < directionStationsCodes.Count; i++)
                                    {
                                        dataContext.DirectionStations.Add(new DirectionStation
                                        {
                                            DirectionID = direction.ID,
                                            StationID = stations.AsParallel().First(x => x.Code.Equals(directionStationsCodes[i])).ID,
                                            Order = i
                                        });
                                    }
                                    dataContext.SaveChanges();
                                }

                                #endregion
                            } break;

                        case ("Троллейбусы"):
                            {
                                #region Up direction

                                String upDirectionUrl = String.Format(ProezdByUrlDictionary.TrolleybusUpDirectionUrl, transport.Name);
                                if (hrefs.Contains(upDirectionUrl))
                                {
                                    pageContent = await client.GetStringAsync(upDirectionUrl);
                                    List<Int32> directionStationsCodes = ExtractDirectionStationsCodes(pageContent);

                                    Direction direction = new Direction
                                    {
                                        Name = transport.Name,
                                        TransportID = transport.ID,
                                        StartID = stations.AsParallel().First(x => x.Code.Equals(directionStationsCodes.First())).ID,
                                        EndID = stations.AsParallel().First(x => x.Code.Equals(directionStationsCodes.Last())).ID
                                    };
                                    dataContext.Directions.Add(direction);
                                    dataContext.SaveChanges();

                                    // Create direction stations
                                    for (Int32 i = 0; i < directionStationsCodes.Count; i++)
                                    {
                                        dataContext.DirectionStations.Add(new DirectionStation
                                        {
                                            DirectionID = direction.ID,
                                            StationID = stations.AsParallel().First(x => x.Code.Equals(directionStationsCodes[i])).ID,
                                            Order = i
                                        });
                                    }
                                    dataContext.SaveChanges();
                                }

                                #endregion

                                #region Down direction

                                String downDirectionUrl = String.Format(ProezdByUrlDictionary.TrolleybusDownDirectionUrl, transport.Name);
                                if (hrefs.Contains(downDirectionUrl))
                                {
                                    pageContent = await client.GetStringAsync(downDirectionUrl);
                                    List<Int32> directionStationsCodes = ExtractDirectionStationsCodes(pageContent);

                                    Direction direction = new Direction
                                    {
                                        Name = transport.Name,
                                        TransportID = transport.ID,
                                        StartID = stations.AsParallel().First(x => x.Code.Equals(directionStationsCodes.First())).ID,
                                        EndID = stations.AsParallel().First(x => x.Code.Equals(directionStationsCodes.Last())).ID
                                    };
                                    dataContext.Directions.Add(direction);
                                    dataContext.SaveChanges();

                                    // Create direction stations
                                    for (Int32 i = 0; i < directionStationsCodes.Count; i++)
                                    {
                                        dataContext.DirectionStations.Add(new DirectionStation
                                        {
                                            DirectionID = direction.ID,
                                            StationID = stations.AsParallel().First(x => x.Code.Equals(directionStationsCodes[i])).ID,
                                            Order = i
                                        });
                                    }
                                    dataContext.SaveChanges();
                                }

                                #endregion
                            } break;
                    }

                    #region Handle progress

                    handledRouteCount++;

                    Info info = new Info()
                        {
                            Progress =
                                ((handledRouteCount == transports.Count)
                                     ? 1
                                     : ((Single) handledRouteCount/transports.Count))*
                                100,
                            Operation = transport.Name
                        };

                    progressHubContext.Clients.All.onProgress(info);

                    #endregion
                }
            }
        }

        private List<String> ExtractDirectionStationsPageUrl(String pageContent)
        {
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(pageContent);

            HtmlNode document = htmlDocument.DocumentNode;
            List<HtmlNode> routeLinks = document.QuerySelectorAll(".naprovlenie_odin_variant a").ToList();

            List<String> hrefs = routeLinks.Select(
                route => ProezdByUrlDictionary.BaseUrl + route.GetAttributeValue("href", String.Empty)).ToList();

            return hrefs;
        }

        private List<Int32> ExtractDirectionStationsCodes(String pageContent)
        {
            String hrefBegin = "transportstop?id=";

            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(pageContent);

            HtmlNode document = htmlDocument.DocumentNode;
            List<HtmlNode> stationsLinks = document.QuerySelectorAll(".vibirai ol a").ToList();

            List<String> hrefs = stationsLinks.Select(
                station => station.GetAttributeValue("href", String.Empty)).ToList();

            List<Int32> stationsCodes = hrefs.Select(x =>
                {
                    String temString = x.Remove(0, hrefBegin.Length);
                    temString = temString.Split('&')[0];

                    return Convert.ToInt32(temString);
                }).ToList();

            return stationsCodes;
        }
    }
}
