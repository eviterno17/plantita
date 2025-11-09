using plantita.ProjectPlantita.iotmonitoring.domain.model.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace plantita.ProjectPlantita.iotmonitoring.domain.repositories
{
    public interface ISensorConfigRepository
    {
        Task<IEnumerable<SensorConfig>> ListAsync();
        Task<SensorConfig> FindByIdAsync(Guid id);
        Task AddAsync(SensorConfig config);
        void Update(SensorConfig config);
        void Remove(SensorConfig config);
    }
}
