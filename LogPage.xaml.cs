using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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

namespace demo
{
	/// <summary>
	/// LogPage.xaml 的交互逻辑
	/// </summary>
	public partial class LogPage : Page
	{
		private Window window = null;

		public LogPage(Window window)
		{
			InitializeComponent();

			this.window = window;

			SetComboBox();
		}

		~LogPage()
		{
			//todayFileStream.Close();
		}

		public void SetComboBox()
		{
			try
            {
				string[] directorieStrings = Directory.GetFiles(FileTools.logDirPath);

				int cnt = 0;
				foreach (string s in directorieStrings)
                {
					Log_File_ComboBox.Items.Add(s);
					++cnt;
				}					
				Log_File_ComboBox.SelectedIndex = cnt - 1;
			}
			catch (ArgumentNullException)
            {
				FileTools.WriteLineFile(DateTime.Now.ToString() + FileTools.exceptionFilePath, " 路径为空！！");
            }
			//Log_File_ComboBox.SelectedIndex = 0;
		}

		//加载记录文件
		public void LoadData(string filePath)
		{
			//DateTime.Now.ToShortTimeString().ToString();
			//DateTime.Now.ToString();
			//DateTime.Now.ToShortTimeString().ToString();

			try
            {
				StreamReader streamReader = new StreamReader(filePath);

				string line;
				string[] data;
				while ((line = streamReader.ReadLine()) != null)
				{
					data = line.Split(new char[] { ';' });
					Log_ListView.Items.Add(new { date = data[0], number = data[1], XML = data[2], ZPL = data[3], printer = data[4], success = data[5] });
				}

				streamReader.Close();
			}
			catch (FileNotFoundException)
            {
				FileTools.WriteLineFile(DateTime.Now.ToString() + " " + FileTools.exceptionFilePath, "未找到文件！");
            }
			
		}

		//写记录到listView
		public void WriteDataToListView(string[] data)
		{
			string date = Log_File_ComboBox.SelectedItem.ToString();
			string todayFile = FileTools.logDirPath + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
			if (date == todayFile && data.Length == 5)
            {
				Log_ListView.Items.Add(new { date = data[0], number = data[1], XML = data[2], ZPL = data[3], print = data[4] });
			}
		}

		private void Log_File_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Log_ListView.Items.Clear();
			LoadData(Log_File_ComboBox.SelectedItem.ToString());
		}
	}
}
