using ArcadeHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcadeHub.DataSourceModels
{
	/// <summary>
	/// 表示一个Arcade项目的数据源。
	/// </summary>
	public struct AdeProjectSource
	{
		/// <summary>
		/// 数据源对应的项目名(通常与曲名一致)。
		/// </summary>
		public string ProjectName { get; private set; }

		/// <summary>
		/// 数据源对应的项目文件(.arcade)的完整绝对路径。
		/// </summary>
		public string ProjFilePath { get; private set; }

		/// <summary>
		/// 数据源对应的项目文件夹的完整绝对路径。
		/// </summary>
		public string ProjectPath { get; private set; }

		/// <summary>
		/// 数据源对应的项目的最后修改日期。
		/// </summary>
		public string LastModifyTimeStr { get; private set; }

		/// <summary>
		/// 使用指定的Arcade项目信息 <see cref="ArcadeProject"/> 初始化 <see cref="AdeProjectSource"/> 的新实例。
		/// </summary>
		/// <param name="project">要使用的Arcade项目信息 <see cref="ArcadeProject"/> 实例。</param>
		/// <returns>初始化后的 <see cref="AdeProjectSource"/> 实例。</returns>
		public static AdeProjectSource FromArcadeProject(ArcadeProject project)
		{
			return new AdeProjectSource()
			{
				ProjectName = project.ProjectName,
				ProjFilePath = project.ProjFilePath,
				ProjectPath = project.ProjectPath,
				LastModifyTimeStr = "最后修改日期: " + project.LastModifyTime.ToString("yyyy-M-d H:mm:ss")
			};
		}
	}
}
