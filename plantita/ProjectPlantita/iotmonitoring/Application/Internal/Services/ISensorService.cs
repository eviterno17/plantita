using plantita.ProjectPlantita.iotmonitoring.domain.model.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace plantita.ProjectPlantita.iotmonitoring.Application.Internal.Services
{
    public interface ISensorService
    {
        Task<IEnumerable<Sensor>> ListAsync();
        Task<Sensor> GetByIdAsync(Guid id);
        Task<IEnumerable<Sensor>> GetAllByDeviceIdAsync(Guid deviceId);
        Task<Sensor> CreateAsync(Sensor sensor);
        Task<Sensor> UpdateAsync(Guid id, Sensor sensor);
        Task DeleteAsync(Guid id);
    }
}
