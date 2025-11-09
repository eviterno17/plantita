using Microsoft.EntityFrameworkCore;
using plantita.ProjectPlantita.plantmanagment.domain.model.Entities;
using plantita.ProjectPlantita.plantmanagment.domain.Repositories;
using plantita.Shared.Infraestructure.Persistences.EFC.Configuration;

namespace plantita.ProjectPlantita.plantmanagment.Infraestructure.EFC.Repositories;

public class CareTaskRepository : ICareTaskRepository
{
    private readonly AppDbContext _context;

    public CareTaskRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(CareTask entity) => await _context.CareTasks.AddAsync(entity);

    public async Task AddSync(CareTask entity)
    {
        _context.CareTasks.Add(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await FindByIdAsync(id);
        if (entity != null) _context.CareTasks.Remove(entity);
    }

    public async Task<CareTask?> FindByIdAsync(int id)
        => await _context.CareTasks.FirstOrDefaultAsync(t => t.TaskId.ToString().EndsWith(id.ToString()));

    public async Task<CareTask?> FindByIdGuidAsync(Guid id)
        => await _context.CareTasks.FindAsync(id);

    public async Task<IEnumerable<CareTask>> ListAsync()
        => await _context.CareTasks.ToListAsync();

    public void Remove(CareTask entity) => _context.CareTasks.Remove(entity);

    public void Update(CareTask entity) => _context.CareTasks.Update(entity);

    public async Task UpdateAsync(CareTask entity)
    {
        _context.CareTasks.Update(entity);
        await Task.CompletedTask;
    }

    public async Task<List<CareTask>> GetByMyPlantIdAsync(Guid myPlantId)
        => await _context.CareTasks.Where(t => t.MyPlantId == myPlantId).ToListAsync();

    public async Task<List<CareTask>> GetPendingTasksAsync(Guid myPlantId)
        => await _context.CareTasks
            .Where(t => t.MyPlantId == myPlantId && t.Status.ToLower() == "pending")
            .ToListAsync();
}