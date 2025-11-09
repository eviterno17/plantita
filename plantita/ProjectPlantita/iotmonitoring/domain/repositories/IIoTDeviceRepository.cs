using plantita.ProjectPlantita.iotmonitoring.domain.model.aggregates;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace plantita.ProjectPlantita.iotmonitoring.domain.repositories
{
    public interface IIoTDeviceRepository
    {
        Task<IEnumerable<IoTDevice>> ListAsync();
        Task<IEnumerable<IoTDevice>> GetAllUsersDevicesAsync(Guid userId);

        Task<IoTDevice> FindByIdAsync(Guid id);
        Task AddAsync(IoTDevice device);
        void Update(IoTDevice device);
        void Remove(IoTDevice device);
    }
}
