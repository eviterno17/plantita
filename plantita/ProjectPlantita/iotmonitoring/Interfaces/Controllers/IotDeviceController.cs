using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using plantita.ProjectPlantita.iotmonitoring.Application.Internal.Services;
using plantita.ProjectPlantita.iotmonitoring.domain.model.aggregates;
using plantita.ProjectPlantita.iotmonitoring.domain.model.Entities;
using plantita.ProjectPlantita.iotmonitoring.Interfaces.Resources;
using plantita.Shared.Infraestructure.Persistences.EFC.Configuration;
using System.Security.Claims;

namespace plantita.ProjectPlantita.iotmonitoring.Interfaces.Controllers
{
    [ApiController]
    [Route("plantita/v1/[controller]")]
    public class IotDeviceController : ControllerBase
    {
        private readonly IIoTDeviceService _iotDeviceService;
        private readonly AppDbContext _context;

        public IotDeviceController(IIoTDeviceService iotDeviceService, AppDbContext context)
        {
            _iotDeviceService = iotDeviceService;
            _context = context;
        }

        // -------------------- CRUD Devices --------------------------

        [HttpGet]
        public async Task<IEnumerable<IoTDeviceResource>> GetAll()
        {
            var devices = await _iotDeviceService.ListAsync();
            return devices.Select(ToResource);
        }

        [HttpGet("me/me")]
        public async Task<IActionResult> GetAllDevicesByUser()
        {
            Console.WriteLine("🔥 GetAllDevicesByUser ejecutado");
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            var userId = Guid.Parse(userIdClaim);
            var myDevices = await _iotDeviceService.GetAllUsersDevicesAsync(userId);

            return Ok(myDevices.Select(ToResource));
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<IoTDeviceResource>> GetById(Guid id)
        {
            var device = await _iotDeviceService.GetByIdAsync(id);
            if (device == null) return NotFound();

            return ToResource(device);
        }

        [HttpPost]
        public async Task<ActionResult<IoTDeviceResource>> Create([FromBody] SaveIoTDeviceResource resource)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            var userId = Guid.Parse(userIdClaim);

            var device = new IoTDevice
            {
                AuthUserId = userId,
                MyPlantId = resource.MyPlantId,
                DeviceName = resource.DeviceName,
                ConnectionType = resource.ConnectionType,
                Location = resource.Location,
                ActivatedAt = resource.ActivatedAt,
                Status = resource.Status,
                FirmwareVersion = resource.FirmwareVersion
            };

            var result = await _iotDeviceService.CreateAsync(device);

            return CreatedAtAction(nameof(GetById), new { id = result.DeviceId }, ToResource(result));
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<IoTDeviceResource>> Update(Guid id, [FromBody] SaveIoTDeviceResource resource)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            var userId = Guid.Parse(userIdClaim);

            var device = new IoTDevice
            {
                AuthUserId = userId,
                MyPlantId = resource.MyPlantId,
                DeviceName = resource.DeviceName,
                ConnectionType = resource.ConnectionType,
                Location = resource.Location,
                ActivatedAt = resource.ActivatedAt,
                Status = resource.Status,
                FirmwareVersion = resource.FirmwareVersion
            };

            var result = await _iotDeviceService.UpdateAsync(id, device);

            if (result == null) return NotFound();

            return ToResource(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _iotDeviceService.DeleteAsync(id);
            return NoContent();
        }

        private IoTDeviceResource ToResource(IoTDevice entity)
        {
            return new IoTDeviceResource
            {
                DeviceId = entity.DeviceId,
                AuthUserId = entity.AuthUserId,
                MyPlantId = entity.MyPlantId,
                DeviceName = entity.DeviceName,
                ConnectionType = entity.ConnectionType,
                Location = entity.Location,
                ActivatedAt = entity.ActivatedAt,
                Status = entity.Status,
                FirmwareVersion = entity.FirmwareVersion
            };
        }

        // -------------------- EnvironmentData POST --------------------------

        [HttpPost("/api/v1/environment/data-records")]
        public async Task<IActionResult> CreateEnvironmentRecord([FromBody] EnvironmentDataRecordDto dto)
        {
            var device = await _context.IoTDevices
                .Include(d => d.Sensors)
                .FirstOrDefaultAsync(d => d.DeviceName == dto.CustomDeviceId);

            if (device == null)
                return NotFound("Device not found");

            var readings = new List<SensorReading>();

            if (dto.Light.HasValue)
                AddReadingIfSensorExists(device, "Light", dto.Light.Value, dto.CreatedAt, readings);

            if (dto.SoilMoisture.HasValue)
                AddReadingIfSensorExists(device, "SoilMoisture", dto.SoilMoisture.Value, dto.CreatedAt, readings);

            if (dto.AirTemperature.HasValue)
                AddReadingIfSensorExists(device, "AirTemperature", dto.AirTemperature.Value, dto.CreatedAt, readings);

            if (dto.AirHumidity.HasValue)
                AddReadingIfSensorExists(device, "AirHumidity", dto.AirHumidity.Value, dto.CreatedAt, readings);

            if (!readings.Any())
                return BadRequest("No valid sensor readings found.");

            await _context.SensorReadings.AddRangeAsync(readings);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Data saved", Count = readings.Count });
        }

        private void AddReadingIfSensorExists(IoTDevice device, string sensorType, decimal value, DateTime timestamp, List<SensorReading> readings)
        {
            var sensor = device.Sensors?.FirstOrDefault(s => s.SensorType == sensorType);
            if (sensor != null)
            {
                readings.Add(new SensorReading
                {
                    SensorId = sensor.SensorId,
                    Value = value,
                    Timestamp = timestamp
                });
            }
        }
    }
}
