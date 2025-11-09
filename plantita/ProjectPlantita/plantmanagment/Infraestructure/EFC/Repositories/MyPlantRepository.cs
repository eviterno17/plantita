using Microsoft.EntityFrameworkCore;
using plantita.ProjectPlantita.plantmanagment.domain.model.aggregates;
using plantita.ProjectPlantita.plantmanagment.domain.Repositories;
using plantita.Shared.Infraestructure.Persistences.EFC.Configuration;

namespace plantita.ProjectPlantita.plantmanagment.Infraestructure.EFC.Repositories;

public class MyPlantRepository : IMyPlantRepository
{
    private readonly AppDbContext _context;

    public MyPlantRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(MyPlant entity) => await _context.MyPlants.AddAsync(entity);
    public async Task AddSync(MyPlant entity)
    {
        _context.MyPlants.Add(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(int id)
    {
        var plant = await FindByIdAsync(id);
        if (plant != null) _context.MyPlants.Remove(plant);
    }

    public async Task Delete(MyPlant myPlant)
    {
        _context.MyPlants.Remove(myPlant);
        await Task.CompletedTask;
    }

    public async Task<MyPlant?> FindByIdAsync(int id)
        => await _context.MyPlants.FirstOrDefaultAsync(p => p.MyPlantId.ToString().EndsWith(id.ToString()));

    public async Task<MyPlant?> FindByIdGuidAsync(Guid id)
        => await _context.MyPlants.FindAsync(id);

    public async Task<IEnumerable<MyPlant>> ListAsync()
        => await _context.MyPlants.ToListAsync();

    public void Remove(MyPlant entity) => _context.MyPlants.Remove(entity);

    public void Update(MyPlant entity) => _context.MyPlants.Update(entity);

    public async Task UpdateAsync(MyPlant entity)
    {
        _context.MyPlants.Update(entity);
        await Task.CompletedTask;
    }

    public async Task<List<MyPlant>> GetByUserIdAsync(Guid userId)
        => await _context.MyPlants.Where(p => p.UserId == userId).ToListAsync();
}