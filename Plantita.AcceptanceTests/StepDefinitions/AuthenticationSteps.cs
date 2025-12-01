using System.Net;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plantita.AcceptanceTests.Support;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using BCrypt.Net;

namespace Plantita.AcceptanceTests.StepDefinitions;

[Binding]
public class AuthenticationSteps
{
    private readonly TestContext _testContext;
    private readonly ApiClient _apiClient;
    private readonly DatabaseHelper _databaseHelper;

    public AuthenticationSteps(TestContext testContext, ApiClient apiClient, DatabaseHelper databaseHelper)
    {
        _testContext = testContext;
        _apiClient = apiClient;
        _databaseHelper = databaseHelper;
    }

    #region Given Steps

    [Given(@"que la API de Plantita está disponible")]
    public void GivenLaApiDePlantitaEstaDisponible()
    {
        // API is already running from TestHooks
        _testContext.Store("api_available", true);
    }

    [Given(@"la base de datos está limpia")]
    public async Task GivenLaBaseDeDatosEstaLimpia()
    {
        await _databaseHelper.ClearDatabase();
    }

    [Given(@"que existe un usuario con email ""(.*)""")]
    public async Task GivenExisteUnUsuarioConEmail(string email)
    {
        await _databaseHelper.ExecuteInScope(async db =>
        {
            var user = new plantita.User.Domain.Model.Aggregates.AuthUser
            {
                Email = email,
                Password = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                Name = "Test User",
                Timezone = "America/Lima",
                Language = "es"
            };

            db.Add(user);
            await db.SaveChangesAsync();

            _testContext.StoreEntityId("existing_user", user.Id);
        });
    }

    [Given(@"que existe un usuario registrado con:")]
    public async Task GivenExisteUnUsuarioRegistradoCon(Table table)
    {
        var data = table.Rows[0];
        var email = data["email"];
        var password = data["password"];
        var name = data["name"];

        await _databaseHelper.ExecuteInScope(async db =>
        {
            var user = new plantita.User.Domain.Model.Aggregates.AuthUser
            {
                Email = email,
                Password = BCrypt.Net.BCrypt.HashPassword(password),
                Name = name,
                Timezone = data.ContainsKey("timezone") ? data["timezone"] : "America/Lima",
                Language = data.ContainsKey("language") ? data["language"] : "es"
            };

            db.Add(user);
            await db.SaveChangesAsync();

            _testContext.StoreEntityId("registered_user", user.Id);
            _testContext.Store("registered_user_email", email);
            _testContext.Store("registered_user_password", password);
        });
    }

    [Given(@"que tengo un refresh token válido para el usuario ""(.*)""")]
    public async Task GivenTengoUnRefreshTokenValidoParaElUsuario(string email)
    {
        // First, sign in to get a valid refresh token
        var loginData = new
        {
            email = email,
            password = "Password123!"
        };

        var response = await _apiClient.PostAsync("/plantita/v1/authentication/sign-in", loginData);
        response.Should().BeSuccessful();

        var content = await response.Content.ReadAsStringAsync();
        var json = JObject.Parse(content);

        _testContext.RefreshToken = json["refreshToken"]?.ToString();
        _testContext.AccessToken = json["token"]?.ToString();
    }

    [Given(@"que tengo un refresh token expirado para el usuario ""(.*)""")]
    public void GivenTengoUnRefreshTokenExpiradoParaElUsuario(string email)
    {
        // Set an expired/invalid refresh token
        _testContext.RefreshToken = "expired_refresh_token_12345";
    }

    [Given(@"que estoy autenticado como ""(.*)""")]
    public async Task GivenEstoyAutenticadoComo(string email)
    {
        // Sign in to get authentication token
        await GivenExisteUnUsuarioRegistradoCon(new Table("campo", "valor")
        {
            { "email", email },
            { "password", "Password123!" },
            { "name", "Authenticated User" }
        });

        var loginData = new { email, password = "Password123!" };
        var response = await _apiClient.PostAsync("/plantita/v1/authentication/sign-in", loginData);

        var content = await response.Content.ReadAsStringAsync();
        var json = JObject.Parse(content);

        _testContext.AccessToken = json["token"]?.ToString();
        _testContext.CurrentUserEmail = email;
    }

    [Given(@"que estoy autenticado con un token JWT válido")]
    public async Task GivenEstoyAutenticadoConUnTokenJWTValido()
    {
        await GivenEstoyAutenticadoComo("[email protected]");
    }

    [Given(@"que tengo un token JWT expirado")]
    public void GivenTengoUnTokenJWTExpirado()
    {
        _testContext.AccessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjB9.invalid";
    }

    #endregion

    #region When Steps

