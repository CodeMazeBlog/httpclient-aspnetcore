using Entities.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CompanyEmployees.Client.Services
{
	public class HttpClientCancellationService : IHttpClientServiceImplementation
	{
		private static readonly HttpClient _httpClient = new HttpClient();
		private readonly JsonSerializerOptions _options;
		private readonly CancellationTokenSource _cancellationTokenSource;

		public HttpClientCancellationService()
		{
			_httpClient.BaseAddress = new Uri("https://localhost:5001/api/");
			_httpClient.Timeout = new TimeSpan(0, 0, 30);
			_httpClient.DefaultRequestHeaders.Clear();

			_options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

			_cancellationTokenSource = new CancellationTokenSource();
		}

		public async Task Execute()
		{
			_cancellationTokenSource.CancelAfter(2000);
			await GetCompaniesAndCancel(_cancellationTokenSource.Token);
		}

		private async Task GetCompaniesAndCancel(CancellationToken token)
		{
			try
			{
				using (var response = await _httpClient.GetAsync("companies",
				HttpCompletionOption.ResponseHeadersRead, token))
				{
					response.EnsureSuccessStatusCode();

					var stream = await response.Content.ReadAsStreamAsync();

					var companies = await JsonSerializer.DeserializeAsync<List<CompanyDto>>(stream, _options);
				}
			}
			catch (OperationCanceledException ocex)
			{
				Console.WriteLine(ocex.Message);
			}
		}
	}
}
