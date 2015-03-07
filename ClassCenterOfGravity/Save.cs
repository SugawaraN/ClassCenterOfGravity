using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Data;
using System.Threading;
using SaveToCSV;

namespace CenterOfGravity
{
    /// <summary>
    /// 1・2列目：重心
    /// 3・4・5・6・7・8列目：1023-圧力センサ値
    /// で出力
    /// 
    /// 表示用bitmap出力
    /// </summary>
    public class Save : Calculate
    {
        private const bool csvMode = true;
        private const string CSV = ".csv";
        private const string TXT = ".txt";
        private const string PRESSURE_GP = "PressureAndGP";
        private const bool Saved = true;
        private const bool NotSaved = false;
        private const int Tyokkei = 20;

        private string filename;
        private bool writemode = true;     //csvmode false:テキストファイル出力 true:CSV出力
        private Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");
        private bool flagMem = false;
        private bool flagSaving = false;
        private List<double[]> data;
        
        /// <summary>
        /// コンストラクタ①
        /// </summary>
        /// <param name="portname">ポート名指定</param>
        public Save(string portname ) : base(portname)
        {
            dataTable_pre();
        }
        
        /// <summary>
        /// コンストラクタ②
        /// </summary>
        public Save()
        {
            dataTable_pre();
        }
        
        /// <summary>
        /// データテーブル準備
        /// </summary>
        private void dataTable_pre()
        {
            data = new List<double[]>();
        }
        
        /// <summary>
        /// タイマーは外部でデータテーブルにのみ登録
        /// </summary>
        public void memData()
        {
            if (flagSaving) return;
            if (!flagMem) flagMem = true;
            double[] d = new double[8];
            for (int i = 0; i < 2; i++) d[i] = GravityPos[i];                 //重心
            for (int i = 0; i < SensorNumber; i++) d[i + 2] = 1023 - DP[i];   //圧力センサ値
            data.Add(d);
            //dataTable.Rows.Add(row);
        }
        /// <summary>
        /// データテーブル書き出し
        /// </summary>
        public void saveData(string filename, bool csvmode)
        {
            this.filename = filename;
            writemode = csvmode;
            stopTimer();
        }
        /// <summary>
        /// データテーブル書き出し
        /// </summary>
        public void saveData(string filename)
        {
            this.filename = filename;
            stopTimer();
        }

        /// <summary>
        /// 記録終了処理
        /// </summary>
        private void stopTimer()
        {
            flagSaving = true;
            flagMem = false;

            lock (data)
            {
                //テキスト形式
                if (!writemode)
                {
                    StreamWriter writer = fileOpen(filename + PRESSURE_GP + TXT);
                    StringBuilder sb = new StringBuilder();
                    foreach (double[] d in data)
                    {
                        int i;
                        for (i = 0; i < SensorNumber + 1; i++) sb.Append(d[i] + ",");
                        sb.Append(d[i] + "\n");
                    }
                    writer.Write(sb.ToString());
                    writer.Close();
                }
                else
                {
                    //CSV形式
                    SaveToCSV.SaveToCSV.DataTableToCsv(data, filename + PRESSURE_GP + CSV);
                }

                //データテーブル初期化
                //dataTable.Clear();
                data.Clear();
            }
            flagSaving = false;
        }
        /// <summary>
        /// ファイルを開く関数
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private StreamWriter fileOpen(string filename)
        {
            try
            {
                StreamWriter writer = new StreamWriter(@filename, false, sjisEnc);
                return writer;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        
    }
}
