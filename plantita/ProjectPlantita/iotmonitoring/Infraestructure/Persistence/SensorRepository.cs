using Microsoft.EntityFrameworkCore;
using plantita.ProjectPlantita.iotmonitoring.domain.model.Entities;
using plantita.ProjectPlantita.iotmonitoring.domain.repositories;
using plantita.Shared.Infraestructure.Persistences.EFC.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace plantita.ProjectPlantita.iotmonitoring.Infraestructure.Persistence
{
    public class SensorRepository : ISensorRepository
    {
        private readonly AppDbContext _context;
        public SensorRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Sensor>> ListAsync()
        {
            return await _context.Sensors.ToListAsync();
        }
        public async Task<IEnumerable<Sensor>> ListByDeviceIdAsync(Guid deviceId)
        {
            return await _context.Sensors
                .Where(s => s.DeviceId == deviceId)
                .ToListAsync();
        }

        public async Task<Sensor> FindByIdAsync(Guid id)
        {
            return await _context.Sensors.FindAsync(id);
        }

        public async Task AddAsync(Sensor sensor)
        {
            await _context.Sensors.AddAsync(sensor);
        }

        public void Update(Sensor sensor)
        {
            _context.Sensors.Update(sensor);
        }

        public void Remove(Sensor sensor)
        {
            _context.Sensors.Remove(sensor);
        }
    }
}
