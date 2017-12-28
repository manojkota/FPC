using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FpcApi.Common;

namespace FpcApi.Models
{
    public class Location
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public Coordinate GetCoordinate()
        {
            return new Coordinate(Convert.ToDecimal(Latitude), Convert.ToDecimal(Longitude));
        }

    }
}