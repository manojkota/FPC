using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FpcApi.Models
{
    public class RequestInput
    {
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string Commodity { get; set; }

        public string Grade { get; set; }

        public string Season { get; set; }

        public bool IsOwnTruck { get; set; }

        public decimal? OwnerCostPerKm { get; set; }

        public int? TruckTypeId { get; set; }

        public double Quantity { get; set; }
    }
}