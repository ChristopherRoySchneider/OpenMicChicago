using System;
using System.Net;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OpenMicChicago.Helpers {
    public static class HelperClass {
        public static void getLatLong (string address, out double latitude, out double longitude) {
            var urlAddress = HttpUtility.UrlEncode (address);
            var _endpoint = $"http://dev.virtualearth.net/REST/v1/Locations/US/{urlAddress}?key=Ai9-KNy6Al-r_ueyLuLXFYB_GlPl-c-_iYtu16byW86qBx9uGbsdJpwvrP4ZUdgD";
            var client = new WebClient ();

            var response = client.DownloadString (_endpoint);

            var jobject = (JObject) JsonConvert.DeserializeObject (response);
            latitude = (double) jobject["resourceSets"][0]["resources"][0]["point"]["coordinates"][0];
            longitude = (double) jobject["resourceSets"][0]["resources"][0]["point"]["coordinates"][1];
        }
    }
    public class Coordinates {
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }

        public Coordinates (double latitude, double longitude) {
            Latitude = latitude;
            Longitude = longitude;
        }
    }
    public static class CoordinatesDistanceExtensions {
        public static double DistanceTo (this Coordinates baseCoordinates, Coordinates targetCoordinates) {
            return DistanceTo (baseCoordinates, targetCoordinates, UnitOfLength.Miles);
        }

        public static double DistanceTo (this Coordinates baseCoordinates, Coordinates targetCoordinates, UnitOfLength unitOfLength) {
            var baseRad = Math.PI * baseCoordinates.Latitude / 180;
            var targetRad = Math.PI * targetCoordinates.Latitude / 180;
            var theta = baseCoordinates.Longitude - targetCoordinates.Longitude;
            var thetaRad = Math.PI * theta / 180;

            double dist =
                Math.Sin (baseRad) * Math.Sin (targetRad) + Math.Cos (baseRad) *
                Math.Cos (targetRad) * Math.Cos (thetaRad);
            dist = Math.Acos (dist);

            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;

            return unitOfLength.ConvertFromMiles (dist);
        }
    }

    public class UnitOfLength {
        public static UnitOfLength Kilometers = new UnitOfLength (1.609344);
        public static UnitOfLength NauticalMiles = new UnitOfLength (0.8684);
        public static UnitOfLength Miles = new UnitOfLength (1);

        private readonly double _fromMilesFactor;

        private UnitOfLength (double fromMilesFactor) {
            _fromMilesFactor = fromMilesFactor;
        }

        public double ConvertFromMiles (double input) {
            return input * _fromMilesFactor;
        }
    }
}