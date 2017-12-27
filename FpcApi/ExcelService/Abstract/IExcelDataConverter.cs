using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelService.Abstract
{
    public interface IExcelDataConverter<fromType, toType>
    {
        toType Convert(fromType input);
    }
}