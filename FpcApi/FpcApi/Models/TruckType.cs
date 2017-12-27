using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FpcApi.Models
{
    public class TruckType
    {
        public long Id { get; set; }

        public string Type { get; set; }

        public double Capacity { get; set; }
    }
}