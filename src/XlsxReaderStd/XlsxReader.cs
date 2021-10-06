using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XlsxReader4_6
{
    public sealed class XlsxReader
    {
        public DataSet Read(String path, XlsxReaderConfig config)
        {
            DataSet ds = new DataSet();
            using(FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(fs, false))
                {
                    WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;
                    Dictionary<int, string> shared = new Dictionary<int, string>();
                    SharedStringTablePart[] sstpart = workbookPart.GetPartsOfType<SharedStringTablePart>().ToArray();
                    if (sstpart.Length > 0)
                    {
                        var sst = sstpart[0].SharedStringTable;
                        var shitems = sst.Elements<SharedStringItem>().ToArray();
                        for (int i = 0; i < shitems.Length; i++)
                        {
                            shared.Add(i, shitems[i].InnerText);
                        }
                    }

                    Sheets shts = workbookPart.Workbook.Sheets;
                    foreach (Sheet sh in shts)
                    {
                        DataTable dt = new DataTable();
                        dt.TableName = sh.Name;
                        ds.Tables.Add(dt);
                        var worksheetPart = (WorksheetPart)workbookPart.GetPartById(sh.Id);
                        int header_skip = config.Headers.ContainsKey(sh.Name) ? config.Headers[sh.Name] : 0;
                        var dims = worksheetPart.Worksheet.Descendants<SheetDimension>().ToArray();

                        var rows = worksheetPart.Worksheet.Descendants<Row>().Skip(header_skip).ToArray();
                        //var cols = worksheetPart.Worksheet.Descendants<Column>().ToArray();
                        var cells2 = worksheetPart.Worksheet.Descendants<Cell>().ToArray();

                        int col_max = rows.Select(rr => rr.Descendants<Cell>().Count()).Max();
                        for (int i = 0; i < col_max; i++)
                        {
                            DataColumn dc = new DataColumn();
                            dc.AllowDBNull = true;
                            dc.ColumnName = getColumnLetters(i);
                            dt.Columns.Add(dc);
                        }

                        foreach (Row row in rows)
                        {
                            Cell[] cells = row.Descendants<Cell>().ToArray();
                            Object[] obs = new object[col_max];

                            int index_cell = 0;
                            for (int index = 0; index < col_max; index++)
                            {

                                if (cells.Length <= index_cell)
                                {
                                    obs[index] = Convert.DBNull;
                                    continue;
                                }
                                String colName = getColumnLetters(index);

                                if (cells[index_cell].CellReference.HasValue &&
                                    !cells[index_cell].CellReference.Value.StartsWith(colName))
                                {
                                    obs[index] = Convert.DBNull;
                                    continue;
                                }
                                Cell c = cells[index_cell++];
                                if (c.CellValue != null)
                                    obs[index] = convert(c, shared);
                                else
                                    obs[index] = Convert.DBNull;
                            }

                            dt.Rows.Add(obs);
                        }
                    }
                }
            }
           
            return ds;
        }

        private Object convert(Cell c, Dictionary<int, string> shared)
        {
            string text = c.CellValue.InnerText;
            if (c.DataType == null)
                return Double.Parse(text, CultureInfo.InvariantCulture);
            else
            {
                switch (c.DataType.Value)
                {
                    case CellValues.Boolean:
                        return Convert.ChangeType(text, typeof(Boolean));
                    case CellValues.Date:
                        return Convert.ChangeType(text, typeof(DateTime));
                    case CellValues.InlineString:
                        return text;
                    case CellValues.Number:
                        return Double.Parse(text, CultureInfo.InvariantCulture);
                    case CellValues.SharedString:
                        int shindex = int.Parse(text);
                        return shared.ContainsKey(shindex) ? shared[shindex] : Convert.DBNull;
                    case CellValues.String:
                        return text;
                    default:
                        return Convert.DBNull;
                }
            }
        }

        private static String alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private String getColumnLetters(int currentColumn)
        {
            int majorColumn = (currentColumn) / alphabet.Length - 1;
            int minorColumn = (currentColumn) % alphabet.Length;
            StringBuilder sb = new StringBuilder();
            if (majorColumn >= 0)
                sb.Append(alphabet[majorColumn].ToString());
            sb.Append(alphabet[minorColumn].ToString());
            return  sb.ToString().Trim();
        }
    }
}
