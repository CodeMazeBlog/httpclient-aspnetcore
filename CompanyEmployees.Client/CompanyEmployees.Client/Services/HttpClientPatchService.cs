using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Entities.DataTransferObjects;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace CompanyEmployees.Client.Services
{
	public class HttpClientPatchService : IHttpClientServiceImplementation
	{
		private static readonly HttpClient _httpClient = new HttpClient();

		public HttpClientPatchService()
		{
			_httpClient.BaseAddress = new Uri("https://localhost:5001/api/");
			_httpClient.Timeout = new TimeSpan(0, 0, 30);
			_httpClient.DefaultRequestHeaders.Clear();
		}

		public async Task Execute()
		{
			//await PatchEmployee();
			await PatchEmployeeWithHttpRequestMessage();
		}

		private async Task PatchEmployee()
		{
			var patchDoc = new JsonPatchDocument<EmployeeForUpdateDto>();
			patchDoc.Replace(e => e.Name, "Sam Raiden Updated");
			patchDoc.Remove(e => e.Age);

			var uri = Path.Combine("companies", "C9D4C053-49B6-410C-BC78-2D54A9991870", "employees", "80ABBCA8-664D-4B20-B5DE-024705497D4A");
			var serializedDoc = JsonConvert.SerializeObject(patchDoc);
			var requestContent = new StringContent(serializedDoc, Encoding.UTF8, "application/json-patch+json");

			var response = await _httpClient.PatchAsync(uri, requestContent);
			response.EnsureSuccessStatusCode();
		}

		private async Task PatchEmployeeWithHttpRequestMessage()
		{
			var patchDoc = new JsonPatchDocument<EmployeeForUpdateDto>();
			patchDoc.Replace(e => e.Name, "Sam Raiden");
			patchDoc.Add(e => e.Age, 28);

			var uri = Path.Combine("companies", "C9D4C053-49B6-410C-BC78-2D54A9991870", "employees", "80ABBCA8-664D-4B20-B5DE-024705497D4A");
			var serializedDoc = JsonConvert.SerializeObject(patchDoc);

			var request = new HttpRequestMessage(HttpMethod.Patch, uri);
			request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			request.Content = new StringContent(serializedDoc, Encoding.UTF8);
			request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json-patch+json");

			var response = await _httpClient.SendAsync(request);
			response.EnsureSuccessStatusCode();
		}
	}
}
