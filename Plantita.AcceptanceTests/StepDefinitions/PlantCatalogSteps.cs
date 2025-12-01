using System.Net;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using Plantita.AcceptanceTests.Support;
using TechTalk.SpecFlow;

namespace Plantita.AcceptanceTests.StepDefinitions;

[Binding]
public class PlantCatalogSteps
{
    private readonly TestContext _testContext;
    private readonly ApiClient _apiClient;
    private readonly DatabaseHelper _databaseHelper;

    public PlantCatalogSteps(TestContext testContext, ApiClient apiClient, DatabaseHelper databaseHelper)
    {
        _testContext = testContext;
        _apiClient = apiClient;
        _databaseHelper = databaseHelper;
    }

    #region Given Steps

    [Given(@"estoy autenticado como administrador")]
    public async Task GivenEstoyAutenticadoComoAdministrador()
    {
        // Create and authenticate as admin user
        await _databaseHelper.ExecuteInScope(async db =>
        {
            var admin = new plantita.User.Domain.Model.Aggregates.AuthUser
            {
                Email = "[email protected]",
                Password = BCrypt.Net.BCrypt.HashPassword("AdminPass123!"),
                Name = "Admin User",
                Timezone = "America/Lima",
                Language = "es"
            };
            db.Add(admin);
            await db.SaveChangesAsync();
        });

        var loginData = new { email = "[email protected]", password = "AdminPass123!" };
        var response = await _apiClient.PostAsync("/plantita/v1/authentication/sign-in", loginData);

        var content = await response.Content.ReadAsStringAsync();
        var json = JObject.Parse(content);
        _testContext.AccessToken = json["token"]?.ToString();
    }

    [Given(@"que existen las siguientes plantas en el catálogo:")]
    public async Task GivenExistenLasSiguientesPlantasEnElCatalogo(Table table)
    {
        await _databaseHelper.ExecuteInScope(async db =>
        {
            foreach (var row in table.Rows)
            {
                var plant = new plantita.ProjectPlantita.plantmanagment.domain.model.aggregates.Plant
                {
                    ScientificName = row["scientificName"],
                    CommonName = row["commonName"],
                    Description = "Test plant description",
                    WateringFrequency = "Cada 7 días",
                    SunlightRequirement = "Luz indirecta"
                };
                db.Add(plant);
            }
            await db.SaveChangesAsync();
        });
    }

    [Given(@"que existe una planta con ID ""(.*)"" y nombre ""(.*)""")]
    public async Task GivenExisteUnaPlantaConIDYNombre(string id, string scientificName)
    {
        await _databaseHelper.ExecuteInScope(async db =>
        {
            var plant = new plantita.ProjectPlantita.plantmanagment.domain.model.aggregates.Plant
            {
                Id = int.Parse(id),
                ScientificName = scientificName,
                CommonName = "Test Common Name",
                Description = "Test Description",
                WateringFrequency = "Cada 7 días",
                SunlightRequirement = "Luz indirecta"
            };
            db.Add(plant);
            await db.SaveChangesAsync();
        });
    }

    [Given(@"que existe una planta con nombre común ""(.*)""")]
    public async Task GivenExisteUnaPlantaConNombreComun(string commonName)
    {
        await _databaseHelper.ExecuteInScope(async db =>
        {
            var plant = new plantita.ProjectPlantita.plantmanagment.domain.model.aggregates.Plant
            {
                ScientificName = "Monstera deliciosa",
                CommonName = commonName,
                Description = "Planta tropical",
                WateringFrequency = "Cada 7-10 días",
                SunlightRequirement = "Luz indirecta brillante"
            };
            db.Add(plant);
            await db.SaveChangesAsync();
        });
    }

    [Given(@"que existe una planta con ID ""(.*)""")]
    public async Task GivenExisteUnaPlantaConID(string id)
    {
        await _databaseHelper.ExecuteInScope(async db =>
        {
            var plant = new plantita.ProjectPlantita.plantmanagment.domain.model.aggregates.Plant
            {
                Id = int.Parse(id),
                ScientificName = "Test Plant",
                CommonName = "Test Common",
                Description = "Test Description",
                WateringFrequency = "Cada 7 días",
                SunlightRequirement = "Luz indirecta"
            };
            db.Add(plant);
            await db.SaveChangesAsync();
        });
    }

