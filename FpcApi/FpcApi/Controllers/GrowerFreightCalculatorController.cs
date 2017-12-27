﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Web.Http;
using FpcApi.Common;
using FpcApi.Models;

namespace FpcApi.Controllers
{
    public class GrowerFreightCalculatorController : ApiController
    {
        [HttpGet]
        public IEnumerable<PriceOutput> Calculate()//[FromBody] RequestInput input)
        {
            var dataLoader = new DataLoader();

            var result = new List<PriceOutput>();
            //if (input != null)
            {
                foreach (var loc in dataLoader.locations)
                {
                    PriceOutput output = new PriceOutput();
                    
                    //calculate distance
                    var distance = 100;
                    var truckTypes = dataLoader.truckTypes.Where(x => 20 <= x.Capacity);

                    List<FrieghtEstimate> frieghtEstimates = new List<FrieghtEstimate>();

                    foreach (var truckType in truckTypes)
                    {
                        var frieghtCosts = dataLoader.frieghtCosts.Where(x => x.TruckTypeId == truckType.Id);
                        foreach (var frieghtCost in frieghtCosts)
                        {
                            FrieghtEstimate estimate = new FrieghtEstimate();
                            FrieghtCompany frieghtCompany = dataLoader.frieghtCompanies.FirstOrDefault(x=> x.Id == frieghtCost.FrieghtCompanyId);

                            estimate.FrieghtCompanyName = frieghtCompany.Name;
                            estimate.TruckType = truckType.Type;
                            estimate.CostPerKm = frieghtCost.CostPerKm;
                            estimate.EstimatedPrice = frieghtCost.CostPerKm * distance;
                            frieghtEstimates.Add(estimate);
                        }
                    }

                    output.FrieghtEstimates = frieghtEstimates;
                    
                    result.Add(output);
                }
            }

            return result;
        }
    }
}
