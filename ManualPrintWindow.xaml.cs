using System;
using System.Windows;
using Zebra.Sdk.Comm;

namespace demo
{
    /// <summary>
    /// ManualPrintWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ManualPrintWindow : Window
    {
        public ManualPrintWindow(Window window)
        {
            InitializeComponent();
            //设置父窗口
            this.Owner = window;

            //加载打印机列表
            Printer.SetPrinters(Printer_List_ComboBox);
        }

        private void Choice_File_Button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button button = sender as System.Windows.Controls.Button;
            if (button.Tag.ToString() == "XML")
                XML_TextBox.Text = FileTools.GetFileShortName(FileTools.OpenFile("*.xml|*.xml", "*.xml"));
            else if (button.Tag.ToString() == "ZPL")
                ZPL_TextBox.Text = FileTools.GetFileShortName(FileTools.OpenFile("*.zpl|*.zpl", "*.zpl"));
        }

        private void Print_Button_Click(object sender, RoutedEventArgs e)
        {
            //链接打印机
            string result = Printer.LinkPrinter(Printer_List_ComboBox.Text, TcpConnection.DEFAULT_ZPL_TCP_PORT);
            if (result == "")
            {
                //发文件
                Printer.SendFile(FileTools.labelDirPath + "\\" + ZPL_TextBox.Text);
                uint num = uint.Parse(Number_TextBox.Text);
                while (num-- > 0)
                    Printer.SendFile(FileTools.labelDirPath + "\\" + XML_TextBox.Text);

            }
            else
            {
                ((MainWindow)this.Owner).PrintState(result);
            }
        }

        public string[] ShowDialogAndResult()
        {
            this.ShowDialog();
            string[] data = new string[] { DateTime.Now.ToShortTimeString().ToString(), XML_TextBox.Text, ZPL_TextBox.Text, Printer_List_ComboBox.Text };
            foreach (string s in data)
            {
                if (s == "")
                    return null;
            }
            return data;
        }
    }
}
