#pragma warning disable IDE0058
using ArcadeHub.Core;
using ArcadeHub.DataSourceModels;
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
using FolderBrowserDialog = System.Windows.Forms.FolderBrowserDialog;
using DialogResult2 = System.Windows.Forms.DialogResult;
using NotifyIcon = System.Windows.Forms.NotifyIcon;
using Path = System.IO.Path;
using ToolStripItem = System.Windows.Forms.ToolStripItem;
using ToolStripMenuItem = System.Windows.Forms.ToolStripMenuItem;
using ToolStripSeparator = System.Windows.Forms.ToolStripSeparator;
using ResMgr = ArcadeHub.Properties.Resources;
using System.Drawing;
using System.Runtime.Remoting.Channels;
using ArcadeHub.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Runtime.InteropServices;

namespace ArcadeHub.Forms
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
					Text = "Project Arcade Hub"
				};
				trayIcon.Visible = true;
				var csp = new ContextMenuStrip();
				var tsmi_ntfi_projects = new ToolStripMenuItem()
				{
					Text = "选择Arcade项目(&P)..."
				};
				foreach (var project in ProjectHelper.ProjectList)
				{
					var tsmi_ntfi_project = new ToolStripMenuItem()
					{
						Text = project.ProjectName
					};
					tsmi_ntfi_project.Click += (sender, e) =>
					{
						var arcadeChanList = ClientHelper.ClientList.FindAll((ArcadeClient client) => { return client.ClientName == "Arcade-Chan"; });
						var arcadeOneList = ClientHelper.ClientList.FindAll((ArcadeClient client) => { return client.ClientName == "Arcade-One"; });
						if (arcadeChanList.Count > 1)
						{
							MessageBox.Show("诶？发行版列表当中怎么出现了好几个Arcade-Chan？(っ °Д °;)っ\n快把重复的卸载掉后重新查找所有Arcade发行版然后再试试……?", "Oops!", MessageBoxButton.OK, MessageBoxImage.Error);
						}
						else if (arcadeOneList.Count > 1)
						{
							MessageBox.Show("诶？发行版列表当中怎么出现了好几个Arcade-One？(っ °Д °;)っ\n快把重复的卸载掉后重新查找所有Arcade发行版然后再试试……?", "Oops!", MessageBoxButton.OK, MessageBoxImage.Error);
						}
						else if (!arcadeChanList.Any() && !arcadeOneList.Any())
						{
							MessageBox.Show("您似乎还未安装Arcade-Chan或Arcade-One发行版的任何版本呢……\nArcade项目管理仅适用于Arcade-Chan发行版\n" +
								"或版本号高于或等于Build 45的Arcade-One发行版哦\n安装一个Arcade-Chan发行版(版本v0.1.0及以上)或Arcade-One发行版(版本号Build 45及以上)\n后重新查找所有Arcade发行版然后再来试试吧", "Oops!",
								MessageBoxButton.OK, MessageBoxImage.Error);
						}
						else
						{
							if (arcadeChanList.Any())
							{
								var chanClient = arcadeChanList.First();
								string chanClientPath = chanClient.ClientPath;
								if (File.Exists(project.ProjFilePath))
								{
									lbl_status.Content = $"启动 {chanClient.ClientName}...";
									Process.Start(new ProcessStartInfo()
									{
										FileName = chanClientPath,
										UseShellExecute = false,
										WindowStyle = ProcessWindowStyle.Normal,
										Arguments = '"' + project.ProjFilePath + '"',
										WorkingDirectory = project.ProjectPath
									});
									WindowState = WindowState.Minimized;
									lbl_status.Content = "就绪";
								}
								else
								{
									MessageBox.Show("这个项目的配置文件似乎不见了呢...\n" +
										"检查一下项目是不是已经挪动位置了qwq\n" +
										"查找的配置文件路径:\n" + project.ProjFilePath, "Oops!", MessageBoxButton.OK, MessageBoxImage.Error);
								}
							}
							else if (arcadeOneList.Any())
							{
								var oneClient = arcadeChanList.First();
								string oneClientPath = oneClient.ClientPath;
								if (File.Exists(project.ProjFilePath))
								{
									lbl_status.Content = $"启动 {oneClient.ClientName}...";
									Process.Start(new ProcessStartInfo()
									{
										FileName = oneClientPath,
										UseShellExecute = false,
										WindowStyle = ProcessWindowStyle.Normal,
										Arguments = '"' + project.ProjFilePath + '"',
										WorkingDirectory = project.ProjectPath
									});
									WindowState = WindowState.Minimized;
									lbl_status.Content = "就绪";
								}
								else
								{
									MessageBox.Show("这个项目的配置文件似乎不见了呢...\n" +
										"检查一下项目是不是已经挪动位置了qwq\n" +
										"查找的配置文件路径:\n" + project.ProjFilePath, "Oops!", MessageBoxButton.OK, MessageBoxImage.Error);
								}
							}
						}
					};
					tsmi_ntfi_projects.DropDownItems.Add(tsmi_ntfi_project);
				}
				csp.Items.Add(tsmi_ntfi_projects);

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
				lbx_clients.ItemsSource = from client in ClientHelper.ClientList select AdeClientSource.FromArcadeClient(client);
				if (HandleArcadeChanOrOneInstallStatus() && File.Exists(Path.Combine(AppContext.BaseDirectory, "projects.dat")))
				{
					ProjectHelper.LoadProjectList();
					lbx_projects.ItemsSource = from project in ProjectHelper.ProjectList select AdeProjectSource.FromArcadeProject(project);
				}
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
					lbx_clients.IsEnabled = false;
					msp_Main.IsEnabled = false;
					new Thread(async () =>
					{
						var r = await ClientHelper.SearchClientsAsync();
						ClientHelper.ClientList = r;
						ClientHelper.SaveClientList();
						Dispatcher.Invoke(() =>
						{
							lbx_clients.ItemsSource = from client in ClientHelper.ClientList select AdeClientSource.FromArcadeClient(client);
							lbx_clients.IsEnabled = true;
							msp_Main.IsEnabled = true;
							lbl_status.Content = $"搜索完成, 共找到 {ClientHelper.ClientList.Count} 个Arcade发行版。";
							AddTrayIcon();
							HandleArcadeChanOrOneInstallStatus();
						});
						StartWatchers();
					})
					{ IsBackground = true }.Start();
				}
			}
		}

		private bool HandleArcadeChanOrOneInstallStatus()
		{
			if (ClientHelper.ClientList.Exists((ArcadeClient client) => { return client.ClientName is "Arcade-Chan" or "Arcade-One"; }))
			{
				lbl_chanNotInstalled.Visibility = Visibility.Collapsed;
				lbx_projects.Visibility = Visibility.Visible;
				tsmi_projects.IsEnabled = true;
				return true;
			}
			else
			{
				lbl_chanNotInstalled.Visibility = Visibility.Visible;
				lbx_projects.Visibility = Visibility.Hidden;
				tsmi_projects.IsEnabled = false;
				return false;
			}
		}

		private void lbx_clients_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (lbx_clients.SelectedValue != null)
			{
				var selectedGrid = (lbx_clients.SelectedValue as AdeClientSource?).Value;
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
					StopWatchers();
					DeleteTrayIcon();
					ClientHelper.ClientList.Clear();
					lbx_clients.ItemsSource = null;
					lbl_status.Content = "正在搜索已安装的Arcade发行版……";
					lbx_clients.IsEnabled = false;
					msp_Main.IsEnabled = false;
					new Thread(async () =>
					{
						var r = await ClientHelper.SearchClientsAsync();
						ClientHelper.ClientList = r;
						ClientHelper.SaveClientList();
						Dispatcher.Invoke(() =>
						{
							lbx_clients.ItemsSource = from client in ClientHelper.ClientList select AdeClientSource.FromArcadeClient(client);
							lbx_clients.IsEnabled = true;
							msp_Main.IsEnabled = true;
							lbl_status.Content = $"搜索完成, 共找到 {ClientHelper.ClientList.Count} 个Arcade发行版。";
							AddTrayIcon();
							HandleArcadeChanOrOneInstallStatus();
						});
						StartWatchers();
					})
					{ IsBackground = true }.Start();
				}
			}
			else
			{
				StopWatchers();
				DeleteTrayIcon();
				lbl_status.Content = "正在搜索已安装的Arcade发行版……";
				lbx_clients.IsEnabled = false;
				msp_Main.IsEnabled = false;
				new Thread(async () =>
				{
					var r = await ClientHelper.SearchClientsAsync();
					ClientHelper.ClientList = r;
					ClientHelper.SaveClientList();
					Dispatcher.Invoke(() =>
					{
						lbx_clients.ItemsSource = from client in ClientHelper.ClientList select AdeClientSource.FromArcadeClient(client);
						lbx_clients.IsEnabled = true;
						msp_Main.IsEnabled = true;
						lbl_status.Content = $"搜索完成, 共找到 {ClientHelper.ClientList.Count} 个Arcade发行版。";
						AddTrayIcon();
						HandleArcadeChanOrOneInstallStatus();
					});
					StartWatchers();
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
				trayIcon.ShowBalloonTip(0, "提示", "Project Arcade Hub已最小化到任务栏, 双击图标显示主窗口", System.Windows.Forms.ToolTipIcon.Info);
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

		private void lbx_projects_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (lbx_projects.SelectedValue != null)
			{
				var arcadeChanList = ClientHelper.ClientList.FindAll((ArcadeClient client) => { return client.ClientName == "Arcade-Chan"; });
				var arcadeOneList = ClientHelper.ClientList.FindAll((ArcadeClient client) => { return client.ClientName == "Arcade-One"; });
				if (arcadeChanList.Count > 1)
				{
					MessageBox.Show("诶？发行版列表当中怎么出现了好几个Arcade-Chan？(っ °Д °;)っ\n快把重复的卸载掉后重新查找所有Arcade发行版然后再试试……?", "Oops!", MessageBoxButton.OK, MessageBoxImage.Error);
				}
				else if (arcadeOneList.Count > 1)
				{
					MessageBox.Show("诶？发行版列表当中怎么出现了好几个Arcade-One？(っ °Д °;)っ\n快把重复的卸载掉后重新查找所有Arcade发行版然后再试试……?", "Oops!", MessageBoxButton.OK, MessageBoxImage.Error);
				}
				else if (!arcadeChanList.Any() && !arcadeOneList.Any())
				{
					MessageBox.Show("您似乎还未安装Arcade-Chan或Arcade-One发行版的任何版本呢……\nArcade项目管理仅适用于Arcade-Chan发行版\n" +
						"或版本号高于或等于Build 45的Arcade-One发行版哦\n安装一个Arcade-Chan发行版(版本v0.1.0及以上)或Arcade-One发行版(版本号Build 45及以上)\n后重新查找所有Arcade发行版然后再来试试吧", "Oops!",
						MessageBoxButton.OK, MessageBoxImage.Error);
				}
				else
				{
					if (arcadeChanList.Any())
					{
						var chanClient = arcadeChanList.First();
						string chanClientPath = chanClient.ClientPath;
						var selectedProj = (lbx_projects.SelectedItem as AdeProjectSource?)!.Value;
						if (File.Exists(selectedProj.ProjFilePath))
						{
							lbl_status.Content = $"启动 {chanClient.ClientName}...";
							Process.Start(new ProcessStartInfo()
							{
								FileName = chanClientPath,
								UseShellExecute = false,
								WindowStyle = ProcessWindowStyle.Normal,
								Arguments = '"' + selectedProj.ProjFilePath + '"',
								WorkingDirectory = selectedProj.ProjectPath
							});
							WindowState = WindowState.Minimized;
							lbl_status.Content = "就绪";
						}
						else
						{
							MessageBox.Show("这个项目的配置文件似乎不见了呢...\n" +
								"检查一下项目是不是已经挪动位置了qwq\n" +
								"查找的配置文件路径:\n" + selectedProj.ProjFilePath, "Oops!", MessageBoxButton.OK, MessageBoxImage.Error);
						}
					}
					else if (arcadeOneList.Any())
					{
						var oneClient = arcadeOneList.First();
						string oneClientPath = oneClient.ClientPath;
						var selectedProj = (lbx_projects.SelectedItem as AdeProjectSource?)!.Value;
						if (File.Exists(selectedProj.ProjFilePath))
						{
							lbl_status.Content = $"启动 {oneClient.ClientName}...";
							Process.Start(new ProcessStartInfo()
							{
								FileName = oneClientPath,
								UseShellExecute = false,
								WindowStyle = ProcessWindowStyle.Normal,
								Arguments = '"' + selectedProj.ProjFilePath + '"',
								WorkingDirectory = selectedProj.ProjectPath
							});
							WindowState = WindowState.Minimized;
							lbl_status.Content = "就绪";
						}
						else
						{
							MessageBox.Show("这个项目的配置文件似乎不见了呢...\n" +
								"检查一下项目是不是已经挪动位置了qwq\n" +
								"查找的配置文件路径:\n" + selectedProj.ProjFilePath, "Oops!", MessageBoxButton.OK, MessageBoxImage.Error);
						}
					}
				}
			}
		}

		private void tsmi_projects_add_Click(object sender, RoutedEventArgs e)
		{
			var odd = new FolderBrowserDialog()
			{
				Description = "选择包含base.ogg/base.mp3/base.wav的Arcade项目文件夹"
			};
			if (odd.ShowDialog() == DialogResult2.OK && Directory.Exists(odd.SelectedPath))
			{
				try
				{
					if (File.Exists(Path.Combine(odd.SelectedPath, "Arcade", "Project.arcade")))
					{
						var projData = JObject.Parse(File.ReadAllText(Path.Combine(odd.SelectedPath, "Arcade", "Project.arcade"), Encoding.UTF8));
						string projName = projData.Value<string>("Title")!;
						if (ProjectHelper.ProjectList.Exists((ArcadeProject project) => { return project.ProjectName == projName; }))
						{
							MessageBox.Show("该项目已在列表当中哦", "Oops!", MessageBoxButton.OK, MessageBoxImage.Error);
						}
						else
						{
							DeleteTrayIcon();
							var addProj = new ArcadeProject()
							{
								ProjectName = projName,
								ProjectPath = odd.SelectedPath,
								ProjFilePath = Path.Combine(odd.SelectedPath, "Arcade", "Project.arcade")
							};
							lbx_projects.ItemsSource = null;
							ProjectHelper.ProjectList = ProjectHelper.ProjectList.Prepend(addProj).ToList();
							ProjectHelper.SaveProjectList();
							lbx_projects.ItemsSource = from project in ProjectHelper.ProjectList select AdeProjectSource.FromArcadeProject(project);
							AddTrayIcon();
						}
					}
				}
				catch(Exception ex)
				{
					MessageBox.Show("读取项目元文件时发生问题......\n要不用Arcade-Chan重新生成一个项目元文件...?\n" +
						$"异常: {ex.GetType()}", "Oops!", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}

		private void tsmi_projects_remove_Click(object sender, RoutedEventArgs e)
		{
			if (lbx_projects.SelectedItem != null)
			{
				bool isDeletePermanently = GetAsyncKeyState(KEY_SHIFT) != 0;
				var selectedProj = (lbx_projects.SelectedItem as AdeProjectSource?)!.Value;
				bool isConfirmedRemove = false;
				try
				{
					if (isDeletePermanently) // 调用函数时(单击时)同时按下了Shift键
					{
						var r = MessageBox.Show("警告: 此操作不可撤销!\n" +
							$"确实要从磁盘上删除Arcade项目【{selectedProj.ProjectName}】吗?", "Arcade项目删除确认", MessageBoxButton.YesNo, MessageBoxImage.Warning);
						if (r == MessageBoxResult.Yes)
						{
							DeleteTrayIcon();
							isConfirmedRemove = true;
							lbx_projects.ItemsSource = null;
							ProjectHelper.ProjectList.RemoveAll((ArcadeProject project) => { return project.ProjFilePath == selectedProj.ProjFilePath; });
							ProjectHelper.SaveProjectList();
							Directory.Delete(selectedProj.ProjectPath,true);
							lbx_projects.ItemsSource = from project in ProjectHelper.ProjectList select AdeProjectSource.FromArcadeProject(project);
						}
					}
					else
					{
						var r = MessageBox.Show("确定要移除Arcade项目【{selectedProj.ProjectName}】吗?\n" +
							"此操作不会删除项目文件夹。", "Arcade项目移除确认", MessageBoxButton.YesNo, MessageBoxImage.Question);
						if (r == MessageBoxResult.Yes)
						{
							DeleteTrayIcon();
							isConfirmedRemove = true;
							lbx_projects.ItemsSource = null;
							ProjectHelper.ProjectList.RemoveAll((ArcadeProject project) => { return project.ProjFilePath == selectedProj.ProjFilePath; });
							ProjectHelper.SaveProjectList();
							lbx_projects.ItemsSource = from project in ProjectHelper.ProjectList select AdeProjectSource.FromArcadeProject(project);
						}
					}
				}
				catch(DirectoryNotFoundException)
				{
					MessageBox.Show($"{(isDeletePermanently ? "删除" : "移除")}Arcade项目【{selectedProj.ProjFilePath}】失败: 项目文件夹不存在", "Oops!", MessageBoxButton.OK, MessageBoxImage.Error);
				}
				catch(Exception ex)
				{
					MessageBox.Show($"{(isDeletePermanently ? "删除" : "移除")}Arcade项目【{selectedProj.ProjFilePath}】失败: 未知的错误\n" +
						$"({ex.GetType()})", "Oops!", MessageBoxButton.OK, MessageBoxImage.Error);
				}
				finally
				{
					if (isConfirmedRemove)
					{
						AddTrayIcon();
					}
				}
			}
		}

		[DllImport("User32")]
		private static extern short GetAsyncKeyState(int vKey);

		private const int KEY_SHIFT = 0x10;

		private void lbx_projects_LostFocus(object sender, RoutedEventArgs e)
		{
			tsmi_projects_remove.IsEnabled = false;
		}

		private void lbx_projects_SelectionChanged(object sender, RoutedEventArgs e)
		{
			tsmi_projects_remove.IsEnabled = true;
		}
	}
}