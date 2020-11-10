using BarcodeScanner;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Zebra.Sdk.Comm;

namespace demo
{
	/// <summary>
	/// MainWindow.xaml 的交互逻辑
	/// </summary>
	public partial class MainWindow : Window
	{
		private RelationPage relationPage;
		private PicturesPage picturesPage;
		private LogPage logPage;

		private ScanerHook listener = new ScanerHook();

		public MainWindow()
		{
			//if (IsOnlyOneProcess())
			{
				InitializeComponent();
				FileTools.Init();
				TabItem tabitem = new TabItem();
				tabitem.Header = "图片";
				Frame tabFrame = new Frame();
				picturesPage = new PicturesPage();
				tabFrame.Content = picturesPage;
				tabitem.Content = tabFrame;
				Displaying_TabControl.Items.Add(tabitem);

				listener.ScanerEvent += ListenerScanerEvent;
			}
			//this.Close();
		}

		//进程判断
		private bool IsOnlyOneProcess()
		{
			int cnt = 0;
			System.Diagnostics.Process[] processList = System.Diagnostics.Process.GetProcesses();
			foreach (System.Diagnostics.Process process in processList)
			{
				if (process.ProcessName.ToLower() == "demo")
					++cnt;
			}

			if (cnt > 1)
				return false;
			else
				return true;
		}

		//钩子事件处理
		private void ListenerScanerEvent(ScanerHook.ScanerCodes codes)
		{
			//设置能效编号
			if (Auto_MenuItem.IsChecked)
            {
				picturesPage.SetNumber(codes.Result);
				AutoPrint(codes.Result);
			}
		}

		//创建打印机设置界面
		private void CreatePrinterWindowClick(object sender, RoutedEventArgs e)
		{
			//PrintDocument printDocument1 = new PrintDocument();
			////设置打印用的纸张,当设置为Custom的时候，可以自定义纸张的大小
			//printDocument1.DefaultPageSettings.PaperSize = new PaperSize("Custum", 500, 500);
			////注册PrintPage事件，打印每一页时会触发该事件
			//printDocument1.PrintPage += new PrintPageEventHandler(this.PrintDocument_PrintPage);

			//System.Windows.Forms.PrintDialog printDialog1 = new System.Windows.Forms.PrintDialog();
			////将PrintDialog.UseEXDialog属性设置为True，才可显示出打印对话框
			//printDialog1.UseEXDialog = true;
			////将printDocument1对象赋值给打印对话框的Document属性
			//printDialog1.Document = printDocument1;
			////打开打印对话框
			//DialogResult result = printDialog1.ShowDialog();
			//if (result == DialogResult.OK)
				;// printDocument1.Print();//开始打印
                 //PrinterWindow printerWindow = new PrinterWindow(this);
                 //printerWindow.ShowDialog();

            System.Windows.Forms.PrintDialog p = new System.Windows.Forms.PrintDialog();
			p.ShowDialog();
		}

		

		//创建用户设置界面
		private void CreateUserWindowClick(object sender, RoutedEventArgs e)
		{
			User userWindow = new User(this);
			userWindow.ShowDialog();
		}

		//创建DataList界面
		private void CreateDataListWindowClick(object sender, RoutedEventArgs e)
		{
            System.Windows.Controls.MenuItem menuItem = sender as System.Windows.Controls.MenuItem;
			if (menuItem.Name == Log_MenuItem.Name && logPage == null)
			{
				TabItem tabitem = new TabItem();
				tabitem.Header = "记录";
				Frame tabFrame = new Frame();
				logPage = new LogPage(this);
				tabFrame.Content = logPage;
				tabitem.Content = tabFrame;
				Displaying_TabControl.Items.Add(tabitem);
				if (relationPage == null)
					Displaying_TabControl.SelectedIndex = 1;
				else
					Displaying_TabControl.SelectedIndex = 2;
			}
			else if (menuItem.Name == Relation_MenuItem.Name && relationPage == null)
			{
				TabItem tabitem = new TabItem();
				tabitem.Header = "关联";
				Frame tabFrame = new Frame();
				relationPage = new RelationPage(this);
				tabFrame.Content = relationPage;
				tabitem.Content = tabFrame;
				Displaying_TabControl.Items.Add(tabitem);
				if (logPage == null)
					Displaying_TabControl.SelectedIndex = 1;
				else
					Displaying_TabControl.SelectedIndex = 2;
			}
		}

		//获取关联页面
		public RelationPage GetRelationPage()
        {
			return relationPage;
        }

		//获取记录页面
		public LogPage GetLogPage()
		{
			return logPage;
		}

		//标题按钮设置
		private void MinWindowButtonClick(object sender, RoutedEventArgs e)
		{
			WindowState = WindowState.Minimized;
		}

