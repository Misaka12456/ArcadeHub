#nullable enable
using System;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Media.Animation;

namespace ArcadeManager.Forms
{
	/// <summary>
	/// Win_Start.xaml 的交互逻辑
	/// </summary>
	public partial class Win_Start : Window
	{
		public Win_Start()
		{
			InitializeComponent();
			lbl_ver.Content = string.Format("v{0}", Assembly.GetExecutingAssembly().GetName().Version.ToString(3));
			(FindResource("FadeIn") as Storyboard)!.Begin(this);
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			new Thread(() =>
			{
				Thread.Sleep(2500);
				Dispatcher.Invoke(new Action(() =>
				{
					(FindResource("FadeOut") as Storyboard)!.Begin(this);
					var main = new Win_Main();
					main.Show();
					Hide();
				}));
			})
			{ IsBackground = true }.Start();
		}
	}
}
