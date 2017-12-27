using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FpcApi.Common
{
    //"AIzaSyDM3VKkOOwNU5JZ05fKpCTWybwMyASqPOU"

    public class DistanceMatrixRequest
    {
        private const string BaseUrl = "https://maps.googleapis.com/maps/api/distancematrix/json";

        private readonly string requestUrl;

        private static readonly JsonSerializerSettings SerialisationSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };

        public DistanceMatrixRequest(IEnumerable<Coordinate> origin, IEnumerable<Coordinate> destination, string apiKey)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            queryString["origins"] = 
                string.Join("|", origin.Select(c => $"{c.Latitude},{c.Longitude}"));
            queryString["destinations"] =
                string.Join("|", destination.Select(c => $"{c.Latitude},{c.Longitude}"));
            queryString["key"] = apiKey;

            requestUrl = $"{BaseUrl}?{queryString.ToString()}";
        }

        public async Task<DistanceMatrixResponse> Get()
        {
            var client = new HttpClient();
            return JsonConvert.DeserializeObject<DistanceMatrixResponse>(await client.GetStringAsync(requestUrl), SerialisationSettings);
        }
    }
}