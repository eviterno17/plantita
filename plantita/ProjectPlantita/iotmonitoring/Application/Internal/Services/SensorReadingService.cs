using plantita.ProjectPlantita.iotmonitoring.domain.model.Entities;
using plantita.ProjectPlantita.iotmonitoring.domain.repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using plantita.Shared.Infraestructure.Persistences.EFC.Configuration;

namespace plantita.ProjectPlantita.iotmonitoring.Application.Internal.Services
{
    public class SensorReadingService : ISensorReadingService
    {
        private readonly ISensorReadingRepository _repository;
        private readonly AppDbContext _context;
        public SensorReadingService(ISensorReadingRepository repository, AppDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IEnumerable<SensorReading>> ListAsync()
        {
            return await _repository.ListAsync();
        }

        public async Task<SensorReading> GetByIdAsync(Guid id)
        {
            return await _repository.FindByIdAsync(id);
        }
        
        public async Task<IEnumerable<SensorReading>> GetBySensorIdAsync(Guid id)
        {
            return await _repository.GetBySensorIdAsync(id);
        }


        public async Task<SensorReading> CreateAsync(SensorReading reading)
        {
            await _repository.AddAsync(reading);
            await _context.SaveChangesAsync();
            return reading;
        }

        public async Task<SensorReading> UpdateAsync(Guid id, SensorReading reading)
        {
            var existing = await _repository.FindByIdAsync(id);
            if (existing == null) return null;
            existing.SensorId = reading.SensorId;
            existing.Value = reading.Value;
            existing.Timestamp = reading.Timestamp;
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
