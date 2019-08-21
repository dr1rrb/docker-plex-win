using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Crawler.Client.VideoLan
{
	public class VideoLanApi : IDisposable
	{
		private readonly Regex _version = new Regex(@"[0-9]+\.[0-9]+\.[0-9]+(\.[0-9]+)?");
		private readonly HttpClient _client;

		public VideoLanApi()
		{
			_client = new HttpClient();
		}

		public async Task<(string version, string url)> GetLatestVersion(CancellationToken ct)
		{
			using (var response = await _client.GetAsync("https://plex.tv/api/downloads/1.json", ct))
			{
				var raw = await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
				var release = JsonConvert.DeserializeObject<ReleaseResponse>(raw).Computer["Windows"];

				return (release.Version, release.Releases.First().Url);
			}
		}

		/// <inheritdoc />
		public void Dispose()
			=> _client.Dispose();
	}

	public class ReleaseResponse
	{
		public Dictionary<string, Platform> Computer { get; set; }
	}


	public class Platform
	{
		public string Id { get; set; }

		public string Version { get; set; }

		public Release[] Releases { get; set; }
	}

	public class Release
	{
		public string Url { get; set; }
	}
}
