using ArcadeHub.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ArcadeHub.DataSourceModels
{
	/// <summary>
	/// 表示一个Arcade发行版客户端信息类型的数据源。
	/// </summary>
	public struct AdeClientSource
	{
		/// <summary>
		/// 数据源对应的Arcade发行版名称。
		/// </summary>
		public string ClientName { get; private set; }

		/// <summary>
		/// 数据源对应的Arcade发行版主程序文件的完整绝对路径。
		/// </summary>
		public string ClientPath { get; private set; }

		/// <summary>
		/// 数据源对应的Arcade发行版主程序的图标。
		/// </summary>
		public ImageSource ClientIconSource
		{
			get
			{
				using var icon = Icon.ExtractAssociatedIcon(ClientPath);
				return Imaging.CreateBitmapSourceFromHIcon(icon.Handle, new Int32Rect(0, 0, icon.Width, icon.Height), BitmapSizeOptions.FromEmptyOptions());
			}
		}

		/// <summary>
		/// 使用指定的Arcade发行版客户端信息 <see cref="ArcadeClient"/> 初始化 <see cref="AdeClientSource"/> 的新实例。
		/// </summary>
		/// <param name="client">要使用的Arcade发行版客户端信息 <see cref="ArcadeClient"/> 实例。</param>
		/// <returns>初始化后的 <see cref="AdeClientSource"/> 实例。</returns>
		public static AdeClientSource FromArcadeClient(ArcadeClient client)
		{
			return new AdeClientSource()
			{
				ClientName = client.ClientName,
				ClientPath = client.ClientPath
			};
		}
	}
}
