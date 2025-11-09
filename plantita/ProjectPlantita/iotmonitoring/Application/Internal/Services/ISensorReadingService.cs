using plantita.ProjectPlantita.iotmonitoring.domain.model.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace plantita.ProjectPlantita.iotmonitoring.Application.Internal.Services
{
    public interface ISensorReadingService
    {
        Task<IEnumerable<SensorReading>> ListAsync();
        Task<SensorReading> GetByIdAsync(Guid id);
        Task<IEnumerable<SensorReading>> GetBySensorIdAsync(Guid id);

        Task<SensorReading> CreateAsync(SensorReading reading);
        Task<SensorReading> UpdateAsync(Guid id, SensorReading reading);
        Task DeleteAsync(Guid id);
    }
}
