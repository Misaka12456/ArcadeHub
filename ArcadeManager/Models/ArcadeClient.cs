#nullable enable
using Newtonsoft.Json;

namespace ArcadeManager.Models
{
	public struct ArcadeClient
	{
		[JsonProperty("name", Required = Required.Always)]
		public string ClientName { get; set; }

		[JsonProperty("clientPath", Required = Required.Always)]
		public string ClientPath { get; set; }

		[JsonProperty("clientBgPath", Required = Required.Always)]
		public string? ClientBackgroundPath { get; set; }

		[JsonProperty("clientSkinPath")]
		public string? ClientSkinPath { get; set; }

		[JsonProperty("developer")]
		public string? Developer { get; set; }
	}
}
