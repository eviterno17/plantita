using Microsoft.EntityFrameworkCore;
using plantita.ProjectPlantita.plantmanagment.domain.model.aggregates.PlantID;
using plantita.ProjectPlantita.plantmanagment.domain.Repositories;
using plantita.Shared.Infraestructure.Persistences.EFC.Configuration;

namespace plantita.ProjectPlantita.plantmanagment.Infraestructure.EFC.Repositories;

public class PlantRepository : IPlantRepository
{
    private readonly AppDbContext _context;

    public PlantRepository(AppDbContext context)
    {
        _context = context;
    }

    // Implementación completa de IBaseRepository<Plant>

    public async Task AddAsync(Plant entity)
    {
        await _context.Plants.AddAsync(entity);
    }

    public async Task AddSync(Plant entity)
    {
        _context.Plants.Add(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await FindByIdAsync(id);
        if (entity != null)
        {
            _context.Plants.Remove(entity);
        }
    }

    public async Task<Plant?> FindByIdAsync(int id)
    {
        // Solo aplica si tienes algún ID entero mapeado.
        return await _context.Plants.FirstOrDefaultAsync(p => p.PlantId.ToString().EndsWith(id.ToString()));
    }

    public async Task<Plant?> FindByIdGuidAsync(Guid id)
    {
        return await _context.Plants.FindAsync(id);
    }

    public async Task<IEnumerable<Plant>> ListAsync()
    {
        return await _context.Plants.ToListAsync();
    }

    public void Remove(Plant entity)
    {
        _context.Plants.Remove(entity);
    }

    public void Update(Plant entity)
    {
        _context.Plants.Update(entity);
    }

    public async Task UpdateAsync(Plant entity)
    {
        _context.Plants.Update(entity);
        await Task.CompletedTask;
    }

    // Métodos personalizados de IPlantRepository

    public async Task<Plant?> GetByScientificNameAsync(string scientificName)
    {
        return await _context.Plants.FirstOrDefaultAsync(p => p.ScientificName == scientificName);
    }

    public async Task<Plant?> GetByCommonNameAsync(string commonName)
    {
        return await _context.Plants.FirstOrDefaultAsync(p => p.CommonName == commonName);
    }

    public async Task DeleteAsync(Plant plant)
    {
        _context.Plants.Remove(plant);
        await Task.CompletedTask;
    }

    public async Task<List<Plant>> GetAllPlantsAsync()
    {
        return await _context.Plants.ToListAsync();
    }
}