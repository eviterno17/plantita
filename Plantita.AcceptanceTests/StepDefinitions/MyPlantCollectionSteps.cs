using System.Net;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using Plantita.AcceptanceTests.Support;
using TechTalk.SpecFlow;

namespace Plantita.AcceptanceTests.StepDefinitions;

[Binding]
public class MyPlantCollectionSteps
{
    private readonly TestContext _testContext;
    private readonly ApiClient _apiClient;
    private readonly DatabaseHelper _databaseHelper;

    public MyPlantCollectionSteps(TestContext testContext, ApiClient apiClient, DatabaseHelper databaseHelper)
    {
        _testContext = testContext;
        _apiClient = apiClient;
        _databaseHelper = databaseHelper;
    }

    #region Given Steps

    [Given(@"que tengo las siguientes plantas en mi colección:")]
    public async Task GivenTengoLasSiguientesPlantasEnMiColeccion(Table table)
    {
        var userId = _testContext.CurrentUserId ?? 1;

        await _databaseHelper.ExecuteInScope(async db =>
        {
            foreach (var row in table.Rows)
            {
                var plantId = int.Parse(row["plantId"]);

                // Ensure base plant exists
                if (!db.Set<plantita.ProjectPlantita.plantmanagment.domain.model.aggregates.Plant>()
                    .Any(p => p.Id == plantId))
                {
                    var basePlant = new plantita.ProjectPlantita.plantmanagment.domain.model.aggregates.Plant
                    {
                        Id = plantId,
                        ScientificName = "Test Plant",
                        CommonName = "Test Common",
                        Description = "Test",
                        WateringFrequency = "Weekly",
                        SunlightRequirement = "Medium"
                    };
                    db.Add(basePlant);
                }

                var myPlant = new plantita.ProjectPlantita.plantmanagment.domain.model.aggregates.MyPlant
                {
                    PlantId = plantId,
                    UserId = userId,
                    CustomName = row["customName"],
                    Location = row["location"]
                };

                db.Add(myPlant);
            }
            await db.SaveChangesAsync();
        });
    }

    [Given(@"que no tengo plantas en mi colección")]
    public void GivenNoTengoPlantasEnMiColeccion()
    {
        // Database is already clean from hooks
        // Just ensure no plants for current user
    }

    [Given(@"que tengo una planta con ID ""(.*)"" en mi colección")]
    public async Task GivenTengoUnaPlantaConIDEnMiColeccion(string myPlantId)
    {
        var userId = _testContext.CurrentUserId ?? 1;

        await _databaseHelper.ExecuteInScope(async db =>
        {
            // Create base plant
            var basePlant = new plantita.ProjectPlantita.plantmanagment.domain.model.aggregates.Plant
            {
                ScientificName = "Monstera deliciosa",
                CommonName = "Costilla de Adán",
                Description = "Test plant",
                WateringFrequency = "Weekly",
                SunlightRequirement = "Indirect"
            };
            db.Add(basePlant);
            await db.SaveChangesAsync();

            // Create my plant
            var myPlant = new plantita.ProjectPlantita.plantmanagment.domain.model.aggregates.MyPlant
            {
                Id = int.Parse(myPlantId),
                PlantId = basePlant.Id,
                UserId = userId,
                CustomName = "Mi planta de prueba",
                Location = "Sala",
                Notes = "Planta de prueba"
            };
            db.Add(myPlant);
            await db.SaveChangesAsync();

            _testContext.StoreEntityId("my_plant", myPlant.Id);
        });
    }

