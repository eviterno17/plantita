using plantita.ProjectPlantita.iotmonitoring.domain.model.aggregates;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace plantita.ProjectPlantita.iotmonitoring.Application.Internal.Services
{
    public interface IIoTDeviceService
    {
        Task<IEnumerable<IoTDevice>> ListAsync();
        Task<IoTDevice> GetByIdAsync(Guid id);
        Task<IEnumerable<IoTDevice>> GetAllUsersDevicesAsync(Guid userId);
        Task<IoTDevice> CreateAsync(IoTDevice device);
        Task<IoTDevice> UpdateAsync(Guid id, IoTDevice device);
        Task DeleteAsync(Guid id);
    }
}
