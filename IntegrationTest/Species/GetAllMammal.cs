using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BimspotTest.Service.IntegrationTest.Species
{
	public class GetAllMammal
	{
		[Fact]
		public async void GetAllMammal_RightScenario()
		{
			// Arrange
			IConfiguration configuration = new ConfigurationBuilder()
																				 .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
																				 .AddEnvironmentVariables()
																				 .Build();

			IServiceCollection services = new ServiceCollection();
			services.AddSingleton(configuration);
			services.ConfigureServices();

			var serviceProvider = services.BuildServiceProvider();

			var speciesService = serviceProvider.GetService<BimspotTest.Service.ISpeciesService>();

			// Act
			var result = await speciesService.GetAllMammals().ConfigureAwait(false);

			// Assert
			Assert.NotNull(result);
		}
	}
}
