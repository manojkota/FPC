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

            var distancesRequest = new DistanceMatrixRequest
                (
                    new [] { new Coordinate(Convert.ToDecimal(input.Latitude), Convert.ToDecimal(input.Longitude)) },
                    dataLoader.locations.Select(l => new Coordinate(Convert.ToDecimal(l.Latitude), Convert.ToDecimal(l.Longitude))),
                    "AIzaSyDM3VKkOOwNU5JZ05fKpCTWybwMyASqPOU"
                );

            var distancesResponse = await distancesRequest.Get();
                
            var distances = distancesResponse.Rows[0].Elements.Select(x => x.Distance.Value);

            //if (input != null)
            {
                foreach (var locAndDistance in dataLoader.locations.Zip(distances, (l, d) => new Tuple<Location, int>(l, d)))
                {
                    var loc = locAndDistance.Item1;

                    //calculate distance
                    var distance = locAndDistance.Item2;
                    var truckTypes = dataLoader.truckTypes.Where(x => 20 >= x.MinCapacity && 20 <= x.MaxCapacity);

                    List<FrieghtEstimate> frieghtEstimates = new List<FrieghtEstimate>();

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
                            estimate.EstimatedPrice = frieghtCost.CostPerKm * distance;
                            frieghtEstimates.Add(estimate);
                        }
                    }

                    var cashPrices = dataLoader.cashPrices.Where(x => x.LocationId == loc.Id);
                    foreach (var cashPrice in cashPrices)
                    {
                        foreach (var frieghtEstimate in frieghtEstimates)
                        {
                            PriceOutput output = new PriceOutput();
                            output.Location = loc;
                            output.BuyerCashPrice = new BuyerCashPrice
                            {
                                BuyerName = dataLoader.buyers.FirstOrDefault(x=> x.Id == cashPrice.BuyerId).Name,
                                BuyerPrice = cashPrice.Price,
                                Commodity = cashPrice.Commodity,
                                Grade = cashPrice.Grade,
                                Season = cashPrice.Season,
                                EstimatedPrice = cashPrice.Price * 100
                            };
                            output.FrieghtEstimate = frieghtEstimate;
                            output.Profit = output.BuyerCashPrice.EstimatedPrice - frieghtEstimate.EstimatedPrice;

                            result.Add(output);
                        }
                    }
                }
            }

            return result;
        }
    }
}
