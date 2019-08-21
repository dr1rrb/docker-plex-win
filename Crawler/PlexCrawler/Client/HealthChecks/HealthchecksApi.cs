using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Crawler.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Crawler.Client.HealthChecks
{
	public class HealthchecksApi : IDisposable
	{
		private readonly IConfigurationRoot _config;
		private readonly ILogger _log;
		private readonly string _defaultIdentifier;
		private readonly HttpClient _client;

		public HealthchecksApi(IConfigurationRoot config, ILogger log, string defaultIdentifier = null)
		{
			_config = config;
			_log = log;
			_defaultIdentifier = defaultIdentifier;
			_client = new HttpClient
			{
				BaseAddress = new Uri("https://hc-ping.com/")
			};
		}

		public Task Report(string identifier, CancellationToken ct) => ReportCore(identifier, string.Empty, ct);
		public Task Start(string identifier, CancellationToken ct) => ReportCore(identifier, "/start", ct);
		public Task Failed(string identifier, CancellationToken ct) => ReportCore(identifier, "/failed", ct);

		public Task Report(CancellationToken ct) => ReportCore(null, string.Empty, ct);
		public Task Start(CancellationToken ct) => ReportCore(null, "/start", ct);
		public Task Failed(CancellationToken ct) => ReportCore(null, "/failed", ct);

		private async Task ReportCore(string identifier, string method, CancellationToken ct)
		{
			try
			{
				identifier = identifier ?? _defaultIdentifier;
				if (identifier == null)
				{
					return;
				}

				var check = _config["hc-" + identifier];
				if (check.IsNullOrWhiteSpace())
				{
					return;
				}

				using (var response = await _client.GetAsync(check + method, ct))
				{
					response.EnsureSuccessStatusCode();
				}
			}
			catch (Exception e)
			{
				_log.LogError(e, "Failed to ping healthchecks.io");
			}
		}

		/// <inheritdoc />
		public void Dispose()
			=> _client.Dispose();
	}
}
