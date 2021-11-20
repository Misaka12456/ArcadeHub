using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcadeHub.DataSourceModels
{

	/// <summary>
	/// 表示一条文件监测事件日志信息的数据源。
	/// </summary>
	public struct WatchEventLogSource
	{
		/// <summary>
		/// 事件发生的日期字符串。
		/// </summary>
		public string LogTime { get; set; }
		
		/// <summary>
		/// 事件来源的Arcade发行版名称。
		/// </summary>
		public string ClientName { get; set; }

		/// <summary>
		/// 事件来源的Arcade文件夹类型。
		/// </summary>
		public string WatchingFolderType { get; set; }
		
		/// <summary>
		/// 事件类型。
		/// </summary>
		public string FileEventType { get; set; }
		
		/// <summary>
		/// 事件对应的文件名。
		/// </summary>
		public string FileName { get; set; }

		/// <summary>
		/// 事件的备注。
		/// </summary>
		public string Comments { get; set; }
	}
}