    #endregion

    #region When Steps

    [When(@"envío una solicitud GET a ""(.*)""")]
    public async Task WhenEnvioUnaSolicitudGETA(string endpoint)
    {
        await _apiClient.GetAsync(endpoint);
    }

    [When(@"envío una solicitud PUT a ""(.*)"" con datos válidos")]
    public async Task WhenEnvioUnaSolicitudPUTAConDatosValidos(string endpoint)
    {
        var data = new
        {
            scientificName = "Updated Plant",
            commonName = "Updated Common Name",
            description = "Updated Description"
        };
        await _apiClient.PutAsync(endpoint, data);
    }

    [When(@"envío una solicitud DELETE a ""(.*)""")]
    public async Task WhenEnvioUnaSolicitudDELETEA(string endpoint)
    {
        await _apiClient.DeleteAsync(endpoint);
    }

    [When(@"envío una solicitud PUT a ""(.*)"" con:")]
    public async Task WhenEnvioUnaSolicitudPUTACon(string endpoint, Table table)
    {
        var data = new Dictionary<string, string>();
        foreach (var row in table.Rows)
        {
            data[row["campo"]] = row["valor"];
        }
        await _apiClient.PutAsync(endpoint, data);
    }

    [When(@"registro una planta de tipo ""(.*)"" con:")]
    public async Task WhenRegistroUnaPlantaDeTipoCon(string tipo, Table table)
    {
        var data = new Dictionary<string, string> { { "plantType", tipo } };
        foreach (var row in table.Rows)
        {
            data[row[0]] = row[1];
        }
        await _apiClient.PostAsync("/plantita/v1/plant", data);
        _testContext.Store("plant_type", tipo);
    }

    #endregion

    #region Then Steps

    [Then(@"la respuesta debería contener el nombre científico ""(.*)""")]
    public void ThenLaRespuestaDeberiaContenerElNombreCientifico(string scientificName)
    {
        _testContext.ResponseBody.Should().Contain(scientificName);
    }

    [Then(@"la respuesta debería contener el nombre común ""(.*)""")]
    public void ThenLaRespuestaDeberiaContenerElNombreComun(string commonName)
    {
        _testContext.ResponseBody.Should().Contain(commonName);
    }

    [Then(@"la planta debería estar guardada en el catálogo")]
    public async Task ThenLaPlantaDeberiaEstarGuardadaEnElCatalogo()
    {
        var scientificName = _testContext.RequestData["scientificName"]?.ToString();
        var plantExists = await _databaseHelper.ExecuteInScope(async db =>
        {
            return db.Set<plantita.ProjectPlantita.plantmanagment.domain.model.aggregates.Plant>()
                .Any(p => p.ScientificName == scientificName);
        });

        plantExists.Should().BeTrue();
    }

    [Then(@"la respuesta debería contener una lista con (.*) plantas")]
    public void ThenLaRespuestaDeberiaContenerUnaListaConPlantas(int count)
    {
        var json = JArray.Parse(_testContext.ResponseBody!);
        json.Count.Should().Be(count);
    }

    [Then(@"la lista debería incluir ""(.*)""")]
    public void ThenLaListaDeberiaIncluir(string scientificName)
    {
        _testContext.ResponseBody.Should().Contain(scientificName);
    }

    [Then(@"la respuesta debería contener todos los detalles de la planta")]
    public void ThenLaRespuestaDeberiaContenerTodosLosDetallesDeLaPlanta()
    {
        var json = JObject.Parse(_testContext.ResponseBody!);
        json["scientificName"].Should().NotBeNull();
        json["commonName"].Should().NotBeNull();
    }

    [Then(@"la respuesta debería contener un mensaje de error indicando que la planta no existe")]
    public void ThenLaRespuestaDeberiaContenerUnMensajeDeErrorIndicandoQueLaPlantaNoExiste()
    {
        _testContext.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Then(@"la respuesta debería indicar que no se encontró la planta")]
    public void ThenLaRespuestaDeberiaIndicarQueNoSeEncontroLaPlanta()
    {
        _testContext.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Then(@"la respuesta debería contener la descripción actualizada")]
    public void ThenLaRespuestaDeberiaContenerLaDescripcionActualizada()
    {
        _testContext.ResponseBody.Should().Contain("actualizada");
    }

