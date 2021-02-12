using Entities.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace CompanyEmployees.Client.Services
{
	public class HttpClientCrudService : IHttpClientServiceImplementation
	{
		private static readonly HttpClient _httpClient = new HttpClient();
		private readonly JsonSerializerOptions _options;

		public HttpClientCrudService()
		{
			_httpClient.BaseAddress = new Uri("https://localhost:5001/api/");
			_httpClient.Timeout = new TimeSpan(0, 0, 30);
			_httpClient.DefaultRequestHeaders.Clear();

			_options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
		}

		public async Task Execute()
		{
			//await GetCompanies();
			await GetCompaniesWithXMLHeader();
		}

		private async Task GetCompanies()
		{
			var response = await _httpClient.GetAsync("companies");
			response.EnsureSuccessStatusCode();

			var content = await response.Content.ReadAsStringAsync();
			var companies = JsonSerializer.Deserialize<List<CompanyDto>>(content, _options);
		}

		private async Task GetCompaniesWithXMLHeader()
		{
			var request = new HttpRequestMessage(HttpMethod.Get, "companies");
			request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));
			var response = await _httpClient.SendAsync(request);

			response.EnsureSuccessStatusCode();

			var content = await response.Content.ReadAsStringAsync();

			var doc = XDocument.Parse(content);
			foreach (var element in doc.Descendants())
			{
				element.Attributes().Where(a => a.IsNamespaceDeclaration).Remove();
				element.Name = element.Name.LocalName;
			}

			var serializer = new XmlSerializer(typeof(List<CompanyDto>));
			var companies = (List<CompanyDto>)serializer.Deserialize(new StringReader(doc.ToString()));
		}
	}
}
