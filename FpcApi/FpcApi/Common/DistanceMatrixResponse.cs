using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FpcApi.Common
{
    [JsonConverter(typeof(StringEnumConverter), true)]
    public enum Status
    {
        Ok
    }

    public class DistanceMatrixResponse
    {
        [JsonProperty("destination_addresses")]
        public IList<string> DestinationAddresses { get; set; }

        [JsonProperty("origin_addresses")]
        public IList<string> OriginAddresses { get; set; }

        public IList<Row> Rows { get; set; }

        public Status Status { get; set; }
    }

    public class Row
    {
        public IList<Route> Elements { get; set; }
    }

    public class Route
    {
        public TextValuePair<int> Distance { get; set; }
        public TextValuePair<int> Duration { get; set; }
        public Status Status { get; set; }
    }

    public class TextValuePair<T>
    {
        public string Text { get; set; }
        public T Value { get; set; }
    }
}