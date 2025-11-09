// IoT Monitoring dependencies
using plantita.ProjectPlantita.iotmonitoring.Application.Internal.Services;
using plantita.ProjectPlantita.iotmonitoring.domain.repositories;
using plantita.ProjectPlantita.iotmonitoring.Infraestructure.Persistence;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using plantita.ProjectPlantita.plantmanagment.Application.Internal.CommandServices;
using plantita.ProjectPlantita.plantmanagment.Application.Internal.QueryServices;
using plantita.ProjectPlantita.plantmanagment.domain.Repositories;
using plantita.ProjectPlantita.plantmanagment.domain.Services;
using plantita.ProjectPlantita.plantmanagment.Infraestructure.EFC.Repositories;
using plantita.Shared.Domain.Repositories;
using plantita.Shared.Infraestructure.Interfaces.ASP.Configuration;
using plantita.Shared.Infraestructure.Persistences.EFC.Configuration;
using plantita.Shared.Infraestructure.Persistences.EFC.Repositories;
using plantita.User.Application.Internal.CommandServices;
using plantita.User.Application.Internal.OutboundServices;
using plantita.User.Application.Internal.QueryServices;
using plantita.User.Domain.Repositories;
using plantita.User.Domain.Services;
using plantita.User.Infraestructure.Hashing.BCrypt.Services;
using plantita.User.Infraestructure.Persistence.EFC.Repositories;
using plantita.User.Infraestructure.Tokens.JWT.Configurations;
using plantita.User.Infraestructure.Tokens.JWT.Services;
using plantita.User.Interfaces.ACL;
using plantita.User.Interfaces.ACL.Services;
using ProjectCalculadoraAMSAC.User.Infraestructure.Persistence.EFC.Repositories;
using Swashbuckle.AspNetCore.SwaggerUI;

Console.WriteLine("🔥 Application starting...");

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine("✅ Builder creado");

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = "Bearer";
        options.DefaultChallengeScheme = "Bearer";
    })
    .AddJwtBearer("Bearer", options =>
    {
        var secret = builder.Configuration["TokenSettings:Secret"];
        if (string.IsNullOrEmpty(secret))
        {
            throw new ArgumentNullException(nameof(secret), "JWT Secret is not configured in appsettings.json.");
        }

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Cookies.ContainsKey("AuthToken"))
                {
                    context.Token = context.Request.Cookies["AuthToken"];
                }
                return Task.CompletedTask;
            }
        };
    });


builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
        .LogTo(Console.WriteLine, LogLevel.Information)
        .EnableSensitiveDataLogging(builder.Environment.IsDevelopment())
        .EnableDetailedErrors(builder.Environment.IsDevelopment());
});
builder.Services.AddControllers(options =>
{
        options.Conventions.Add(new KebabCaseRouteNamingConvention());
});

// Configurar opciones de enrutamiento
builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",policy =>
    {
        policy.WithOrigins("http://localhost:8080")  
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();  
    });
});



builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true; // 🔹 Evita acceso JS
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // 🔹 Solo HTTPS en producción
    options.Cookie.SameSite = SameSiteMode.None; // 🔹 Necesario para Swagger y CORS
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Plantita API",
        Version = "v1",
        Description = "Plantita Platform API",
        TermsOfService = new Uri("http://localhost:5000/swagger/index.html"),
        Contact = new OpenApiContact
        {
            Name = "LlanterosTech",
            Email = "erickpalomino0723@gmail.com"
        },
        License = new OpenApiLicense
        {
            Name = "Apache 2.0",
            Url = new Uri("https://www.apache.org/licenses/LICENSE-2.0.html")
        }
    });

    // 🔹 Definir el esquema de autenticación JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Ingrese el token JWT con el prefijo 'Bearer '",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.Configure<TokenSettings>(builder.Configuration.GetSection("TokenSettings"));
builder.Services.AddScoped<IAuthUserRepository, AuthUserRepository>();
builder.Services.AddScoped<IAuthUserRefreshTokenRepository, AuthUserRefreshTokenRepository>();

builder.Services.AddScoped<IAuthUserCommandService, AuthUserCommandService>();
builder.Services.AddScoped<IAuthUserQueryService, AuthUserQueryService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IHashingService, HashingService>();
builder.Services.AddScoped<IIamContextFacade, IamContextFacade>();

builder.Services.AddScoped<IPlantRepository, PlantRepository>();
builder.Services.AddScoped<IMyPlantRepository, MyPlantRepository>();
builder.Services.AddScoped<ICareTaskRepository, CareTaskRepository>();
builder.Services.AddScoped<IPlantHealthLogRepository, PlantHealthLogRepository>();
builder.Services.AddScoped<IPlantCommandService, PlantCommandService>();
builder.Services.AddScoped<IPlantQueryService, PlantQueryService>();
builder.Services.AddScoped<IMyPlantCommandService, MyPlantCommandService>();
builder.Services.AddScoped<IMyPlantQueryService, MyPlantQueryService>();

builder.Services.AddHttpClient<IPlantIdentificationService, PlantIdentificationService>();

// IoT Monitoring DI
builder.Services.AddScoped<IIoTDeviceRepository, IoTDeviceRepository>();
builder.Services.AddScoped<ISensorRepository, SensorRepository>();
builder.Services.AddScoped<ISensorConfigRepository, SensorConfigRepository>();
builder.Services.AddScoped<ISensorReadingRepository, SensorReadingRepository>();
builder.Services.AddScoped<IIoTDeviceService, IoTDeviceService>();
builder.Services.AddScoped<ISensorService, SensorService>();
builder.Services.AddScoped<ISensorConfigService, SensorConfigService>();
builder.Services.AddScoped<ISensorReadingService, SensorReadingService>();


var app = builder.Build();
Console.WriteLine("✅ App construida");

// Log Server Addresses
var serverAddresses = app.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>();


foreach (var address in serverAddresses.Addresses)
{
    Console.WriteLine($"Listening on {address}");
}

// Ensure Database is Created
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
}

// Configure Middleware Pipeline
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SupportedSubmitMethods(new[] { SubmitMethod.Get, SubmitMethod.Post });
    c.ConfigObject.AdditionalItems["withCredentials"] = true;
});

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseStaticFiles(); // Asegúrate de tener esto configurado
app.Run();
Console.WriteLine("✅ App corriendo");
