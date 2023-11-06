using GMap.NET;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.WindowsForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET.MapProviders;
using static GMap.NET.Entity.OpenStreetMapRouteEntity;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;
using System.Drawing.Drawing2D;
using System.Xml.Linq;

namespace Prekapcanje3
{
    public partial class Form1 : Form
    {
        private int numChecksAll = 0;
        private int numChecksTres = 0;
        private int numChecksTres2 = 0;
        private Point mouseDownLocation;
        public int discPeriodMin=10;
        private bool isDragging = false;
        Vehicle v1, v2;
        // Create a new overlay for the route
        GMapOverlay routeOverlay;
        GMapOverlay addItionalRouteOverlay;

        GMapOverlay markersOverlay;

        GMapOverlay circlesOverlay;

        DateTime currentTime;
        List<GMapMarker> plotedMarkes= new List<GMapMarker>();
        public Form1()
        {
            Globals.counter = 0;
            Globals.lastMinuteCalls = new List<DateTime>();
            InitializeComponent();
            gmap.MapProvider = OpenStreetMapProvider.Instance;
            gmap.Position = new PointLatLng(49.7913, 9.9534); // Set initial position
            gmap.MinZoom = 1;
            gmap.MaxZoom = 18;
            gmap.Zoom = 6;

            routeOverlay = new GMapOverlay("route");
            markersOverlay = new GMapOverlay("markersOverlay");
            circlesOverlay= new GMapOverlay("circlesOverlay");
            addItionalRouteOverlay = new GMapOverlay("addItionalRouteOverlay");
            // Add the overlay to the map control
            gmap.Overlays.Add(routeOverlay);
            gmap.Overlays.Add(markersOverlay);
            gmap.Overlays.Add(circlesOverlay);
            gmap.Overlays.Add(addItionalRouteOverlay);
        }

        private Change spatioTemporallClose(Vehicle v1, Vehicle v2, DateTime timeInFuture)
        {
            int indV1 = v1.getIntervalIndex(timeInFuture);
            int indV2 = v2.getIntervalIndex(timeInFuture);
            numChecksAll++;
            if (indV1 == -1 || indV2 == -1)
            {
                //Cannot perform switch
                return null;
            }
            else
            {
                numChecksTres++;
                AggStep stepV1 = v1.planningHorizon[indV1];
                AggStep stepV2 = v2.planningHorizon[indV2];
                double distance = Misc.airalDistHaversine(stepV1.startLonLat.Lng, stepV1.startLonLat.Lat, stepV2.startLonLat.Lng, stepV2.startLonLat.Lat);
                if (distance > 100000)
                {
                    return null;
                }
                else
                {
                    numChecksTres2++;
                    //Evaluate the change

                    // Set the radius of the circle in meters (50 km = 50000 meters)
                    double radius = 50000;
                    PointLatLng center = new PointLatLng((stepV1.startLonLat.Lat+ stepV2.startLonLat.Lat)/2, (stepV1.startLonLat.Lng + stepV2.startLonLat.Lng) / 2);
                    // Create a list of points that make up the circle
                    List<PointLatLng> points = new List<PointLatLng>();
                    for (int i = 0; i <= 360; i++)
                    {
                        double angle = i * Math.PI / 180;
                        double lat = center.Lat + (radius / 111000) * Math.Sin(angle);
                        double lng = center.Lng + (radius / (111000 * Math.Cos(center.Lat * Math.PI / 180))) * Math.Cos(angle);
                        points.Add(new PointLatLng(lat, lng));
                    }

                    // Create a new GMapPolygon using the list of points
                    GMapPolygon circle = new GMapPolygon(points, "circle");

                    // Set the color and opacity of the circle
                    circle.Fill = new SolidBrush(Color.FromArgb(50, Color.PaleVioletRed));
                    circle.Stroke = new Pen(Color.PaleVioletRed, 2);

                    // Add the circle to the overlay
                    circlesOverlay.Polygons.Add(circle);
                    List<PointLatLng> points1=null;
                    List<PointLatLng> points2 = null;
                    DateTime t1= v1.evaluateChange(stepV1, v2, stepV2, ref points1);
                    DateTime t2 = v2.evaluateChange(stepV2, v1, stepV1, ref points2);
                    Change c = new Change(t1,t2, v1, v2,points1,points2);
                    return c;
                }
            }
        }