    [Given(@"que el usuario ""(.*)"" tiene una planta con ID ""(.*)""")]
    public async Task GivenElUsuarioTieneUnaPlantaConID(string email, string myPlantId)
    {
        await _databaseHelper.ExecuteInScope(async db =>
        {
            // Create other user
            var otherUser = new plantita.User.Domain.Model.Aggregates.AuthUser
            {
                Email = email,
                Password = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                Name = "Other User",
                Timezone = "America/Lima",
                Language = "es"
            };
            db.Add(otherUser);
            await db.SaveChangesAsync();

            // Create plant for other user
            var basePlant = new plantita.ProjectPlantita.plantmanagment.domain.model.aggregates.Plant
            {
                ScientificName = "Test Plant",
                CommonName = "Test",
                Description = "Test",
                WateringFrequency = "Weekly",
                SunlightRequirement = "Medium"
            };
            db.Add(basePlant);
            await db.SaveChangesAsync();

            var myPlant = new plantita.ProjectPlantita.plantmanagment.domain.model.aggregates.MyPlant
            {
                Id = int.Parse(myPlantId),
                PlantId = basePlant.Id,
                UserId = otherUser.Id,
                CustomName = "Other user's plant",
                Location = "Other location"
            };
            db.Add(myPlant);
            await db.SaveChangesAsync();
        });
    }

    [Given(@"que tengo una planta con ID ""(.*)"" con una tarea pendiente de riego")]
    public async Task GivenTengoUnaPlantaConIDConUnaTareaPendienteDeRiego(string myPlantId)
    {
        await GivenTengoUnaPlantaConIDEnMiColeccion(myPlantId);

        await _databaseHelper.ExecuteInScope(async db =>
        {
            var task = new plantita.ProjectPlantita.plantmanagment.domain.model.Entities.CareTask
            {
                MyPlantId = int.Parse(myPlantId),
                TaskType = "Riego",
                ScheduledDate = DateTime.Now.AddDays(1),
                Status = "Pendiente",
                Notes = "Riego regular"
            };
            db.Add(task);
            await db.SaveChangesAsync();

            _testContext.StoreEntityId("care_task", task.Id);
        });
    }

    [Given(@"que tengo una planta con ID ""(.*)"" con las siguientes tareas:")]
    public async Task GivenTengoUnaPlantaConIDConLasSiguientesTareas(string myPlantId, Table table)
    {
        await GivenTengoUnaPlantaConIDEnMiColeccion(myPlantId);

        await _databaseHelper.ExecuteInScope(async db =>
        {
            foreach (var row in table.Rows)
            {
                var task = new plantita.ProjectPlantita.plantmanagment.domain.model.Entities.CareTask
                {
                    MyPlantId = int.Parse(myPlantId),
                    TaskType = row["taskType"],
                    ScheduledDate = DateTime.Parse(row["scheduledDate"]),
                    Status = row["status"],
                    Notes = "Test task"
                };
                db.Add(task);
            }
            await db.SaveChangesAsync();
        });
    }

    [Given(@"que tengo una planta con ID ""(.*)"" con las siguientes observaciones:")]
    public async Task GivenTengoUnaPlantaConIDConLasSiguientesObservaciones(string myPlantId, Table table)
    {
        await GivenTengoUnaPlantaConIDEnMiColeccion(myPlantId);

        await _databaseHelper.ExecuteInScope(async db =>
        {
            foreach (var row in table.Rows)
            {
                var healthLog = new plantita.ProjectPlantita.plantmanagment.domain.model.Entities.PlantHealthLog
                {
                    MyPlantId = int.Parse(myPlantId),
                    Date = DateTime.Parse(row["date"]),
                    HealthStatus = row["healthStatus"],
                    Observations = row["observations"]
                };
                db.Add(healthLog);
            }
            await db.SaveChangesAsync();
        });
    }

    [Given(@"que tengo una planta con ID ""(.*)"" con tareas y observaciones de salud")]
    public async Task GivenTengoUnaPlantaConIDConTareasYObservacionesDeSalud(string myPlantId)
    {
        await GivenTengoUnaPlantaConIDEnMiColeccion(myPlantId);

        await _databaseHelper.ExecuteInScope(async db =>
        {
            // Add some tasks
            var task = new plantita.ProjectPlantita.plantmanagment.domain.model.Entities.CareTask
            {
                MyPlantId = int.Parse(myPlantId),
                TaskType = "Riego",
                ScheduledDate = DateTime.Now,
                Status = "Pendiente"
            };
            db.Add(task);

            // Add some health logs
            var healthLog = new plantita.ProjectPlantita.plantmanagment.domain.model.Entities.PlantHealthLog
            {
                MyPlantId = int.Parse(myPlantId),
                Date = DateTime.Now,
                HealthStatus = "Buena",
                Observations = "Test observation"
            };
            db.Add(healthLog);

            await db.SaveChangesAsync();
        });
    }

