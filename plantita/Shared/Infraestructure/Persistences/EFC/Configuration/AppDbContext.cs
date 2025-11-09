using EntityFrameworkCore.CreatedUpdatedDate.Extensions;
using Microsoft.EntityFrameworkCore;
using plantita.ProjectPlantita.communityandsupport.Domain.model.Entities;
using plantita.ProjectPlantita.communityandsupport.Domain.model.aggregates;
using plantita.ProjectPlantita.diagnosisandproblems.domain.model.aggregates;
using plantita.ProjectPlantita.diagnosisandproblems.domain.model.Entities;
using plantita.ProjectPlantita.iotmonitoring.domain.model.aggregates;
using plantita.ProjectPlantita.iotmonitoring.domain.model.Entities;
using plantita.ProjectPlantita.learningandeducation.domain.model.Entities;
using plantita.ProjectPlantita.notifications.domain.model.aggregates;
using plantita.ProjectPlantita.plantmanagment.domain.model.aggregates;
using plantita.ProjectPlantita.plantmanagment.domain.model.aggregates.PlantID;
using plantita.ProjectPlantita.plantmanagment.domain.model.Entities;
using plantita.User.Domain.Model.Aggregates;

namespace plantita.Shared.Infraestructure.Persistences.EFC.Configuration
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

     
        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
            builder.EnableSensitiveDataLogging();
            builder.AddCreatedUpdatedInterceptor();

            base.OnConfiguring(builder);
        }

    // Usuario
    public DbSet<AuthUser> AuthUsers { get; set; }
    public DbSet<AuthUserRefreshToken> AuthUsersRefreshTokens { get; set; }


    // Plantas y cuidado
    public DbSet<MyPlant> MyPlants { get; set; }
    public DbSet<PlantHealthLog> PlantHealthLogs { get; set; }
    public DbSet<CareTask> CareTasks { get; set; }
    public DbSet<Plant> Plants { get; set; }

    // IoT
    public DbSet<IoTDevice> IoTDevices { get; set; }
    public DbSet<Sensor> Sensors { get; set; }
    public DbSet<SensorConfig> SensorConfigs { get; set; }
    public DbSet<SensorReading> SensorReadings { get; set; }

    // Alertas
    public DbSet<Alert> Alerts { get; set; }

    // Diagnóstico
    public DbSet<ProblemPlants> ProblemPlants { get; set; }
    public DbSet<Recommendation> Recommendations { get; set; }

    // Foro
    public DbSet<QuestionForum> QuestionForums { get; set; }
    public DbSet<AnswerForum> AnswerForums { get; set; }

    // Educación
    public DbSet<EducationalContent> EducationalContents { get; set; }
    public DbSet<UserContentProgress> UserContentProgresses { get; set; }

    // Notificaciones
    public DbSet<Notification> Notifications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        modelBuilder.Entity<AuthUser>(authUser =>
        {
            authUser.HasKey(u => u.Id);
            authUser.Property(u => u.Id).IsRequired();
            authUser.Property(u => u.Email).IsRequired().HasMaxLength(255);
            authUser.HasIndex(u => u.Email)
                .IsUnique()
                .HasDatabaseName("IX_AuthUser_Email");
            authUser.Property(u => u.PasswordHash).IsRequired();

            authUser.HasMany(u => u.myPlant)
                .WithOne(e => e.AuthUser)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        modelBuilder.Entity<AuthUserRefreshToken>(refreshToken =>
        {
            refreshToken.HasKey(rt => rt.Id);

            refreshToken.HasOne(rt => rt.AuthUser)
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
         // MyPlant → HealthLogs, Tasks, Alerts
            modelBuilder.Entity<MyPlant>(entity =>
            {
                entity.HasKey(p => p.MyPlantId);
                entity.HasMany(p => p.HealthLogs)
                      .WithOne()
                      .HasForeignKey(h => h.MyPlantId);
                entity.HasMany(p => p.CareTasks)
                      .WithOne()
                      .HasForeignKey(t => t.MyPlantId);
                entity.HasMany(p => p.Alerts)
                      .WithOne(a => a.PlantInstance)
                      .HasForeignKey(a => a.PlantInstanceId);
            });
            
            modelBuilder.Entity<IoTDevice>(entity =>
            {
                entity.HasKey(d => d.DeviceId);

                // Relación con MyPlant
                entity.HasOne(d => d.MyPlant)
                    .WithMany(p => p.IoTDevices) 
                    .HasForeignKey(d => d.MyPlantId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relación con Sensors
                entity.HasMany(d => d.Sensors)
                    .WithOne(s => s.Device)
                    .HasForeignKey(s => s.DeviceId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            
            modelBuilder.Entity<Plant>(entity =>
            {
                entity.HasKey(p => p.PlantId);
                entity.Property(p => p.ScientificName).IsRequired().HasMaxLength(255);
                entity.Property(p => p.CommonName).HasMaxLength(255);
                entity.Property(p => p.Description).HasMaxLength(2000);
                entity.Property(p => p.Watering).HasMaxLength(100);
                entity.Property(p => p.Sunlight).HasMaxLength(200);
                entity.Property(p => p.WikiUrl).HasMaxLength(500);
                entity.Property(p => p.ImageUrl).HasMaxLength(500);
            });

            modelBuilder.Entity<CareTask>(entity =>
            {
                entity.HasKey(ct => ct.TaskId); // ✅ define la clave

                entity.Property(ct => ct.TaskType)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(ct => ct.Status)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(ct => ct.ScheduledFor)
                    .IsRequired();

                entity.HasOne<MyPlant>()
                    .WithMany(p => p.CareTasks)
                    .HasForeignKey(ct => ct.MyPlantId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<PlantHealthLog>(entity =>
            {
                entity.HasKey(h => h.HealthLogId); // 🔑 Clave primaria

                entity.Property(h => h.HealthStatus)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(h => h.Timestamp)
                    .IsRequired();

                entity.Property(h => h.Source)
                    .HasMaxLength(50);

                entity.HasOne<MyPlant>()
                    .WithMany(p => p.HealthLogs)
                    .HasForeignKey(h => h.MyPlantId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            
            // Sensor → Config y Readings
            modelBuilder.Entity<Sensor>(entity =>
            {
                entity.HasKey(s => s.SensorId);
                entity.HasOne(s => s.Configuration)
                      .WithOne()
                      .HasForeignKey<SensorConfig>(c => c.SensorId);
                entity.HasMany(s => s.Readings)
                      .WithOne()
                      .HasForeignKey(r => r.SensorId);
            });

            // SensorReading
            modelBuilder.Entity<SensorReading>(entity =>
            {
                entity.HasKey(r => r.ReadingId);
                entity.Property(r => r.Value).IsRequired();
            });

            modelBuilder.Entity<SensorConfig>(entity =>
            {
                entity.HasKey(sc => sc.ConfigId); // ✅ Definir la clave primaria

                entity.Property(sc => sc.ThresholdMin).IsRequired();
                entity.Property(sc => sc.ThresholdMax).IsRequired();
                entity.Property(sc => sc.FrequencyMinutes).IsRequired();
                entity.Property(sc => sc.AutoNotify).IsRequired();
                entity.Property(sc => sc.ConfiguredAt).IsRequired();

                entity.HasOne<Sensor>()
                    .WithOne(s => s.Configuration)
                    .HasForeignKey<SensorConfig>(sc => sc.SensorId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            
            // Alert
            modelBuilder.Entity<Alert>(entity =>
            {
                entity.HasKey(a => a.AlertId);
                entity.HasOne(a => a.Sensor)
                      .WithMany()
                      .HasForeignKey(a => a.SensorId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // ProblemPlants → Recommendations
            modelBuilder.Entity<ProblemPlants>(entity =>
            {
                entity.HasKey(p => p.ProblemId);
                entity.HasMany(p => p.Recommendations)
                      .WithOne()
                      .HasForeignKey(r => r.ProblemId);
            });

            // AnswerForum
            modelBuilder.Entity<AnswerForum>(entity =>
            {
                entity.HasKey(a => a.AnswerId); // 👈 esto es lo que faltaba

                entity.Property(a => a.Content)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(a => a.AnsweredAt)
                    .IsRequired();

                entity.Property(a => a.IsBestAnswer)
                    .HasDefaultValue(false);

                entity.HasOne<QuestionForum>()
                    .WithMany(q => q.Answers)
                    .HasForeignKey(a => a.QuestionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            
            // QuestionForum → AnswerForum
            modelBuilder.Entity<QuestionForum>(entity =>
            {
                entity.HasKey(q => q.QuestionId);
                entity.HasMany(q => q.Answers)
                      .WithOne()
                      .HasForeignKey(a => a.QuestionId);
            });

            // UserContentProgress (clave compuesta)
            modelBuilder.Entity<UserContentProgress>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.ContentId });
            });
            
            modelBuilder.Entity<EducationalContent>(entity =>
            {
                entity.HasKey(ec => ec.ContentId);

                entity.Property(ec => ec.Title).IsRequired().HasMaxLength(255);
                entity.Property(ec => ec.Description).HasMaxLength(1000);
                entity.Property(ec => ec.Type).HasMaxLength(50);
                entity.Property(ec => ec.Level).HasMaxLength(50);
                entity.Property(ec => ec.Url).IsRequired();
                entity.Property(ec => ec.PublishedAt).IsRequired();
            });
       
    }
            
            
}
}
