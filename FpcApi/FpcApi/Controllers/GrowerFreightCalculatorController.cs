using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web.Http;
using FpcApi.Common;
using FpcApi.Models;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace FpcApi.Controllers
{
    [RoutePrefix("api/GrowerFreightCalculator")]
    public class GrowerFreightCalculatorController : ApiController
    {
        private readonly DataLoader dataLoader;
        public GrowerFreightCalculatorController()
        {
            dataLoader = new DataLoader();
        }

        [HttpPost, Route("CalculateForLocation")]
        public async Task<IEnumerable<PriceOutput>> CalculateForLocation([FromBody] RequestInputWithLocation input)
        {
            if (input != null)
            {
                var location = dataLoader.locations.Single(l => l.Id == input.LocationId);

                var directionsRequest = new DirectionsRequest
                (
                    new Coordinate(Convert.ToDecimal(input.Origin.Latitude), Convert.ToDecimal(input.Origin.Longitude)),
                    location.GetCoordinate(),
                    input.Waypoints,
                    "AIzaSyDEh46aAEsIpv0Mu202-glkK9aEkeh2z8E"
                );

                var directionsResponse = await directionsRequest.Get();

                var distance = (int) directionsResponse["routes"][0]["legs"][0]["distance"]["value"] / 1000;

                var distanceEstimator =
                    new List<Tuple<Location, decimal>>() {new Tuple<Location, decimal>(location, distance)};
                                                                
                return GetPriceOutput(distanceEstimator, input.Origin).OrderByDescending(x => x.Profit);
            }

            return null;
        }

        [HttpPost, Route("Calculate")]
        public async Task<IEnumerable<PriceOutput>> Calculate([FromBody] RequestInput input)
        {
            if (input != null)
            {
                var distancesRequest = new DistanceMatrixRequest
                (
                    new[] { new Coordinate(Convert.ToDecimal(input.Latitude), Convert.ToDecimal(input.Longitude)) },
                    dataLoader.locations.Select(l => new Coordinate(Convert.ToDecimal(l.Latitude), Convert.ToDecimal(l.Longitude))),
                    ConfigurationManager.AppSettings["GoogleMapsKey"].ToString()
                );

                var distancesResponse = await distancesRequest.Get();

                var distances = distancesResponse.Rows[0].Elements.Select(x => x.Distance.Value);

                var distanceEstimator = dataLoader.locations.Zip(distances, (l, d) => new Tuple<Location, decimal>(l, (d/1000))).ToList();

                var results = GetPriceOutput(distanceEstimator, input).OrderByDescending(x => x.Profit);

                SendTextMessage(results.FirstOrDefault());

                return results;
            }

            return null;
        }

        private IList<PriceOutput> GetPriceOutput(IList<Tuple<Location, decimal>> distanceEstimator, RequestInput input)
        {
            var locationsWithDistances = distanceEstimator.Select(d => d.Item1.Id).ToList();

            var cashPrices = dataLoader.cashPrices.Where(x =>
                x.Grade.Equals(input.Grade, StringComparison.InvariantCultureIgnoreCase)
                && x.Commodity.Equals(input.Commodity, StringComparison.InvariantCultureIgnoreCase)
                && x.Season.Equals(input.Season, StringComparison.InvariantCultureIgnoreCase)
                && locationsWithDistances.Contains(x.LocationId)).OrderByDescending(y => y.Price);

            var result = new List<PriceOutput>();

            foreach (var cashPrice in cashPrices)
            {
                //calculate distance
                var distance = distanceEstimator.FirstOrDefault(x => x.Item1.Id == cashPrice.LocationId).Item2;

                List<FrieghtEstimate> frieghtEstimates = new List<FrieghtEstimate>();

                int noOfTrips = 1;

                if (input.IsOwnTruck)
                {
                    FrieghtEstimate estimate = new FrieghtEstimate();

                    var selectedTruckType = input.TruckTypeId.HasValue ? dataLoader.truckTypes.FirstOrDefault(x => x.Id == input.TruckTypeId.Value) : null;

                    estimate.FrieghtCompanyName = string.Empty;
                    estimate.TruckType = input.TruckTypeId.HasValue ? selectedTruckType.Type : string.Empty;
                    estimate.CostPerKm = input.OwnerCostPerKm.Value;
                    estimate.EstimatedPrice = input.OwnerCostPerKm.Value * distance * Convert.ToDecimal(input.TruckTypeId.HasValue ? selectedTruckType.MaxCapacity : input.Quantity);

                    var currentCapacity = selectedTruckType?.MaxCapacity;
                    while (currentCapacity < input.Quantity)
                    {
                        noOfTrips = noOfTrips + 1;
                        currentCapacity = currentCapacity * noOfTrips;
                    }
                    estimate.NoOfTrips = noOfTrips;

                    frieghtEstimates.Add(estimate);
                }
                else
                {
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
                        EstimatedPrice = cashPrice.Price * Convert.ToDecimal(input.Quantity),
                        PaymentTerms = dataLoader.buyers.FirstOrDefault(x => x.Id == cashPrice.BuyerId).PaymentTerms
                    };
                    output.FrieghtEstimate = frieghtEstimate;
                    output.Profit = (output.BuyerCashPrice.EstimatedPrice - frieghtEstimate.EstimatedPrice);

                    result.Add(output);
                }
            }

            return result.Where(x=> x.Profit > 0).ToList();
        }

        private async Task SendTextMessage(PriceOutput output)
        {
            try
            {
                if (output != null)
                {
                    if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IsTest"].ToString()))
                    {
                        var accountSid = "AC6ead790059d3102fbd033485340ac87f";
                        var authToken = "590de52713c995976373811ee4508604";

                        TwilioClient.Init(accountSid, authToken);

                        var message = MessageResource.Create(
                            to: new PhoneNumber(ConfigurationManager.AppSettings["SmsToNumber"].ToString()),
                            from: new PhoneNumber("+61451562474"),
                            body:
                            $"Buyer: {output.BuyerCashPrice.BuyerName}, Location: {output.Location.Name}, Profit: {output.Profit:c}");
                    }
                }
            }
            catch (Exception e)
            {
            }
        }
    }
}