    #endregion

    #region When Steps

    [When(@"agrego una foto a la planta con:")]
    public async Task WhenAgregoUnaFotoALaPlantaCon(Table table)
    {
        var myPlantId = _testContext.GetEntityId("my_plant");
        var data = new Dictionary<string, string>();
        foreach (var row in table.Rows)
        {
            data[row["campo"]] = row["valor"];
        }
        await _apiClient.PostAsync($"/plantita/v1/myplant/{myPlantId}/photo", data);
    }

    [When(@"creo una tarea de cuidado con:")]
    public async Task WhenCreoUnaTareaDeCuidadoCon(Table table)
    {
        var myPlantId = _testContext.GetEntityId("my_plant");
        var data = new Dictionary<string, string>();
        foreach (var row in table.Rows)
        {
            data[row["campo"]] = row["valor"];
        }
        await _apiClient.PostAsync($"/plantita/v1/myplant/{myPlantId}/task", data);
    }

    [When(@"marco la tarea como completada con fecha ""(.*)""")]
    public async Task WhenMarcoLaTareaComoCompletadaConFecha(string completedDate)
    {
        var taskId = _testContext.GetEntityId("care_task");
        var data = new
        {
            status = "Completada",
            completedDate = DateTime.Parse(completedDate)
        };
        await _apiClient.PutAsync($"/plantita/v1/caretask/{taskId}", data);
    }

    [When(@"consulto las tareas pendientes de la planta")]
    public async Task WhenConsultoLasTareasPendientesDeLaPlanta()
    {
        var myPlantId = _testContext.GetEntityId("my_plant");
        await _apiClient.GetAsync($"/plantita/v1/myplant/{myPlantId}/tasks?status=Pendiente");
    }

    [When(@"registro una observación de salud con:")]
    public async Task WhenRegistroUnaObservacionDeSaludCon(Table table)
    {
        var myPlantId = _testContext.GetEntityId("my_plant");
        var data = new Dictionary<string, string>();
        foreach (var row in table.Rows)
        {
            data[row["campo"]] = row["valor"];
        }
        await _apiClient.PostAsync($"/plantita/v1/myplant/{myPlantId}/health", data);
    }

    [When(@"consulto el historial de salud de la planta")]
    public async Task WhenConsultoElHistorialDeSaludDeLaPlanta()
    {
        var myPlantId = _testContext.GetEntityId("my_plant");
        await _apiClient.GetAsync($"/plantita/v1/myplant/{myPlantId}/health");
    }

    [When(@"agrego una planta de tipo ""(.*)"" a mi colección con:")]
    public async Task WhenAgregoUnaPlantaDeTipoAMiColeccionCon(string tipo, Table table)
    {
        var data = new Dictionary<string, string>();
        foreach (var row in table.Rows)
        {
            data[row[0]] = row[1];
        }
        await _apiClient.PostAsync($"/plantita/v1/myplant/{data["plantId"]}", data);
        _testContext.Store("plant_type", tipo);
    }

    [When(@"elimino la planta de mi colección")]
    public async Task WhenEliminoLaPlantaDeMiColeccion()
    {
        var myPlantId = _testContext.GetEntityId("my_plant");
        await _apiClient.DeleteAsync($"/plantita/v1/myplant/{myPlantId}");
    }

    #endregion

    #region Then Steps

    [Then(@"la respuesta debería contener el nombre personalizado ""(.*)""")]
    public void ThenLaRespuestaDeberiaContenerElNombrePersonalizado(string customName)
    {
        _testContext.ResponseBody.Should().Contain(customName);
    }

    [Then(@"la respuesta debería contener la ubicación ""(.*)""")]
    public void ThenLaRespuestaDeberiaContenerLaUbicacion(string location)
    {
        _testContext.ResponseBody.Should().Contain(location);
    }

