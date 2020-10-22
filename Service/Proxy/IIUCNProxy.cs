using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BimspotTest.Service.Proxy
{
	public interface IIUCNProxy
	{
		Task<DTO.GetRegionResponse> GetRegions();
		Task<DTO.GetSpeciesOfRegionResponse> GetSpeciesOfRegion(DTO.GetSpeciesOfRegionRequest request);
		Task<DTO.GetSpeciesConservationMeasureseResponse> GetSpeciesConservationMeasures(int speciesIdentifier);
	}
}
