using plantita.ProjectPlantita.iotmonitoring.domain.model.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace plantita.ProjectPlantita.iotmonitoring.Application.Internal.Services
{
    public interface ISensorConfigService
    {
        Task<IEnumerable<SensorConfig>> ListAsync();
        Task<SensorConfig> GetByIdAsync(Guid id);
        Task<SensorConfig> CreateAsync(SensorConfig config);
        Task<SensorConfig> UpdateAsync(Guid id, SensorConfig config);
        Task DeleteAsync(Guid id);
    }
}
