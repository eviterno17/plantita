using plantita.ProjectPlantita.plantmanagment.domain.model.Entities;
using plantita.Shared.Domain.Repositories;

namespace plantita.ProjectPlantita.plantmanagment.domain.Repositories;


public interface IPlantHealthLogRepository : IBaseRepository<PlantHealthLog>
{
    Task<List<PlantHealthLog>> GetByMyPlantIdAsync(Guid myPlantId);
}