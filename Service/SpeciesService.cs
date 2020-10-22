using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace BimspotTest.Service
{
	internal class SpeciesService : ISpeciesService
	{
		private Proxy.IIUCNProxy iUCNProxy;
		private Proxy.DTO.Region selectedRegion;
		private readonly object selectedRegionLocker = new Object();
		private Proxy.DTO.Region SelectedRegion
		{
			set
			{
				lock (selectedRegionLocker)
				{
					selectedRegion = value;
				}
			}
			get
			{
				lock (selectedRegionLocker)
				{
					return selectedRegion;
				}
			}
		}
		private readonly object speciesesLocker = new Object();
		private Proxy.DTO.GetSpeciesOfRegionResponse specieses;
		private Proxy.DTO.GetSpeciesOfRegionResponse Specieses
		{
			set
			{
				lock (speciesesLocker)
				{
					specieses = value;
				}
			}
			get
			{
				lock (speciesesLocker)
				{
					return specieses;
				}
			}
		}
		private System.Collections.Concurrent.BlockingCollection<Proxy.DTO.GetSpeciesConservationMeasureseResponse> allConservationMeasures;

		public SpeciesService(Proxy.IIUCNProxy iUCNProxy)
		{
			this.iUCNProxy = iUCNProxy;
			this.SelectedRegion = null;
			this.Specieses = null;
			this.allConservationMeasures = null;
		}

		#region Implementaion Of SpeciesService

		public async Task<BimspotTestServiceResult<DTO.Species.GetSpeciesResponse>> GetCriticalEndangered()
		{
			if (this.Specieses == null)
			{
				var getAllSpeciesError = await GetAllSpecies().ConfigureAwait(false);
				if (getAllSpeciesError != null)
					return new BimspotTestServiceResult<DTO.Species.GetSpeciesResponse>(getAllSpeciesError);
			}

			var criterialEndangeredSpecies = this.Specieses.Result.Where(x => x.Category.ToLower() == "cr");

			if (criterialEndangeredSpecies.Count() > 0)
			{
				if (this.allConservationMeasures == null)
				{
					var getGetAllConservationMeasuresError = await GetAllConservationMeasures().ConfigureAwait(false);
					if (getGetAllConservationMeasuresError != null)
						return new BimspotTestServiceResult<DTO.Species.GetSpeciesResponse>(getGetAllConservationMeasuresError);
				}
			}

			var result = GenerateCriticalEndangeredResponse(criterialEndangeredSpecies.ToList());

			return new BimspotTestServiceResult<DTO.Species.GetSpeciesResponse>(new DTO.Species.GetSpeciesResponse
			{
				Species = result,
				RegionIdentifier = this.SelectedRegion.Identifier
			});
		}

		public async Task<BimspotTestServiceResult<DTO.Species.GetSpeciesResponse>> GetAllMammals()
		{
			if (this.Specieses == null)
			{
				var getAllSpeciesError = await GetAllSpecies().ConfigureAwait(false);
				if (getAllSpeciesError != null)
					return new BimspotTestServiceResult<DTO.Species.GetSpeciesResponse>(getAllSpeciesError);
			}

			var mammalSpecies = this.Specieses.Result.Where(x => x.ClassName.ToLower() == "mammalia").ToList();
			if (mammalSpecies != null && mammalSpecies.Count() > 0)
			{
				if (this.allConservationMeasures == null)
				{
					var getGetAllConservationMeasuresError = await GetAllConservationMeasures().ConfigureAwait(false);
					if (getGetAllConservationMeasuresError != null)
						return new BimspotTestServiceResult<DTO.Species.GetSpeciesResponse>(getGetAllConservationMeasuresError);
				}
			}



			var result = GenerateCriticalEndangeredResponse(mammalSpecies);

			return new BimspotTestServiceResult<DTO.Species.GetSpeciesResponse>(new DTO.Species.GetSpeciesResponse
			{
				Species = result,
				RegionIdentifier = this.SelectedRegion.Identifier
			});
		}

		#endregion

		#region Internal Methods

		// this method has been become internal to use in unit and integration tests
		internal async Task<BimspotTestErrorResult> LoadRandomRegion()
		{
			var regions = await iUCNProxy.GetRegions().ConfigureAwait(false);

			if (regions == null)
				return new BimspotTestErrorResult { Type = Enumeration.ErrorType.Regions_NullResponse, Message = "Regions Returns NULL" };

			if (regions.Count < 1 || regions.Results == null || regions.Results.Count() < 1 || !regions.Results.Any(x => x != null))
				return new BimspotTestErrorResult { Type = Enumeration.ErrorType.Regions_WithoutRegion, Message = "There Is No Item In Regions" };

			var correctRegions = regions.Results.Where(x => x != null).ToList();

			var randomRegionIndex = new Random().Next(0, correctRegions.Count - 1);

			var randomRegion = correctRegions[randomRegionIndex];

			this.SelectedRegion = randomRegion;

			return null;
		}

		// this method has been become internal to use in unit and integration tests
		internal async Task<BimspotTestErrorResult> GetAllSpecies()
		{
			if (this.SelectedRegion == null)
			{

				var loadRandomRegionError = await LoadRandomRegion().ConfigureAwait(false);
				if (loadRandomRegionError != null)
					return loadRandomRegionError;
			}

			if (this.Specieses == null || this.Specieses.RegionIdentifier != this.SelectedRegion.Identifier)
			{

				var speciesOfRegion = await iUCNProxy.GetSpeciesOfRegion(new Proxy.DTO.GetSpeciesOfRegionRequest
				{
					PageIndex = 0,
					RegionIdentifier = this.SelectedRegion.Identifier
				}).ConfigureAwait(false);

				if (speciesOfRegion == null || speciesOfRegion.Result == null || !speciesOfRegion.Result.Any(x => x != null))
					return new BimspotTestErrorResult { Type = Enumeration.ErrorType.Species_NullResponse, Message = "Species Returns NULL" };


				if (speciesOfRegion.Count < 1 || speciesOfRegion.Result.Count() < 1)
					this.Specieses = new Proxy.DTO.GetSpeciesOfRegionResponse
					{
						Count = 0,
						Page = 0,
						Result = null,
						RegionIdentifier = this.SelectedRegion.Identifier
					};
				else
					this.Specieses = new Proxy.DTO.GetSpeciesOfRegionResponse
					{
						Count = speciesOfRegion.Count,
						RegionIdentifier = speciesOfRegion.RegionIdentifier,
						Page = speciesOfRegion.Page,
						Result = speciesOfRegion.Result.Where(x => x != null).ToList()
					};
			}
			return null;
		}

		// this method has been become internal to use in unit and integration tests
		internal async Task<BimspotTestErrorResult> GetAllConservationMeasures()
		{
			if (this.SelectedRegion == null || this.Specieses == null)
			{
				var getAllSpeciesError = await GetAllSpecies().ConfigureAwait(false);
				if (getAllSpeciesError != null)
					return getAllSpeciesError;
			}

			this.allConservationMeasures = new System.Collections.Concurrent.BlockingCollection<Proxy.DTO.GetSpeciesConservationMeasureseResponse>();

			var getConservationMeasuresTasks = new List<Task<Proxy.DTO.GetSpeciesConservationMeasureseResponse>>();

			foreach (var species in this.Specieses.Result)
			{
				getConservationMeasuresTasks.Add(iUCNProxy.GetSpeciesConservationMeasures(species.TaxonId));
			}

			await Task.WhenAll(getConservationMeasuresTasks).ConfigureAwait(false);

			foreach (var task in getConservationMeasuresTasks)
				if (task.Result != null)
				{
					allConservationMeasures.Add(task.Result);
				}

			return null;
		}

		#endregion

		#region Private Methods

		private IEnumerable<DTO.Species.Species> GenerateCriticalEndangeredResponse(IEnumerable<Proxy.DTO.Species> specieses)
		{
			var result = new List<DTO.Species.Species>();

			foreach (var species in specieses)
			{
				result.Add(new DTO.Species.Species
				{
					Category = species.Category,
					ClassName = species.ClassName,
					FamilyName = species.FamilyName,
					GenusName = species.GenusName,
					InfraName = species.InfraName,
					InfraRank = species.InfraRank,
					KingdomName = species.KingdomName,
					OrderName = species.OrderName,
					PhylumName = species.PhylumName,
					Population = species.Population,
					ScientificName = species.ScientificName,
					TaxonId = species.TaxonId,
					ConservationMeasures = string.Join(",", this.allConservationMeasures?.FirstOrDefault(x => x.Id == species.TaxonId)?.ConservationMeasures.ToList().Select(x => x.Title) ?? null)
				});
			}

			return result;
		}

		#endregion
	}
}
