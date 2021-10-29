#pragma warning disable IDE0058
using ArcadeManager.Core;
using ArcadeManager.DataSourceModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ContextMenuStrip = System.Windows.Forms.ContextMenuStrip;
using NotifyIcon = System.Windows.Forms.NotifyIcon;
using Path = System.IO.Path;
using ToolStripMenuItem = System.Windows.Forms.ToolStripMenuItem;
using ToolStripSeparator = System.Windows.Forms.ToolStripSeparator;
using ResMgr = ArcadeManager.Properties.Resources;
using System.Drawing;
using System.Runtime.Remoting.Channels;

namespace ArcadeManager.Forms
{
	/// <summary>
	/// Win_Main.xaml 的交互逻辑
	/// </summary>
	public partial class Win_Main : Window
	{
		private bool isClosing;

		private static NotifyIcon trayIcon;
		public Win_Main()
		{
			InitializeComponent();
			Title += $" - Alpha {Assembly.GetExecutingAssembly().GetName().Version.ToString(3)} (开发中版本, 可能仍存在问题)";
		}

		private void AddTrayIcon()
		{
			if (trayIcon == null)
			{
				trayIcon = new NotifyIcon()
				{
					Icon = ResMgr.Logo,
					Text = "Project Arcade Manager"
				};
				trayIcon.Visible = true;
				var csp = new ContextMenuStrip();
				var tsmi_ntfi_clients = new ToolStripMenuItem()
				{
					Text = "选择Arcade发行版(&S)..."
				};
				foreach (var client in ClientHelper.ClientList)
				{
					var tsmi_ntfi_client = new ToolStripMenuItem()
					{
						Text = client.ClientName
					};
					tsmi_ntfi_client.Click += (sender, e) =>
					{
						lbl_status.Content = $"启动 {client.ClientName}...";
						Process.Start(new ProcessStartInfo()
						{
							FileName = client.ClientPath,
							UseShellExecute = false,
							WindowStyle = ProcessWindowStyle.Normal,
							WorkingDirectory = new FileInfo(client.ClientPath).DirectoryName
						});
						if (Visibility == Visibility.Visible)
						{
							WindowState = WindowState.Minimized;
						}
						lbl_status.Content = "就绪";
					};
					tsmi_ntfi_clients.DropDownItems.Add(tsmi_ntfi_client);
				}
				csp.Items.Add(tsmi_ntfi_clients);
				csp.Items.Add(new ToolStripSeparator());
				var tsmi_ntfi_file = new ToolStripMenuItem()
				{
					Text = "文件(&F)"
				};
				var tsmi_ntfi_file_showWatcherLog = new ToolStripMenuItem()
				{
					Text = "查看文件监测日志(&L)"
				};
				tsmi_ntfi_file_showWatcherLog.Click += (sender, e) =>
				{
					tsmi_file_showWatcherLog_Click(null, null);
				};
				tsmi_ntfi_file.DropDownItems.Add(tsmi_ntfi_file_showWatcherLog);
				csp.Items.Add(tsmi_ntfi_file);
				csp.Items.Add(new ToolStripSeparator());
				var tsmi_ntfi_showMain = new ToolStripMenuItem()
				{
					Text = "显示主窗口(&S)"
				};
				tsmi_ntfi_showMain.Click += (sender, e) =>
				{
					if (Visibility != Visibility.Visible)
					{
						Show();
					}
					Activate();
				};
				csp.Items.Add(tsmi_ntfi_showMain);
				csp.Items.Add(new ToolStripSeparator());
				var tsmi_ntfi_exit = new ToolStripMenuItem()
				{
					Text = "退出 Project Arcade Manager(&E)"
				};
				tsmi_ntfi_exit.Click += (sender, e) =>
				{
					var r = MessageBox.Show("退出程序后将无法继续监视所有Arcade发行版文件夹中的文件变动事件。\n" +
						"请确保在退出前关闭所有Arcade发行版主程序。\n" +
						"您确定要退出Project Arcade Manager吗?", "提示", MessageBoxButton.YesNo, MessageBoxImage.Warning);
					if (r == MessageBoxResult.Yes)
					{
						StopWatchers();
						isClosing = true;
						Application.Current.Shutdown();
					}
				};
				csp.Items.Add(tsmi_ntfi_exit);
				trayIcon.DoubleClick += (sender, e) =>
				{
					if (Visibility != Visibility.Visible)
					{
						Show();
					}
					Activate();
				};
				trayIcon.ContextMenuStrip = csp;
			}
		}

