using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransporteEmpresarial.Domain.Entities;

namespace TransporteEmpresarial.Infrastructure.Context.Configurations
{
    public class VehiculoConfiguration : IEntityTypeConfiguration<Vehiculo>
    {
        public void Configure(EntityTypeBuilder<Vehiculo> builder)
        {
            builder.ToTable("Vehiculos", t =>
                t.HasCheckConstraint("CK_Vehiculos_Capacidad", "[Capacidad] > 0"));

            builder.HasKey(v => v.Id);

            builder.Property(v => v.Placa).IsRequired().HasMaxLength(10);
            builder.Property(v => v.Marca).IsRequired().HasMaxLength(50);
            builder.Property(v => v.Modelo).IsRequired().HasMaxLength(50);

            // El estado se guarda como texto para que la tabla se entienda sola
            builder.Property(v => v.Estado).HasConversion<string>().HasMaxLength(20);

            // Placa única entre los vehículos que no han sido eliminados
            builder.HasIndex(v => v.Placa).IsUnique().HasFilter("[Eliminado] = 0");
        }
    }
}
