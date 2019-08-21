using System;
using System.Linq;
using Newtonsoft.Json;

namespace Crawler.Client.AzureDevOps
{
	internal class GetVariableGroupsResponse
	{
		[JsonProperty("value")]
		public VariableGroup[] Groups { get; set; }
	}
}