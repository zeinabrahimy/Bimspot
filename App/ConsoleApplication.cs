using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

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
			await DisplayMenu().ConfigureAwait(false);

		}

		private async Task GetCriticalEndangered()
		{
			Console.WriteLine("Start Getting Data... Please Be Patient :-)");
			var result = await this.speciesService.GetCriticalEndangered().ConfigureAwait(false);
			DisplayResult(result, "Critical Endangered");
			await DisplayMenu();
		}
		private async Task GetMammal()
		{
			Console.WriteLine("Start Getting Data... Please Be Patient :-)");
			var result = await this.speciesService.GetAllMammals().ConfigureAwait(false);
			DisplayResult(result, "Mammal");

		}

		private async Task DisplayMenu()
		{

			Console.WriteLine("Choose an option:");
			Console.WriteLine("1) Critical Endangered Animals");
			Console.WriteLine("2) Mammal Animals");
			Console.WriteLine("3) Exit");
			Console.Write("\r\nSelect an option: ");

			switch (Console.ReadLine())
			{
				case "1":
					await GetCriticalEndangered().ConfigureAwait(false);
					break;
				case "2":
					await GetMammal().ConfigureAwait(false);
					break;
				case "3":
				default:
					Console.Clear();
					break;
			}


			Console.ReadLine();
		}



		private void DisplayResult(BimspotTestServiceResult<DTO.Species.GetSpeciesResponse> result, string message)
		{

			if (result != null && result.Success)
			{
				Console.WriteLine("Successful");
				if (result.Result != null && result.Result.Species != null)
				{

					Console.WriteLine($"There Is(Are) {message} {result.Result.Species.Count()} Animal In {result.Result.RegionIdentifier}");
					int counter = 0;
					foreach (var item in result.Result.Species)
					{
						counter++;
						Console.WriteLine($@"{counter} 
																- TaxonId: {item.TaxonId} *** 
																  Scientific Name: {item.ScientificName} *** 
																  Population: {item.Population} *** 
																  Phylum Name: {item.PhylumName} *** 
																  Order Name: {item.OrderName} *** 
																  Kingdom Name:{item.KingdomName} *** 
																  Infra Rank: {item.InfraRank} *** 
																  Infra Name: {item.InfraName} *** 
																  Genus Name:{item.GenusName} *** 
																  Family Name: {item.FamilyName} *** 
																  Conservation Measures: {item.ConservationMeasures} *** 
																  Class Name: {item.ClassName} ***  
																  Category: {item.Category}");
					}

					return;
				}
				Console.WriteLine("There Is No Data");
			}
			if (!result.Success)
			{
				Console.WriteLine(string.Format("An error occured: {0}", result.Errors?.FirstOrDefault().Message ?? "Unknown Error"));
			}

		}
	}
}
