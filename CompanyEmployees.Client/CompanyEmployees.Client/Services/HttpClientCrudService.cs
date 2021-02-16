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
			//await GetCompaniesWithXMLHeader();
			//await CreateCompany();
			//await CreateCompanyWithHttpRequestMessage();
			//await UpdateCompany();
			//await UpdateCompanyWithHttpRequestMessage();
			//await DeleteCompany();
			await DeleteCompanyWithHttpResponseMessage();
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

		private async Task CreateCompany()
		{
			var companyForCreation = new CompanyForCreationDto
			{
				Name = "Eagle IT Ltd.",
				Country = "USA",
				Address	= "Eagle IT Street 289"
			};

			var company = JsonSerializer.Serialize(companyForCreation);

			var requestContent = new StringContent(company, Encoding.UTF8, "application/json");

			var response = await _httpClient.PostAsync("companies", requestContent);
			response.EnsureSuccessStatusCode();

			var content = await response.Content.ReadAsStringAsync();
			var createdCompany = JsonSerializer.Deserialize<CompanyDto>(content, _options);
		}

		private async Task CreateCompanyWithHttpRequestMessage()
		{
			var companyForCreation = new CompanyForCreationDto
			{
				Name = "Hawk IT Ltd.",
				Country = "USA",
				Address = "Hawk IT Street 365"
			};

			var company = JsonSerializer.Serialize(companyForCreation);

			var request = new HttpRequestMessage(HttpMethod.Post, "companies");
			request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			request.Content = new StringContent(company, Encoding.UTF8);
			request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			var response = await _httpClient.SendAsync(request);
			response.EnsureSuccessStatusCode();

			var content = await response.Content.ReadAsStringAsync();
			var createdCompany = JsonSerializer.Deserialize<CompanyDto>(content, _options);
		}

		private async Task UpdateCompany()
		{
			var updatedCompany = new CompanyForUpdateDto
			{
				Name = "Eagle IT Ltd.",
				Country = "USA",
				Address = "Eagle IT Street 289 Updated"
			};

			var company = JsonSerializer.Serialize(updatedCompany);

			var requestContent = new StringContent(company, Encoding.UTF8, "application/json");

			var uri = Path.Combine("companies", "fc12c11e-33a3-45e2-f11e-08d8bdb38ded");
			var response = await _httpClient.PutAsync(uri, requestContent);
			response.EnsureSuccessStatusCode();
		}

		private async Task UpdateCompanyWithHttpRequestMessage()
		{
			var updatedCompany = new CompanyForCreationDto
			{
				Name = "Hawk IT Ltd.",
				Country = "USA",
				Address = "Hawk IT Street 365 Updated"
			};

			var company = JsonSerializer.Serialize(updatedCompany);

			var uri = Path.Combine("companies", "29bc0429-eb4d-4eeb-f11d-08d8bdb38ded");
			var request = new HttpRequestMessage(HttpMethod.Put, uri);
			request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			request.Content = new StringContent(company, Encoding.UTF8);
			request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			var response = await _httpClient.SendAsync(request);
			response.EnsureSuccessStatusCode();
		}

		private async Task DeleteCompany()
		{
			var uri = Path.Combine("companies", "fc12c11e-33a3-45e2-f11e-08d8bdb38ded");

			var response = await _httpClient.DeleteAsync(uri);
			response.EnsureSuccessStatusCode();
		}

		private async Task DeleteCompanyWithHttpResponseMessage()
		{
			var uri = Path.Combine("companies", "29bc0429-eb4d-4eeb-f11d-08d8bdb38ded");
			var request = new HttpRequestMessage(HttpMethod.Delete, uri);
			request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			var response = await _httpClient.SendAsync(request);
			response.EnsureSuccessStatusCode();
		}
	}
}
