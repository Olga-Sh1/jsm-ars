using BaseElements;
using ExcelDataReader;
using JSMBase.Flights;
using JSMBase.RNK;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBase.Medicine.RWServices.Excel
{
    public abstract class ParserCommon
    {
        protected static readonly String TABLE_PARAMETERS_NAME = "Parameters";
        protected static readonly String TABLE_PASSPORT_NAME = "Passport";
        protected static readonly String TIME_COLUMN = "_Время";
        protected static readonly String TIME2_COLUMN = "_KEY";
        protected static readonly String FILE_SUMMARY_NAME = "summary.xls";
        protected static readonly String ID_NAME = "Date";

        /// <summary>Получить данные</summary>
        /// <param name="path">Директория</param>
        /// <returns></returns>
        public abstract Task<IEnumerable<Flight>> Parse(string path, IProgress<ProgressReport> pr);

        /// <summary>Получить данные по одному полету</summary>
        /// <param name="stream">Файл</param>
        /// <returns></returns>
        protected Flight ParseFlight(Stream stream, String fileName)
        {
            Flight f = new Flight();
            IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
            fillFlight(f, excelReader);
            //SetIds(f, fileName);
            return f;
        }
        /// <summary>Получить данные по одному полету</summary>
        /// <param name="fileName">Имя файла</param>
        /// <returns></returns>
        protected Flight ParseFlight(String fileName)
        {
            Flight f = new Flight();
            using (FileStream stream = File.Open(fileName, FileMode.Open, FileAccess.Read))
            {
                //1. Reading from a binary Excel file ('97-2003 format; *.xls)
                IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
                fillFlight(f, excelReader);
            }
            SetIds(f, fileName);
            return f;

        }

        protected void SetIds(Flight f, String fileName)
        {
            IPropInfo infID = null;
            int index = -1;
            for (int i = 0; i < Flight.Infos.Length; i++)
            {
                if (Flight.Infos[i].Name == "ID")
                {
                    infID = Flight.Infos[i];
                    index = i;
                }

            }
            if (infID != null)
            {
                f.PropValues[index] = new PropValueId(infID as PropInfoId, Path.GetFileNameWithoutExtension(fileName));
            }
            
        }

        protected void fillFlight(Flight f, IExcelDataReader excelReader)
        {
            //4. DataSet - Create column names from first row
            ExcelDataSetConfiguration conf = new ExcelDataSetConfiguration() { ConfigureDataTable = (_) => new ExcelDataTableConfiguration() { UseHeaderRow = true } };
            using (DataSet result = excelReader.AsDataSet(conf))
            {
                foreach (DataTable tbl in result.Tables)
                {
                    int rowNum = -1;
                    /*
                    if (tbl.TableName.StartsWith(ParserCommon.TABLE_PARAMETERS_NAME))
                    {
                        #region Временной ряд
                        foreach (DataRow row in tbl.Rows)
                        {
                            int num = (int)row.Field<double>(ParserCommon.TIME_COLUMN);
                            if (num != rowNum)
                            {
                                rowNum = num;
                                List<IPropValue> vals = new List<IPropValue>();
                                foreach (DataColumn dc in tbl.Columns)
                                {
                                    //if (dc.ColumnName != TIME_COLUMN && dc.ColumnName != TIME2_COLUMN)
                                    //{
                                    object val = row.ItemArray[dc.Ordinal];
                                    IPropInfo inf = Flight.TableInfos.FirstOrDefault(t => t.Name == dc.ColumnName);
                                    if (inf != null)
                                        vals.Add(inf.CreateValue(val));
                                    //}
                                }
                                if (!f.TablePropValues.ContainsKey(num))
                                    f.TablePropValues.Add(num, vals.ToArray());
                            }
                        }
                        #endregion
                    }
                    */
                }

                #region Общие данные из паспорта
                var passportTable = result.Tables[ParserCommon.TABLE_PASSPORT_NAME];
                if (passportTable != null)
                {
                    if (passportTable.Rows.Count > 0)
                    {
                        DataRow mainRow = passportTable.Rows[0];
                        int i = 0;
                        foreach (IPropInfo pr in Flight.Infos)
                        {
                            try
                            {
                                var ind = passportTable.Columns.IndexOf(pr.Name);
                                if (ind >= 0) f.PropValues[i] = Flight.Infos[i].CreateValue(mainRow[ind]);
                            }
                            catch
                            {
                                continue;
                            }
                            finally { i++; }
                        }
                    }
                }
                #endregion
            }
        }

        protected void AddInfo(String fileName, List<Flight> fls)
        {
            using (FileStream stream = File.Open(fileName, FileMode.Open, FileAccess.Read))
            {
                AddInfo(stream, fls);
            }
        }

        protected void AddInfo(Stream stream, List<Flight> fls)
        {
            //1. Reading from a binary Excel file ('97-2003 format; *.xls)
            IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);

            //4. DataSet - Create column names from first row
            ExcelDataSetConfiguration conf  = new ExcelDataSetConfiguration() { ConfigureDataTable = (_) => new ExcelDataTableConfiguration() { UseHeaderRow = true } };
            using (DataSet result = excelReader.AsDataSet(conf))
            {
                foreach (DataTable tbl in result.Tables)
                {
                    foreach (DataRow row in tbl.Rows)
                    {
                        int colId = tbl.Columns.IndexOf(ParserCommon.ID_NAME);
                        Flight f = null;
                        if (colId >= 0)
                        {
                            DateTime id = (DateTime)CustomConverter.TryConvert(row.ItemArray[colId], typeof(DateTime));
                            String sid = tbl.TableName + "_" + id.ToString("ddMMyyyy");
                            f = fls.FirstOrDefault(ff => ff.ID == sid);
                        }

                        if (f == null) continue;
                        for (int i = 0; i < Flight.Infos.Length; i++)
                        {
                            if (Flight.Infos[i] is PropInfoId) continue;
                            int col = tbl.Columns.IndexOf(Flight.Infos[i].Name);
                            if (col < 0) continue;
                            try
                            {
                                f.PropValues[i] = Flight.Infos[i].CreateValue(row.ItemArray[col]);
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                }


            }
        }
    }
}
