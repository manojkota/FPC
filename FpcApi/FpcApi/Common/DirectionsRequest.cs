using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FpcApi.Common
{
    //AIzaSyDEh46aAEsIpv0Mu202-glkK9aEkeh2z8E
    public class DirectionsRequest
    {
        private const string BaseUrl = "https://maps.googleapis.com/maps/api/directions/json";

        private readonly string requestUrl;

        public DirectionsRequest(Coordinate origin, Coordinate destination, IEnumerable<Coordinate> weighPoints, string apiKey)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            queryString["origin"] = $"{origin.Latitude},{origin.Longitude}";
            queryString["destination"] = $"{destination.Latitude},{destination.Longitude}";
            queryString["key"] = apiKey;
            queryString["waypoints"] = string.Join("|", weighPoints.Select(c => $"{c.Latitude},{c.Longitude}"));


            requestUrl = $"{BaseUrl}?{queryString.ToString()}";
        }

        public async Task<JObject> Get()
        {
            var client = new HttpClient();
            return JObject.Parse(await client.GetStringAsync(requestUrl));
        }
    }
}