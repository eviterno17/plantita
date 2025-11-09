using Microsoft.EntityFrameworkCore;
using plantita.ProjectPlantita.plantmanagment.domain.model.Entities;
using plantita.ProjectPlantita.plantmanagment.domain.Repositories;
using plantita.Shared.Infraestructure.Persistences.EFC.Configuration;

namespace plantita.ProjectPlantita.plantmanagment.Infraestructure.EFC.Repositories;

public class PlantHealthLogRepository : IPlantHealthLogRepository
{
    private readonly AppDbContext _context;

    public PlantHealthLogRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(PlantHealthLog entity) => await _context.PlantHealthLogs.AddAsync(entity);

    public async Task AddSync(PlantHealthLog entity)
    {
        _context.PlantHealthLogs.Add(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await FindByIdAsync(id);
        if (entity != null) _context.PlantHealthLogs.Remove(entity);
    }

    public async Task<PlantHealthLog?> FindByIdAsync(int id)
        => await _context.PlantHealthLogs.FirstOrDefaultAsync(h => h.HealthLogId.ToString().EndsWith(id.ToString()));

    public async Task<PlantHealthLog?> FindByIdGuidAsync(Guid id)
        => await _context.PlantHealthLogs.FindAsync(id);

    public async Task<IEnumerable<PlantHealthLog>> ListAsync()
        => await _context.PlantHealthLogs.ToListAsync();

    public void Remove(PlantHealthLog entity) => _context.PlantHealthLogs.Remove(entity);

    public void Update(PlantHealthLog entity) => _context.PlantHealthLogs.Update(entity);

    public async Task UpdateAsync(PlantHealthLog entity)
    {
        _context.PlantHealthLogs.Update(entity);
        await Task.CompletedTask;
    }

    public async Task<List<PlantHealthLog>> GetByMyPlantIdAsync(Guid myPlantId)
        => await _context.PlantHealthLogs.Where(h => h.MyPlantId == myPlantId).ToListAsync();
}