using BimspotTest.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace BimspotTest.App
{
	class Program
	{
		static async Task Main(string[] args)
		{
			IConfiguration configuration = 
				new ConfigurationBuilder()
						.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
						.AddEnvironmentVariables()
						.AddCommandLine(args)
						.Build();

			var services = ConfigureServices(configuration);
			
			var serviceProvider = services.BuildServiceProvider();

			await serviceProvider.GetService<ConsoleApplication>().Run();
		}

		private static IServiceCollection ConfigureServices(IConfiguration configuration)
		{
			IServiceCollection services = new ServiceCollection();
			services.AddSingleton(configuration);
			services.ConfigureServices();
			services.AddSingleton<ConsoleApplication>();
			return services;
		}
	}
}
