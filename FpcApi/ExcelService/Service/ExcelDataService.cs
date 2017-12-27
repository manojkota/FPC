using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelService.Abstract;
using ExcelService.Converters;
using ExcelService.Model;

namespace ExcelService.Service
{
    public class ExcelDataService
    {
        private IWorkbook _workbook;

        private IWorkbook workbook
        {
            get { return _workbook = _workbook ?? new Workbook(); }
        }

        private IExcelDataConverter<Worksheet, DataTable> _dataConverter;

        private IExcelDataConverter<Worksheet, DataTable> dataConverter
        {
            get { return _dataConverter = _dataConverter ?? new DataTableConverter(); }
        }

        public ExcelDataService()
        {
        }

        public DataSet GetDataFromExcelSheet(String fileName)
        {
            DataSet excelData = new DataSet();
            var worksheets = workbook.Worksheets(fileName);

            if (worksheets != null && worksheets.Any())
            {
                foreach (var item in worksheets)
                {
                    DataTable dt = dataConverter.Convert(item);
                    if (dt != null)
                    {
                        excelData.Tables.Add(dt);
                    }
                }
            }

            return excelData;
        }
    }
}