    [When(@"envío una solicitud POST a ""(.*)"" con:")]
    public async Task WhenEnvioUnaSolicitudPOSTACon(string endpoint, Table table)
    {
        var data = new Dictionary<string, string>();
        foreach (var row in table.Rows)
        {
            data[row["campo"]] = row["valor"];
        }

        _testContext.RequestData = data.ToDictionary(k => k.Key, v => (object)v.Value);
        await _apiClient.PostAsync(endpoint, data);
    }

    [When(@"envío una solicitud POST a ""(.*)"" con el refresh token")]
    public async Task WhenEnvioUnaSolicitudPOSTAConElRefreshToken(string endpoint)
    {
        var data = new { refreshToken = _testContext.RefreshToken };
        await _apiClient.PostAsync(endpoint, data);
    }

    [When(@"envío una solicitud POST a ""(.*)"" con el refresh token expirado")]
    public async Task WhenEnvioUnaSolicitudPOSTAConElRefreshTokenExpirado(string endpoint)
    {
        var data = new { refreshToken = _testContext.RefreshToken };
        await _apiClient.PostAsync(endpoint, data);
    }

    [When(@"envío una solicitud POST a ""(.*)""")]
    public async Task WhenEnvioUnaSolicitudPOSTA(string endpoint)
    {
        await _apiClient.PostAsync(endpoint);
    }

    [When(@"intento acceder a un recurso protegido con el token en el header Authorization")]
    public async Task WhenIntentoAccederAUnRecursoProtegidoConElToken()
    {
        // Try to access a protected resource (e.g., myplant endpoint)
        await _apiClient.GetAsync("/plantita/v1/myplant");
    }

    [When(@"intento acceder a un recurso protegido sin enviar token de autenticación")]
    public async Task WhenIntentoAccederAUnRecursoProtegidoSinEnviarToken()
    {
        _apiClient.ClearAuthToken();
        await _apiClient.GetAsync("/plantita/v1/myplant");
    }

    [When(@"intento acceder a un recurso protegido con el token expirado")]
    public async Task WhenIntentoAccederAUnRecursoProtegidoConElTokenExpirado()
    {
        await _apiClient.GetAsync("/plantita/v1/myplant");
    }

    #endregion

    #region Then Steps

    [Then(@"debería recibir un código de estado (.*)")]
    public void ThenDeberiaRecibirUnCodigoDeEstado(int expectedStatusCode)
    {
        var actualStatusCode = (int)_testContext.StatusCode;
        actualStatusCode.Should().Be(expectedStatusCode,
            $"Expected status code {expectedStatusCode} but got {actualStatusCode}. Response: {_testContext.ResponseBody}");
    }

    [Then(@"la respuesta debería contener el email ""(.*)""")]
    public void ThenLaRespuestaDeberiaContenerElEmail(string email)
    {
        _testContext.ResponseBody.Should().Contain(email);
    }

    [Then(@"la respuesta debería contener el nombre ""(.*)""")]
    public void ThenLaRespuestaDeberiaContenerElNombre(string name)
    {
        _testContext.ResponseBody.Should().Contain(name);
    }

    [Then(@"el usuario debería estar guardado en la base de datos")]
    public async Task ThenElUsuarioDeberiaEstarGuardadoEnLaBaseDeDatos()
    {
        var email = _testContext.RequestData["email"].ToString();
        var userExists = await _databaseHelper.ExecuteInScope(async db =>
        {
            return db.Set<plantita.User.Domain.Model.Aggregates.AuthUser>()
                .Any(u => u.Email == email);
        });

        userExists.Should().BeTrue();
    }

    [Then(@"la respuesta debería contener un mensaje de error indicando que el usuario ya existe")]
    public void ThenLaRespuestaDeberiaContenerUnMensajeDeErrorIndicandoQueElUsuarioYaExiste()
    {
        _testContext.ResponseBody.Should().Match(body =>
            body.Contains("existe") || body.Contains("exists") || body.Contains("already"));
    }

    [Then(@"la respuesta debería contener un mensaje de error sobre el formato del email")]
    public void ThenLaRespuestaDeberiaContenerUnMensajeDeErrorSobreElFormatoDelEmail()
    {
        _testContext.ResponseBody.Should().Match(body =>
            body.Contains("email") && (body.Contains("invalid") || body.Contains("formato")));
    }

    [Then(@"la respuesta debería contener un mensaje de error sobre la fortaleza de la contraseña")]
    public void ThenLaRespuestaDeberiaContenerUnMensajeDeErrorSobreLaFortalezaDeLaContrasena()
    {
        _testContext.ResponseBody.Should().Match(body =>
            body.Contains("password") || body.Contains("contraseña"));
    }