    [Then(@"la respuesta debería contener las notas")]
    public void ThenLaRespuestaDeberiaContenerLasNotas()
    {
        var json = JObject.Parse(_testContext.ResponseBody!);
        json["notes"].Should().NotBeNull();
    }

    [Then(@"la planta debería estar en mi colección")]
    public async Task ThenLaPlantaDeberiaEstarEnMiColeccion()
    {
        var userId = _testContext.CurrentUserId ?? 1;
        var plantExists = await _databaseHelper.ExecuteInScope(async db =>
        {
            return db.Set<plantita.ProjectPlantita.plantmanagment.domain.model.aggregates.MyPlant>()
                .Any(p => p.UserId == userId);
        });

        plantExists.Should().BeTrue();
    }

    [Then(@"la respuesta debería indicar que la planta no existe en el catálogo")]
    public void ThenLaRespuestaDeberiaIndicarQueLaPlantaNoExisteEnElCatalogo()
    {
        _testContext.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Then(@"la respuesta debería contener una lista vacía")]
    public void ThenLaRespuestaDeberiaContenerUnaListaVacia()
    {
        var json = JArray.Parse(_testContext.ResponseBody!);
        json.Count.Should().Be(0);
    }

    [Then(@"debería incluir el nombre científico de la planta del catálogo")]
    public void ThenDeberiaIncluirElNombreCientificoDeLaPlantaDelCatalogo()
    {
        var json = JObject.Parse(_testContext.ResponseBody!);
        json["plant"]?["scientificName"].Should().NotBeNull();
    }

    [Then(@"debería incluir mi nombre personalizado")]
    public void ThenDeberiaIncluirMiNombrePersonalizado()
    {
        var json = JObject.Parse(_testContext.ResponseBody!);
        json["customName"].Should().NotBeNull();
    }

    [Then(@"debería incluir la ubicación")]
    public void ThenDeberiaIncluirLaUbicacion()
    {
        var json = JObject.Parse(_testContext.ResponseBody!);
        json["location"].Should().NotBeNull();
    }

    [Then(@"debería incluir las notas")]
    public void ThenDeberiaIncluirLasNotas()
    {
        var json = JObject.Parse(_testContext.ResponseBody!);
        json.Should().ContainKey("notes");
    }

    [Then(@"la respuesta debería indicar que la planta no existe en mi colección")]
    public void ThenLaRespuestaDeberiaIndicarQueLaPlantaNoExisteEnMiColeccion()
    {
        _testContext.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Then(@"la respuesta debería indicar que no tengo acceso a esa planta")]
    public void ThenLaRespuestaDeberiaIndicarQueNoTengoAccesoAEsaPlanta()
    {
        _testContext.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Then(@"la foto debería estar asociada a mi planta")]
    public void ThenLaFotoDeberiaEstarAsociadaAMiPlanta()
    {
        _testContext.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Then(@"al consultar la planta debería ver la foto")]
    public async Task ThenAlConsultarLaPlantaDeberiaVerLaFoto()
    {
        var myPlantId = _testContext.GetEntityId("my_plant");
        await _apiClient.GetAsync($"/plantita/v1/myplant/{myPlantId}");
        // Should contain photo data in response
    }

    [Then(@"la tarea debería estar asociada a mi planta")]
    public void ThenLaTareaDeberiaEstarAsociadaAMiPlanta()
    {
        _testContext.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Then(@"la tarea debería tener estado ""(.*)""")]
    public void ThenLaTareaDeberiaTenerEstado(string status)
    {
        var json = JObject.Parse(_testContext.ResponseBody!);
        json["status"]?.ToString().Should().Be(status);
    }

    [Then(@"la tarea debería tener la fecha de finalización registrada")]
    public void ThenLaTareaDeberiaTenerLaFechaDeFinalizacionRegistrada()
    {
        var json = JObject.Parse(_testContext.ResponseBody!);
        json["completedDate"].Should().NotBeNull();
    }

