using CANNetwork.DataClass;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CANNetwork
{
    /// <summary>
    /// CalibrationPage.xaml 的交互逻辑
    /// </summary>
    public partial class CalibrationPage : Page
    {


        DataTable tblDatas;
        public CalibrationPage()
        {
            InitializeComponent();
            UpdatatoDG();

            DataProcess.OnMsgReceived += new DataProcess.ReceiveMsgHandler(Logger);
            if (DataProcess.Canstate)
            {
                Logger("通信正常！");
            }
            else
            {
                for (int i = 0; i < 100; i++)
                {
                    Logger("通信异常！");

                }

            }

        }

        private void Logger(string msgdata)
        {

            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                TxtMsg.AppendText(msgdata + "\r\n");  //添加文本
                TxtMsg.ScrollToLine(TxtMsg.LineCount-1);    //自动显示至最后行

                //进行界面赋值
            //    TxtMsg.Text += msgdata+"\r\n";
            }));

        }

        private void UpdatatoDG()
        {
            if(DataProcess.ModelNumer!="")
            {
                tblDatas = new System.Data.DataTable("Datas");
                string sql = "select " + "*" + " from " + DataProcess.ModelNumer;

                tblDatas = AccessHelper.SelectToDataTable_DataGrid(sql, "");
                DG1.ItemsSource = tblDatas.DefaultView;
            }

        }

        //private void btnConnect_Click(object sender, RoutedEventArgs e)
        //{
        //    if (DataProcess.ConnetCan()==true)
        //    {
        //        btnConnect.Visibility = Visibility.Collapsed;
        //        btnDisConnect.Visibility = Visibility.Visible;
        //    }
        //    else
        //    {
        //        btnConnect.Visibility = Visibility.Visible;
        //        btnDisConnect.Visibility = Visibility.Collapsed;
        //    }
        //}

        //private void btnDisConnect_Click(object sender, RoutedEventArgs e)
        //{
        //    if (DataProcess.DisConnetCan() == true)
        //    {
        //        btnConnect.Visibility = Visibility.Visible;
        //        btnDisConnect.Visibility = Visibility.Collapsed;
        //    }
        //    else
        //    {
        //        btnConnect.Visibility = Visibility.Collapsed;
        //        btnDisConnect.Visibility = Visibility.Visible;
        //    }

        //}


        /// <summary>
        /// 单通道标定
        /// </summary>
        private void SingleChannelCalibration(string[] data)
        {
            string[] SendData=new string[5];
            SendData[0] =AdjustID(Convert.ToInt32(data[0]) + 1, data[1]);
            SendData[1] = AdjustParameter(Convert.ToInt32(data[0]) + 9, Convert.ToInt32(data[2]));
            SendData[2] = AdjustParameter(Convert.ToInt32(data[0]) + 17, Convert.ToInt32(data[3]));
            SendData[3] = AdjustByte(Convert.ToInt32(data[0]) + 33, Convert.ToInt32(data[4]), Convert.ToInt32(5));
            SendData[4] = AdjustEnable(Convert.ToInt32(data[0]) + 25, Convert.ToInt32(data[6]));
            DataProcess.DeviceDemarcate(SendData);
        }


        private void SingleRowCalibration(string HeaderName, string Value,string[] data)
        {
            
            switch (HeaderName)
            {
                case "ID标定值":
                    
                    DataProcess.SendOrderToCan(AdjustID(Convert.ToInt32(data[0]) + 1, Value));
                    break;
                case "参数a":
                    DataProcess.SendOrderToCan(AdjustParameter(Convert.ToInt32(data[0]) + 9, Convert.ToInt32(Value))); 
                    break;

                case "参数b":
                    DataProcess.SendOrderToCan(AdjustParameter(Convert.ToInt32(data[0]) + 17, Convert.ToInt32(Value))); 
                    break;

                case "开始字节":
                    DataProcess.SendOrderToCan(AdjustByte(Convert.ToInt32(data[0]) + 33, Convert.ToInt32(data[4]), Convert.ToInt32(5)));
                    break;

                case "末始字节":
                    DataProcess.SendOrderToCan(AdjustByte(Convert.ToInt32(data[0]) + 33, Convert.ToInt32(data[4]), Convert.ToInt32(5)));
                    break;

                case "是否干预":
                    DataProcess.SendOrderToCan(AdjustEnable(Convert.ToInt32(data[0]) + 25, Convert.ToInt32(Value)));
                    break;



            }
        }

        /// <summary>
        /// 调节ID类
        /// </summary>
        private string AdjustID(int VariableNumber,string Updata)
        {
            string data = "01 00 " + ValueToHex(VariableNumber)+" "+ ValueToHex(Int32.Parse(Updata, System.Globalization.NumberStyles.HexNumber))+" 00 00";
           
            return data;
        }

        /// <summary>
        /// 调节参数类
        /// </summary>
        /// <param name="VariableNumber"></param>
        /// <param name="Updata"></param>
        /// <returns></returns>
        private string AdjustParameter(int VariableNumber, int Updata)
        {
            string data = "01 00 " + ValueToHex(VariableNumber) + " " + ValueToHex(Updata) + " 00 00";
            return data;
        }

        /// <summary>
        /// 调节字节类
        /// </summary>
        /// <returns></returns>
        private string AdjustByte(int VariableNumber, int Updata1, int Updata2)
        {
            string data = "01 00 " + ValueToHex(VariableNumber) + " " + Updata1.ToString("X2")+" " + Updata2.ToString("X2") + " 00 00";
            return data;

        }


        /// <summary>
        /// 调节干预类
        /// </summary>
        /// <param name="VariableNumber"></param>
        /// <param name="Updata"></param>
        private string AdjustEnable(int VariableNumber, int Updata)
        {
            string data = "01 00 " + ValueToHex(VariableNumber) + " " + ValueToHex(Updata) + " 00 00";
            return data;

        }

        /// <summary>
        /// 数值装四位十六进制
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private string ValueToHex(int data)
        {
            var valueH = (data / 256).ToString("X2");
            var valueL = (data % 256).ToString("X2");
            return valueL + " " + valueH;
        }

        /// <summary>
        /// 发生于一个单元格编辑已被确认或取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DG1_CellEditEnded(object sender, Telerik.Windows.Controls.GridViewCellEditEndedEventArgs e)
        {
            Telerik.Windows.Controls.GridViewCellEditEndedEventArgs edata = e;
            if (edata.NewData != "")
            {
                DataRowView mySelectedElement = (DataRowView)DG1.SelectedItem;
                string[] data = new string[7];

                //要添加这条件
                if (mySelectedElement != null)
                {
                    for (int i = 0; i < 7; i++)
                    {
                        data[i] = mySelectedElement.Row[i].ToString();
                    }
                    //    SingleChannelCalibration(data);
                }

                if (edata.NewData != edata.OldData && edata.NewData.ToString() != "")
                {
                    SingleRowCalibration(edata.Cell.Column.Header.ToString(), edata.NewData.ToString(), data);

                }
            }
            else
            {
                MessageBox.Show("输入格式不正确！");
            }
        }

        private void btnSingleCalibration_Click(object sender, RoutedEventArgs e)
        {
            DataRowView mySelectedElement = (DataRowView)DG1.SelectedItem;
            string[] data = new string[7];
            //要添加这条件
            if (mySelectedElement != null)
            {
                for (int i = 0; i < 7; i++)
                {
                    data[i] = mySelectedElement.Row[i].ToString();
                }
                    SingleChannelCalibration(data);
            }

        }

        private void btnAdddata_Click(object sender, RoutedEventArgs e)
        {
            string[] data = new string[7];
            DataTable DGDt = DataGridToDatable(DG1);
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    data[j] = DGDt.Rows[i][j].ToString();
                }

                SingleChannelCalibration(data);
            }

        }

        /// <summary>
        /// DataGrid转Datable
        /// </summary>
        /// <param name="dg"></param>
        /// <returns></returns>
        public System.Data.DataTable DataGridToDatable(Telerik.Windows.Controls.RadGridView dg)
        {
            try
            {


                System.Data.DataTable tmpdt = ((DataView)dg.ItemsSource).Table;
                return tmpdt;
            }
            catch (System.Exception ex)
            {
                throw ex;
                //  return null;
            }
        }

    }
}
