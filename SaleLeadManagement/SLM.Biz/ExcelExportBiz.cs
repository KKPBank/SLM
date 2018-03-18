using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.IO;

namespace SLM.Biz
{
    public class ExcelExportBiz
    {
        string _error = "";
        public string ErrorMessage {  get { return _error; } }
        public enum ColumnType
        {
            Text,
            Number,
            DateTime,
            Money
        }

        public partial class ColumnItem
        {
            public string ColumnName { get; set; }
            public ColumnType ColumnDataType { get; set; }
        }

        public partial class SheetItem
        {
            public SheetItem()
            {
                RowPerSheet = 0;
                SheetName = "Sheet";
            }

            public List<ColumnItem> Columns { get; set; }
            public List<object[]> Data { get; set; }
            public string SheetName { get; set; }
            public int RowPerSheet { get; set; }
        }
 
        public bool CreateCSV(string Filename, List<SheetItem> Sheets)
        {
            bool ret = true;
            string sql;
            try
            {
                if (File.Exists(Filename)) File.Delete(Filename);
                using (OleDbConnection objConn = new System.Data.OleDb.OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Path.GetDirectoryName(Filename) + ";Extended Properties='text;FMT=Delimited;HDR=YES;Mode=ReadWrite;ReadOnly=false;MaxScanRows=0;IMEX=0'"))
                {
                    using (OleDbCommand objCmd = new System.Data.OleDb.OleDbCommand())
                    {

                        objConn.Open();
                        objCmd.Connection = objConn;
                        objCmd.CommandType = System.Data.CommandType.Text;

                        // csv support only 1 sheet
                        if (Sheets.Count > 0)
                        {
                            var sh = Sheets[0];
                            var fname = Path.GetFileName(Filename);
                            var tmpf = Path.GetDirectoryName(Filename) + "\\" + sh.SheetName + "$.txt";

                            if (File.Exists(tmpf)) File.Delete(tmpf);

                            File.WriteAllText(tmpf, String.Join(",", sh.Columns.Select(c => c.ColumnName).ToArray()), Encoding.Default);
                            if (sh.Data == null || sh.Columns == null) throw new Exception("No Data");

                            sh.RowPerSheet = sh.Data.Count;

                            // insert data
                            for (int r = 0; r < sh.Data.Count; r++)
                            {
                                var lst = sh.Data[r];
                                sql = "insert into [" + sh.SheetName + "$] values (";
                                for (int i = 0; i < lst.Length; i++)
                                {
                                    if (i > 0) sql += ",";

                                    if (lst[i] == null || lst[i].ToString().Trim() == "") sql += "null";
                                    else
                                        sql += "'" + lst[i].ToString().Replace("'", "''") + "'";
                                }
                                sql += ")";
                                objCmd.CommandText = sql;
                                objCmd.ExecuteNonQuery();
                            }

                            if (File.Exists(Filename)) File.Delete(Filename);
                            if (File.Exists(tmpf)) File.Move(tmpf, Filename);
                        }

                        objConn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                ret = false;
                _error = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            }
            return ret;
        }



        public bool CreateExcel(string Filename, List<SheetItem> Sheets)
        {
            bool ret = true;
            string sql;
            try
            {
                if (File.Exists(Filename)) File.Delete(Filename);
                using (OleDbConnection objConn = new System.Data.OleDb.OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Filename + ";Extended Properties='Excel 8.0;Mode=ReadWrite;ReadOnly=false;HDR=YES;MaxScanRows=0;IMEX=0'"))
                {
                    using (OleDbCommand objCmd = new System.Data.OleDb.OleDbCommand())
                    {

                        objConn.Open();
                        objCmd.Connection = objConn;
                        objCmd.CommandType = System.Data.CommandType.Text;

                        foreach (var sh in Sheets)
                        {
                            if (sh.Data == null || sh.Columns == null) continue;

                            int page = 0;
                            int start = 0;
                            int lastrow = 0;

                            if (sh.RowPerSheet == 0) sh.RowPerSheet = sh.Data.Count;

                            while (lastrow < sh.Data.Count)
                            {
                                string cursheetname = sh.SheetName + (page == 0 ? "" : page.ToString());
                                // create table
                                sql = "create table [" + cursheetname + "]  (";
                                for (int i = 0; i < sh.Columns.Count; i++)
                                {
                                    var clm = sh.Columns[i];
                                    if (i > 0) sql += ",";
                                    sql += "[" + clm.ColumnName + "] ";
                                    switch (clm.ColumnDataType)
                                    {
                                        case ColumnType.Number:
                                            sql += "numeric(18,2)";
                                            break;

                                        case ColumnType.DateTime:
                                            sql += "date";
                                            break;

                                        case ColumnType.Money:
                                            sql += "currency";
                                            break;

                                        default:
                                            sql += "longtext";
                                            break;
                                    }
                                }
                                sql += ")";
                                objCmd.CommandText = sql;
                                objCmd.ExecuteNonQuery();

                                lastrow += sh.RowPerSheet;
                                if (lastrow > sh.Data.Count) lastrow = sh.Data.Count;

                                // insert data
                                for (int r = start; r < lastrow; r++)
                                {
                                    var lst = sh.Data[r];
                                    //sql = "insert into [" + cursheetname + "$] values (";
                                    sql = "insert into [" + cursheetname + "] values (";
                                    for (int i = 0; i < lst.Length; i++)
                                    {
                                        if (i > 0) sql += ",";

                                        if (lst[i] == null || lst[i].ToString().Trim() == "") sql += "null";
                                        else
                                            sql += "'" + lst[i].ToString().Replace("'", "''") + "'";
                                    }
                                    sql += ")";
                                    objCmd.CommandText = sql;
                                    objCmd.ExecuteNonQuery();
                                }

                                start += sh.RowPerSheet;
                                page++;
                            }
                        }

                        objConn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                ret = false;
                _error = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            }
            return ret;
        }

    }
}
