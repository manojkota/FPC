using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelService.Abstract;

namespace ExcelService.Converters
{
    public class DataTableConverter : IExcelDataConverter<Model.Worksheet, DataTable>
    {
        public DataTable Convert(Model.Worksheet input)
        {
            DataTable dtData = null;

            if (input != null && input.Rows != null && input.Rows.Any())
            {
                dtData = new DataTable();

                for (int i = 0; i < input.Rows[0].Cells.Length; i++)
                {
                    dtData.Columns.Add(new DataColumn("Column" + i.ToString(), typeof(System.String)));
                }

                foreach (Model.Row row in input.Rows)
                {
                    if (row != null)
                    {
                        var dataRow = dtData.NewRow();
                        int cellCount = 0;
                        foreach (Model.Cell cell in row.Cells)
                        {
                            if (cell != null)
                            {
                                dataRow["Column" + cellCount.ToString()] = cell.Text;
                                cellCount++;
                            }
                        }

                        dtData.Rows.Add(dataRow);
                    }
                }
            }
            return dtData;
        }
    }
}