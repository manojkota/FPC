using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Web;

namespace FpcApi.Models
{
    public class FrieghtCost
    {
        public long Id { get; set; }

        public long FrieghtCompanyId { get; set; }

        public long TruckTypeId { get; set; }

        public decimal CostPerKm { get; set; }
    }
}