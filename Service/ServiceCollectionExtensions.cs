using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("BimspotTest.Service.UnitTest")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace BimspotTest.Service
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection ConfigureServices(this IServiceCollection services)
		{
			services.AddSingleton<ISpeciesService, SpeciesService>();
			services.AddSingleton<Proxy.IIUCNProxy, Proxy.IUCNProxy>();

			return services;
		}
	}
}
