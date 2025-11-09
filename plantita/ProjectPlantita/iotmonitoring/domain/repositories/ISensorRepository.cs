using plantita.ProjectPlantita.iotmonitoring.domain.model.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace plantita.ProjectPlantita.iotmonitoring.domain.repositories
{
    public interface ISensorRepository
    {
        Task<IEnumerable<Sensor>> ListAsync();
        Task<IEnumerable<Sensor>> ListByDeviceIdAsync(Guid deviceId);

        Task<Sensor> FindByIdAsync(Guid id);
        Task AddAsync(Sensor sensor);
        void Update(Sensor sensor);
        void Remove(Sensor sensor);
    }
}
