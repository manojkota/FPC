using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using ExcelService.Abstract;

namespace ExcelService.Model
{
    public class Workbook : IWorkbook
    {
        public static sst SharedStrings;

        /// <summary>
        /// All worksheets in the Excel workbook deserialized
        /// </summary>
        /// <param name="ExcelFileName">Full path and filename of the Excel xlsx-file</param>
        /// <returns></returns>
        public IEnumerable<Worksheet> Worksheets(string ExcelFileName)
        {
            Worksheet ws;

            using (ZipArchive zipArchive = ZipFile.Open(ExcelFileName, ZipArchiveMode.Read))
            {
                SharedStrings = DeserializedZipEntry<sst>(GetZipArchiveEntry(zipArchive, @"xl/sharedStrings.xml"));
                foreach (var worksheetEntry in (WorkSheetFileNames(zipArchive)).OrderBy(x => x.FullName))
                {
                    ws = DeserializedZipEntry<Worksheet>(worksheetEntry);
                    ws.NumberOfColumns = Worksheet.MaxColumnIndex + 1;
                    ws.ExpandRows();
                    yield return ws;
                }
            }
        }

        /// <summary>
        /// Method converting an Excel cell value to a date
        /// </summary>
        /// <param name="ExcelCellValue"></param>
        /// <returns></returns>
        private DateTime DateFromExcelFormat(string ExcelCellValue)
        {
            return DateTime.FromOADate(Convert.ToDouble(ExcelCellValue));
        }

        private ZipArchiveEntry GetZipArchiveEntry(ZipArchive ZipArchive, string ZipEntryName)
        {
            return ZipArchive.Entries.First<ZipArchiveEntry>(n => n.FullName.Equals(ZipEntryName));
        }

        private IEnumerable<ZipArchiveEntry> WorkSheetFileNames(ZipArchive ZipArchive)
        {
            foreach (var zipEntry in ZipArchive.Entries)
                if (zipEntry.FullName.StartsWith("xl/worksheets/sheet"))
                    yield return zipEntry;
        }

        private T DeserializedZipEntry<T>(ZipArchiveEntry ZipArchiveEntry)
        {
            using (Stream stream = ZipArchiveEntry.Open())
                return (T)new XmlSerializer(typeof(T)).Deserialize(XmlReader.Create(stream));
        }
    }
}