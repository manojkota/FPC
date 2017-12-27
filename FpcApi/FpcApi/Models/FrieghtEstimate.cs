using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace FpcApi.Models
{
    public class FrieghtEstimate
    {
        public string FrieghtCompanyName { get; set; }

        public string TruckType { get; set; }

        public decimal CostPerKm { get; set; }

        public decimal EstimatedPrice { get; set; }
    }
}