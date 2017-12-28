using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FpcApi.Common;

namespace FpcApi.Models
{
    public class RequestInputWithLocation
    {
        public RequestInput Origin { get; set; }

        public IList<Coordinate> Waypoints { get; set; }

        public long LocationId { get; set; }
    }
}