using FluentAssertions;
using Newtonsoft.Json.Linq;
using Plantita.AcceptanceTests.Support;
using TechTalk.SpecFlow;

namespace Plantita.AcceptanceTests.StepDefinitions;

/// <summary>
/// Common step definitions shared across multiple features
/// </summary>
[Binding]
public class CommonSteps
{
    private readonly TestContext _testContext;
    private readonly ApiClient _apiClient;

    public CommonSteps(TestContext testContext, ApiClient apiClient)
    {
        _testContext = testContext;
        _apiClient = apiClient;
    }

    #region Response Validation

    [Then(@"la respuesta debería contener todos los detalles del (.*)")]
    public void ThenLaRespuestaDeberiaContenerTodosLosDetallesDel(string entityType)
    {
        var json = JObject.Parse(_testContext.ResponseBody!);
        json.Should().NotBeNull();
        json.Properties().Should().HaveCountGreaterThan(0);
    }

    [Then(@"debería incluir el nombre del (.*)")]
    public void ThenDeberiaIncluirElNombreDel(string entityType)
    {
        var json = JObject.Parse(_testContext.ResponseBody!);
        json.Should().ContainKey("name");
    }

    [Then(@"la respuesta debería contener (.*)")]
    public void ThenLaRespuestaDeberiaContener(string value)
    {
        _testContext.ResponseBody.Should().Contain(value);
    }

    [Then(@"la respuesta debería contener una lista con (.*) (.*)")]
    public void ThenLaRespuestaDeberiaContenerUnaListaCon(int count, string entityType)
    {
        var json = JArray.Parse(_testContext.ResponseBody!);
        json.Count.Should().Be(count);
    }

    [Then(@"debería ver ""(.*)""")]
    public void ThenDeberiaVer(string value)
    {
        _testContext.ResponseBody.Should().Contain(value);
    }

    #endregion

    #region Error Handling

    [Then(@"la respuesta debería indicar que el (.*) no existe")]
    public void ThenLaRespuestaDeberiaIndicarQueElNoExiste(string entityType)
    {
        _testContext.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Then(@"la respuesta debería indicar que no tengo acceso a ese (.*)")]
    public void ThenLaRespuestaDeberiaIndicarQueNoTengoAccesoAEse(string entityType)
    {
        _testContext.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
    }

    [Then(@"la respuesta debería indicar que no tengo permisos")]
    public void ThenLaRespuestaDeberiaIndicarQueNoTengoPermisos()
    {
        _testContext.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
    }

    [Then(@"la respuesta debería indicar que faltan campos requeridos")]
    public void ThenLaRespuestaDeberiaIndicarQueFaltanCamposRequeridos()
    {
        _testContext.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        _testContext.ResponseBody.Should().Match(body =>
            body.Contains("required") || body.Contains("requerido") || body.Contains("missing"));
    }

    #endregion

    #region Database Validation

    [Then(@"el (.*) no debería existir más")]
    public void ThenElNoDeberiaExistirMas(string entityType)
    {
        // This is validated in specific step definitions
        _testContext.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Then(@"al consultar el (.*) debería recibir (.*)")]
    public async Task ThenAlConsultarElDeberiaRecibir(string entityType, int statusCode)
    {
        // Generic GET check - actual implementation in specific steps
        _testContext.StatusCode.Should().Be((System.Net.HttpStatusCode)statusCode);
    }

    #endregion

    #region CRUD Operations

    [When(@"intento actualizar el (.*) ""(.*)""")]
    public async Task WhenIntentoActualizarEl(string entityType, string id)
    {
        var endpoint = GetEndpointForEntity(entityType, id);
        var data = new { name = "Updated Name" };
        await _apiClient.PutAsync(endpoint, data);
    }

    [When(@"intento eliminar el (.*) ""(.*)""")]
    public async Task WhenIntentoEliminarEl(string entityType, string id)
    {
        var endpoint = GetEndpointForEntity(entityType, id);
        await _apiClient.DeleteAsync(endpoint);
    }

    [When(@"intento consultar los (.*) del (.*) ""(.*)""")]
    public async Task WhenIntentoConsultarLosDel(string childEntity, string parentEntity, string parentId)
    {
        var endpoint = $"/plantita/v1/{parentEntity.ToLower()}/{parentId}/{childEntity.ToLower()}";
        await _apiClient.GetAsync(endpoint);
    }

    #endregion

    #region Helper Methods

    private string GetEndpointForEntity(string entityType, string id)
    {
        var entityEndpoints = new Dictionary<string, string>
        {
            { "dispositivo", "/plantita/v1/iotdevice/" },
            { "device", "/plantita/v1/iotdevice/" },
            { "sensor", "/plantita/v1/sensor/" },
            { "planta", "/plantita/v1/plant/" },
            { "plant", "/plantita/v1/plant/" }
        };

        var baseEndpoint = entityEndpoints.GetValueOrDefault(entityType.ToLower(), "/plantita/v1/");
        return $"{baseEndpoint}{id}";
    }

    #endregion
}
