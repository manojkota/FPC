using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FpcApi.Models
{
    public class Location
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }
}