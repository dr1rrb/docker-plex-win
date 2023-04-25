using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Crawler.Client.AzureDevOps;
using Crawler.Client.HealthChecks;
using Crawler.Client.VideoLan;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Crawler
{
	public static class ReleaseCrawler
	{
		[FunctionName("ScheduledCrawl")]
		public static async Task RunScheduledCrawl(
			[TimerTrigger("0 0 3 * * *")] TimerInfo myTimer,
			ILogger log,
			CancellationToken ct)
		{
			log.LogInformation($"Launching crawler ({DateTime.Now})");
			try
			{
				await CrawlReleases(log, ct);
			}
			catch (Exception e)
			{
				log.LogError(e, "Failed to crawl releases.");
			}
		}

		[FunctionName("ImmediateCrawl")]
		public static async Task<IActionResult> RunImmediateCrawl(
			[HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "crawl")] HttpRequest req,
			ILogger log,
			CancellationToken ct)
		{
			try
			{
				return new OkObjectResult(await CrawlReleases(log, ct));
			}
			catch (Exception e)
			{
				log.LogError(e, "Failed to crawl releases.");

				throw;
			}
		}

		private static async Task<UpdateResult> CrawlReleases(ILogger log, CancellationToken ct)
		{
			var config = new ConfigurationBuilder()
				.AddJsonFile("host.json", optional: true, reloadOnChange: true)
				.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
				.AddEnvironmentVariables()
				.Build();

			var status = "starting";
			using (var hc = new HealthchecksApi(config, log, "PLEX_CRAWLER"))
			using (var videoLan = new VideoLanApi())
			using (var azure = new AzureDevOpsApi(config["AZURE_AUTH"]))
			{
				try
				{
					status = "reporting start to healthchecks";
					await hc.Start(ct);

					status = "searching latest available version";
					var (version, url) = await videoLan.GetLatestVersion(ct);

					status = "loading azure config";
					var variables = await azure.GetBuildVariables(ct);

					if (variables.TryUpdate("latest.version", version))
					{
						variables.TryUpdate("latest.url", url);

						status = "updating azure config";
						await azure.UpdateBuildVariables(variables, ct);

						status = "queuing new build";
						await azure.QueueBuild(ct);

						status = "reporting success to healthchecks";
						await hc.Report(ct);

						return UpdateResult.Succeeded();
					}
					else
					{
						status = "reporting success (not changed) to healthchecks";
						await hc.Report(ct);

						return UpdateResult.NotChanged();
					}
				}
				catch (Exception e)
				{
					await hc.Failed(ct); // cannot fail

					return UpdateResult.Failed(status, e);
				}

			}
		}
	}
}
