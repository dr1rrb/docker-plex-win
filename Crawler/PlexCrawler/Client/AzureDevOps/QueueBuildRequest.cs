using System;
using System.Linq;
using Newtonsoft.Json;

namespace Crawler.Client.AzureDevOps
{
	public class QueueBuildRequest
	{
		[JsonProperty("definition")]
		public BuildDefinition Definition { get; set; }

		[JsonProperty("parameters")]
		public string Parameters { get; set; }
	}
}