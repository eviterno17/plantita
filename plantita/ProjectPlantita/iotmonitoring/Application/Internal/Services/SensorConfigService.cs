using plantita.ProjectPlantita.iotmonitoring.domain.model.Entities;
using plantita.ProjectPlantita.iotmonitoring.domain.repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using plantita.Shared.Infraestructure.Persistences.EFC.Configuration;

namespace plantita.ProjectPlantita.iotmonitoring.Application.Internal.Services
{
    public class SensorConfigService : ISensorConfigService
    {
        private readonly ISensorConfigRepository _repository;
        private readonly AppDbContext _context;
        public SensorConfigService(ISensorConfigRepository repository, AppDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IEnumerable<SensorConfig>> ListAsync()
        {
            return await _repository.ListAsync();
        }

        public async Task<SensorConfig> GetByIdAsync(Guid id)
        {
            return await _repository.FindByIdAsync(id);
        }

        public async Task<SensorConfig> CreateAsync(SensorConfig config)
        {
            await _repository.AddAsync(config);
            await _context.SaveChangesAsync();
            return config;
        }

        public async Task<SensorConfig> UpdateAsync(Guid id, SensorConfig config)
        {
            var existing = await _repository.FindByIdAsync(id);
            if (existing == null) return null;
            existing.ThresholdMin = config.ThresholdMin;
            existing.ThresholdMax = config.ThresholdMax;
            existing.FrequencyMinutes = config.FrequencyMinutes;
            existing.AutoNotify = config.AutoNotify;
            existing.ConfiguredAt = config.ConfiguredAt;
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
