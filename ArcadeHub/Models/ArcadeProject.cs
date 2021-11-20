using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcadeHub.Models
{
	public struct ArcadeProject
	{
		[JsonProperty("projName", Required = Required.Always)]
		public string ProjectName { get; set; }

		[JsonProperty("projFilePath", Required = Required.Always)]
		public string ProjFilePath { get; set; }

		[JsonProperty("projPath", Required = Required.Always)]
		public string ProjectPath { get; set; }

		[JsonIgnore]
		public DateTime LastModifyTime { get => File.Exists(ProjFilePath) ? File.GetLastWriteTime(ProjFilePath) : DateTime.Now; }
	}
}
