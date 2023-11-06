using GMap.NET.MapProviders;
using GMap.NET;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GMap.NET.WindowsForms;
using System.Data.Entity.Core.Metadata.Edm;

namespace Prekapcanje3
{
    public class GpsData
    {
        public DateTime timeStamp;
        public PointLatLng point;
        public GpsData(PointLatLng point, DateTime timeStamp, bool active)
        {
            this.timeStamp = timeStamp;
            this.point = point;
            this.active = active;
        }
        public bool active; //true, false
    }

    public class AggStep
    {
        public DateTime dtBegin;

        public double beginRouteDistance;
        public double endRouteDistance;

        public double beginRouteDistanceLonLat;
        public double endRouteDistanceLonLat;

        public PointLatLng startLonLat;
        public PointLatLng endLonLat;

        public Step step;

        public double drivingTimeLast7Days;
        public double drivingTimeLast14Days;
        public double drivingTimeLastDay;
        public int numbefOfTimes10HoursInLastSevenDays;
        public double drivingTimeFromLastDailyPause;
        public int type; //0 driving, 1 - pause

        public double currentPauseDuration;

        public void setAggSteps(DateTime dtBegin, double beginRouteDistance, double endRouteDistance,
            double drivingTimeLast7Days, double drivingTimeLast14Days, double drivingTimeLastDay, double drivingTimeFromLastDailyPause,
            double beginRouteDistanceLonLat, double currentPauseDuration,
            int type, Step step)
        {
            this.dtBegin = dtBegin;
            this.beginRouteDistance = beginRouteDistance;
            this.endRouteDistance = endRouteDistance;
            this.drivingTimeLast7Days = drivingTimeLast7Days;
            this.drivingTimeLast14Days = drivingTimeLast14Days;
            this.drivingTimeLastDay = drivingTimeLastDay;
            this.drivingTimeFromLastDailyPause = drivingTimeFromLastDailyPause;
            this.beginRouteDistanceLonLat = beginRouteDistanceLonLat;
            this.endRouteDistanceLonLat = beginRouteDistanceLonLat;
            this.startLonLat = PointLatLng.Empty;
            this.endLonLat = PointLatLng.Empty;
            this.currentPauseDuration = currentPauseDuration;
            this.type = type;
            this.step = step;
        }

        public void setAggSteps(AggStep aggstepPrevious, double discPeriodMin, double equalDistSize, Step step, int type)
        {
            this.dtBegin = Misc.newDateTime(aggstepPrevious.dtBegin);
            this.dtBegin = this.dtBegin.AddMinutes(discPeriodMin);
            this.beginRouteDistance = aggstepPrevious.endRouteDistance;
            double addTime = discPeriodMin;
            if (type == 1)
            {
                addTime = 0;
            }
            this.endRouteDistance = this.beginRouteDistance + equalDistSize;
            this.drivingTimeLast7Days = aggstepPrevious.drivingTimeLast7Days + addTime;
            this.drivingTimeLast14Days = aggstepPrevious.drivingTimeLast14Days + addTime;
            this.drivingTimeLastDay = aggstepPrevious.drivingTimeLastDay + addTime;
            this.drivingTimeFromLastDailyPause = aggstepPrevious.drivingTimeFromLastDailyPause + addTime;
            this.beginRouteDistanceLonLat = aggstepPrevious.endRouteDistanceLonLat;
            this.endRouteDistanceLonLat = aggstepPrevious.endRouteDistanceLonLat;
            this.startLonLat = aggstepPrevious.endLonLat;
            this.endLonLat = aggstepPrevious.endLonLat;
            this.currentPauseDuration = aggstepPrevious.currentPauseDuration;
            this.type = type;
            this.step = step;
        }
    }

    public class LonLatRoutePair
    {
        public PointLatLng start;
        public PointLatLng end;
        public bool startVisited;
        public bool endVisited;
        public LonLatRoutePair(PointLatLng start, PointLatLng end)
        {
            this.start = start;
            this.end = end;
            this.startVisited = false;
            this.endVisited = false;
        }
    }

