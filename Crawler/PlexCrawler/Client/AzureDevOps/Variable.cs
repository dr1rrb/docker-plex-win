using System;
using System.Linq;
using Newtonsoft.Json;

namespace Crawler.Client.AzureDevOps
{
	public class Variable
	{
		[JsonProperty("value")]
		public string Value { get; set; }
	}
}