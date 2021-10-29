#pragma warning disable IDE0044
using ArcadeManager.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcadeManager.Core
{
	public static class ClientHelper
	{
		/// <summary>
		/// Arcade客户端的列表。
		/// </summary>
		public static List<ArcadeClient> ClientList { get; set; }

		private static List<FileInfo> tempData;
		static ClientHelper()
		{
			ClientList = new List<ArcadeClient>();
			tempData = new List<FileInfo>();
		}

		/// <summary>
		/// 搜索本地计算机上所有可能的Arcade发行版客户端。
		/// </summary>
		public static async Task<List<ArcadeClient>> SearchClientsAsync()
		{
			return await Task.Run(new Func<List<ArcadeClient>>(() =>
			{
				string[] drives = Directory.GetLogicalDrives();
				foreach (string drive in drives)
				{
					tempData.AddRange(from file in new MFTScanner().EnumerateFiles(drive) where 
									  (new FileInfo(file).Name.StartsWith("Arcade") && new FileInfo(file).Name.ToLower().EndsWith(".exe")
									  && Directory.Exists(Path.Combine(new FileInfo(file).Directory.FullName,$"{new FileInfo(file).Name.Replace(".exe",string.Empty)}_Data"))) select new FileInfo(file));
				}
				var r = new List<ArcadeClient>();
				foreach (var tempClient in tempData)
				{
					switch (tempClient.Name.Split(new[] { '.' }, count: 2)[0].ToLower())
					{
						case "arcade":
							switch (tempClient.DirectoryName.ToLower())
							{
								case "arcade-zero":
									r.Add(new ArcadeClient()
									{
										ClientName = "Arcade-Zero",
										ClientPath = tempClient.FullName,
										ClientBackgroundPath = Path.Combine(tempClient.Directory.FullName, "自定义背景(User Backgrounds)"),
										ClientSkinPath = null,
										Developer = "Tempestissiman"
									});
									break;
								case "arcade-one":
									r.Add(new ArcadeClient()
									{
										ClientName = "Arcade-One",
										ClientPath = tempClient.FullName,
										ClientBackgroundPath = Path.Combine(tempClient.Directory.FullName, "自定义背景(User Backgrounds)"),
										ClientSkinPath = null,
										Developer = "yyyr"
									});
									break;
								default:
									r.Add(new ArcadeClient()
									{
										ClientName = "Arcade",
										ClientPath = tempClient.FullName,
										ClientBackgroundPath = Path.Combine(tempClient.Directory.FullName, "自定义背景(User Backgrounds)"),
										ClientSkinPath = null,
										Developer = "Schwarzer"
									});
									break;
							}
							break;
						case "arcade-plus":
							r.Add(new ArcadeClient()
							{
								ClientName = "Arcade-Plus",
								ClientPath = tempClient.FullName,
								ClientBackgroundPath = Path.Combine(tempClient.Directory.FullName, "Background"),
								ClientSkinPath = Path.Combine(tempClient.Directory.FullName, "Skin"),
								Developer = "Benpigchu"
							});
							break;
						default:
							r.Add(new ArcadeClient()
							{
								ClientName = tempClient.Name.Split(new[] { '.' }, count: 2)[0],
								ClientPath = tempClient.FullName,
								ClientBackgroundPath = null,
								ClientSkinPath = null,
								Developer = "Unknown"
							});
							break;
					}
				}
				return r;
			}));
		}

		/// <summary>
		/// 保存Arcade客户端的列表信息。
		/// </summary>
		public static void SaveClientList()
		{
			var r = JArray.FromObject(ClientList);
			File.WriteAllText(Path.Combine(AppContext.BaseDirectory, "config.dat"), Convert.ToBase64String(Encoding.UTF8.GetBytes(r.ToString())), Encoding.ASCII);
		}

		public static void LoadClientList()
		{
			string r = Encoding.UTF8.GetString(Convert.FromBase64String(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "config.dat"), Encoding.ASCII)));
			ClientList = JArray.Parse(r).ToObject<List<ArcadeClient>>();
		}
	}
}
