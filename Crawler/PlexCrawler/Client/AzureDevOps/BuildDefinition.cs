using System;
using System.Linq;
using Newtonsoft.Json;

namespace Crawler.Client.AzureDevOps
{
	public class BuildDefinition
	{
		[JsonProperty("id")]
		public int Id { get; set; }
	}
}