		private void DeleteTrayIcon()
		{
			trayIcon.Visible = false;
			trayIcon.Dispose();
			trayIcon = null;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			if (File.Exists(Path.Combine(AppContext.BaseDirectory, "config.dat")))
			{
				ClientHelper.LoadClientList();
				lbx_Main.ItemsSource = from client in ClientHelper.ClientList select AdeClientSource.FromArcadeClient(client);
				StartWatchers();
				AddTrayIcon();
			}
			else
			{
				var inputResult = MessageBox.Show("系统检测到您是首次启动Project Arcade Manager, 是否立即搜索已安装的Arcade发行版?\n" +
					"您也可以稍后点击主界面菜单栏中的\"管理\"-\"重新搜索已安装的Arcade发行版\"手动搜索。", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question);
				if (inputResult == MessageBoxResult.Yes)
				{
					lbl_status.Content = "正在搜索已安装的Arcade发行版……";
					lbx_Main.IsEnabled = false;
					msp_Main.IsEnabled = false;
					new Thread(async () =>
					{
						var r = await ClientHelper.SearchClientsAsync();
						ClientHelper.ClientList = r;
						ClientHelper.SaveClientList();
						Dispatcher.Invoke(() =>
						{
							lbx_Main.ItemsSource = from client in ClientHelper.ClientList select AdeClientSource.FromArcadeClient(client);
							lbx_Main.IsEnabled = true;
							msp_Main.IsEnabled = true;
							lbl_status.Content = $"搜索完成, 共找到 {ClientHelper.ClientList.Count} 个Arcade发行版。";
							AddTrayIcon();
						});
						StartWatchers();
					})
					{ IsBackground = true }.Start();
				}
			}
		}

		private void lbx_Main_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			var selectedGrid = (lbx_Main.SelectedValue as AdeClientSource?)!.Value;
			string selectedClientName = selectedGrid.ClientName;
			var selectedClient = (from client in ClientHelper.ClientList where client.ClientName == selectedClientName select client).First();
			lbl_status.Content = $"启动 {selectedClientName}...";
			Process.Start(new ProcessStartInfo()
			{
				FileName = selectedClient.ClientPath,
				UseShellExecute = false,
				WindowStyle = ProcessWindowStyle.Normal,
				WorkingDirectory = new FileInfo(selectedClient.ClientPath).DirectoryName
			});
			WindowState = WindowState.Minimized;
			lbl_status.Content = "就绪";
		}

