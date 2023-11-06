using GMap.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prekapcanje3
{
    public static class Misc
    {
        public static DateTime convertTimeToNearestLowerTime(DateTime timeNow, int discPeriod)
        {
            int timeOfDayInMin = timeNow.Hour * 60 + timeNow.Minute;
            int timeInMin = (timeOfDayInMin / discPeriod) * discPeriod;
            int hourNew = timeInMin / 60;
            int minNew = timeInMin - hourNew * 60;
            return newDateTime(timeNow, hourNew, minNew, 0);
        }

        public static DateTime newDateTime(DateTime old)
        {
            return new DateTime(old.Year, old.Month, old.Day, old.Hour, old.Minute, old.Second);
        }

        public static DateTime newDateTime(DateTime old, int hour, int min, int sec)
        {
            return new DateTime(old.Year, old.Month, old.Day, hour, min, sec);
        }
        public static double airalDistHaversine(PointLatLng b, PointLatLng e)
        {
            return airalDistHaversine(b.Lng, b.Lat, e.Lng, e.Lat);
        }

        public static bool equalDoubleVals(double val1, double val2)
        {
            if (Math.Abs(val1 - val2) < 0.01)
            {
                return true;
            }
            return false;
        }

        public static double airalDistHaversine(double lon1, double lat1, double lon2, double lat2)
        {

            double R = 6371000; // metres
            double phi1 = lat1 * Math.PI / 180; // φ, λ in radians
            double phi2 = lat2 * Math.PI / 180;
            double deltaphi = (lat2 - lat1) * Math.PI / 180;
            double deltaLambda = (lon2 - lon1) * Math.PI / 180;

            double a = Math.Sin(deltaphi / 2) * Math.Sin(deltaphi / 2) +
                      Math.Cos(phi1) * Math.Cos(phi2) *
                      Math.Sin(deltaLambda / 2) * Math.Sin(deltaLambda / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            double d = R * c;
            return d;
        }

    }
}
