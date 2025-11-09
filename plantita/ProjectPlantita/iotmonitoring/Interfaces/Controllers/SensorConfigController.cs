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
    public class SensorConfigController : ControllerBase
    {
        private readonly ISensorConfigService _sensorConfigService;
        public SensorConfigController(ISensorConfigService sensorConfigService)
        {
            _sensorConfigService = sensorConfigService;
        }

        [HttpGet]
        public async Task<IEnumerable<SensorConfigResource>> GetAll()
        {
            var configs = await _sensorConfigService.ListAsync();
            var resources = new List<SensorConfigResource>();
            foreach (var c in configs)
            {
                resources.Add(new SensorConfigResource { ConfigId = c.ConfigId, SensorId = c.SensorId, ThresholdMin = c.ThresholdMin, ThresholdMax = c.ThresholdMax, FrequencyMinutes = c.FrequencyMinutes, AutoNotify = c.AutoNotify, ConfiguredAt = c.ConfiguredAt });
            }
            return resources;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SensorConfigResource>> GetById(Guid id)
        {
            var config = await _sensorConfigService.GetByIdAsync(id);
            if (config == null) return NotFound();
            return new SensorConfigResource { ConfigId = config.ConfigId, SensorId = config.SensorId, ThresholdMin = config.ThresholdMin, ThresholdMax = config.ThresholdMax, FrequencyMinutes = config.FrequencyMinutes, AutoNotify = config.AutoNotify, ConfiguredAt = config.ConfiguredAt };
        }

        [HttpPost]
        public async Task<ActionResult<SensorConfigResource>> Create([FromBody] SaveSensorConfigResource resource)
        {
            var config = new SensorConfig { SensorId = resource.SensorId, ThresholdMin = resource.ThresholdMin, ThresholdMax = resource.ThresholdMax, FrequencyMinutes = resource.FrequencyMinutes, AutoNotify = resource.AutoNotify, ConfiguredAt = resource.ConfiguredAt };
            var result = await _sensorConfigService.CreateAsync(config);
            return CreatedAtAction(nameof(GetById), new { id = result.ConfigId }, new SensorConfigResource { ConfigId = result.ConfigId, SensorId = result.SensorId, ThresholdMin = result.ThresholdMin, ThresholdMax = result.ThresholdMax, FrequencyMinutes = result.FrequencyMinutes, AutoNotify = result.AutoNotify, ConfiguredAt = result.ConfiguredAt });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<SensorConfigResource>> Update(Guid id, [FromBody] SaveSensorConfigResource resource)
        {
            var config = new SensorConfig { SensorId = resource.SensorId, ThresholdMin = resource.ThresholdMin, ThresholdMax = resource.ThresholdMax, FrequencyMinutes = resource.FrequencyMinutes, AutoNotify = resource.AutoNotify, ConfiguredAt = resource.ConfiguredAt };
            var result = await _sensorConfigService.UpdateAsync(id, config);
            if (result == null) return NotFound();
            return new SensorConfigResource { ConfigId = result.ConfigId, SensorId = result.SensorId, ThresholdMin = result.ThresholdMin, ThresholdMax = result.ThresholdMax, FrequencyMinutes = result.FrequencyMinutes, AutoNotify = result.AutoNotify, ConfiguredAt = result.ConfiguredAt };
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _sensorConfigService.DeleteAsync(id);
            return NoContent();
        }
    }
}
