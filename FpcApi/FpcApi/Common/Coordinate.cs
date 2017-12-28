using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FpcApi.Common
{
    public class Coordinate
    {
        public Coordinate(decimal latitude, decimal longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
}