		private void MaxWindowButtonClick(object sender, RoutedEventArgs e)
		{
			if (WindowState == WindowState.Maximized)
				WindowState = WindowState.Normal;
			else
				WindowState = WindowState.Maximized;
		}

		private void ExitWindowButtonClick(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void WindowTitleMouseLeftButtonDown(object sender, RoutedEventArgs e)
		{
			DragMove();
		}

		//打印模式选择
		private void PrintModeClick(object sender, RoutedEventArgs e)
        {
			System.Windows.Controls.MenuItem menuItem = sender as System.Windows.Controls.MenuItem;
			if (menuItem.Name == "Auto_MenuItem")
            {
				listener.Start();
				Manual_MenuItem.IsChecked = false;
				Auto_MenuItem.IsChecked = true;
				Start_Print_MenuItem.IsEnabled = false;
				picturesPage.HideControl(true);
			}
			else if (menuItem.Name == "Manual_MenuItem")
            {
				listener.Stop();
				Auto_MenuItem.IsChecked = false;
				Manual_MenuItem.IsChecked = true;
				Start_Print_MenuItem.IsEnabled = true;
				picturesPage.HideControl(false);
			}

			picturesPage.EE_Number_TextBlock.Text = "";
			picturesPage.EE_Piceture_Image.Source = null;
			picturesPage.Overprint_Number_TextBlock.Text = "";
			picturesPage.Overprint_Picture_Image.Source = null;
			picturesPage.Preview_Number_TextBlock.Text = "";
			picturesPage.Preview_Picture_Image.Source = null;
        }

		//自动打印
		private void AutoPrint(string number)
        {
			picturesPage.LoadPicture();
			string[] data = GetRelationData((string)number);
			if (data == null)
				return;
			//链接打印机
			string result = Printer.LinkPrinter(data[3], TcpConnection.DEFAULT_ZPL_TCP_PORT);
			if (result != "")
            {
				State_TextBox.Text = result;
				return;
            }
			//发送文件
			bool zpl = Printer.SendFile(FileTools.labelDirPath + "\\" + data[1] + ".zpl"); //发送ZPL
			bool xml = Printer.SendFile(FileTools.labelDirPath + "\\" + data[2] + ".xml"); //发送XML
			if (!zpl && !xml)
            {
				State_TextBox.Text = "文件发送失败！";
				FileTools.WriteLineFile(DateTime.Now.ToString() + " " + FileTools.exceptionFilePath, data[3] + " " + State_TextBox.Text);
            }
			//记录写入
			string[] line = { DateTime.Now.ToShortTimeString().ToString(), data[0], data[1], data[2], data[3] };
			if (logPage != null)
				logPage.WriteDataToListView(line);
			FileTools.WriteLineFile(FileTools.logDirPath + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt",
									line[0] + ';' + line[1] + ';' + line[2] + ';' + line[3] + ';' + line[4] + ';' + "成功");
			Printer.ClosePrinter();
		}

		//手动打印
		private void ManualPrint()
		{
			//输入编号，加载图片
			string result = picturesPage.LoadPicture();
			if (result != "")
			{
				State_TextBox.Text = result;
				return;
			}
			//打印机操作
			ManualPrintWindow manualPrintWindow = new ManualPrintWindow(this);
			string[] data = manualPrintWindow.ShowDialogAndResult();
			Printer.ClosePrinter();
			if (data != null && data.Length == 4)
			{
				string[] line = { data[0], picturesPage.GetNumber(), data[1], data[2], data[3] };
				if (logPage != null)
					logPage.WriteDataToListView(line);
				FileTools.WriteLineFile(FileTools.logDirPath + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt",
								line[0] + ';' + line[1] + ';' + line[2] + ';' + line[3] + ';' + line[4] + ';' + "成功");
			}
		}

		//获取关联记录
		private string[] GetRelationData(string number)
        {
			StreamReader streamReader = new StreamReader(FileTools.relationFilePath);
			string line = "";
			string[] data;
			
			while ((line = streamReader.ReadLine()) != null)
            {
				data = line.Split(new char[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries);
				if (data.Length == 4 && number == data[0])
					return data;
			}

			return null;
		}

		//开始手动打印
		private void PrintClick(object sender, RoutedEventArgs e)
		{
			ManualPrint();
		}

		//消息显示
		public void PrintState(string msg)
        {
			State_TextBox.Text = msg;
        }

        private void MainWindowDeactivated(object sender, EventArgs e)
        {
			this.listener.Stop();
        }

        private void MainWindowActivated(object sender, EventArgs e)
        {
			this.listener.Start();
		}

        private void Displaying_TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
