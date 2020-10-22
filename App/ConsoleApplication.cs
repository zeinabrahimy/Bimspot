using System;
using System.Linq;
using System.Threading.Tasks;

namespace BimspotTest.App
{
	public class ConsoleApplication
	{
		private Service.ISpeciesService speciesService;
		public ConsoleApplication(Service.ISpeciesService speciesService)
		{
			this.speciesService = speciesService;
		}

		// Application starting point
		public async Task Run()
		{
			Console.Clear();

		}

	}
}