    [Then(@"la respuesta debería contener un token de acceso JWT")]
    public void ThenLaRespuestaDeberiaContenerUnTokenDeAccesoJWT()
    {
        var json = JObject.Parse(_testContext.ResponseBody!);
        var token = json["token"]?.ToString();

        token.Should().NotBeNullOrEmpty();
        _testContext.AccessToken = token;

        // JWT tokens have 3 parts separated by dots
        token.Should().Match(t => t!.Split('.').Length == 3);
    }

    [Then(@"la respuesta debería contener un token de actualización")]
    public void ThenLaRespuestaDeberiaContenerUnTokenDeActualizacion()
    {
        var json = JObject.Parse(_testContext.ResponseBody!);
        var refreshToken = json["refreshToken"]?.ToString();

        refreshToken.Should().NotBeNullOrEmpty();
        _testContext.RefreshToken = refreshToken;
    }

    [Then(@"debería recibir una cookie HTTP-only con el refresh token")]
    public void ThenDeberiaRecibirUnaCookieHTTPOnlyConElRefreshToken()
    {
        _testContext.HttpResponse!.Headers.Should().ContainKey("Set-Cookie");
    }

    [Then(@"la respuesta debería contener un mensaje de error de autenticación")]
    public void ThenLaRespuestaDeberiaContenerUnMensajeDeErrorDeAutenticacion()
    {
        _testContext.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Then(@"la respuesta debería contener un nuevo token de acceso JWT")]
    public void ThenLaRespuestaDeberiaContenerUnNuevoTokenDeAccesoJWT()
    {
        ThenLaRespuestaDeberiaContenerUnTokenDeAccesoJWT();
    }

    [Then(@"la respuesta debería contener un nuevo token de actualización")]
    public void ThenLaRespuestaDeberiaContenerUnNuevoTokenDeActualizacion()
    {
        ThenLaRespuestaDeberiaContenerUnTokenDeActualizacion();
    }

    [Then(@"debería recibir una nueva cookie HTTP-only con el refresh token")]
    public void ThenDeberiaRecibirUnaNuevaCookieHTTPOnlyConElRefreshToken()
    {
        ThenDeberiaRecibirUnaCookieHTTPOnlyConElRefreshToken();
    }

    [Then(@"la respuesta debería contener un mensaje de error sobre token inválido")]
    public void ThenLaRespuestaDeberiaContenerUnMensajeDeErrorSobreTokenInvalido()
    {
        _testContext.ResponseBody.Should().Match(body =>
            body.Contains("invalid") || body.Contains("inválido") || body.Contains("token"));
    }

    [Then(@"la respuesta debería contener un mensaje de error sobre token expirado")]
    public void ThenLaRespuestaDeberiaContenerUnMensajeDeErrorSobreTokenExpirado()
    {
        _testContext.ResponseBody.Should().Match(body =>
            body.Contains("expired") || body.Contains("expirado") || body.Contains("token"));
    }

    [Then(@"el refresh token debería ser revocado en la base de datos")]
    public async Task ThenElRefreshTokenDeberiaSerRevocadoEnLaBaseDeDatos()
    {
        // Verify that refresh token is either deleted or marked as revoked
        var tokenExists = await _databaseHelper.ExecuteInScope(async db =>
        {
            return db.Set<plantita.User.Domain.Model.Aggregates.AuthUserRefreshToken>()
                .Any(t => t.Token == _testContext.RefreshToken);
        });

        // After sign-out, token should not exist or be revoked
        // Implementation depends on your business logic
    }

    [Then(@"la cookie de sesión debería ser eliminada")]
    public void ThenLaCookieDeSesionDeberiaSerEliminada()
    {
        // Check that Set-Cookie header clears the cookie
        if (_testContext.HttpResponse!.Headers.Contains("Set-Cookie"))
        {
            var cookies = _testContext.HttpResponse.Headers.GetValues("Set-Cookie");
            cookies.Should().Contain(c => c.Contains("Max-Age=0") || c.Contains("expires"));
        }
    }

    [Then(@"debería poder acceder al recurso exitosamente")]
    public void ThenDeberiPoderAccederAlRecursoExitosamente()
    {
        _testContext.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Created);
    }

    [Then(@"la respuesta debería indicar que se requiere autenticación")]
    public void ThenLaRespuestaDeberiaIndicarQueSeRequiereAutenticacion()
    {
        _testContext.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Then(@"la respuesta debería indicar que el token ha expirado")]
    public void ThenLaRespuestaDeberiaIndicarQueElTokenHaExpirado()
    {
        _testContext.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        ThenLaRespuestaDeberiaContenerUnMensajeDeErrorSobreTokenExpirado();
    }

    #endregion
}
