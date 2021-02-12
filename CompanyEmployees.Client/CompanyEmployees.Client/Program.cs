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
			services.AddScoped<IHttpClientServiceImplementation, HttpClientCrudService>();
		}
	}
}
