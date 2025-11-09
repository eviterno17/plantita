using plantita.ProjectPlantita.iotmonitoring.domain.model.aggregates;
using plantita.ProjectPlantita.iotmonitoring.domain.repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using plantita.Shared.Infraestructure.Persistences.EFC.Configuration;

namespace plantita.ProjectPlantita.iotmonitoring.Application.Internal.Services
{
    public class IoTDeviceService : IIoTDeviceService
    {
        private readonly IIoTDeviceRepository _repository;
        private readonly AppDbContext _context;
        public IoTDeviceService(IIoTDeviceRepository repository, AppDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IEnumerable<IoTDevice>> ListAsync()
        {
            return await _repository.ListAsync();
        }
        
        public async Task<IEnumerable<IoTDevice>> GetAllUsersDevicesAsync(Guid userId)
        {
            return await _repository.GetAllUsersDevicesAsync(userId);
        }

        public async Task<IoTDevice> GetByIdAsync(Guid id)
        {
            return await _repository.FindByIdAsync(id);
        }

        public async Task<IoTDevice> CreateAsync(IoTDevice device)
        {
            await _repository.AddAsync(device);
            await _context.SaveChangesAsync();
            return device;
        }

        public async Task<IoTDevice> UpdateAsync(Guid id, IoTDevice device)
        {
            var existing = await _repository.FindByIdAsync(id);
            if (existing == null) return null;
            existing.AuthUserId = device.AuthUserId;
            existing.DeviceName = device.DeviceName;
            existing.ConnectionType = device.ConnectionType;
            existing.Location = device.Location;
            existing.ActivatedAt = device.ActivatedAt;
            existing.Status = device.Status;
            existing.FirmwareVersion = device.FirmwareVersion;
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
