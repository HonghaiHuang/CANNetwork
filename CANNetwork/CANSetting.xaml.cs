using CANNetwork.DataClass;
using System;
using System.Collections.Generic;
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
    /// CANSetting.xaml 的交互逻辑
    /// </summary>
    public partial class CANSetting : UserControl
    {
        public CANSetting()
        {
            InitializeComponent();
            InitCombo();
        }

        private void InitCombo()
        {
            string[] ModelData = AccessHelper.GetTableName();
            if(ModelData.Length>0)
            {
                foreach (var data in ModelData)
                {
                    ComboModel.Items.Add(data);
                }
            }
            else
            {
                MessageBox.Show("没有找到车型号码，请到车型管理添加车型模板！");
            }


        }


        private void btnStarCAN_Click(object sender, RoutedEventArgs e)
        {
            if(IntData()==true)
            {
                if (DataProcess.ConnetCan() == true)
                {
                    DataProcess.SendOrderToCan(AdjustID(1, txtCanID.Text));

                    btnStarCAN.Visibility = Visibility.Collapsed;
                    btnStopCAN.Visibility = Visibility.Visible;
                }
            }
            else
            {
                btnStarCAN.Visibility = Visibility.Visible;
                btnStopCAN.Visibility = Visibility.Collapsed;
            }

        }

        private void btnStopCAN_Click(object sender, RoutedEventArgs e)
        {
            if (DataProcess.DisConnetCan() == true)
            {
                btnStarCAN.Visibility = Visibility.Visible;
                btnStopCAN.Visibility = Visibility.Collapsed;
            }
            else
            {
                btnStarCAN.Visibility = Visibility.Collapsed;
                btnStopCAN.Visibility = Visibility.Visible;
            }

        }

        private bool IntData()
        {
            if(txtCanID.Text!="")
            {
                DataProcess.SetCanID(txtCanID.Text);

                if (ComboModel.Text!="")
                {
                    DataProcess.ModelNumer = ComboModel.Text;
                    return true;
                }
                else
                {
                    DataProcess.ModelNumer = "";
                    return false;
                }
            }
            else
            {
                MessageBox.Show("请输入正确的ID!");
                return false;
            }
        }


        /// <summary>
        /// 调节ID类
        /// </summary>
        private string AdjustID(int VariableNumber, string Updata)
        {
            string data = "01 00 " + ValueToHex(VariableNumber) + " " + ValueToHex(Int32.Parse(Updata, System.Globalization.NumberStyles.HexNumber)) + " 00 00";
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
