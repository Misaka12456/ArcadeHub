using ArcadeHub.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcadeHub.Core
{
	public static class ProjectHelper
	{
		/// <summary>
		/// Arcade项目的列表。
		/// </summary>
		public static List<ArcadeProject> ProjectList { get; set; }

		static ProjectHelper()
		{
			ProjectList = new List<ArcadeProject>();
		}

		public static void LoadProjectList()
		{
			string r = Encoding.UTF8.GetString(Convert.FromBase64String(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "projects.dat"), Encoding.ASCII)));
			ProjectList = JArray.Parse(r).ToObject<List<ArcadeProject>>();
		}

		/// <summary>
		/// 保存Arcade项目的列表信息。
		/// </summary>
		public static void SaveProjectList()
		{
			var r = JArray.FromObject(ProjectList);
			File.WriteAllText(Path.Combine(AppContext.BaseDirectory, "projects.dat"), Convert.ToBase64String(Encoding.UTF8.GetBytes(r.ToString())), Encoding.ASCII);
		}
	}
}
