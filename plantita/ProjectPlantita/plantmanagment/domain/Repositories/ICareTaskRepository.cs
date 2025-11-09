using plantita.ProjectPlantita.plantmanagment.domain.model.Entities;
using plantita.Shared.Domain.Repositories;

namespace plantita.ProjectPlantita.plantmanagment.domain.Repositories;

public interface ICareTaskRepository : IBaseRepository<CareTask>
{
    Task<List<CareTask>> GetByMyPlantIdAsync(Guid myPlantId);
    Task<List<CareTask>> GetPendingTasksAsync(Guid myPlantId);
}