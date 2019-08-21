using System;
using System.Linq;

namespace Crawler
{
	public class UpdateResult
	{
		public static UpdateResult NotChanged() => new UpdateResult
		{
			Result = "not_changed",
			Message = $"Latest version already up-to-date."
		};

		public static UpdateResult Succeeded() => new UpdateResult
		{
			Result = "succeeded",
			Message = $"Successfully updated latest version and queued a new build."
		};

		public static UpdateResult Failed(string status, Exception error) => new UpdateResult
		{
			Result = "failed",
			Message = $"Failed to update latest version, an exception occurred while {status}: \r\n{error.Message}",
			Error = error
		};

		public string Result { get; set; }

		public string Message { get; set; }
			
		public Exception Error { get; set; }
	}
}