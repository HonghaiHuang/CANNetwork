
/**
* 命名空间: CANNetwork.DataClass
*
* 功 能： N/A
* 类 名： DataProcess
*
* Ver 变更日期 负责人 变更内容
* ───────────────────────────────────
* V1.0.0 2017/1/8 19:32:01 tsunami(HHH)
*
* Copyright (c) 2016 Lir Corporation. All rights reserved.
*┌──────────────────────────────────┐
*│　此技术信息为本公司机密信息，未经本公司书面同意禁止向第三方披露．　│
*│　版权所有：广州市富元电子有限公司 　　　　　　　　　　　　　　│
*└──────────────────────────────────┘
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CVTSTS.DataClass;

namespace CANNetwork.DataClass
{
    public static class DataProcess
    {
        /// <summary>
        /// CAN处理线程
        /// </summary>
        private static Thread thr_RecvData;

        /// <summary>
        /// Can连接状态
        /// </summary>
        private static bool Canstate = false;

        /// <summary>
        /// 下位机指令列表
        /// </summary>
        private static volatile ArrayList TableOrderList = new ArrayList();

        /// <summary>
        /// 下位机指令下发间隔
        /// </summary>
        private static DateTime tableOrderInterval = DateTime.Now;

        /// <summary>
        /// 设定ID
        /// </summary>
        private static string CanSendID = "7ec";

        /// <summary>
        /// CAN控制模块对象
        /// </summary>
        private static CANControl CAN = new CANControl();


        public static string ModelNumer = "通用车型";

        /// <summary>
        /// 连接Can
        /// </summary>
        /// <returns></returns>
        public static bool ConnetCan()
        {

            if (CAN.ConncetCan())
            {
                Canstate = true;
                thr_RecvData = new Thread(DataProcessThread) { IsBackground = true };
                thr_RecvData.Priority = ThreadPriority.Highest;
                thr_RecvData.Start();
            }
            else
            {
                Canstate = false;
            }
            return Canstate;
        }

        /// <summary>
        /// 设置ID
        /// </summary>
        /// <param name="IDdata"></param>
        public static void SetCanID(string IDdata)
        {
            CanSendID = IDdata;
            CAN.CanID = IDdata;
        }

        /// <summary>
        /// 断开Can连接
        /// </summary>
        /// <returns></returns>
        public static bool DisConnetCan()
        {
            if(thr_RecvData!=null)
            {
                thr_RecvData.Abort();
                thr_RecvData = null;
            }
            if (CAN.DisConnectCan() == true)
            {
                Canstate = false;
            }
            else
            {
                Canstate = true;
            }
            return Canstate;
        }


        /// <summary>
        /// 数据处理线程
        /// </summary>
        private static void DataProcessThread()
        {
            var mcuData = new string[33];
            var mcuDataCount = 0;
            var collocetMcuData = false;
            var recvData = new string[8] { "", "", "", "", "", "", "", "" };
            while (true)
            {
                if (CAN != null)
                {
                    #region 数据接收部分

                //    thrSleepTime = DateTime.Now.AddMilliseconds(0);
                    //Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                CAN接收数据开始");

                    var data = CAN.RecviveData();
                    //Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                CAN接收数据结束");

                    if (data.Length != 0)
                    {
                        foreach (string t in data)
                        {
                            recvData = t.Split(' ');
                            // Debug.WriteLine(" 接到一组数据:" + t);

                            //处理下位机采集数据
                            if (recvData[0] == "00")
                            {
                                if (recvData[1] == "00")
                                {
                                    //Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff ")+"                   接到一组数据");
                                    collocetMcuData = true;
                                    mcuDataCount = 0;
                                }
                                if (collocetMcuData)
                                {
                                    
                                }
                            }

                            for (var a = 0; a < recvData.Length; a++)
                            {
                                recvData[a] = "";
                            }
                        }
                    }

                    #endregion


                    #region 数据处理部分

                    //发送下位机指令List中的指令
                    if (TableOrderList.Count > 0)
                    {
                        if (DateTime.Now > tableOrderInterval)
                        {
                            if (TableOrderList.Count > 0)
                            {
                                CAN.SendData(CanSendID, TableOrderList[0].ToString());
                                /* Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") +
                                                    "                           向下位机发送指令：" + TableOrderList[0].ToString());*/

                                tableOrderInterval = DateTime.Now.AddMilliseconds(10);
                                TableOrderList.RemoveAt(0);
                            }
                        }
                    }

                    #endregion

                }
            }

        }


        /// <summary>
        /// 将指令发送到CAN上
        /// </summary>
        /// <param name="order">指令</param>
        public static void SendOrderToCan(string order)
        {
            TableOrderList.Add(order);
            //Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                          "+ order);
        }

        /// <summary>
        /// 设备标定
        /// </summary>
        /// <param name="data">标定值</param>
        public static void DeviceDemarcate(string[] data)
        {
            foreach (var t in data)
            {
                TableOrderList.Add(t);
            }
        }


    }
}
