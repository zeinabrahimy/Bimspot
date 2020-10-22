using System.Collections.Generic;
using Newtonsoft.Json;

namespace BimspotTest.Service.Proxy.DTO
{
	public class GetSpeciesOfRegionResponse
	{
		public int Count { get; set; }

		[JsonProperty("region_identifier")]
		public string RegionIdentifier { get; set; }
		public int Page { get; set; } 
		public IEnumerable<DTO.Species> Result { get; set; }
	}
}
