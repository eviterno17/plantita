using plantita.ProjectPlantita.plantmanagment.domain.model.aggregates;
using plantita.ProjectPlantita.plantmanagment.domain.model.Entities;
using plantita.ProjectPlantita.plantmanagment.domain.Repositories;
using plantita.ProjectPlantita.plantmanagment.domain.Services;

namespace plantita.ProjectPlantita.plantmanagment.Application.Internal.QueryServices;

public class MyPlantQueryService : IMyPlantQueryService
{
    private readonly IMyPlantRepository _myPlantRepository;
    private readonly ICareTaskRepository _careTaskRepository;
    private readonly IPlantHealthLogRepository _healthLogRepository;

    public MyPlantQueryService(
        IMyPlantRepository myPlantRepository,
        ICareTaskRepository careTaskRepository,
        IPlantHealthLogRepository healthLogRepository)
    {
        _myPlantRepository = myPlantRepository;
        _careTaskRepository = careTaskRepository;
        _healthLogRepository = healthLogRepository;
    }

    public async Task<MyPlant?> GetByIdAsync(Guid myPlantId)
    {
        return await _myPlantRepository.FindByIdGuidAsync(myPlantId);
    }

    public async Task<IEnumerable<MyPlant>> GetAllByUserIdAsync(Guid userId)
    {
        return await _myPlantRepository.GetByUserIdAsync(userId);
    }

    public async Task<IEnumerable<CareTask>> GetCareTasksAsync(Guid myPlantId)
    {
        return await _careTaskRepository.GetByMyPlantIdAsync(myPlantId);
    }

    public async Task<CareTask?> GetCareTaskByIdAsync(Guid taskId)
    {
        return await _careTaskRepository.FindByIdGuidAsync(taskId);
    }

    public async Task<IEnumerable<PlantHealthLog>> GetHealthLogsAsync(Guid myPlantId)
    {
        return await _healthLogRepository.GetByMyPlantIdAsync(myPlantId);
    }
}