using GMap.NET.MapProviders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Prekapcanje3
{
    // Define the data model for the route response JSON
    public class Route
    {
        public string type { get; set; }
        public List<Feature> Features { get; set; }
    }

    public class Feature
    {
        public double[] bbox { get; set; }
        public string type { get; set; }
        public Geometry Geometry { get; set; }
        public PropertiesCall Properties { get; set; }
    }

    public class Geometry
    {
        public string type { get; set; }
        public List<double[]> coordinates { get; set; }
    }

    public class PropertiesCall
    {
        public List<SubSegmentSummary> Segments { get; set; }
        public SegmentSummary summary;
    }

    public class Step
    {
        public double distance { get; set; }
        public double duration { get; set; }
        public int type { get; set; }
        public string instruction { get; set; }
        public string name { get; set; }
        //public double[] way_points { get; set; }

        public override string ToString()
        {
            return distance + ";" + duration + ";" + type + ";" + instruction + ";" + name;
        }
    }

    public class SubSegmentSummary
    {
        public double distance { get; set; }
        public double duration { get; set; }
        public List<Step> steps { get; set; }
    }

    public class SegmentSummary
    {
        public double distance { get; set; }
        public double duration { get; set; }
    }

    public static class GetRoute
    {
        public static bool compareRoutes(List<LonLatRoutePair> points, List<LonLatRoutePair> prevPoints)
        {
            if(points == null || prevPoints == null)
            {
                return false;
            }
            else if (points.Count != prevPoints.Count)
            {
                return false;
            }
            else
            {
                for(int i=0;i<points.Count; i++)
                {
                    if (!Misc.equalDoubleVals(points[i].start.Lng, prevPoints[i].start.Lng) ||
                        !Misc.equalDoubleVals(points[i].start.Lat, prevPoints[i].start.Lat) ||
                        !Misc.equalDoubleVals(points[i].end.Lng, prevPoints[i].end.Lng) ||
                        !Misc.equalDoubleVals(points[i].end.Lat, prevPoints[i].end.Lat))
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        public static List<Route> ComputeRoute(List<LonLatRoutePair> points)
        {
          
            // Define the API endpoint and parameters
            string apiUrl = "https://api.openrouteservice.org/v2/directions/driving-car";
            string apiKey = "YOUR_API_KEY";

            List<Route> routes = new List<Route>();
            try
            {
                // Create a new HttpClient instance with the API key header
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", apiKey);

                // Define the list of start and end coordinates
                var startPoints = new List<string>();
                var endPoints = new List<string>();
                for (int i = 0; i < points.Count; i++)
                {
                    LonLatRoutePair point = points[i];
                    startPoints.Add(point.start.Lng.ToString().Replace(',', '.') + "," + point.start.Lat.ToString().Replace(',', '.'));
                    endPoints.Add(point.end.Lng.ToString().Replace(',', '.') + "," + point.end.Lat.ToString().Replace(',', '.'));
                }

                double time = 0;
                double distance = 0;
                // Send a request to the API for each pair of start and end coordinates
                for (int i = 0; i < startPoints.Count; i++)
                {
                    // Define the request parameters
                    var parameters = new Dictionary<string, string>
                {
                    { "start", startPoints[i] },
                    { "end", endPoints[i] },
                     { "geometry", "true" },
                };

                    //Send the HTTP request
                    var ex = apiUrl + "?" + BuildQueryString(parameters);
                    var response = client.GetAsync(apiUrl + "?" + BuildQueryString(parameters)).Result;
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception("Error in get route: " + response.StatusCode);
                    }

                    ////Deserialize the response JSON into a Route object
                    string json = response.Content.ReadAsStringAsync().Result;
                    Globals.lastMinuteCalls.Add(DateTime.Now);
                    
                    Globals.counter += 1;
                    Route route = JsonConvert.DeserializeObject<Route>(json);

                    routes.Add(route);

                    for (int j = 0; j < route.Features.Count; j++)
                    {
                        Feature f = route.Features[j];
                        for (int k = 0; k < f.Properties.Segments.Count; k++)
                        {
                            SubSegmentSummary subs = f.Properties.Segments[k];
                            for (int l = 0; l < subs.steps.Count; l++)
                            {
                                Step s = subs.steps[l];
                                time += s.duration;
                                distance += s.distance;
                            }
                        }
                    }

                }
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return routes;
        }
        // Helper method to build a query string from a dictionary of parameters
        private static string BuildQueryString(Dictionary<string, string> parameters)
        {
            var queryString = new List<string>();
            foreach (var pair in parameters)
            {
                queryString.Add(pair.Key + "=" + pair.Value);
            }
            return string.Join("&", queryString);
        }
    }
}
