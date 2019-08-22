using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Crawler.Extensions;
using Newtonsoft.Json;

namespace Crawler.Client.AzureDevOps
{
	public class AzureDevOpsApi : IDisposable
	{
		private readonly HttpClient _client;

		public AzureDevOpsApi(string auth)
		{
			_client = new HttpClient
			{
				BaseAddress = new Uri("https://dev.azure.com/dr1rrb/docker-plex-win/_apis/"),
				DefaultRequestHeaders =
				{
					{ "Authorization", auth}
				}
			};
		}

		public async Task<VariableGroup> GetBuildVariables(CancellationToken ct)
		{
			using (var response = await _client.GetAsync("distributedtask/variablegroups?api-version=5.0-preview.1", ct))
			{
				var raw = await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
				var groups = JsonConvert.DeserializeObject<GetVariableGroupsResponse>(raw).Groups;

				return groups.Single(g => g.Name == "plex");
			}
		}

		public async Task UpdateBuildVariables(VariableGroup group, CancellationToken ct)
		{
			var body = new StringContent(JsonConvert.SerializeObject(group, Formatting.None), Encoding.UTF8, "application/json");

			using (var response = await _client.PutAsync($"distributedtask/variablegroups/{@group.Id}?api-version=5.0-preview.1", body, ct))
			{
				response.EnsureSuccessStatusCode();
			}
		}

		public async Task QueueBuild(CancellationToken ct)
		{
			var request = new QueueBuildRequest
			{
				Definition = new BuildDefinition
				{
					Id = 5
				},
			};
			var body = new StringContent(JsonConvert.SerializeObject(request, Formatting.None), Encoding.UTF8, "application/json");

			using (var response = await _client.PostAsync("build/builds?api-version=5.0", body, ct))
			{
				response.EnsureSuccessStatusCode();
			}
		}

		/// <inheritdoc />
		public void Dispose()
			=> _client.Dispose();

	}
}