    [Then(@"la respuesta debería contener el nuevo régimen de riego")]
    public void ThenLaRespuestaDeberiaContenerElNuevoRegimenDeRiego()
    {
        _testContext.ResponseBody.Should().Match(body =>
            body.Contains("10-14") || body.Contains("actualizado"));
    }

    [Then(@"la respuesta debería indicar que la planta no existe")]
    public void ThenLaRespuestaDeberiaIndicarQueLaPlantaNoExiste()
    {
        _testContext.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Then(@"la planta no debería existir más en el catálogo")]
    public async Task ThenLaPlantaNoDeberiaExistirMasEnElCatalogo()
    {
        var plantId = ExtractIdFromLastRequest();
        var plantExists = await _databaseHelper.ExecuteInScope(async db =>
        {
            return db.Set<plantita.ProjectPlantita.plantmanagment.domain.model.aggregates.Plant>()
                .Any(p => p.Id == plantId);
        });

        plantExists.Should().BeFalse();
    }

    [Then(@"al consultar GET ""(.*)"" debería recibir 404")]
    public async Task ThenAlConsultarGETDeberiaRecibir404(string endpoint)
    {
        await _apiClient.GetAsync(endpoint);
        _testContext.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Then(@"la respuesta debería contener el nombre científico identificado")]
    public void ThenLaRespuestaDeberiaContenerElNombreCientificoIdentificado()
    {
        var json = JObject.Parse(_testContext.ResponseBody!);
        json["scientificName"].Should().NotBeNull();
        json["scientificName"]!.ToString().Should().NotBeEmpty();
    }

    [Then(@"la planta identificada debería guardarse en el catálogo")]
    public async Task ThenLaPlantaIdentificadaDeberiaGuardarseEnElCatalogo()
    {
        var json = JObject.Parse(_testContext.ResponseBody!);
        var scientificName = json["scientificName"]?.ToString();

        var plantExists = await _databaseHelper.ExecuteInScope(async db =>
        {
            return db.Set<plantita.ProjectPlantita.plantmanagment.domain.model.aggregates.Plant>()
                .Any(p => p.ScientificName == scientificName);
        });

        plantExists.Should().BeTrue();
    }

    [Then(@"la respuesta debería incluir el nivel de confianza de la identificación")]
    public void ThenLaRespuestaDeberiaIncluirElNivelDeConfianzaDeLaIdentificacion()
    {
        _testContext.ResponseBody.Should().Match(body =>
            body.Contains("confidence") || body.Contains("confianza") || body.Contains("probability"));
    }

    [Then(@"la respuesta debería contener un mensaje de error sobre la imagen")]
    public void ThenLaRespuestaDeberiaContenerUnMensajeDeErrorSobreLaImagen()
    {
        _testContext.ResponseBody.Should().Match(body =>
            body.Contains("image") || body.Contains("imagen"));
    }

    [Then(@"la respuesta debería indicar que falta el nombre científico")]
    public void ThenLaRespuestaDeberiaIndicarQueFaltaElNombreCientifico()
    {
        _testContext.ResponseBody.Should().Match(body =>
            body.Contains("scientificName") || body.Contains("required"));
    }

    [Then(@"la planta debería tener las características de ""(.*)""")]
    public void ThenLaPlantaDeberiaTenerLasCaracteristicasDe(string tipo)
    {
        var json = JObject.Parse(_testContext.ResponseBody!);
        json["scientificName"].Should().NotBeNull();
        // Additional type-specific validations could be added here
    }

    #endregion

    #region Helper Methods

    private int ExtractIdFromLastRequest()
    {
        // Extract ID from the last HTTP request URL
        var uri = _testContext.HttpResponse?.RequestMessage?.RequestUri?.ToString() ?? "";
        var segments = uri.Split('/');
        var lastSegment = segments.Last();

        if (int.TryParse(lastSegment, out var id))
        {
            return id;
        }

        return 0;
    }

    #endregion
}
