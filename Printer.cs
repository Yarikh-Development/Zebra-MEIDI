using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Net.NetworkInformation;
using System.Windows.Controls;
using Zebra.Sdk.Comm;
using Zebra.Sdk.Printer;

namespace demo
{
	public class City
	{
		public int ID { get; set; }
		public string Name { get; set; }
	}

	class Printer
	{
		private static ZebraPrinter printer = null;

		public static void GetPrinterInfo()
		{
			
		}

		//链接打印机
		public static string LinkPrinter(string printerName, int port)
		{
			try
			{
				string ip = GetRegistryData(printerName + "\\DsSpooler", "portName");

				if (ip == "" || ip.Split(new char[] { '.' }).Length != 4)
					throw new ConnectionException("没找到IP");
				foreach (string s in ip.Split(new char[] { '.' }))
					int.Parse(s);

				Ping pingSender = new Ping();
				PingReply reply = pingSender.Send(ip, 1);//第一个参数为ip地址，第二个参数为ping的时间
				if (reply.Status != IPStatus.Success)
					throw new ConnectionException("链接失败！");

				TcpConnection connection = new TcpConnection(ip, port);
				connection.Open();
				printer = ZebraPrinterFactory.GetInstance(connection);
			}
			catch(ConnectionException e)
			{
				printer = null;
				FileTools.WriteLineFile(FileTools.exceptionFilePath, DateTime.Now.ToString() + " " + printerName + e.Message);
				return printerName + e.Message;
			}
			catch(FormatException e)
            {
				printer = null;
				FileTools.WriteLineFile(FileTools.exceptionFilePath, DateTime.Now.ToString() + " " + printerName + "IP地址不正确！");
				return printerName + "IP地址不正确！";
			}

			return "";
		}

		//关闭链接
		public static void ClosePrinter()
		{
			if (printer != null)
            {
				printer.Connection.Close();
				printer = null;
            }
		}

		//发送文件
		public static bool SendFile(string filePath)
		{
			if (printer != null && filePath != "")
			{
				printer.SendFileContents(filePath);
				return true;
			}
			return false;
		}

		//判断是否为斑马打印机
		public static bool IsZebraPrinter(string printerName)
        {
			string name = GetRegistryData(printerName + "\\PnPData", "HardwareID");
			return name.ToLower().Contains("zebra");
		}

		//从注册表获取打印机数据
		public static string GetRegistryData(string subKey, string name)
        {
			string portQuery = @"System\CurrentControlSet\Control\Print\Printers\" + subKey;

			RegistryKey portKey = Registry.LocalMachine.OpenSubKey(portQuery, RegistryKeyPermissionCheck.Default,
								  System.Security.AccessControl.RegistryRights.QueryValues);
			if (portKey != null)
			{
				object IPValue = portKey.GetValue(name, String.Empty, RegistryValueOptions.DoNotExpandEnvironmentNames);
				if (IPValue == null)
					return "";
				else if (IPValue.ToString() != "System.String[]")
					return IPValue.ToString();
				else
					return ((string[])IPValue)[0];
			}

			return "";
		}

		//获取斑马打印机列表
		public static void SetPrinters(ComboBox comboBox)
        {
			int cnt = 0;
			List<City> list = new List<City>();
			foreach (string printerName in PrinterSettings.InstalledPrinters)
			{
				if (IsZebraPrinter(printerName))
				{
					list.Add(new City { ID = ++cnt, Name = printerName });
				}
			}
			comboBox.ItemsSource = list;
			comboBox.SelectedIndex = 0;
		}
	}
}
