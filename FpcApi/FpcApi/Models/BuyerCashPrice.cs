using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FpcApi.Models
{
    public class BuyerCashPrice
    {
        public string BuyerName { get; set; }

        public string Commodity { get; set; }

        public string Grade { get; set; }

        public string Season { get; set; }

        public decimal BuyerPrice { get; set; }

        public decimal EstimatedPrice { get; set; }
    }
}