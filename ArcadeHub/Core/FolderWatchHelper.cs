using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ArcadeHub.Core
{
	public static class FolderWatchHelper
	{
		/// <summary>
		/// 对Arcade发行版"背景"文件夹的监测。
		/// </summary>
		public static List<FileSystemWatcher> ClientBgWatchers { get; set; }

		/// <summary>
		/// 正在同步数据的Arcade发行版的背景文件夹的完整绝对路径。
		/// </summary>
		public static List<string> BgSyncingFolderPathes { get; set; }

		/// <summary>
		/// 文件监测的日志数据。
		/// <para>
		/// Item1 = Arcade发行版名称<br />
		/// Item2 = 监测的文件夹类型<br />
		/// Item3 = 文件变动类型<br />
		/// Item4 = 对应文件名<br />
		/// Item5 = 备注</para>
		/// </summary>
		public static Dictionary<DateTime, Tuple<string, string, string, string, string>> WatchLogs { get; set; }

		static FolderWatchHelper()
		{
			ClientBgWatchers = new List<FileSystemWatcher>();
			BgSyncingFolderPathes = new List<string>();
			WatchLogs = new Dictionary<DateTime, Tuple<string, string, string, string, string>>();
		}
	}
}
