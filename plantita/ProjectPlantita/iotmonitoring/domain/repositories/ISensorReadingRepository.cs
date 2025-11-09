using plantita.ProjectPlantita.iotmonitoring.domain.model.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace plantita.ProjectPlantita.iotmonitoring.domain.repositories
{
    public interface ISensorReadingRepository
    {
        Task<IEnumerable<SensorReading>> ListAsync();
        Task<SensorReading> FindByIdAsync(Guid id);
        Task<IEnumerable<SensorReading>> GetBySensorIdAsync(Guid id);

        Task AddAsync(SensorReading reading);
        void Update(SensorReading reading);
        void Remove(SensorReading reading);
    }
}
