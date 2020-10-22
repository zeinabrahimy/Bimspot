using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace BimspotTest.Service.Proxy
{
	internal class IUCNProxy : IIUCNProxy
	{
		private readonly string token;
		private readonly string serverBaseAddress;
		private readonly string getRegionsRelativeUrl = "api/v3/region/list?token=";
		private readonly string getSpeciesOfRegionRelativeUrl = "api/v3/species/region/{0}/page/{1}?token={2}";
		private readonly string getSpeciesConservationMeasuresRelativeUrl = "api/v3/measures/species/id/{0}?token={1}";

		

		public IUCNProxy(IConfiguration configuration)
		{
			this.token = configuration.GetSection("IUCN").GetSection("Token").Value;
			serverBaseAddress = configuration.GetSection("IUCN").GetSection("BaseAddress").Value;
		}


		#region Implementation Of IIUCNProxy

		public async Task<DTO.GetRegionResponse> GetRegions()
		{
			using (var client = new HttpClient())
			{
				var response = await client.GetAsync($"{this.serverBaseAddress}{getRegionsRelativeUrl}{this.token}");
				if (response.StatusCode == System.Net.HttpStatusCode.OK)
					return await response.Content.ReadAsAsync<DTO.GetRegionResponse>();

				return null;
			}
		}

		public async Task<DTO.GetSpeciesOfRegionResponse> GetSpeciesOfRegion(DTO.GetSpeciesOfRegionRequest request)
		{
			using (var client = new HttpClient())
			{
				var response = await client.GetAsync($"{this.serverBaseAddress}{string.Format(getSpeciesOfRegionRelativeUrl, request.RegionIdentifier, request.PageIndex, this.token)}");

				if (response.StatusCode == System.Net.HttpStatusCode.OK)
					return await response.Content.ReadAsAsync<DTO.GetSpeciesOfRegionResponse>();

				return null;
			}
		}

		public async Task<DTO.GetSpeciesConservationMeasureseResponse> GetSpeciesConservationMeasures(int speciesIdentifier)
		{
			using (var client = new HttpClient())
			{
				var response = await client.GetAsync($"{this.serverBaseAddress}{string.Format(getSpeciesConservationMeasuresRelativeUrl, speciesIdentifier, this.token)}");

				if (response.StatusCode == System.Net.HttpStatusCode.OK)
					return await response.Content.ReadAsAsync<DTO.GetSpeciesConservationMeasureseResponse>();

				return null;
			}

		}


		#endregion
	}
}