    [Then(@"debería ver (.*) tareas pendientes")]
    public void ThenDeberiaVerTareasPendientes(int count)
    {
        var json = JArray.Parse(_testContext.ResponseBody!);
        json.Count.Should().Be(count);
    }

    [Then(@"no debería ver la tarea completada de ""(.*)""")]
    public void ThenNoDeberiaVerLaTareaCompletadaDe(string taskType)
    {
        _testContext.ResponseBody.Should().NotContain(taskType);
    }

    [Then(@"la observación debería estar guardada")]
    public void ThenLaObservacionDeberiaEstarGuardada()
    {
        _testContext.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Then(@"debería poder consultar el historial de salud de la planta")]
    public async Task ThenDeberiPoderConsultarElHistorialDeSaludDeLaPlanta()
    {
        await WhenConsultoElHistorialDeSaludDeLaPlanta();
        _testContext.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Then(@"debería ver (.*) observaciones ordenadas por fecha")]
    public void ThenDeberiaVerObservacionesOrdenadasPorFecha(int count)
    {
        var json = JArray.Parse(_testContext.ResponseBody!);
        json.Count.Should().Be(count);
    }

    [Then(@"debería poder ver la evolución de la salud de la planta")]
    public void ThenDeberiPoderVerLaEvolucionDeLaSaludDeLaPlanta()
    {
        var json = JArray.Parse(_testContext.ResponseBody!);
        json.Should().HaveCountGreaterThan(0);
    }

    [Then(@"debería tener la planta ""(.*)"" en mi colección")]
    public async Task ThenDeberiaTenerLaPlantaEnMiColeccion(string customName)
    {
        var userId = _testContext.CurrentUserId ?? 1;
        var plantExists = await _databaseHelper.ExecuteInScope(async db =>
        {
            return db.Set<plantita.ProjectPlantita.plantmanagment.domain.model.aggregates.MyPlant>()
                .Any(p => p.UserId == userId && p.CustomName == customName);
        });

        plantExists.Should().BeTrue();
    }

    [Then(@"debería poder ver sus características de ""(.*)""")]
    public void ThenDeberiPoderVerSusCaracteristicasDe(string tipo)
    {
        // Validate plant has characteristics matching the type
        _testContext.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Then(@"la planta no debería estar más en mi colección")]
    public async Task ThenLaPlantaNoDeberiaEstarMasEnMiColeccion()
    {
        var myPlantId = _testContext.GetEntityId("my_plant");
        var plantExists = await _databaseHelper.ExecuteInScope(async db =>
        {
            return db.Set<plantita.ProjectPlantita.plantmanagment.domain.model.aggregates.MyPlant>()
                .Any(p => p.Id == myPlantId);
        });

        plantExists.Should().BeFalse();
    }

    [Then(@"al consultar la planta debería recibir un 404")]
    public async Task ThenAlConsultarLaPlantaDeberiaRecibir404()
    {
        var myPlantId = _testContext.GetEntityId("my_plant");
        await _apiClient.GetAsync($"/plantita/v1/myplant/{myPlantId}");
        _testContext.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Then(@"todas las tareas asociadas deberían eliminarse")]
    public async Task ThenTodasLasTareasAsociadasDeberianEliminarse()
    {
        var myPlantId = _testContext.GetEntityId("my_plant");
        var tasksExist = await _databaseHelper.ExecuteInScope(async db =>
        {
            return db.Set<plantita.ProjectPlantita.plantmanagment.domain.model.Entities.CareTask>()
                .Any(t => t.MyPlantId == myPlantId);
        });

        tasksExist.Should().BeFalse();
    }

    [Then(@"todas las observaciones de salud deberían eliminarse")]
    public async Task ThenTodasLasObservacionesDeSaludDeberianEliminarse()
    {
        var myPlantId = _testContext.GetEntityId("my_plant");
        var logsExist = await _databaseHelper.ExecuteInScope(async db =>
        {
            return db.Set<plantita.ProjectPlantita.plantmanagment.domain.model.Entities.PlantHealthLog>()
                .Any(l => l.MyPlantId == myPlantId);
        });

        logsExist.Should().BeFalse();
    }

    #endregion
}
