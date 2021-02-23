using Entities.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CompanyEmployees.Client.Services
{
	public class HttpClientStreamService : IHttpClientServiceImplementation
	{
		private static readonly HttpClient _httpClient = new HttpClient();
		private readonly JsonSerializerOptions _options;

		public HttpClientStreamService()
		{
			_httpClient.BaseAddress = new Uri("https://localhost:5001/api/");
			_httpClient.Timeout = new TimeSpan(0, 0, 30);
			_httpClient.DefaultRequestHeaders.Clear();

			_options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
		}

		public async Task Execute()
		{
			await GetCompaniesWithStream();
			//await CreateCompanyWithStream();
		}

		private async Task GetCompaniesWithStream()
		{
			using (var response = await _httpClient.GetAsync("companies", HttpCompletionOption.ResponseHeadersRead))
			{
				response.EnsureSuccessStatusCode();

				var stream = await response.Content.ReadAsStreamAsync();

				var companies = await JsonSerializer.DeserializeAsync<List<CompanyDto>>(stream, _options);
			}
		}

		private async Task CreateCompanyWithStream()
		{
			var companyForCreation = new CompanyForCreationDto
			{
				Name = "Eagle IT Ltd.",
				Country = "USA",
				Address = "Eagle IT Street 289"
			};

			var ms = new MemoryStream();
			await JsonSerializer.SerializeAsync(ms, companyForCreation);
			ms.Seek(0, SeekOrigin.Begin);

			var request = new HttpRequestMessage(HttpMethod.Post, "companies");
			request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			using (var requestContent = new StreamContent(ms))
			{
				request.Content = requestContent;
				requestContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

				using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
				{
					response.EnsureSuccessStatusCode();

					var content = await response.Content.ReadAsStreamAsync();
					var createdCompany = await JsonSerializer.DeserializeAsync<CompanyDto>(content, _options);
				}	
			}	
		}
	}
}
