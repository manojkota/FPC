using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web.Http;
using FpcApi.Common;
using FpcApi.Models;

namespace FpcApi.Controllers
{
    public class GrowerFreightCalculatorController : ApiController
    {
        [HttpPost]
        public async Task<IEnumerable<PriceOutput>> Calculate([FromBody] RequestInput input)
        {
            var dataLoader = new DataLoader();

            var result = new List<PriceOutput>();
            if (input != null)
            {
                var distancesRequest = new DistanceMatrixRequest
                (
                    new[] { new Coordinate(Convert.ToDecimal(input.Latitude), Convert.ToDecimal(input.Longitude)) },
                    dataLoader.locations.Select(l => new Coordinate(Convert.ToDecimal(l.Latitude), Convert.ToDecimal(l.Longitude))),
                    "AIzaSyDM3VKkOOwNU5JZ05fKpCTWybwMyASqPOU"
                );

                var distancesResponse = await distancesRequest.Get();

                var distances = distancesResponse.Rows[0].Elements.Select(x => x.Distance.Value);

                var distanceEstimator = dataLoader.locations.Zip(distances, (l, d) => new Tuple<Location, decimal>(l, (d/1000)));

                foreach (var cashPrice in dataLoader.cashPrices.Where(x => x.Grade.Equals(input.Grade, StringComparison.InvariantCultureIgnoreCase)
                                                                            && x.Commodity.Equals(input.Commodity, StringComparison.InvariantCultureIgnoreCase)
                                                                          && x.Season.Equals(input.Season, StringComparison.InvariantCultureIgnoreCase)).OrderByDescending(y => y.Price))
                {
                    //calculate distance
                    var distance = distanceEstimator.FirstOrDefault(x=> x.Item1.Id == cashPrice.LocationId).Item2;

                    List<FrieghtEstimate> frieghtEstimates = new List<FrieghtEstimate>();

                    if (input.IsOwnTruck)
                    {
                        FrieghtEstimate estimate = new FrieghtEstimate();

                        estimate.FrieghtCompanyName = string.Empty;
                        estimate.TruckType = input.TruckTypeId.HasValue ? dataLoader.truckTypes.FirstOrDefault(x => x.Id == input.TruckTypeId.Value).Type : string.Empty;
                        estimate.CostPerKm = input.OwnerCostPerKm.Value;
                        estimate.EstimatedPrice = input.OwnerCostPerKm.Value * distance * Convert.ToDecimal(input.Quantity);
                        frieghtEstimates.Add(estimate);
                    }
                    else
                    {
                        int noOfTrips = 1;
                        var truckTypes = dataLoader.truckTypes.Where(x => x.MinCapacity <= input.Quantity && input.Quantity <= x.MaxCapacity);
                        while (!truckTypes.Any())
                        {
                            noOfTrips = noOfTrips + 1;
                            var dividedQuantity = input.Quantity / noOfTrips;
                            truckTypes = dataLoader.truckTypes.Where(x =>
                                x.MinCapacity <= dividedQuantity && dividedQuantity <= x.MaxCapacity);
                        }

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
                                estimate.EstimatedPrice = frieghtCost.CostPerKm * distance * Convert.ToDecimal(truckType.MaxCapacity);
                                estimate.NoOfTrips = noOfTrips;
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

            return result.OrderByDescending(x => x.Profit);
        }
    }
}
