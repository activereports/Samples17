using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace GrapeCity.ActiveReports.Samples.CustomTileProviders
{
	internal static class WebRequestHelper
	{
		/// <summary>
		/// Load raw data into MemoryStream from specified Url.
		/// </summary>
		/// <param name="url">Data source Url</param>
		/// <param name="timeoutMilliseconds">Timeout in milliseconds. If -1 the default timeout will  used.</param>
		/// <param name="success">Success callback.</param>
		/// <param name="error">Error callback.</param>
		/// <param name="userAgent">User-Agent for request.</param>
		/// <returns></returns>
		public static void DownloadDataAsync(string url, int timeoutMilliseconds, Action<MemoryStream, string> success, Action<Exception> error, string userAgent = null)
		{
			var request = WebRequest.CreateHttp(url);

			if (!string.IsNullOrEmpty(userAgent))
				request.UserAgent = userAgent;

			if (timeoutMilliseconds > 0)
			{
				request.Timeout = timeoutMilliseconds;
			}

			request.BeginGetResponse(ar =>
			{
				try
				{
					var response = request.EndGetResponse(ar);

					//Copy data from buffer (It must be done, otherwise the buffer overflow and we stop to receive responses).
					var stream = new MemoryStream();
					var responseStream = response.GetResponseStream();
					if (responseStream != null)
					{
						responseStream.CopyTo(stream);
						success(stream, response.ContentType);
					}
					else
					{
						error(new NullReferenceException(nameof(responseStream)));
					}
				}
				catch (Exception exception)
				{
					error(exception);
				}
			}, null);

		}
	}
}
