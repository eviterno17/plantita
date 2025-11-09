using plantita.ProjectPlantita.plantmanagment.domain.model.aggregates;
using plantita.Shared.Domain.Repositories;

namespace plantita.ProjectPlantita.plantmanagment.domain.Repositories;


public interface IMyPlantRepository : IBaseRepository<MyPlant>
{
    Task<List<MyPlant>> GetByUserIdAsync(Guid userId);
    Task Delete(MyPlant myPlant);
}