using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using ExcelService.Model;
using ExcelService.Service;
using FpcApi.Models;

namespace FpcApi.Common
{
    public class DataLoader
    {
        private static List<Location> _locations;

        public List<Location> locations
        {
            get
            {
                if (_locations == null)
                {
                    LoadAllTablesData();
                }
                return _locations;
            }
        }

        private static List<Buyer> _buyers;

        public List<Buyer> buyers
        {
            get
            {
                if (_buyers == null)
                {
                    LoadAllTablesData();
                }
                return _buyers;
            }
        }

        private static List<FrieghtCompany> _frieghtCompanies;

        public List<FrieghtCompany> frieghtCompanies
        {
            get
            {
                if (_frieghtCompanies == null)
                {
                    LoadAllTablesData();
                }
                return _frieghtCompanies;
            }
        }

        private static List<TruckType> _truckTypes;

        public List<TruckType> truckTypes
        {
            get
            {
                if (_truckTypes == null)
                {
                    LoadAllTablesData();
                }
                return _truckTypes;
            }
        }

        private static List<FrieghtCost> _frieghtCosts;

        public List<FrieghtCost> frieghtCosts
        {
            get
            {
                if (_frieghtCosts == null)
                {
                    LoadAllTablesData();
                }
                return _frieghtCosts;
            }
        }

        private static List<CashPrice> _cashPrices;

        public List<CashPrice> cashPrices
        {
            get
            {
                if (_cashPrices == null)
                {
                    LoadAllTablesData();
                }
                return _cashPrices;
            }
        }

        private ExcelDataService _excelService;

        private ExcelDataService excelService
        {
            get { return _excelService = _excelService ?? new ExcelDataService(); }
        }

        public DataLoader()
        {
            if (_locations == null)
            {
                LoadAllTablesData();
            }
        }

        private void LoadAllTablesData()
        {
            try
            {
                var dataSet = excelService.GetDataFromExcelSheet(HostingEnvironment.MapPath("~\\App_Data\\Data.xlsx"));
                if (dataSet != null && dataSet.Tables.Count > 0)
                {
                    LoadLocationsData(dataSet.Tables[0]);
                    LoadBuyersData(dataSet.Tables[1]);
                    LoadFrieghtCompanyData(dataSet.Tables[2]);
                    LoadTruckTypesData(dataSet.Tables[3]);
                    LoadFrieghtCostsData(dataSet.Tables[4]);
                    LoadCashPricesData(dataSet.Tables[5]);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void LoadLocationsData(DataTable dataTable)
        {
            _locations?.Clear();
            _locations = new List<Location>();
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                if (i < 1)
                {
                    continue;
                }

                Location location = new Location
                {
                    Id = Convert.ToInt64(dataTable.Rows[i][0].ToString()),
                    Name = dataTable.Rows[i][1].ToString(),
                    Latitude = Convert.ToDouble(dataTable.Rows[i][2].ToString().Replace("\"","")),
                    Longitude = Convert.ToDouble(dataTable.Rows[i][3].ToString().Replace("\"", ""))
                };
                _locations.Add(location);
            }
        }

        private void LoadBuyersData(DataTable dataTable)
        {
            _buyers?.Clear();
            _buyers = new List<Buyer>();
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                if (i < 1)
                {
                    continue;
                }

                Buyer buyer = new Buyer
                {
                    Id = Convert.ToInt64(dataTable.Rows[i][0].ToString().Replace(",","")),
                    Name = dataTable.Rows[i][1].ToString()
                };
                _buyers.Add(buyer);
            }
        }

        private void LoadFrieghtCompanyData(DataTable dataTable)
        {
            _frieghtCompanies?.Clear();
            _frieghtCompanies = new List<FrieghtCompany>();
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                if (i < 1)
                {
                    continue;
                }

                FrieghtCompany obj = new FrieghtCompany
                {
                    Id = Convert.ToInt64(dataTable.Rows[i][0].ToString().Replace(",", "")),
                    Name = dataTable.Rows[i][1].ToString()
                };
                _frieghtCompanies.Add(obj);
            }
        }

        private void LoadTruckTypesData(DataTable dataTable)
        {
            _truckTypes?.Clear();
            _truckTypes = new List<TruckType>();
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                if (i < 1)
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(dataTable.Rows[i][0].ToString()))
                {
                    TruckType obj = new TruckType
                    {
                        Id = Convert.ToInt64(dataTable.Rows[i][0].ToString().Replace(",", "")),
                        Type = dataTable.Rows[i][1].ToString(),
                        MinCapacity = Convert.ToDouble(dataTable.Rows[i][2].ToString()),
                        MaxCapacity = Convert.ToDouble(dataTable.Rows[i][3].ToString())
                    };
                    _truckTypes.Add(obj);
                }
            }
        }

        private void LoadFrieghtCostsData(DataTable dataTable)
        {
            _frieghtCosts?.Clear();
            _frieghtCosts = new List<FrieghtCost>();
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                if (i < 1)
                {
                    continue;
                }
                if (!string.IsNullOrEmpty(dataTable.Rows[i][0].ToString()))
                {
                    FrieghtCost obj = new FrieghtCost
                    {
                        Id = Convert.ToInt64(dataTable.Rows[i][0].ToString().Replace(",", "")),
                        FrieghtCompanyId = Convert.ToInt64(dataTable.Rows[i][1].ToString().Replace(",", "")),
                        TruckTypeId = Convert.ToInt64(dataTable.Rows[i][2].ToString().Replace(",", "")),
                        CostPerKm = Convert.ToDecimal(dataTable.Rows[i][3].ToString())
                    };
                    _frieghtCosts.Add(obj);
                }
            }
        }

        private void LoadCashPricesData(DataTable dataTable)
        {
            _cashPrices?.Clear();
            _cashPrices = new List<CashPrice>();
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                if (i < 1)
                {
                    continue;
                }
                if (!string.IsNullOrEmpty(dataTable.Rows[i][0].ToString()))
                {
                    CashPrice obj = new CashPrice
                    {
                        Id = Convert.ToInt64(dataTable.Rows[i][0].ToString().Replace(",", "")),
                        BuyerId = Convert.ToInt64(dataTable.Rows[i][1].ToString().Replace(",", "")),
                        LocationId = Convert.ToInt64(dataTable.Rows[i][2].ToString().Replace(",", "")),
                        Commodity = dataTable.Rows[i][3].ToString(),
                        Grade = dataTable.Rows[i][4].ToString(),
                        Season = dataTable.Rows[i][5].ToString(),
                        Price = Convert.ToDecimal(dataTable.Rows[i][6].ToString())
                    };
                    _cashPrices.Add(obj);
                }
            }
        }
    }
}