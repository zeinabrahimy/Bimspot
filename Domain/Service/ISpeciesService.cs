using System.Threading.Tasks;

namespace BimspotTest.Service
{
	public interface ISpeciesService
	{
		Task<BimspotTestServiceResult<DTO.Species.GetSpeciesResponse>> GetCriticalEndangered();
		Task<BimspotTestServiceResult<DTO.Species.GetSpeciesResponse>> GetAllMammals();

	}
}
