using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace BimspotTest.Service
{
	internal class SpeciesService : ISpeciesService
	{
		private Proxy.IIUCNProxy iUCNProxy;
		public SpeciesService(Proxy.IIUCNProxy iUCNProxy)
		{
			this.iUCNProxy = iUCNProxy;
		}

		#region Implementaion Of SpeciesService

		public async Task<BimspotTestServiceResult<DTO.Species.GetSpeciesResponse>> GetCriticalEndangered()
		{
			throw new Exception("Not implemented yet!");
		}

		public async Task<BimspotTestServiceResult<DTO.Species.GetSpeciesResponse>> GetAllMammals()
		{
			throw new Exception("Not implemented yet!");

		}

		#endregion
	}
}
