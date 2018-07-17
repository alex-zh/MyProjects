using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Common
{
    //загружает данные из Excel
    public class ExcelLoader
    {
        string _filename;

        public ExcelLoader(string filename)
        {
            _filename = filename;
        }

        //range has format A2:D44
        public IEnumerable<Candle> Load(string sheetName, string range)
        {
            var result = new List<Candle>();

            using (SpreadsheetDocument myDoc = SpreadsheetDocument.Open(_filename, true))
            {
                    WorkbookPart workbookPart = myDoc.WorkbookPart;
                WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();

                WorksheetPart sheetPart = GetWorksheetPart(workbookPart, sheetName);

                SheetData sheetdata = sheetPart.Worksheet.GetFirstChild<SheetData>();

                foreach (Row r in sheetdata.Elements<Row>())
                {
                    foreach (Cell c in r.Elements<Cell>())
                    {                        
                        string text = c.CellValue.Text;
                    }
                }
            }

            return result;
        }

        public WorksheetPart GetWorksheetPart(WorkbookPart workbookPart, string sheetName)
        {
            string relId = workbookPart.Workbook.Descendants<Sheet>().First(s => sheetName.Equals(s.Name)).Id;
            return (WorksheetPart)workbookPart.GetPartById(relId);
        }
    }
}
