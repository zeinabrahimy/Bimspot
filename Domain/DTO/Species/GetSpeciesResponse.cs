using System.Collections.Generic;
namespace BimspotTest.DTO.Species
{
	public class GetSpeciesResponse
	{
		public string RegionIdentifier { get; set; }
		public IEnumerable<Species> Species { get; set; }
	}
}
