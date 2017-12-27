using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Web;

namespace FpcApi.Models
{
    public class CashPrice
    {
        public long Id { get; set; }

        public long BuyerId { get; set; }

        public long LocationId { get; set; }

        public string Commodity { get; set; }

        public string Grade { get; set; }

        public string Season { get; set; }

        public decimal Price { get; set; }
    }
}