		private void tsmi_manage_reSearchClients_Click(object sender, RoutedEventArgs e)
		{
			if (ClientHelper.ClientList.Any())
			{
				var r = MessageBox.Show("重新搜索操作会清空当前已保存的Arcade发行版列表并以搜索结果作为新的列表保存。\n" +
					"此操作不可撤销!\n" +
					"您确定要继续执行操作吗?", "注意", MessageBoxButton.YesNo, MessageBoxImage.Warning);
				if (r == MessageBoxResult.Yes)
				{
					DeleteTrayIcon();
					ClientHelper.ClientList.Clear();
					lbx_Main.ItemsSource = null;
					lbl_status.Content = "正在搜索已安装的Arcade发行版……";
					lbx_Main.IsEnabled = false;
					msp_Main.IsEnabled = false;
					new Thread(async () =>
					{
						var r = await ClientHelper.SearchClientsAsync();
						ClientHelper.ClientList = r;
						ClientHelper.SaveClientList();
						Dispatcher.Invoke(() =>
						{
							lbx_Main.ItemsSource = from client in ClientHelper.ClientList select AdeClientSource.FromArcadeClient(client);
							lbx_Main.IsEnabled = true;
							msp_Main.IsEnabled = true;
							lbl_status.Content = $"搜索完成, 共找到 {ClientHelper.ClientList.Count} 个Arcade发行版。";
							AddTrayIcon();
						});
					})
					{ IsBackground = true }.Start();
				}
			}
			else
			{
				DeleteTrayIcon();
				lbl_status.Content = "正在搜索已安装的Arcade发行版……";
				lbx_Main.IsEnabled = false;
				msp_Main.IsEnabled = false;
				new Thread(async () =>
				{
					var r = await ClientHelper.SearchClientsAsync();
					ClientHelper.ClientList = r;
					ClientHelper.SaveClientList();
					Dispatcher.Invoke(() =>
					{
						lbx_Main.ItemsSource = from client in ClientHelper.ClientList select AdeClientSource.FromArcadeClient(client);
						lbx_Main.IsEnabled = true;
						msp_Main.IsEnabled = true;
						lbl_status.Content = $"搜索完成, 共找到 {ClientHelper.ClientList.Count} 个Arcade发行版。";
						AddTrayIcon();
					});
				})
				{ IsBackground = true }.Start();
			}
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (!isClosing)
			{
				e.Cancel = true;
				Hide();
				trayIcon.ShowBalloonTip(0, "提示", "Project Arcade Manager已最小化到任务栏, 双击图标显示主窗口", System.Windows.Forms.ToolTipIcon.Info);
			}
		}

		private void StartWatchers()
		{
			Task.Run(() =>
			{
				foreach (var client in ClientHelper.ClientList)
				{
					if (client.ClientBackgroundPath != null)
					{
						var watcher = new FileSystemWatcher(client.ClientBackgroundPath!);
						watcher.Created += Watcher_Created;
						watcher.Renamed += Watcher_Renamed;
						watcher.Deleted += Watcher_Deleted;
						watcher.EnableRaisingEvents = true;
						FolderWatchHelper.ClientBgWatchers.Add(watcher);
					}
				}
				Task.Delay(-1);
			});
		}

		private void StopWatchers()
		{
			var rawWatcherList = FolderWatchHelper.ClientBgWatchers.ToList();
			foreach (var watcher in rawWatcherList)
			{
				watcher.EnableRaisingEvents = false;
				watcher.Dispose();
				FolderWatchHelper.ClientBgWatchers.Remove(watcher);
			}
		}

		private void Watcher_Created(object sender, FileSystemEventArgs e)
		{
			if (File.Exists(e.FullPath)) // 文件被创建
			{
				var affectedClientNames = new List<string>();
				foreach (var watcher in FolderWatchHelper.ClientBgWatchers)
				{
					if (watcher.Path != new FileInfo(e.FullPath).DirectoryName)
					{
						if (!FolderWatchHelper.BgSyncingFolderPathes.Contains(watcher.Path))
						{
							watcher.EnableRaisingEvents = false;
							File.Copy(e.FullPath, Path.Combine(watcher.Path, e.Name));
							watcher.EnableRaisingEvents = true;
							affectedClientNames.AddRange(from client in ClientHelper.ClientList
														 where client.ClientBackgroundPath == watcher.Path
														 select client.ClientName);
						}
					}
				}
				string eventClientName = (from client in ClientHelper.ClientList
										  where client.ClientBackgroundPath == new FileInfo(e.FullPath).DirectoryName
										  select client.ClientName).First();
				var sb = new StringBuilder();
				foreach (string affectedClientName in affectedClientNames)
				{
					sb.Append(affectedClientName);
					if (affectedClientName != affectedClientNames.Last()) { sb.Append(", "); }
				}
				FolderWatchHelper.WatchLogs.Add(DateTime.Now, new Tuple<string, string, string, string, string>(eventClientName, "游玩背景文件夹", "创建", e.Name,
					$"同时同步了这些Arcade发行版的数据: {sb}"));
				Dispatcher.Invoke(() =>
				{
					lbl_status.Content = $"文件 {e.Name} 已被创建, 系统已同步了其它 {FolderWatchHelper.ClientBgWatchers.Count - 1} 个Arcade发行版的数据文件夹。";
				});
			}
		}

