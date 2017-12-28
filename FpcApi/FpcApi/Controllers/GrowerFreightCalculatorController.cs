using System;
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
        [HttpPost]
        public IEnumerable<PriceOutput> Calculate([FromBody] RequestInput input)
        {
            var dataLoader = new DataLoader();

            var result = new List<PriceOutput>();
            if (input != null)
            {
                foreach (var cashPrice in dataLoader.cashPrices.Where(x=> x.Grade.Equals(input.Grade, StringComparison.InvariantCultureIgnoreCase)
                                                                            && x.Commodity.Equals(input.Commodity, StringComparison.InvariantCultureIgnoreCase)
                                                                          && x.Season.Equals(input.Season, StringComparison.InvariantCultureIgnoreCase)).OrderByDescending(y => y.Price))
                {
                    //calculate distance
                    var distance = 100;
                    
                    List<FrieghtEstimate> frieghtEstimates = new List<FrieghtEstimate>();

                    if (input.IsOwnTruck)
                    {
                        FrieghtEstimate estimate = new FrieghtEstimate();

                        estimate.FrieghtCompanyName = string.Empty;
                        estimate.TruckType = input.TruckTypeId.HasValue ? dataLoader.truckTypes.FirstOrDefault(x=> x.Id == input.TruckTypeId.Value).Type : string.Empty;
                        estimate.CostPerKm = input.OwnerCostPerKm.Value;
                        estimate.EstimatedPrice = input.OwnerCostPerKm.Value * distance * Convert.ToDecimal(input.Quantity);
                        frieghtEstimates.Add(estimate);
                    }
                    else
                    {
                        var truckTypes = dataLoader.truckTypes.Where(x =>x.MinCapacity <= input.Quantity && input.Quantity <= x.MaxCapacity);

                        foreach (var truckType in truckTypes)
                        {
                            var frieghtCosts = dataLoader.frieghtCosts.Where(x => x.TruckTypeId == truckType.Id);
                            foreach (var frieghtCost in frieghtCosts)
                            {
                                FrieghtEstimate estimate = new FrieghtEstimate();
                                FrieghtCompany frieghtCompany = dataLoader.frieghtCompanies.FirstOrDefault(x => x.Id == frieghtCost.FrieghtCompanyId);

                                estimate.FrieghtCompanyName = frieghtCompany.Name;
                                estimate.TruckType = truckType.Type;
                                estimate.CostPerKm = frieghtCost.CostPerKm;
                                estimate.EstimatedPrice = frieghtCost.CostPerKm * distance * Convert.ToDecimal(input.Quantity);
                                frieghtEstimates.Add(estimate);
                            }
                        }
                    }

                    foreach (var frieghtEstimate in frieghtEstimates)
                    {
                        PriceOutput output = new PriceOutput();
                        output.Location = dataLoader.locations.FirstOrDefault(x => x.Id == cashPrice.LocationId);
                        output.BuyerCashPrice = new BuyerCashPrice
                        {
                            BuyerName = dataLoader.buyers.FirstOrDefault(x => x.Id == cashPrice.BuyerId).Name,
                            BuyerPrice = cashPrice.Price,
                            Commodity = cashPrice.Commodity,
                            Grade = cashPrice.Grade,
                            Season = cashPrice.Season,
                            EstimatedPrice = cashPrice.Price * Convert.ToDecimal(input.Quantity)
                        };
                        output.FrieghtEstimate = frieghtEstimate;
                        output.Profit = (output.BuyerCashPrice.EstimatedPrice - frieghtEstimate.EstimatedPrice);

                        result.Add(output);
                    }
                }
            }

            return result.OrderByDescending(x=> x.Profit);
        }
    }
}
