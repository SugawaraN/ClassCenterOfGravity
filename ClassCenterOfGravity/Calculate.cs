using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Net.Sockets;
using System.IO;
using System.Data;
using System.Threading;
using SaveToCSV;

namespace CenterOfGravity
{
    /***
     * 重心計算に特化
     */
    public class Calculate
    {
        //----定数-----------------
        //private const int sensorNumber = 6;
        private const int sensorNumber = 6;
        private const int pressureMax = 1023;
        private double[,] sensorPos = new double[,] { { 3, 1 }, { 5, 1 }, { 7, 3 }, { 1, 3 }, { 3, 5 }, { 5, 5 } }; //センサの位置
        private int[] centerPos = { 4, 3 }; //センサの座標のど真ん中
        
        //----変数-----------------
        private string portname = "COM4";
        private SerialPort myPort;                                      //使用するシリアルポート
        private string data;                                           //受信データ処理に使用
        private string[] strP;                                          //受信データ処理に使用
        private int[] dP = { 1023, 1023, 1023, 1023, 1023, 1023 };      //受信データ処理に使用
        private double[] gravityPos = { 0, 0 };                         //重心座標

        public int SensorNumber { get { return sensorNumber; } }
        public int PressureMax { get { return pressureMax; } }
        public double[,] SensorPos { get { return sensorPos; } }
        public int[] CenterPos { get { return centerPos; } }
        public int[] DP { get { return dP; } }
        public double[] GravityPos { get { return gravityPos; } }
        
        /// <summary>
        /// コンストラクタ①
        /// </summary>
        protected Calculate()
        {
            myPort_Start();
            data = "";
        }
        /// <summary>
        /// コンストラクタ②
        /// </summary>
        protected Calculate(string portname)
        {
            this.portname = portname;
            myPort_Start();
            data = "";
        }
        
        /// <summary>
        /// シリアルポートの設定
        /// </summary>
        private void myPort_Start()
        {
            if (myPort != null) myPort.Close();
            myPort = new SerialPort(portname, 9600, Parity.None, 8, StopBits.One);
            myPort.DataReceived += new SerialDataReceivedEventHandler(myPort_DataReceived);
            try
            {
                myPort.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return;
            }
            myPort.DtrEnable = false;
            myPort.RtsEnable = false;
        }

        /// <summary>
        /// データ受信時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void myPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (myPort != null)
            {
                data = myPort.ReadLine();
                strP = data.Split(',');

                //データ数SensorNumber+2以外は受け付けない
                if (strP.Count() != sensorNumber + 2)
                    return;
            }
            //重心計算
            double sum = 0;
            for (int j = 0; j < 2; j++) gravityPos[j] = 0;

            for (int i = 0; i < sensorNumber; i++)
            {
                if (int.TryParse(strP[i], out dP[i]))
                    for (int j = 0; j < 2; j++)
                        gravityPos[j] += (pressureMax - dP[i]) * sensorPos[i, j];
                else return;
                sum += pressureMax - dP[i];
            }
            //0割回避 座ってない時
            if (sum == 0) for (int j = 0; j < 2; j++) gravityPos[j] = 0;
            else for (int j = 0; j < 2; j++)　gravityPos[j] /= sum;   
        }
    }

}
