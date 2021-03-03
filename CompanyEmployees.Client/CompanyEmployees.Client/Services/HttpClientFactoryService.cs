using CompanyEmployees.Client.Clients;
using Entities.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CompanyEmployees.Client.Services
{
	public class HttpClientFactoryService : IHttpClientServiceImplementation
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly CompaniesClient _companiesClient;
		private readonly JsonSerializerOptions _options;

		public HttpClientFactoryService(IHttpClientFactory httpClientFactory, CompaniesClient companiesClient)
		{
			_httpClientFactory = httpClientFactory;
			_companiesClient = companiesClient;

			_options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
		}

		public async Task Execute()
		{
			//await GetCompaniesWithHttpClientFactory();
			await GetCompaniesWithTypedClient();
		}

		private async Task GetCompaniesWithHttpClientFactory()
		{
			var httpClient = _httpClientFactory.CreateClient("CompaniesClient");

			using (var response = await httpClient.GetAsync("companies", HttpCompletionOption.ResponseHeadersRead))
			{
				response.EnsureSuccessStatusCode();

				var stream = await response.Content.ReadAsStreamAsync();

				var companies = await JsonSerializer.DeserializeAsync<List<CompanyDto>>(stream, _options);
			}
		}

		private async Task GetCompaniesWithTypedClient() => await _companiesClient.GetCompanies();
	}
}
