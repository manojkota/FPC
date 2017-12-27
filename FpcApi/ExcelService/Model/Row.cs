using System.Xml.Serialization;

namespace ExcelService.Model
{
    public class Row
    {
        [XmlElement("c")]
        public Cell[] FilledCells;

        [XmlIgnore]
        public Cell[] Cells;

        public void ExpandCells(int NumberOfColumns)
        {
            Cells = new Cell[NumberOfColumns];
            if (FilledCells != null)
            {
                foreach (var cell in FilledCells)
                    Cells[cell.ColumnIndex] = cell;
            }
            FilledCells = null;
        }
    }
}