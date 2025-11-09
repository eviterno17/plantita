using Microsoft.AspNetCore.Mvc;
using plantita.ProjectPlantita.iotmonitoring.Application.Internal.Services;
using plantita.ProjectPlantita.iotmonitoring.domain.model.Entities;
using plantita.ProjectPlantita.iotmonitoring.Interfaces.Resources;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace plantita.ProjectPlantita.iotmonitoring.Interfaces.Controllers
{
    [ApiController]
    [Route("plantita/v1/[controller]")]
    public class SensorController : ControllerBase
    {
        private readonly ISensorService _sensorService;
        public SensorController(ISensorService sensorService)
        {
            _sensorService = sensorService;
        }

        [HttpGet]
        public async Task<IEnumerable<SensorResource>> GetAll()
        {
            var sensors = await _sensorService.ListAsync();
            var resources = new List<SensorResource>();
            foreach (var s in sensors)
            {
                resources.Add(new SensorResource {
                    SensorId = s.SensorId,
                    DeviceId = s.DeviceId,
                    SensorType = s.SensorType,
                    Unit = s.Unit,
                    RangeMin = s.RangeMin,
                    RangeMax = s.RangeMax,
                    Model = s.Model,
                    InstalledAt = s.InstalledAt,
                    IsActive = s.IsActive
                });
            }
            return resources;
        }
        
        [HttpGet("device/{deviceId}")]
        public async Task<IEnumerable<SensorResource>> GetAllByDeviceId(Guid deviceId)
        {
            var sensors = await _sensorService.GetAllByDeviceIdAsync(deviceId);
            var resources = new List<SensorResource>();
            foreach (var s in sensors)
            {
                resources.Add(new SensorResource {
                    SensorId = s.SensorId,
                    DeviceId = s.DeviceId,
                    SensorType = s.SensorType,
                    Unit = s.Unit,
                    RangeMin = s.RangeMin,
                    RangeMax = s.RangeMax,
                    Model = s.Model,
                    InstalledAt = s.InstalledAt,
                    IsActive = s.IsActive
                });
            }
            return resources;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SensorResource>> GetById(Guid id)
        {
            var sensor = await _sensorService.GetByIdAsync(id);
            if (sensor == null) return NotFound();
            return new SensorResource {
                SensorId = sensor.SensorId,
                DeviceId = sensor.DeviceId,
                SensorType = sensor.SensorType,
                Unit = sensor.Unit,
                RangeMin = sensor.RangeMin,
                RangeMax = sensor.RangeMax,
                Model = sensor.Model,
                InstalledAt = sensor.InstalledAt,
                IsActive = sensor.IsActive
            };
        }
        private string InferUnit(string sensorType)
        {
            return sensorType switch
            {
                "Temperatura" => "°C",
                "Humedad" => "%",
                "Luminosidad" => "lux",
                "CO2" => "ppm",
                _ => "N/A"
            };
        }

        [HttpPost]
        public async Task<ActionResult<SensorResource>> Create([FromBody] SaveSensorResource resource)
        {
            
            var sensor = new Sensor {
                DeviceId = resource.DeviceId,
                SensorType = resource.SensorType,
                Unit = InferUnit(resource.SensorType),
                RangeMin = 0,
                RangeMax = 100,
                Model = "DefaultModel",
                InstalledAt = DateTime.UtcNow,
                IsActive = true
            };
            var result = await _sensorService.CreateAsync(sensor);
            return CreatedAtAction(nameof(GetById), new { id = result.SensorId }, new SensorResource {
                SensorId = result.SensorId,
                DeviceId = result.DeviceId,
                SensorType = result.SensorType,
                Unit = result.Unit,
                RangeMin = result.RangeMin,
                RangeMax = result.RangeMax,
                Model = result.Model,
                InstalledAt = result.InstalledAt,
                IsActive = result.IsActive
            });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<SensorResource>> Update(Guid id, [FromBody] SaveSensorResource resource)
        {
            var sensor = new Sensor {
                DeviceId = resource.DeviceId,
                SensorType = resource.SensorType,
                Unit = InferUnit(resource.SensorType),
                RangeMin = 0,
                RangeMax = 100,
                Model = "DefaultModel",
                InstalledAt = DateTime.UtcNow,
                IsActive = true
            };
            var result = await _sensorService.UpdateAsync(id, sensor);
            if (result == null) return NotFound();
            return new SensorResource {
                SensorId = result.SensorId,
                DeviceId = result.DeviceId,
                SensorType = result.SensorType,
                Unit = result.Unit,
                RangeMin = result.RangeMin,
                RangeMax = result.RangeMax,
                Model = result.Model,
                InstalledAt = result.InstalledAt,
                IsActive = result.IsActive
            };
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _sensorService.DeleteAsync(id);
            return NoContent();
        }
    }
}
