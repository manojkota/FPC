using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelService.Model;

namespace ExcelService.Abstract
{
    public interface IWorkbook
    {
        IEnumerable<Worksheet> Worksheets(string ExcelFileName);
    }
}