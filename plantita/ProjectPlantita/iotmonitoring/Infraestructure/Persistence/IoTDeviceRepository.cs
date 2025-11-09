using Microsoft.EntityFrameworkCore;
using plantita.ProjectPlantita.iotmonitoring.domain.model.aggregates;
using plantita.ProjectPlantita.iotmonitoring.domain.repositories;
using plantita.Shared.Infraestructure.Persistences.EFC.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace plantita.ProjectPlantita.iotmonitoring.Infraestructure.Persistence
{
    public class IoTDeviceRepository : IIoTDeviceRepository
    {
        private readonly AppDbContext _context;
        public IoTDeviceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<IoTDevice>> ListAsync()
        {
            return await _context.IoTDevices.ToListAsync();
        }
        
        public async Task<IEnumerable<IoTDevice>> GetAllUsersDevicesAsync(Guid userId)
        {
            return await _context.IoTDevices
                .Where(d => d.AuthUserId == userId)
                .Include(d => d.Sensors)         // opcional: trae sensores
                .Include(d => d.MyPlant)         // opcional: trae planta asignada
                .ToListAsync();
        }


        public async Task<IoTDevice> FindByIdAsync(Guid id)
        {
            return await _context.IoTDevices.FindAsync(id);
        }

        public async Task AddAsync(IoTDevice device)
        {
            await _context.IoTDevices.AddAsync(device);
        }

        public void Update(IoTDevice device)
        {
            _context.IoTDevices.Update(device);
        }

        public void Remove(IoTDevice device)
        {
            _context.IoTDevices.Remove(device);
        }
    }
}
