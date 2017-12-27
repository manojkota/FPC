using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FpcApi.Models
{
    public class PriceOutput
    {
        public Location Location { get; set; }

        public FrieghtEstimate FrieghtEstimate { get; set; }

        public BuyerCashPrice BuyerCashPrice { get; set; }

        public decimal Profit { get; set; }
    }
}