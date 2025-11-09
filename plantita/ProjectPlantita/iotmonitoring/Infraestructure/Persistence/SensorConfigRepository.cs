using Microsoft.EntityFrameworkCore;
using plantita.ProjectPlantita.iotmonitoring.domain.model.Entities;
using plantita.ProjectPlantita.iotmonitoring.domain.repositories;
using plantita.Shared.Infraestructure.Persistences.EFC.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace plantita.ProjectPlantita.iotmonitoring.Infraestructure.Persistence
{
    public class SensorConfigRepository : ISensorConfigRepository
    {
        private readonly AppDbContext _context;
        public SensorConfigRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SensorConfig>> ListAsync()
        {
            return await _context.SensorConfigs.ToListAsync();
        }

        public async Task<SensorConfig> FindByIdAsync(Guid id)
        {
            return await _context.SensorConfigs.FindAsync(id);
        }

        public async Task AddAsync(SensorConfig config)
        {
            await _context.SensorConfigs.AddAsync(config);
        }

        public void Update(SensorConfig config)
        {
            _context.SensorConfigs.Update(config);
        }

        public void Remove(SensorConfig config)
        {
            _context.SensorConfigs.Remove(config);
        }
    }
}