        private void gmap_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseDownLocation = e.Location;
                isDragging = true;
            }
        }

        private void gmap_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point newLocation = e.Location;
                int deltaX = newLocation.X - mouseDownLocation.X;
                int deltaY = newLocation.Y - mouseDownLocation.Y;

                gmap.Position = new PointLatLng(gmap.Position.Lat + deltaY / gmap.Zoom / 20.0,
                    gmap.Position.Lng - deltaX / gmap.Zoom / 10.0);
            }
        }

        private void gmap_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
            }
        }

        public GMapRoute addRouteOnMap(IEnumerable<PointLatLng> points, Pen style, System.Drawing.Drawing2D.DashStyle dashStyle, GMapOverlay routeOverlayArg)
        {          
            GMapRoute route = new GMapRoute(points, "route");
            // Set the properties of the route
            route.Stroke = style;
            route.Stroke.DashStyle = dashStyle;
            // Add the route to the overlay
            routeOverlayArg.Routes.Add(route);
            return route;
        }

        public GMapMarker addMarkerOnMap(PointLatLng point, string name, GMarkerGoogleType typeMarker)
        {
            GMapMarker startMarker = new GMarkerGoogle(point, typeMarker);
            startMarker.ToolTipText = name;
            markersOverlay.Markers.Add(startMarker);
            return startMarker;
        }

        public void addRouteMarkersOnMap(PointLatLng start, PointLatLng end, string name)
        {
            addMarkerOnMap(start, "Begin for " + name + "!", GMarkerGoogleType.green);
            addMarkerOnMap(end, "End for " + name + "!", GMarkerGoogleType.red);
        }

        private void btnUcitaj_Click_1(object sender, EventArgs e)
        {
            try
            {
                //Replace files ST1 and ST2 with your own data
                v1 = new Vehicle("ST1.csv", discPeriodMin);
                //addRouteOnMap(v1.gpsRoute.Select(x => x.point), new Pen(Color.Black, 3), System.Drawing.Drawing2D.DashStyle.Solid,routeOverlay);
                //addRouteOnMap(v1.locationsFromReport.Select(x => x.point), new Pen(Color.Purple, 3));
                addRouteOnMap(v1.routePointsBetweenLocationsInReport, new Pen(Color.Red, 3), System.Drawing.Drawing2D.DashStyle.Solid, routeOverlay);
                addRouteMarkersOnMap(v1.routePointsBetweenLocationsInReport.First(), v1.routePointsBetweenLocationsInReport.Last(), v1.name);
                v2 = new Vehicle("ST2.csv", discPeriodMin);
                //addRouteOnMap(v2.gpsRoute.Select(x => x.point), new Pen(Color.Black, 3), System.Drawing.Drawing2D.DashStyle.Solid, routeOverlay);
                //addRouteOnMap(v2.locationsFromReport.Select(x => x.point), new Pen(Color.Magenta, 3));
                addRouteOnMap(v2.routePointsBetweenLocationsInReport, new Pen(Color.Blue, 3), System.Drawing.Drawing2D.DashStyle.Solid, routeOverlay);
                addRouteMarkersOnMap(v2.routePointsBetweenLocationsInReport.First(), v2.routePointsBetweenLocationsInReport.Last(), v2.name);

                //Postavi trenutno vrijeme
                currentTime = v1.startTimePred;
                if (DateTime.Compare(v2.startTimePred, currentTime) < 0)
                {
                    currentTime = v2.startTimePred;
                }
                txtCurrentEvalTime.Text = currentTime.ToString();
                if (checkPrekapcanje.Checked)
                {
                    checkAllSwitchsInFuture();
                }
                //labNumCalls.Text = Globals.counter.ToString();
                enumerateLastMinute();

                txtVozilo1EndTime.Text = "30.1.2023. 1:20:00";
                txtVozilo2EndTime.Text = "28.1.2023. 1:40:00";
                rtxtEvaluiranaPrekapcanja.Text = "No possible exchange points!";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void enumerateLastMinute()
        {
            DateTime dtNow = DateTime.Now;
            for(int i=Globals.lastMinuteCalls.Count-1; i>=0; i--)
            {
                if((dtNow- Globals.lastMinuteCalls[i]).TotalMinutes > 1)
                {
                    Globals.lastMinuteCalls.RemoveAt(i);
                }
            }
            //labLastMin.Text = Globals.lastMinuteCalls.Count.ToString();
        }


        public void checkAllSwitchsInFuture()
        {
            //check all possible switchs....TODO
            DateTime timeInFuture = Misc.newDateTime(currentTime);
            List<Change> changeList = new List<Change>();
            circlesOverlay.Polygons.Clear();
            int possibleSwitches = 0;
            while (true)
            {
                if (DateTime.Compare(timeInFuture, v1.endTimePred) >= 0 || DateTime.Compare(timeInFuture, v2.endTimePred) >= 0)
                {
                    break;
                }
                Change change = spatioTemporallClose(v1, v2, timeInFuture);
                if (change!=null)
                {
                    if (change.savings > 0)
                    {
                        changeList.Add(change);
                    }
                    else
                    {
                        possibleSwitches++;
                    }
                }
                timeInFuture = timeInFuture.AddMinutes(60);
            }

            rtxtEvaluiranaPrekapcanja.Text = "";

            if (possibleSwitches>0)
            {
                rtxtEvaluiranaPrekapcanja.Text = possibleSwitches+" possible exchanges but without savings in duration!";
            }
            else if (changeList.Count == 0)
            {
                rtxtEvaluiranaPrekapcanja.Text = "No possible exchnage point!";
            }
            else
            {
                rtxtEvaluiranaPrekapcanja.Text = "Total number of exchange points:" + changeList.Count + "!\n";
                changeList.Sort((x, y) => x.savings.CompareTo(y.savings));
                foreach(Change change in changeList)
                {
                    rtxtEvaluiranaPrekapcanja.Text += "Savings: " + Math.Round(change.savings,2)+" h\n ST1: " + change.v2EndTimePred.ToString() + "\nST2: " + change.v1EndTimePred + "\n-------------------\n";
                }
                Change best = changeList[0];
                addRouteOnMap(best.v1Points, new Pen(Color.Pink, 7), DashStyle.Dot, addItionalRouteOverlay);
                addRouteOnMap(best.v2Points, new Pen(Color.Yellow, 7), DashStyle.Dot, addItionalRouteOverlay);
            }

        }

        private void timerRealTime_Tick(object sender, EventArgs e)
        {
            addItionalRouteOverlay.Routes.Clear();
            if (v1.routePredicted != null)
            {
                routeOverlay.Routes.Remove(v1.routePredicted);
                v1.routePredicted = null;
            }

            if (v1.currentMarker != null)
            {
                markersOverlay.Markers.Remove(v1.currentMarker);
                v1.currentMarker = null;
            }
            if (v2.routePredicted != null)
            {
                routeOverlay.Routes.Remove(v2.routePredicted);
                v2.routePredicted = null;
            }

            if (v2.currentMarker != null)
            {
                markersOverlay.Markers.Remove(v2.currentMarker);
                v2.currentMarker = null;
            }
            if (plotedMarkes.Count > 0)
            {
                foreach(GMapMarker mark in plotedMarkes)
                {
                    markersOverlay.Markers.Remove(mark);
                }
                plotedMarkes.Clear();
            }

            DateTime currentEnd = Misc.newDateTime(currentTime);
            currentEnd=currentEnd.AddHours(1);
            txtCurrentEvalTime.Text = currentTime.ToString();
            List<GpsData> traveresedV1 = v1.gpsRoute.Where(x => (DateTime.Compare(x.timeStamp, currentTime) >= 0 && DateTime.Compare(x.timeStamp, currentEnd) <= 0)).ToList();
            List<GpsData> traveresedV2 = v2.gpsRoute.Where(x => (DateTime.Compare(x.timeStamp, currentTime) >= 0 && DateTime.Compare(x.timeStamp, currentEnd) <= 0)).ToList();
            if (traveresedV1.Count == 0 && traveresedV2.Count == 0)
            {
                timerRealTime.Enabled = false;
                return;
            }
            procesRoutes(traveresedV1, v1, new Pen(Color.Red, 4));
            procesRoutes(traveresedV2, v2, new Pen(Color.Blue, 4));
            

            txtVozilo1EndTime.Text = v1.endTimePred.ToString();
            //txtRealV1.Text = v1.endTimeReal.ToString();

            txtVozilo2EndTime.Text = v2.endTimePred.ToString();
            //txtRealV2.Text = v2.endTimeReal.ToString();

            if (checkPrekapcanje.Checked)
            {
                checkAllSwitchsInFuture();
            }
            //labNumCalls.Text = Globals.counter.ToString();
            enumerateLastMinute();
            currentTime = currentEnd;
            //timerRealTime.Enabled = false;
        }

        public void procesRoutes(List<GpsData> traversedRoute, Vehicle v, Pen style)
        {

            if (traversedRoute.Count > 0)
            {
                obradiRealTimeGPS(v.currentFirstStep, traversedRoute);
                v.updatePlanningHorizon(v.currentFirstStep, traversedRoute);
                v.coveredRoute.AddRange(traversedRoute);
                v.routePredicted = addRouteOnMap(v.routePointsBetweenLocationsInReport, style , System.Drawing.Drawing2D.DashStyle.Solid, routeOverlay);
                v.currentMarker = addMarkerOnMap(v.currentFirstStep.endLonLat, "Current postion", GMarkerGoogleType.blue);
                foreach (LonLatRoutePair posi in v.lonlatCustLocations)
                {
                    if (posi.startVisited == true)
                    {
                        var mark = addMarkerOnMap(posi.start, "", GMarkerGoogleType.blue_small);
                        plotedMarkes.Add(mark);
                    }
                    if (posi.startVisited == false)
                    {
                        var mark = addMarkerOnMap(posi.start, "", GMarkerGoogleType.gray_small);
                        plotedMarkes.Add(mark);
                    }
                }
            }
            addRouteOnMap(v.coveredRoute.Select(x => x.point), new Pen(Color.Gray, 5), System.Drawing.Drawing2D.DashStyle.Solid, routeOverlay);

        }

        public void obradiRealTimeGPS(AggStep currentStep, List<GpsData> traveresedGPS)
        {
            bool pause = false;
            double pauseDuration = 0;
            for(int i= 1; i < traveresedGPS.Count; i++)
            {
                GpsData bef = traveresedGPS[i-1];
                GpsData now=traveresedGPS[i];
                double duration = (now.timeStamp - bef.timeStamp).TotalMinutes;
                if (bef.active == false)
                {
                    pause = true;
                    pauseDuration += duration;
                }
                else
                {
                    if(pause)
                    {
                        currentStep.currentPauseDuration += pauseDuration;
                        if (currentStep.currentPauseDuration > 45)
                        {
                            currentStep.drivingTimeLastDay = 0;
                            currentStep.drivingTimeFromLastDailyPause = 0;
                            currentStep.drivingTimeLast7Days = 0;
                            currentStep.currentPauseDuration = 0;
                        }
                        else if (currentStep.currentPauseDuration > 12 * 60)
                        {
                            currentStep.drivingTimeLastDay = 0;
                            currentStep.drivingTimeFromLastDailyPause = 0;
                            currentStep.currentPauseDuration = 0;
                        }
                        else if (currentStep.currentPauseDuration > 45)
                        {
                            currentStep.drivingTimeFromLastDailyPause = 0;
                        }
                        pauseDuration = 0;
                        pause = false;
                    }
                    currentStep.drivingTimeFromLastDailyPause += duration;
                    currentStep.drivingTimeLast14Days += duration;
                    currentStep.drivingTimeLast7Days += duration;
                    currentStep.drivingTimeLastDay += duration;
                }
            }
            GpsData last = traveresedGPS.Last();
            currentStep.dtBegin = Misc.convertTimeToNearestLowerTime(last.timeStamp, discPeriodMin);

            currentStep.beginRouteDistance = 0;
            currentStep.endRouteDistance = 0;
            currentStep.beginRouteDistanceLonLat = 0;
            currentStep.endRouteDistanceLonLat = 0;

            currentStep.startLonLat=last.point;
            currentStep.endLonLat = last.point;

            currentStep.step = null;
            currentStep.type = last.active ? 0:1;

        }

        private void btnZavrsi_Click(object sender, EventArgs e)
        {
            timerRealTime.Enabled = false;
        }

        private void btnSimRealTime_Click(object sender, EventArgs e)
        {
            //Inspect switch
            routeOverlay.Routes.Clear();
            timerRealTime.Interval = 500;
            timerRealTime.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timerRealTime.Enabled = true;
        }
    }
    public class Change
    {
        public Change(DateTime v1EndTimePred, DateTime v2EndTimePred, Vehicle v1, Vehicle v2, List<PointLatLng> v1Points, List<PointLatLng> v2Points)
        {
            this.v1EndTimePred = v1EndTimePred;
            this.v2EndTimePred = v2EndTimePred;
            savings = (v1.endTimePred - v1EndTimePred).TotalHours + (v2.endTimePred - v2EndTimePred).TotalHours;
            this.v1Points = v1Points;
            this.v2Points = v2Points;
        }
        public DateTime v1EndTimePred;
        public DateTime v2EndTimePred;
        public double savings;
        public List<PointLatLng> v1Points;
        public List<PointLatLng> v2Points;
    }

    public static class Globals
    {
        // global int
        public static int counter;
        //Last minute
        public static List<DateTime> lastMinuteCalls;
    }

}
