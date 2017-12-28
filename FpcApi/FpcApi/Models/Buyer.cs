using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FpcApi.Models
{
    public class Buyer
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string PaymentTerms { get; set; }
    }
}