using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace GrapeCity.ActiveReports.Samples.JsonDataSource
{
	// Provides the data used in the sample.
	internal sealed class DataLayer
	{
		public static string CreateData()
		{
			const string sourceUrl = @"http://localhost:30187/customers/GetJson";

			using (var httpClient = new HttpClient())
			{
				httpClient.DefaultRequestHeaders.Authorization 
					= AuthenticationHeaderValue.Parse("Basic " + Convert.ToBase64String(Encoding.Default.GetBytes("admin:1")));
				httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				var request = new HttpRequestMessage(HttpMethod.Get, sourceUrl);
				var response = httpClient.SendAsync(request).Result;
				var json = response.Content.ReadAsStringAsync().Result;
				return json;
			}
		}
	}
}
