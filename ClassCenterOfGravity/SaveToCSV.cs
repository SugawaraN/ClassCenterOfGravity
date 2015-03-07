using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace SaveToCSV
{
    public class SaveToCSV
    {
        // <summary>
        /// DataTableからCSVファイルを作成します。以下、引用。
        /// http://okwakatta.net/code/dst24.html
        /// </summary>
        /// <param name="dt">データテーブル</param>
        /// <param name="filePath">ファイルパス</param>
        /// <param name="header">ヘッダーを出力するかどうか true:出力する false:出力しない</param>
        static public void DataTableToCsv(DataTable dt, string filePath, bool header)
        {
            string sp = string.Empty;
            System.IO.StreamWriter sw = null;
            List<int> filterIndex = new List<int>();

            try
            {
                sw = new System.IO.StreamWriter(filePath, false, System.Text.Encoding.GetEncoding("Shift_JIS"));

                //----------------------------------------------------------//
                // DataColumnの型から値を出力するかどうか判別します         //
                // 出力対象外となった項目は[データ]という形で出力します     //
                //----------------------------------------------------------//
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    switch (dt.Columns[i].DataType.ToString())
                    {
                        case "System.Boolean":
                        case "System.Byte":
                        case "System.Char":
                        case "System.DateTime":
                        case "System.Decimal":
                        case "System.Double":
                        case "System.Int16":
                        case "System.Int32":
                        case "System.Int64":
                        case "System.SByte":
                        case "System.Single":
                        case "System.String":
                        case "System.TimeSpan":
                        case "System.UInt16":
                        case "System.UInt32":
                        case "System.UInt64":
                            break;

                        default:
                            filterIndex.Add(i);
                            break;
                    }
                }

                //----------------------------------------------------------//
                // ヘッダーを出力します。                                   //
                //----------------------------------------------------------//
                if (header)
                {
                    foreach (DataColumn col in dt.Columns)
                    {
                        sw.Write(sp + "\"" + col.ToString().Replace("\"", "\"\"") + "\"");
                        sp = ",";
                    }
                    sw.WriteLine();
                }

                //----------------------------------------------------------//
                // 内容を出力します。                                       //
                //----------------------------------------------------------//
                foreach (DataRow row in dt.Rows)
                {
                    sp = string.Empty;
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        if (filterIndex.Contains(i))
                        {
                            sw.Write(sp + "\"[データ]\"");
                            sp = ",";
                        }
                        else
                        {
                            //sw.Write(sp + "\"" + row[i].ToString().Replace("\"", "\"\"") + "\"");
                            sw.Write(sp + row[i].ToString().Replace("\"", "\"\""));
                            sp = ",";
                        }
                    }
                    sw.WriteLine();
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                }
            }
        }
    }
}