    public class Location
    {
        public string name;
        public PointLatLng point;
        public string description;
        public DateTime RealBegin;
        public DateTime RealEnd;
    }
    public class Vehicle
    {
        public string name { get; set; }
        public List<GpsData> gpsRoute;
        public List<Location> locationsFromReport;
        public List<Location> setLocations;
        public List<Route> routesBetweenLocationsInReport;
        public List<PointLatLng> routePointsBetweenLocationsInReport;
        public SubSegmentSummary summary;

        //Working hour parameters
        int discPeriodMin;
        DateTime startTimeReal;
        public DateTime endTimeReal;
        public AggStep currentFirstStep;

        public DateTime startTimePred;
        public DateTime endTimePred;

        public List<LonLatRoutePair> lonlatCustLocations;
        public List<LonLatRoutePair> previousListCall;

        public List<AggStep> planningHorizon;

        public GMapRoute routePredicted;
        public GMapMarker currentMarker;
        public List<GpsData> coveredRoute = new List<GpsData>();
        public Vehicle(string fileName, int discPeriodMin)
        {
            name = fileName.Replace(".csv", "");
            gpsRoute = new List<GpsData>();
            string format = "dd.MM.yyyy HH:mm:ss";
            this.discPeriodMin = discPeriodMin;
            locationsFromReport = new List<Location>();
            try
            {
                StreamReader citanje = new StreamReader(name + "_stajanja.csv");
                currentFirstStep = new AggStep();
                string linija = citanje.ReadLine();
                startTimeReal = DateTime.ParseExact(linija.Split(';')[1].Trim(), format, CultureInfo.InvariantCulture);
                currentFirstStep.dtBegin = startTimeReal;
                currentFirstStep.drivingTimeLast7Days = Convert.ToDouble(citanje.ReadLine().Split(';')[1].Replace('.', ',').Trim());
                currentFirstStep.drivingTimeLast14Days = Convert.ToDouble(citanje.ReadLine().Split(';')[1].Replace('.', ',').Trim());
                currentFirstStep.drivingTimeLastDay = Convert.ToDouble(citanje.ReadLine().Split(';')[1].Replace('.', ',').Trim());
                currentFirstStep.numbefOfTimes10HoursInLastSevenDays = Convert.ToInt32(citanje.ReadLine().Split(';')[1].Trim());
                currentFirstStep.drivingTimeFromLastDailyPause = 0;
                currentFirstStep.currentPauseDuration = 0;

                citanje.ReadLine();
                while (!citanje.EndOfStream)
                {
                    string[] dijelovi = citanje.ReadLine().Split(';');
                    if (string.IsNullOrEmpty(dijelovi.Last().Trim()))
                    {
                        continue;
                    }
                    Location l = new Location();
                    l.RealBegin = DateTime.ParseExact(dijelovi[0], format, CultureInfo.InvariantCulture);
                    l.RealEnd = DateTime.ParseExact(dijelovi[1], format, CultureInfo.InvariantCulture);
                    l.description = dijelovi.Last();
                    l.name = dijelovi[dijelovi.Length - 3];
                    string[] points = dijelovi[dijelovi.Length - 2].Split(',');
                    PointLatLng p = new PointLatLng(
                        Convert.ToDouble(points[0].Replace('.', ',').Trim()),
                         Convert.ToDouble(points[1].Replace('.', ',').Trim()));
                    l.point = p;
                    locationsFromReport.Add(l);
                }
                endTimeReal = locationsFromReport.Last().RealBegin;
                citanje.Close();

                lonlatCustLocations = new List<LonLatRoutePair>();
                for (int i = 1; i < locationsFromReport.Count; i++)
                {
                    Location locBef = locationsFromReport[i - 1];
                    Location locAft = locationsFromReport[i];
                    lonlatCustLocations.Add(new LonLatRoutePair(locBef.point, locAft.point));

                }

                routePointsBetweenLocationsInReport = new List<PointLatLng>();
                //Call to open route service
                if (!GetRoute.compareRoutes(lonlatCustLocations, previousListCall))
                {
                    routesBetweenLocationsInReport = GetRoute.ComputeRoute(lonlatCustLocations);
                    previousListCall = new List<LonLatRoutePair>(lonlatCustLocations);
                }
                


                StreamReader citanjeRoute = new StreamReader(name + "_comproute.txt");
                summary = new SubSegmentSummary();
                summary.steps = new List<Step>();
                string activeString = citanjeRoute.ReadLine();
                while (!citanjeRoute.EndOfStream)
                {
                    string line = citanjeRoute.ReadLine();
                    if (line == "Segmensts:")
                    {
                        activeString = line;
                        continue;
                    }

                    string[] dl = line.Split(';');
                    if (activeString == "Geometry:")
                    {
                        PointLatLng p = new PointLatLng(Convert.ToDouble(dl[1].Replace('.', ',').Trim()), Convert.ToDouble(dl[0].Replace('.', ',').Trim()));
                        routePointsBetweenLocationsInReport.Add(p);
                    }
                    else if (activeString == "Segmensts:")
                    {
                        Step step = new Step();
                        step.distance = Convert.ToDouble(dl[0].Replace('.', ',').Trim());
                        step.duration = Convert.ToDouble(dl[1].Replace('.', ',').Trim());
                        step.type = Convert.ToInt32(dl[2].Trim());
                        summary.duration += step.duration;
                        summary.distance += step.distance;
                        summary.steps.Add(step);
                    }
                }
                citanjeRoute.Close();


                determineFirstPlanningHorizon(currentFirstStep);

              
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }


            try
            {
                StreamReader citanje = new StreamReader(fileName);
                citanje.ReadLine();
                while (!citanje.EndOfStream)
                {
                    string[] dijelovi = citanje.ReadLine().Split(';');

                    DateTime dateTime = DateTime.ParseExact(dijelovi[0], format, CultureInfo.InvariantCulture);
                    string[] coords = dijelovi[1].Split(',');
                    PointLatLng point = new PointLatLng(Convert.ToDouble(coords[0].Replace('.', ',').Trim()), Convert.ToDouble(coords[1].Replace('.', ',').Trim()));
                    bool active= dijelovi.Last().Trim()=="Aktivan"?true:false;
                    gpsRoute.Add(new GpsData(point, dateTime,active));
                }
                citanje.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



        public void determineFirstPlanningHorizon(AggStep aggstepPrevious)
        {
            DateTime startDiscTime = Misc.convertTimeToNearestLowerTime(aggstepPrevious.dtBegin, discPeriodMin);

            AggStep curStep = null;

            int lastRoutePointIndex = 0;
            planningHorizon = new List<AggStep>();
            double addTimeToNextStep = 0;
            double addDistanceToNextStep = 0;
            bool first = true;

            for (int i = 0; i < summary.steps.Count; i++)
            {
                double durationInMin = (summary.steps[i].duration / 60.0) + addTimeToNextStep;
                double distanceInM = (summary.steps[i].distance) + addDistanceToNextStep;
                int smallSteps = Convert.ToInt32(Math.Floor(durationInMin / discPeriodMin));
                addTimeToNextStep = durationInMin - smallSteps * discPeriodMin;
                //int smallStepsDist = Convert.ToInt32(Math.Ceiling(durationInMin / discPeriodMin));
                double equalDistSize = smallSteps == 0 ? 0 : distanceInM / smallSteps;
                addDistanceToNextStep = distanceInM - equalDistSize * smallSteps;
                for (int j = 0; j < smallSteps; j++)
                {
                    curStep = new AggStep();
                    if (first)
                    {
                        curStep.setAggSteps(Misc.newDateTime(startDiscTime), 0, equalDistSize,
                            currentFirstStep.drivingTimeLast7Days + discPeriodMin, currentFirstStep.drivingTimeLast14Days + discPeriodMin,
                            currentFirstStep.drivingTimeLastDay + discPeriodMin, currentFirstStep.drivingTimeFromLastDailyPause,0,aggstepPrevious.currentPauseDuration, 0, summary.steps[i]);
                        first = false;
                    }
                    else
                    {
                        curStep.setAggSteps(aggstepPrevious, discPeriodMin, equalDistSize, summary.steps[i], 0);
                    }


                    //aggstep.endRouteDistanceLonLat = aggstep.beginRouteDistanceLonLat;
                    lastRoutePointIndex = lastRoutePointIndex < routePointsBetweenLocationsInReport.Count ? lastRoutePointIndex : routePointsBetweenLocationsInReport.Count - 1;
                    curStep.startLonLat = routePointsBetweenLocationsInReport[lastRoutePointIndex];
                    PointLatLng pointBef = curStep.startLonLat;
                    while (true)
                    {
                        if (curStep.endRouteDistanceLonLat > curStep.endRouteDistance
                            ||
                            lastRoutePointIndex >= routePointsBetweenLocationsInReport.Count)
                        {
                            curStep.endLonLat = pointBef;
                            lastRoutePointIndex--;
                            break;
                        }
                        PointLatLng pointNow = routePointsBetweenLocationsInReport[lastRoutePointIndex];
                        curStep.endRouteDistanceLonLat += Misc.airalDistHaversine(pointBef.Lng, pointBef.Lat, pointNow.Lng, pointNow.Lat);

                        lastRoutePointIndex++;
                        pointBef = pointNow;
                    }
                    planningHorizon.Add(curStep);
                    checkWorkingHours(curStep, planningHorizon);
                    aggstepPrevious = planningHorizon.Last();
                }
            }
            if (addTimeToNextStep > 0)
            {
                curStep = new AggStep();
                curStep.setAggSteps(aggstepPrevious, discPeriodMin, addDistanceToNextStep, summary.steps.Last(), 0);
                curStep.endLonLat = routePointsBetweenLocationsInReport.Last();
                addTimeToNextStep = 0;
                addDistanceToNextStep = 0;

                for (int ii = lastRoutePointIndex + 1; ii < routePointsBetweenLocationsInReport.Count; ii++)
                {
                    PointLatLng pointNow = routePointsBetweenLocationsInReport[lastRoutePointIndex];
                    PointLatLng pointBef = routePointsBetweenLocationsInReport[lastRoutePointIndex - 1];
                    curStep.endRouteDistanceLonLat += Misc.airalDistHaversine(pointBef.Lng, pointBef.Lat, pointNow.Lng, pointNow.Lat);
                }
                planningHorizon.Add(curStep);
            }

            startTimePred = Misc.newDateTime(planningHorizon[0].dtBegin);
            endTimePred = Misc.newDateTime(planningHorizon.Last().dtBegin);
            endTimePred = endTimePred.AddMinutes(discPeriodMin);
        }

        public void updatePlanningHorizon(AggStep aggstepPrevious, List<GpsData> GPSDataUpToCurrentTime)
        {
            DateTime startDiscTime = Misc.convertTimeToNearestLowerTime(aggstepPrevious.dtBegin, discPeriodMin);

            foreach(GpsData gpsData in GPSDataUpToCurrentTime)
            {
                foreach(LonLatRoutePair pair in lonlatCustLocations)
                {
                    if (Misc.airalDistHaversine(gpsData.point, pair.start) < 100)
                    {
                        pair.startVisited = true;
                    }
                    if (Misc.airalDistHaversine(gpsData.point, pair.end) < 100)
                    {
                        pair.endVisited = true;
                    }
                }

            }
            List<LonLatRoutePair> pairs = new List<LonLatRoutePair>();
            foreach (LonLatRoutePair pair in lonlatCustLocations)
            {
                if(pair.startVisited && pair.endVisited)
                {
                    continue;
                }
                else if(pair.startVisited && pair.endVisited == false)
                {
                    LonLatRoutePair pairNew=new LonLatRoutePair(GPSDataUpToCurrentTime.Last().point, pair.end);
                    pairs.Add(pairNew);
                }
                else
                {
                    pairs.Add(pair);
                }
            }

            if (!GetRoute.compareRoutes(pairs, previousListCall))
                {
                routesBetweenLocationsInReport = GetRoute.ComputeRoute(pairs);
                previousListCall = new List<LonLatRoutePair>(pairs);
            }
            

            
            extractRoutePointsFromCalledRoute();

            AggStep curStep = null;

            int lastRoutePointIndex = 0;
            planningHorizon = new List<AggStep>();
            double addTimeToNextStep = 0;
            double addDistanceToNextStep = 0;
            bool first = true;

            for (int i = 0; i < summary.steps.Count; i++)
            {
                double durationInMin = (summary.steps[i].duration / 60.0) + addTimeToNextStep;
                double distanceInM = (summary.steps[i].distance) + addDistanceToNextStep;
                int smallSteps = Convert.ToInt32(Math.Floor(durationInMin / discPeriodMin));
                addTimeToNextStep = durationInMin - smallSteps * discPeriodMin;
                //int smallStepsDist = Convert.ToInt32(Math.Ceiling(durationInMin / discPeriodMin));
                double equalDistSize = smallSteps == 0 ? 0 : distanceInM / smallSteps;
                addDistanceToNextStep = distanceInM - equalDistSize * smallSteps;
                for (int j = 0; j < smallSteps; j++)
                {
                    curStep = new AggStep();
                    if (first)
                    {
                        curStep.setAggSteps(Misc.newDateTime(startDiscTime), 0, equalDistSize,
                            currentFirstStep.drivingTimeLast7Days + discPeriodMin, currentFirstStep.drivingTimeLast14Days + discPeriodMin,
                            currentFirstStep.drivingTimeLastDay + discPeriodMin, currentFirstStep.drivingTimeFromLastDailyPause, 0,aggstepPrevious.currentPauseDuration, 0, summary.steps[i]);
                        first = false;
                    }
                    else
                    {
                        curStep.setAggSteps(aggstepPrevious, discPeriodMin, equalDistSize, summary.steps[i], 0);
                    }


                    //aggstep.endRouteDistanceLonLat = aggstep.beginRouteDistanceLonLat;
                    lastRoutePointIndex = lastRoutePointIndex < routePointsBetweenLocationsInReport.Count ? lastRoutePointIndex : routePointsBetweenLocationsInReport.Count - 1;
                    curStep.startLonLat = routePointsBetweenLocationsInReport[lastRoutePointIndex];
                    PointLatLng pointBef = curStep.startLonLat;
                    while (true)
                    {
                        if (curStep.endRouteDistanceLonLat > curStep.endRouteDistance
                            ||
                            lastRoutePointIndex >= routePointsBetweenLocationsInReport.Count)
                        {
                            curStep.endLonLat = pointBef;
                            lastRoutePointIndex--;
                            break;
                        }
                        PointLatLng pointNow = routePointsBetweenLocationsInReport[lastRoutePointIndex];
                        curStep.endRouteDistanceLonLat += Misc.airalDistHaversine(pointBef.Lng, pointBef.Lat, pointNow.Lng, pointNow.Lat);

                        lastRoutePointIndex++;
                        pointBef = pointNow;
                    }
                    planningHorizon.Add(curStep);
                    checkWorkingHours(curStep, planningHorizon);
                    aggstepPrevious = planningHorizon.Last();
                }
            }
            if (addTimeToNextStep > 0)
            {
                curStep = new AggStep();
                curStep.setAggSteps(aggstepPrevious, discPeriodMin, addDistanceToNextStep, summary.steps.Last(), 0);
                curStep.endLonLat = routePointsBetweenLocationsInReport.Last();
                addTimeToNextStep = 0;
                addDistanceToNextStep = 0;

                for (int ii = lastRoutePointIndex + 1; ii < routePointsBetweenLocationsInReport.Count; ii++)
                {
                    PointLatLng pointNow = routePointsBetweenLocationsInReport[ii];
                    PointLatLng pointBef = routePointsBetweenLocationsInReport[ii - 1];
                    curStep.endRouteDistanceLonLat += Misc.airalDistHaversine(pointBef.Lng, pointBef.Lat, pointNow.Lng, pointNow.Lat);
                }
                planningHorizon.Add(curStep);
            }

            startTimePred = planningHorizon[0].dtBegin;
            endTimePred = planningHorizon.Last().dtBegin;
            endTimePred = endTimePred.AddMinutes(discPeriodMin);
        }

        public int getIntervalIndex(DateTime now)
        {
            if (DateTime.Compare(now, startTimePred) < 0 || DateTime.Compare(now, endTimePred) > 0)
            {
                return -1;
            }
            else
            {
                for (int i = 0; i < planningHorizon.Count; i++)
                {
                    DateTime intBegin = planningHorizon[i].dtBegin;
                    DateTime intEnd = Misc.newDateTime(intBegin);
                    intEnd.AddMinutes(discPeriodMin);
                    if (DateTime.Compare(now, intBegin) >= 0 && DateTime.Compare(now, intEnd) <= 0)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public void checkWorkingHours(AggStep curStep, List<AggStep> planningHorizon)
        {
            int pauseDuration = 0;
            AggStep newStep = new AggStep();
            newStep.setAggSteps(curStep, 0, 0, curStep.step, 1);
            if (curStep.drivingTimeFromLastDailyPause >= 4.5 * 60) //Pauza svaka 4 sata
            {
                pauseDuration = 45;
                newStep.drivingTimeFromLastDailyPause = 0;
                newStep.currentPauseDuration = 0;
            }

            if (curStep.drivingTimeLastDay > 9 * 60)
            {
                pauseDuration = 12 * 60;
                newStep.drivingTimeLastDay = 0;
                newStep.drivingTimeFromLastDailyPause = 0;
                newStep.currentPauseDuration = 0;
            }

            if (curStep.drivingTimeLast7Days > 56 * 60)
            {
                pauseDuration = 48 * 60;
                newStep.drivingTimeLastDay = 0;
                newStep.drivingTimeFromLastDailyPause = 0;
                newStep.drivingTimeLast7Days = 0;
                newStep.currentPauseDuration = 0;
            }

            if (pauseDuration > 0)
            {
                int countSteps = Convert.ToInt32(Math.Ceiling(pauseDuration / (double)discPeriodMin));
                for (int i = 0; i < countSteps; i++)
                {
                    AggStep stepPause = new AggStep();
                    stepPause.setAggSteps(newStep, discPeriodMin, 0, newStep.step, 1);
                    planningHorizon.Add(stepPause);
                    newStep = stepPause;
                }
            }
        }

        public void extractRoutePointsFromCalledRoute()
        {
            summary.steps = new List<Step>();
            routePointsBetweenLocationsInReport.Clear();
            for (int i = 0; i < routesBetweenLocationsInReport.Count; i++)
            {
                Route r = routesBetweenLocationsInReport[i];
                for (int j = 0; j < r.Features.Count; j++)
                {
                    Feature f = r.Features[j];
                    for (int k = 0; k < f.Geometry.coordinates.Count; k++)
                    {
                        double[] arr = f.Geometry.coordinates[k];
                        PointLatLng p = new PointLatLng(arr[1], arr[0]);
                        routePointsBetweenLocationsInReport.Add(p);
                    }
                    for (int k = 0; k < f.Properties.Segments.Count; k++)
                    {
                        SubSegmentSummary subs = f.Properties.Segments[k];
                        for (int l = 0; l < subs.steps.Count; l++)
                        {
                            Step s = subs.steps[l];
                            summary.steps.Add(s);
                        }
                    }
                }
            }
        }

        public DateTime evaluateChange(AggStep aggstepPrevious, Vehicle other, AggStep currentStepOtherVehicle, ref List<PointLatLng> points)
        {
            List<AggStep> oldPlannigHorizon = new List<AggStep>(planningHorizon);
            List<Route> oldRoutesBetweenLocationsInReport = new List<Route>(routesBetweenLocationsInReport);
            List<Step> oldStepsSummary = new List<Step>(summary.steps);
            List<PointLatLng> oldroutePointsBetweenLocationsInReport = new List<PointLatLng>(routePointsBetweenLocationsInReport);



            DateTime startDiscTime = aggstepPrevious.dtBegin.AddMinutes(discPeriodMin);

            List<LonLatRoutePair> pairs = new List<LonLatRoutePair>();
            foreach (LonLatRoutePair pair in other.lonlatCustLocations)
            {
                if (pair.startVisited && pair.endVisited)
                {
                    continue;
                }
                else if (pair.startVisited && pair.endVisited == false)
                {
                    LonLatRoutePair pairNew = new LonLatRoutePair(aggstepPrevious.endLonLat, currentStepOtherVehicle.endLonLat);
                    pairs.Add(pairNew);
                    LonLatRoutePair pairNew2 = new LonLatRoutePair(currentStepOtherVehicle.endLonLat, pair.end);
                    pairs.Add(pairNew2);
                }
                else
                {
                    pairs.Add(pair);
                }
            }

            if (!GetRoute.compareRoutes(pairs, previousListCall))
            {
                routesBetweenLocationsInReport = GetRoute.ComputeRoute(pairs);
                previousListCall = new List<LonLatRoutePair>(pairs);
            }



            extractRoutePointsFromCalledRoute();

            AggStep curStep = null;

            int lastRoutePointIndex = 0;
            planningHorizon = new List<AggStep>();
            double addTimeToNextStep = 0;
            double addDistanceToNextStep = 0;
            bool first = true;

            for (int i = 0; i < summary.steps.Count; i++)
            {
                double durationInMin = (summary.steps[i].duration / 60.0) + addTimeToNextStep;
                double distanceInM = (summary.steps[i].distance) + addDistanceToNextStep;
                int smallSteps = Convert.ToInt32(Math.Floor(durationInMin / discPeriodMin));
                addTimeToNextStep = durationInMin - smallSteps * discPeriodMin;
                //int smallStepsDist = Convert.ToInt32(Math.Ceiling(durationInMin / discPeriodMin));
                double equalDistSize = smallSteps == 0 ? 0 : distanceInM / smallSteps;
                addDistanceToNextStep = distanceInM - equalDistSize * smallSteps;
                for (int j = 0; j < smallSteps; j++)
                {
                    curStep = new AggStep();
                    if (first)
                    {
                        curStep.setAggSteps(Misc.newDateTime(startDiscTime), 0, equalDistSize,
                            currentFirstStep.drivingTimeLast7Days + discPeriodMin, currentFirstStep.drivingTimeLast14Days + discPeriodMin,
                            currentFirstStep.drivingTimeLastDay + discPeriodMin, currentFirstStep.drivingTimeFromLastDailyPause, 0, aggstepPrevious.currentPauseDuration, 0, summary.steps[i]);
                        first = false;
                    }
                    else
                    {
                        curStep.setAggSteps(aggstepPrevious, discPeriodMin, equalDistSize, summary.steps[i], 0);
                    }


                    //aggstep.endRouteDistanceLonLat = aggstep.beginRouteDistanceLonLat;
                    lastRoutePointIndex = lastRoutePointIndex < routePointsBetweenLocationsInReport.Count ? lastRoutePointIndex : routePointsBetweenLocationsInReport.Count - 1;
                    curStep.startLonLat = routePointsBetweenLocationsInReport[lastRoutePointIndex];
                    PointLatLng pointBef = curStep.startLonLat;
                    while (true)
                    {
                        if (curStep.endRouteDistanceLonLat > curStep.endRouteDistance
                            ||
                            lastRoutePointIndex >= routePointsBetweenLocationsInReport.Count)
                        {
                            curStep.endLonLat = pointBef;
                            lastRoutePointIndex--;
                            break;
                        }
                        PointLatLng pointNow = routePointsBetweenLocationsInReport[lastRoutePointIndex];
                        curStep.endRouteDistanceLonLat += Misc.airalDistHaversine(pointBef.Lng, pointBef.Lat, pointNow.Lng, pointNow.Lat);

                        lastRoutePointIndex++;
                        pointBef = pointNow;
                    }
                    planningHorizon.Add(curStep);
                    checkWorkingHours(curStep, planningHorizon);
                    aggstepPrevious = planningHorizon.Last();
                }
            }
            if (addTimeToNextStep > 0)
            {
                curStep = new AggStep();
                curStep.setAggSteps(aggstepPrevious, discPeriodMin, addDistanceToNextStep, summary.steps.Last(), 0);
                curStep.endLonLat = routePointsBetweenLocationsInReport.Last();
                addTimeToNextStep = 0;
                addDistanceToNextStep = 0;

                for (int ii = lastRoutePointIndex + 1; ii < routePointsBetweenLocationsInReport.Count; ii++)
                {
                    PointLatLng pointNow = routePointsBetweenLocationsInReport[ii];
                    PointLatLng pointBef = routePointsBetweenLocationsInReport[ii - 1];
                    curStep.endRouteDistanceLonLat += Misc.airalDistHaversine(pointBef.Lng, pointBef.Lat, pointNow.Lng, pointNow.Lat);
                }
                planningHorizon.Add(curStep);
            }



            DateTime predEndTimeEval =  Misc.newDateTime(planningHorizon.Last().dtBegin);
            predEndTimeEval = predEndTimeEval.AddMinutes(discPeriodMin);

            points = new List<PointLatLng>(routePointsBetweenLocationsInReport);
            //restore old values
            planningHorizon = oldPlannigHorizon;
            routesBetweenLocationsInReport=oldRoutesBetweenLocationsInReport;
            summary.steps = oldStepsSummary;
            routePointsBetweenLocationsInReport = oldroutePointsBetweenLocationsInReport;
            return predEndTimeEval;
        }

    }
}
