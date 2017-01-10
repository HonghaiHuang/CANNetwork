using CANNetwork.DataClass;
using System;
using System.Collections.Generic;
using System.Data;
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
            // string[] daf= AccessHelper.GetTableName();
            UpdatatoDG();
          //  MessageBox.Show(AdjustID(40, "6fa"));
        }


        private void UpdatatoDG()
        {

            tblDatas = new System.Data.DataTable("Datas");
            string sql = "select " + "*" + " from " + "通用车型";
            //     tblDatas = AccessTestInfo.SelectToDataTable_DataGrid(sql, "");

            tblDatas = AccessHelper.SelectToDataTable_DataGrid(sql, "");
            DG1.ItemsSource = tblDatas.DefaultView;

        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            if (DataProcess.ConnetCan()==true)
            {
                btnConnect.Visibility = Visibility.Collapsed;
                btnDisConnect.Visibility = Visibility.Visible;
            }
            else
            {
                btnConnect.Visibility = Visibility.Visible;
                btnDisConnect.Visibility = Visibility.Collapsed;
            }
        }

        private void btnDisConnect_Click(object sender, RoutedEventArgs e)
        {
            if (DataProcess.DisConnetCan() == true)
            {
                btnConnect.Visibility = Visibility.Visible;
                btnDisConnect.Visibility = Visibility.Collapsed;
            }
            else
            {
                btnConnect.Visibility = Visibility.Collapsed;
                btnDisConnect.Visibility = Visibility.Visible;
            }

        }



        /// <summary>
        /// 单通道标定
        /// </summary>
        private void SingleChannelCalibration(string[] dada)
        {
            string[] SendData=new string[2];
            SendData[0] =AdjustID(Convert.ToInt32(dada[0]) + 1, dada[1]);
            SendData[1] = AdjustParameter(Convert.ToInt32(dada[0]) + 9, Convert.ToInt32(dada[1]));
            DataProcess.DeviceDemarcate(SendData);
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

    }
}
