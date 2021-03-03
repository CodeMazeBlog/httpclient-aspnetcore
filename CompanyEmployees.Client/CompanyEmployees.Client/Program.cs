using CompanyEmployees.Client.Clients;
using CompanyEmployees.Client.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace CompanyEmployees.Client
{
	class Program
	{
		static async Task Main(string[] args)
		{
			var services = new ServiceCollection();

			ConfigureServices(services);

			var provider = services.BuildServiceProvider();

			try
			{
				await provider.GetRequiredService<IHttpClientServiceImplementation>()
					.Execute();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Something went wrong: {ex}");
			}
		}

		private static void ConfigureServices(IServiceCollection services)
		{
			services.AddHttpClient("CompaniesClient", config =>
			{
				config.BaseAddress = new Uri("https://localhost:5001/api/");
				config.Timeout = new TimeSpan(0, 0, 30);
				config.DefaultRequestHeaders.Clear();
			});

			services.AddHttpClient<CompaniesClient>();

			//services.AddScoped<IHttpClientServiceImplementation, HttpClientCrudService>();
			//services.AddScoped<IHttpClientServiceImplementation, HttpClientPatchService>();
			//services.AddScoped<IHttpClientServiceImplementation, HttpClientStreamService>();
			//services.AddScoped<IHttpClientServiceImplementation, HttpClientCancellationService>();
			services.AddScoped<IHttpClientServiceImplementation, HttpClientFactoryService>();
		}
	}
}
