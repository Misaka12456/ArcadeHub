using ArcadeManager.Core;
using ArcadeManager.DataSourceModels;
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
using System.Windows.Shapes;

namespace ArcadeManager.Forms
{
	/// <summary>
	/// Win_WatcherLog.xaml 的交互逻辑
	/// </summary>
	public partial class Win_WatcherLog : Window
	{
		public Win_WatcherLog()
		{
			InitializeComponent();
			RefreshLogTable();
		}

		private void RefreshLogTable()
		{
			dgrd_watcherLog.ItemsSource = from watchLog in FolderWatchHelper.WatchLogs
										  orderby watchLog.Key descending
										  select new WatchEventLogSource()
										  {
											  LogTime = watchLog.Key.ToString("yyyy-M-d H:mm:ss"),
											  ClientName = watchLog.Value.Item1,
											  WatchingFolderType = watchLog.Value.Item2,
											  FileEventType = watchLog.Value.Item3,
											  FileName = watchLog.Value.Item4,
											  Comments = watchLog.Value.Item5 + "   "
										  };
		}

		private void tsmi_refresh_Click(object sender, RoutedEventArgs e)
		{
			RefreshLogTable();
		}
	}
}
