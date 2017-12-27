using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FpcApi.Models
{
    public class PriceOutput
    {
        public Location Location { get; set; }

        public IEnumerable<FrieghtEstimate> FrieghtEstimates { get; set; }

        public IEnumerable<BuyerCashPrice> BuyerCashPrices { get; set; }
    }
}