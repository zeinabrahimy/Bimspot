using System.Collections.Generic;

namespace BimspotTest.Service.Proxy.DTO
{
	public class GetRegionResponse
	{
		public int Count { get; set; }
		public IEnumerable<DTO.Region> Results { get; set; }
	}
}
