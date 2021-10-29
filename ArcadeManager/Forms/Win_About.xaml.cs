#nullable enable
#pragma warning disable IDE0058
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Media.Animation;

namespace ArcadeManager.Forms
{
	/// <summary>
	/// Win_Start.xaml 的交互逻辑
	/// </summary>
	public partial class Win_About : Window
	{
		public Win_About()
		{
			InitializeComponent();
			lbl_ver.Content = string.Format("v{0}", Assembly.GetExecutingAssembly().GetName().Version.ToString(3));
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{

		}

		private void TextBlock_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			Process.Start("https://github.com/Misaka12456/ArcadeManager");
		}

		private void Window_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			Close();
		}
	}
}
