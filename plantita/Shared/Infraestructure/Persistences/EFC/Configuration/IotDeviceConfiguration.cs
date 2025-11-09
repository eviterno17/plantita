using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using plantita.ProjectPlantita.iotmonitoring.domain.model.aggregates;

public class IoTDeviceConfiguration : IEntityTypeConfiguration<IoTDevice>
{
    public void Configure(EntityTypeBuilder<IoTDevice> builder)
    {
        builder.HasKey(d => d.DeviceId);

        builder.Property(d => d.DeviceName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.ConnectionType)
            .HasMaxLength(50);

        builder.Property(d => d.Location)
            .HasMaxLength(100);

        builder.Property(d => d.Status)
            .HasMaxLength(50);

        builder.Property(d => d.FirmwareVersion)
            .HasMaxLength(50);

        builder.HasMany(d => d.Sensors)
            .WithOne()
            .HasForeignKey(s => s.DeviceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}