		private void Watcher_Deleted(object sender, FileSystemEventArgs e)
		{
			if (!File.Exists(e.FullPath) && !Directory.Exists(e.FullPath)) // 文件被删除
			{
				var affectedClientNames = new List<string>();
				foreach (var watcher in FolderWatchHelper.ClientBgWatchers)
				{
					if (watcher.Path != new FileInfo(e.FullPath).DirectoryName)
					{
						watcher.EnableRaisingEvents = false;
						File.Delete(Path.Combine(watcher.Path, e.Name));
						watcher.EnableRaisingEvents = true;
						affectedClientNames.AddRange(from client in ClientHelper.ClientList
																						 where client.ClientBackgroundPath == watcher.Path
																						 select client.ClientName);
					}
				}
				string eventClientName = (from client in ClientHelper.ClientList
										  where client.ClientBackgroundPath == new FileInfo(e.FullPath).DirectoryName
										  select client.ClientName).First();
				var sb = new StringBuilder();
				foreach (string affectedClientName in affectedClientNames)
				{
					sb.Append(affectedClientName);
					if (affectedClientName != affectedClientNames.Last()) { sb.Append(", "); }
				}
				FolderWatchHelper.WatchLogs.Add(DateTime.Now, new Tuple<string, string, string, string, string>(eventClientName, "游玩背景文件夹", "删除", e.Name,
					$"同时同步了这些Arcade发行版的数据: {sb}"));
				Dispatcher.Invoke(() =>
				{
					lbl_status.Content = $"文件 {e.Name} 已被删除, 系统已同步了其它 {FolderWatchHelper.ClientBgWatchers.Count - 1} 个Arcade发行版的数据文件夹。";
				});
			}
		}

		private void Watcher_Renamed(object sender, RenamedEventArgs e)
		{
			if (File.Exists(e.FullPath)) // 文件名发生了变化
			{
				var affectedClientNames = new List<string>();
				foreach (var watcher in FolderWatchHelper.ClientBgWatchers)
				{
					if (watcher.Path != new FileInfo(e.FullPath).DirectoryName)
					{
						watcher.EnableRaisingEvents = false;
						if (File.Exists(Path.Combine(watcher.Path, e.OldName)))
						{
							File.Move(Path.Combine(watcher.Path, e.OldName), Path.Combine(watcher.Path, e.Name));
						}
						else
						{
							File.Copy(e.FullPath, Path.Combine(watcher.Path, e.Name));
						}
						watcher.EnableRaisingEvents = true; ;
						affectedClientNames.AddRange(from client in ClientHelper.ClientList
													 where client.ClientBackgroundPath == watcher.Path
													 select client.ClientName);
					}
				}
				string eventClientName = (from client in ClientHelper.ClientList
										  where client.ClientBackgroundPath == new FileInfo(e.FullPath).DirectoryName
										  select client.ClientName).First();
				var sb = new StringBuilder();
				foreach (string affectedClientName in affectedClientNames)
				{
					sb.Append(affectedClientName);
					if (affectedClientName != affectedClientNames.Last()) { sb.Append(", "); }
				}
				FolderWatchHelper.WatchLogs.Add(DateTime.Now, new Tuple<string, string, string, string, string>(eventClientName, "游玩背景文件夹", "重命名", e.Name,
					$"原文件名:{e.OldName}; 同时同步了这些Arcade发行版的数据: {sb}"));
				Dispatcher.Invoke(() =>
				{
					lbl_status.Content = $"文件 {e.OldName} 已被重命名为 {e.Name}, 系统已同步了其它 {FolderWatchHelper.ClientBgWatchers.Count - 1} 个Arcade发行版的数据文件夹。";
				});
			}
		}

		private void tsmi_file_showWatcherLog_Click(object sender, RoutedEventArgs e)
		{
			var watcherLog = new Win_WatcherLog();
			watcherLog.Show();
		}

		private void tsmi_help_about_Click(object sender, RoutedEventArgs e)
		{
			var about = new Win_About();
			about.Show();
		}
	}
}