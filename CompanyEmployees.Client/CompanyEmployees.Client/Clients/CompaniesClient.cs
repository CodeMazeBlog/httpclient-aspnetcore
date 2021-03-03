using Entities.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CompanyEmployees.Client.Clients
{
	public class CompaniesClient
	{
		private readonly HttpClient _client;
		private readonly JsonSerializerOptions _options;

		public CompaniesClient(HttpClient client)
		{
			_client = client;

			_client.BaseAddress = new Uri("https://localhost:5001/api/");
			_client.Timeout = new TimeSpan(0, 0, 30);
			_client.DefaultRequestHeaders.Clear();

			_options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
		}

		public async Task<List<CompanyDto>> GetCompanies()
		{
			using (var response = await _client.GetAsync("companies", HttpCompletionOption.ResponseHeadersRead))
			{
				response.EnsureSuccessStatusCode();

				var stream = await response.Content.ReadAsStreamAsync();

				var companies = await JsonSerializer.DeserializeAsync<List<CompanyDto>>(stream, _options);

				return companies;
			}
		}
	}
}
