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
                 //   DataProcess.SendOrderToCan(AdjustID(1, txtCanID.Text));
                    SetID();
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

        private string SaveOldID="";
        private void SetID()
        {
            if(SaveOldID == "")
            {
                DataProcess.SetCanID(txtCanID.Text);
                DataProcess.SendOrderToCan(AdjustID(1, txtCanID.Text));
            }
            else
            {
                if (SaveOldID != txtCanID.Text)
                {
                    DataProcess.SendOrderToCan(AdjustID(1, txtCanID.Text));
                    System.Threading.Thread.Sleep(50);
                    DataProcess.SetCanID(txtCanID.Text);
                }
                else
                {
                    DataProcess.SendOrderToCan(AdjustID(1, txtCanID.Text));
                    DataProcess.SetCanID(txtCanID.Text);
                }
            }
            SaveOldID = txtCanID.Text;


            DataProcess.SendOrderToCan(AdjustSleep(ComboSleep.SelectedIndex));
            DataProcess.SendOrderToCan(Adjustwatchdog(Combowatchdog.SelectedIndex));

            //  DataProcess.SetCanID(SaveOldID);


        }

        private bool IntData()
        {
            if(txtCanID.Text!="")
            {

                
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
        /// 睡眠模式，1是睡眠模式，0是非睡眠模式
        /// </summary>
        /// <param name="Senddata"></param>
        /// <returns></returns>
        private string AdjustSleep(int Senddata)
        {
            string data = "01 00 2b 00 "   + ValueToHex(Senddata) + " 00 00";
            return data;

        }

        /// <summary>
        /// 看门狗模式，1是打开看门狗模式，0是关闭看门狗模式
        /// </summary>
        /// <param name="Senddata"></param>
        /// <returns></returns>
        private string Adjustwatchdog(int Senddata)
        {
            string data = "01 00 2c 00 " + ValueToHex(Senddata) + " 00 00";
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
