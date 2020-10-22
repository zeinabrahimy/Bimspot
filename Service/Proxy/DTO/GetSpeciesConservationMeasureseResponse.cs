using System.Collections.Generic;

namespace BimspotTest.Service.Proxy.DTO
{
	public class GetSpeciesConservationMeasureseResponse
	{
		public int Id { get; set; }
		public IEnumerable<DTO.ConservationMeasure> ConservationMeasures { get; set; }
	}
}
