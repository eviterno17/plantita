using Microsoft.EntityFrameworkCore;
using plantita.ProjectPlantita.iotmonitoring.domain.model.Entities;
using plantita.ProjectPlantita.iotmonitoring.domain.repositories;
using plantita.Shared.Infraestructure.Persistences.EFC.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace plantita.ProjectPlantita.iotmonitoring.Infraestructure.Persistence
{
    public class SensorReadingRepository : ISensorReadingRepository
    {
        private readonly AppDbContext _context;
        public SensorReadingRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SensorReading>> ListAsync()
        {
            return await _context.SensorReadings.ToListAsync();
        }

        public async Task<SensorReading> FindByIdAsync(Guid id)
        {
            return await _context.SensorReadings.FindAsync(id);
        }
        public async Task<IEnumerable<SensorReading>> GetBySensorIdAsync(Guid sensorId)
        {
            return await _context.SensorReadings
                .Where(s => s.SensorId == sensorId)
                .ToListAsync();
        }
        public async Task<SensorReading> FindBySensorIdAsync(Guid id)
        {
            return await _context.SensorReadings.FindAsync(id);
        }

        public async Task AddAsync(SensorReading reading)
        {
            await _context.SensorReadings.AddAsync(reading);
        }

        public void Update(SensorReading reading)
        {
            _context.SensorReadings.Update(reading);
        }

        public void Remove(SensorReading reading)
        {
            _context.SensorReadings.Remove(reading);
        }
    }
}
