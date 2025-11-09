using plantita.ProjectPlantita.iotmonitoring.domain.model.Entities;
using plantita.ProjectPlantita.iotmonitoring.domain.repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using plantita.Shared.Infraestructure.Persistences.EFC.Configuration;

namespace plantita.ProjectPlantita.iotmonitoring.Application.Internal.Services
{
    public class SensorService : ISensorService
    {
        private readonly ISensorRepository _repository;
        private readonly AppDbContext _context;
        public SensorService(ISensorRepository repository, AppDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IEnumerable<Sensor>> ListAsync()
        {
            return await _repository.ListAsync();
        }

        public async Task<Sensor> GetByIdAsync(Guid id)
        {
            return await _repository.FindByIdAsync(id);
        }
        
        public async Task<IEnumerable<Sensor>> GetAllByDeviceIdAsync(Guid deviceId)
        {
            return await _repository.ListByDeviceIdAsync(deviceId);
        }
        
        public async Task<Sensor> CreateAsync(Sensor sensor)
        {
            await _repository.AddAsync(sensor);
            await _context.SaveChangesAsync();
            return sensor;
        }

        public async Task<Sensor> UpdateAsync(Guid id, Sensor sensor)
        {
            var existing = await _repository.FindByIdAsync(id);
            if (existing == null) return null;
            existing.DeviceId = sensor.DeviceId;
            existing.SensorType = sensor.SensorType;
            existing.Unit = sensor.Unit;
            existing.RangeMin = sensor.RangeMin;
            existing.RangeMax = sensor.RangeMax;
            existing.Model = sensor.Model;
            existing.InstalledAt = sensor.InstalledAt;
            existing.IsActive = sensor.IsActive;
            _repository.Update(existing);
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task DeleteAsync(Guid id)
        {
            var existing = await _repository.FindByIdAsync(id);
            if (existing == null) return;
            _repository.Remove(existing);
            await _context.SaveChangesAsync();
        }
    }
}
