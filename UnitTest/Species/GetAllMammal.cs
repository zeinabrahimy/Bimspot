using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BimspotTest.UnitTest
{
	public class GetAllMammal
	{
		[Theory]
		[MemberData(nameof(GetInvalidRegions))]
		public async Task GetAllMammal_InavalidRegions_ReturnsError(Service.Proxy.DTO.GetRegionResponse regions, string message, BimspotTestErrorResult errorResult)
		{
			// Arrange
			var iUCNProxy = new Moq.Mock<Service.Proxy.IIUCNProxy>();
			var speciesService = new BimspotTest.Service.SpeciesService(iUCNProxy.Object);

			iUCNProxy.Setup(x => x.GetRegions()).ReturnsAsync(regions);

			// Act
			var result = await speciesService.GetAllMammals().ConfigureAwait(false);

			// Assert
			Assert.NotNull(result);
			Assert.False(result.Success);
			Assert.NotNull(result.Errors);
			Assert.True(result.Errors.Count() == 1);
			Assert.Equal(errorResult.Type, result.Errors.FirstOrDefault().Type);
			Assert.Equal(errorResult.Message, result.Errors.FirstOrDefault().Message);

			iUCNProxy.Verify(x => x.GetRegions(), Times.Once);

			iUCNProxy.Verify(x => x.GetSpeciesOfRegion(It.IsAny<Service.Proxy.DTO.GetSpeciesOfRegionRequest>()), Times.Never);
			iUCNProxy.Verify(x => x.GetSpeciesConservationMeasures(It.IsAny<int>()), Times.Never);
		}

		[Theory]
		[MemberData(nameof(GetInvalidSpecies))]
		public async Task GetAllMammal_InvalidSpecies_ReturnsError(Service.Proxy.DTO.GetSpeciesOfRegionResponse species, string message)
		{
			// Arrange
			var iUCNProxy = new Moq.Mock<Service.Proxy.IIUCNProxy>();
			var speciesService = new BimspotTest.Service.SpeciesService(iUCNProxy.Object);

			var region = new Service.Proxy.DTO.Region
			{
				Identifier = "northeastern_africa",
				Name = "Northeastern Africa"
			};
			var regions = new Service.Proxy.DTO.GetRegionResponse()
			{
				Count = 1,
				Results = new List<Service.Proxy.DTO.Region> { region }
			};
			iUCNProxy.Setup(x => x.GetRegions()).ReturnsAsync(regions);

			Service.Proxy.DTO.GetSpeciesOfRegionRequest getSpeciesOfRegionRequest = null;
			iUCNProxy.Setup(x => x.GetSpeciesOfRegion(It.IsAny<Service.Proxy.DTO.GetSpeciesOfRegionRequest>()))
							 .Callback<Service.Proxy.DTO.GetSpeciesOfRegionRequest>(x => getSpeciesOfRegionRequest = x)
							 .ReturnsAsync(species);


			// Act
			var result = await speciesService.GetAllMammals().ConfigureAwait(false);

			// Assert
			Assert.NotNull(result);
			Assert.False(result.Success);
			Assert.NotNull(result.Errors);
			Assert.True(result.Errors.Count() == 1);
			Assert.Equal(Enumeration.ErrorType.Species_NullResponse, result.Errors.FirstOrDefault().Type);
			Assert.Equal("Species Returns NULL", result.Errors.FirstOrDefault().Message);

			iUCNProxy.Verify(x => x.GetRegions(), Times.Once);
			iUCNProxy.Verify(x => x.GetSpeciesOfRegion(It.IsAny<Service.Proxy.DTO.GetSpeciesOfRegionRequest>()), Times.Once);
			Assert.NotNull(getSpeciesOfRegionRequest);
			Assert.Equal(0, getSpeciesOfRegionRequest.PageIndex);
			Assert.NotNull(getSpeciesOfRegionRequest.RegionIdentifier);
			Assert.Equal(region.Identifier, getSpeciesOfRegionRequest.RegionIdentifier);

			iUCNProxy.Verify(x => x.GetSpeciesConservationMeasures(It.IsAny<int>()), Times.Never);

		}

		[Fact]
		public async Task GetAllMammal_ThereIsNoMammalInSpecies_ReturnsEmptyList()
		{
			// Arrange
			var iUCNProxy = new Moq.Mock<Service.Proxy.IIUCNProxy>();
			var speciesService = new BimspotTest.Service.SpeciesService(iUCNProxy.Object);

			var region = new Service.Proxy.DTO.Region
			{
				Identifier = "northeastern_africa",
				Name = "Northeastern Africa"
			};
			var regions = new Service.Proxy.DTO.GetRegionResponse()
			{
				Count = 1,
				Results = new List<Service.Proxy.DTO.Region> { region }
			};
			iUCNProxy.Setup(x => x.GetRegions()).ReturnsAsync(regions);

			var species = new Service.Proxy.DTO.GetSpeciesOfRegionResponse
			{
				Page = 0,
				RegionIdentifier = region.Identifier,
				Count = 1,
				Result = new List<Service.Proxy.DTO.Species>()
				{
					new Service.Proxy.DTO.Species
					{
						Category = "DD",
						ClassName = "GASTROPODA",
						FamilyName = "VALLONIIDAE",
						GenusName = "Acanthinula",
						InfraName = null,
						InfraRank = null,
						KingdomName = "ANIMALIA",
						OrderName = "STYLOMMATOPHORA",
						PhylumName = "MOLLUSCA",
						Population = null,
						ScientificName = "Acanthinula spinifera",
						TaxonId = 59
					}
				}
			};

			Service.Proxy.DTO.GetSpeciesOfRegionRequest getSpeciesOfRegionRequest = null;
			iUCNProxy.Setup(x => x.GetSpeciesOfRegion(It.IsAny<Service.Proxy.DTO.GetSpeciesOfRegionRequest>()))
							 .Callback<Service.Proxy.DTO.GetSpeciesOfRegionRequest>(x => getSpeciesOfRegionRequest = x)
							 .ReturnsAsync(species);


			// Act
			var result = await speciesService.GetAllMammals().ConfigureAwait(false);

			// Assert
			Assert.NotNull(result);
			Assert.True(result.Success);
			Assert.True(result.Errors == null || result.Errors.Count() == 0);
			Assert.NotNull(result.Result);
			Assert.Equal(region.Identifier, result.Result.RegionIdentifier);

			iUCNProxy.Verify(x => x.GetRegions(), Times.Once);
			iUCNProxy.Verify(x => x.GetSpeciesOfRegion(It.IsAny<Service.Proxy.DTO.GetSpeciesOfRegionRequest>()), Times.Once);
			Assert.NotNull(getSpeciesOfRegionRequest);
			Assert.Equal(0, getSpeciesOfRegionRequest.PageIndex);
			Assert.NotNull(getSpeciesOfRegionRequest.RegionIdentifier);
			Assert.Equal(region.Identifier, getSpeciesOfRegionRequest.RegionIdentifier);

			iUCNProxy.Verify(x => x.GetSpeciesConservationMeasures(It.IsAny<int>()), Times.Never);
		}

		[Fact]
		public async Task GetAllMammal_ThereIsOneMammalInSpecies_ReturnsOneSpecies()
		{
			// Arrange
			var iUCNProxy = new Moq.Mock<Service.Proxy.IIUCNProxy>();
			var speciesService = new BimspotTest.Service.SpeciesService(iUCNProxy.Object);

			var region = new Service.Proxy.DTO.Region
			{
				Identifier = "northeastern_africa",
				Name = "Northeastern Africa"
			};
			var regions = new Service.Proxy.DTO.GetRegionResponse()
			{
				Count = 1,
				Results = new List<Service.Proxy.DTO.Region> { region }
			};
			iUCNProxy.Setup(x => x.GetRegions()).ReturnsAsync(regions);

			int mammalSpeciesTaxonId = 13653;
			var mammalSpecies = new Service.Proxy.DTO.Species
			{
				Category = "CR",
				ClassName = "MAMMALIA",
				FamilyName = "PHOCIDAE",
				GenusName = "Monachus",
				InfraName = null,
				InfraRank = null,
				KingdomName = "ANIMALIA",
				OrderName = "CARNIVORA",
				PhylumName = "CHORDATA",
				Population = null,
				ScientificName = "Monachus monachus",
				TaxonId = mammalSpeciesTaxonId
			};

			var speciesResponse = new Service.Proxy.DTO.GetSpeciesOfRegionResponse
			{
				Page = 0,
				RegionIdentifier = region.Identifier,
				Count = 1,
				Result = new List<Service.Proxy.DTO.Species>() { mammalSpecies }
			};

			Service.Proxy.DTO.GetSpeciesOfRegionRequest getSpeciesOfRegionRequest = null;
			iUCNProxy.Setup(x => x.GetSpeciesOfRegion(It.IsAny<Service.Proxy.DTO.GetSpeciesOfRegionRequest>()))
							 .Callback<Service.Proxy.DTO.GetSpeciesOfRegionRequest>(x => getSpeciesOfRegionRequest = x)
							 .ReturnsAsync(speciesResponse);

			var mammalConservationMeasurese = new Service.Proxy.DTO.GetSpeciesConservationMeasureseResponse
			{
				Id = mammalSpeciesTaxonId,
				ConservationMeasures = new List<Service.Proxy.DTO.ConservationMeasure>
				{
					new Service.Proxy.DTO.ConservationMeasure
					{
						Code = "5.4",
						Title = "Compliance and enforcement",
					},
					new Service.Proxy.DTO.ConservationMeasure
					{
						Code = "5.4.2",
						Title = "National level"
					}
				}
			};

			int getSpeciesConservationMeasuresRequest = -1000;
			iUCNProxy.Setup(x => x.GetSpeciesConservationMeasures(It.IsAny<int>()))
							 .Callback<int>(x => getSpeciesConservationMeasuresRequest = x)
							 .ReturnsAsync(mammalConservationMeasurese);


			// Act
			var result = await speciesService.GetAllMammals().ConfigureAwait(false);

			// Assert
			Assert.NotNull(result);
			Assert.True(result.Success);
			Assert.True(result.Errors == null || result.Errors.Count() == 0);
			Assert.NotNull(result.Result);
			Assert.Equal(region.Identifier, result.Result.RegionIdentifier);


			var returnedSpecieses = result.Result.Species;
			Assert.NotNull(returnedSpecieses);
			Assert.True(returnedSpecieses.Count() == 1);
			var returnedSpecies = returnedSpecieses.FirstOrDefault();
			Assert.NotNull(returnedSpecies);
			Assert.Equal(mammalSpecies.Category, returnedSpecies.Category);
			Assert.Equal(mammalSpecies.Population, returnedSpecies.Population);
			Assert.Equal(mammalSpecies.InfraName, returnedSpecies.InfraName);
			Assert.Equal(mammalSpecies.InfraRank, returnedSpecies.InfraRank);
			Assert.Equal(mammalSpecies.ScientificName, returnedSpecies.ScientificName);
			Assert.Equal(mammalSpecies.GenusName, returnedSpecies.GenusName);
			Assert.Equal(mammalSpecies.FamilyName, returnedSpecies.FamilyName);
			Assert.Equal(mammalSpecies.OrderName, returnedSpecies.OrderName);
			Assert.Equal(mammalSpecies.ClassName, returnedSpecies.ClassName);
			Assert.Equal(mammalSpecies.PhylumName, returnedSpecies.PhylumName);
			Assert.Equal(mammalSpecies.KingdomName, returnedSpecies.KingdomName);
			Assert.Equal(mammalSpecies.TaxonId, returnedSpecies.TaxonId);
			Assert.NotNull(returnedSpecies.ConservationMeasures);
			Assert.True(returnedSpecies.ConservationMeasures != string.Empty);
			var returnedConservationMeasures = returnedSpecies.ConservationMeasures.Split(',');
			Assert.NotNull(returnedConservationMeasures);
			Assert.True(returnedConservationMeasures.Count() == 2);
			Assert.Contains("Compliance and enforcement", returnedConservationMeasures);
			Assert.Contains("National level", returnedConservationMeasures);



			iUCNProxy.Verify(x => x.GetRegions(), Times.Once);
			iUCNProxy.Verify(x => x.GetSpeciesOfRegion(It.IsAny<Service.Proxy.DTO.GetSpeciesOfRegionRequest>()), Times.Once);
			Assert.NotNull(getSpeciesOfRegionRequest);
			Assert.Equal(0, getSpeciesOfRegionRequest.PageIndex);
			Assert.NotNull(getSpeciesOfRegionRequest.RegionIdentifier);
			Assert.Equal(region.Identifier, getSpeciesOfRegionRequest.RegionIdentifier);



			iUCNProxy.Verify(x => x.GetSpeciesConservationMeasures(It.IsAny<int>()), Times.Once);
			Assert.Equal(mammalSpeciesTaxonId, getSpeciesConservationMeasuresRequest);
		}

		[Fact]
		public async Task GetAllMammal_ThereIsOneMammalInSpeciesOneAnotherAnimal_ReturnOneMammalSpecies()
		{

			// Arrange
			var iUCNProxy = new Moq.Mock<Service.Proxy.IIUCNProxy>();
			var speciesService = new BimspotTest.Service.SpeciesService(iUCNProxy.Object);

			var region = new Service.Proxy.DTO.Region
			{
				Identifier = "northeastern_africa",
				Name = "Northeastern Africa"
			};
			var regions = new Service.Proxy.DTO.GetRegionResponse()
			{
				Count = 1,
				Results = new List<Service.Proxy.DTO.Region> { region }
			};
			iUCNProxy.Setup(x => x.GetRegions()).ReturnsAsync(regions);

			int speciesTaxonId = 224;
			var species = new Service.Proxy.DTO.Species
			{
				Category = "CR",
				ClassName = "ACTINOPTERYGII",
				FamilyName = "ACIPENSERIDAE",
				GenusName = "Acipenser",
				InfraName = null,
				InfraRank = null,
				KingdomName = "ANIMALIA",
				OrderName = "ACIPENSERIFORMES",
				PhylumName = "CHORDATA",
				Population = null,
				ScientificName = "Acipenser naccarii",
				TaxonId = speciesTaxonId
			};

			int mammalSpeciesTaxonId = 13653;
			var mammalSpecies = new Service.Proxy.DTO.Species
			{
				Category = "CR",
				ClassName = "MAMMALIA",
				FamilyName = "PHOCIDAE",
				GenusName = "Monachus",
				InfraName = null,
				InfraRank = null,
				KingdomName = "ANIMALIA",
				OrderName = "CARNIVORA",
				PhylumName = "CHORDATA",
				Population = null,
				ScientificName = "Monachus monachus",
				TaxonId = mammalSpeciesTaxonId
			};

			var speciesResponse = new Service.Proxy.DTO.GetSpeciesOfRegionResponse
			{
				Page = 0,
				RegionIdentifier = region.Identifier,
				Count = 2,
				Result = new List<Service.Proxy.DTO.Species>() { species, mammalSpecies }
			};

			Service.Proxy.DTO.GetSpeciesOfRegionRequest getSpeciesOfRegionRequest = null;
			iUCNProxy.Setup(x => x.GetSpeciesOfRegion(It.IsAny<Service.Proxy.DTO.GetSpeciesOfRegionRequest>()))
							 .Callback<Service.Proxy.DTO.GetSpeciesOfRegionRequest>(x => getSpeciesOfRegionRequest = x)
							 .ReturnsAsync(speciesResponse);

			var conservationMeasurese = new Service.Proxy.DTO.GetSpeciesConservationMeasureseResponse
			{
				Id = speciesTaxonId,
				ConservationMeasures = new List<Service.Proxy.DTO.ConservationMeasure>
				{
					new Service.Proxy.DTO.ConservationMeasure
					{
						Code = "1.1",
						Title = "Site / area protection",
					},
					new Service.Proxy.DTO.ConservationMeasure
					{
						Code = "2.1",
						Title = "Site / area management"
					}
				}
			};

			iUCNProxy.Setup(x => x.GetSpeciesConservationMeasures(It.Is<int>(c => c == speciesTaxonId)))
							 .ReturnsAsync(conservationMeasurese);

			var mammalConservationMeasurese = new Service.Proxy.DTO.GetSpeciesConservationMeasureseResponse
			{
				Id = mammalSpeciesTaxonId,
				ConservationMeasures = new List<Service.Proxy.DTO.ConservationMeasure>
				{
					new Service.Proxy.DTO.ConservationMeasure
					{
						Code = "5.4",
						Title = "Compliance and enforcement",
					},
					new Service.Proxy.DTO.ConservationMeasure
					{
						Code = "5.4.2",
						Title = "National level"
					}
				}
			};

			iUCNProxy.Setup(x => x.GetSpeciesConservationMeasures(It.Is<int>(c => c == mammalSpeciesTaxonId)))
							 .ReturnsAsync(mammalConservationMeasurese);


			// Act
			var result = await speciesService.GetCriticalEndangered().ConfigureAwait(false);

			// Assert
			Assert.NotNull(result);
			Assert.True(result.Success);
			Assert.True(result.Errors == null || result.Errors.Count() == 0);
			Assert.NotNull(result.Result);
			Assert.Equal(region.Identifier, result.Result.RegionIdentifier);


			var returnedSpecieses = result.Result.Species;
			Assert.NotNull(returnedSpecieses);
			Assert.True(returnedSpecieses.Count() == 2);

			var firstSpecies = returnedSpecieses.FirstOrDefault(x => x.TaxonId == speciesTaxonId);
			Assert.NotNull(firstSpecies);
			Assert.Equal(species.Category, firstSpecies.Category);
			Assert.Equal(species.Population, firstSpecies.Population);
			Assert.Equal(species.InfraName, firstSpecies.InfraName);
			Assert.Equal(species.InfraRank, firstSpecies.InfraRank);
			Assert.Equal(species.ScientificName, firstSpecies.ScientificName);
			Assert.Equal(species.GenusName, firstSpecies.GenusName);
			Assert.Equal(species.FamilyName, firstSpecies.FamilyName);
			Assert.Equal(species.OrderName, firstSpecies.OrderName);
			Assert.Equal(species.ClassName, firstSpecies.ClassName);
			Assert.Equal(species.PhylumName, firstSpecies.PhylumName);
			Assert.Equal(species.KingdomName, firstSpecies.KingdomName);
			Assert.Equal(species.TaxonId, firstSpecies.TaxonId);
			Assert.NotNull(firstSpecies.ConservationMeasures);
			Assert.True(firstSpecies.ConservationMeasures != string.Empty);
			var returnedConservationMeasures = firstSpecies.ConservationMeasures.Split(',');
			Assert.NotNull(returnedConservationMeasures);
			Assert.True(returnedConservationMeasures.Count() == 2);
			Assert.Contains("Site / area protection", returnedConservationMeasures);
			Assert.Contains("Site / area management", returnedConservationMeasures);



			var secondSpecies = returnedSpecieses.FirstOrDefault(x => x.TaxonId == mammalSpeciesTaxonId);
			Assert.NotNull(secondSpecies);
			Assert.Equal(mammalSpecies.Category, secondSpecies.Category);
			Assert.Equal(mammalSpecies.Population, secondSpecies.Population);
			Assert.Equal(mammalSpecies.InfraName, secondSpecies.InfraName);
			Assert.Equal(mammalSpecies.InfraRank, secondSpecies.InfraRank);
			Assert.Equal(mammalSpecies.ScientificName, secondSpecies.ScientificName);
			Assert.Equal(mammalSpecies.GenusName, secondSpecies.GenusName);
			Assert.Equal(mammalSpecies.FamilyName, secondSpecies.FamilyName);
			Assert.Equal(mammalSpecies.OrderName, secondSpecies.OrderName);
			Assert.Equal(mammalSpecies.ClassName, secondSpecies.ClassName);
			Assert.Equal(mammalSpecies.PhylumName, secondSpecies.PhylumName);
			Assert.Equal(mammalSpecies.KingdomName, secondSpecies.KingdomName);
			Assert.Equal(mammalSpecies.TaxonId, secondSpecies.TaxonId);
			Assert.NotNull(secondSpecies.ConservationMeasures);
			Assert.True(secondSpecies.ConservationMeasures != string.Empty);
			var returnedSecondConservationMeasures = secondSpecies.ConservationMeasures.Split(',');
			Assert.NotNull(returnedSecondConservationMeasures);
			Assert.True(returnedSecondConservationMeasures.Count() == 2);
			Assert.Contains("Compliance and enforcement", returnedSecondConservationMeasures);
			Assert.Contains("National level", returnedSecondConservationMeasures);



			iUCNProxy.Verify(x => x.GetRegions(), Times.Once);
			iUCNProxy.Verify(x => x.GetSpeciesOfRegion(It.IsAny<Service.Proxy.DTO.GetSpeciesOfRegionRequest>()), Times.Once);
			Assert.NotNull(getSpeciesOfRegionRequest);
			Assert.Equal(0, getSpeciesOfRegionRequest.PageIndex);
			Assert.NotNull(getSpeciesOfRegionRequest.RegionIdentifier);
			Assert.Equal(region.Identifier, getSpeciesOfRegionRequest.RegionIdentifier);



			iUCNProxy.Verify(x => x.GetSpeciesConservationMeasures(It.Is<int>(c => c == speciesTaxonId)), Times.Once);
			iUCNProxy.Verify(x => x.GetSpeciesConservationMeasures(It.Is<int>(c => c == mammalSpeciesTaxonId)), Times.Once);
		}

		public static TheoryData<Service.Proxy.DTO.GetRegionResponse, string, BimspotTestErrorResult> GetInvalidRegions()
		{
			var result = new TheoryData<Service.Proxy.DTO.GetRegionResponse, string, BimspotTestErrorResult>();

			result.Add(null, "Result Is NULL", new BimspotTestErrorResult { Type = Enumeration.ErrorType.Regions_NullResponse, Message = "Regions Returns NULL" });
			result.Add(new Service.Proxy.DTO.GetRegionResponse(), "Without Pass Any Parameter", new BimspotTestErrorResult { Type = Enumeration.ErrorType.Regions_WithoutRegion, Message = "There Is No Item In Regions" });
			result.Add(new Service.Proxy.DTO.GetRegionResponse { Count = -1 }, "Count Is -1", new BimspotTestErrorResult { Type = Enumeration.ErrorType.Regions_WithoutRegion, Message = "There Is No Item In Regions" });
			result.Add(new Service.Proxy.DTO.GetRegionResponse { Count = 1, Results = null }, "Regions Are NULL", new BimspotTestErrorResult { Type = Enumeration.ErrorType.Regions_WithoutRegion, Message = "There Is No Item In Regions" });
			result.Add(new Service.Proxy.DTO.GetRegionResponse { Count = 1, Results = new List<Service.Proxy.DTO.Region>() }, "Regions Without Item", new BimspotTestErrorResult { Type = Enumeration.ErrorType.Regions_WithoutRegion, Message = "There Is No Item In Regions" });
			result.Add(new Service.Proxy.DTO.GetRegionResponse { Count = 1, Results = new List<Service.Proxy.DTO.Region> { null } }, "Regions Only Has One Null", new BimspotTestErrorResult { Type = Enumeration.ErrorType.Regions_WithoutRegion, Message = "There Is No Item In Regions" });

			return result;
		}

		public static TheoryData<Service.Proxy.DTO.GetSpeciesOfRegionResponse, string> GetInvalidSpecies()
		{
			var result = new TheoryData<Service.Proxy.DTO.GetSpeciesOfRegionResponse, string>();

			result.Add(null, "Response Is Null");
			result.Add(new Service.Proxy.DTO.GetSpeciesOfRegionResponse { Result = null }, "Species Is Null");
			result.Add(new Service.Proxy.DTO.GetSpeciesOfRegionResponse { Result = new List<Service.Proxy.DTO.Species> { null } }, "Species Has Only Null Item");

			return result;
		}
	}
}
