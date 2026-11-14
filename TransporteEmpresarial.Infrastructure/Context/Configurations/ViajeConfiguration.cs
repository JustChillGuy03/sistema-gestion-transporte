using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransporteEmpresarial.Domain.Entities;

namespace TransporteEmpresarial.Infrastructure.Context.Configurations
{
    public class ViajeConfiguration : IEntityTypeConfiguration<Viaje>
    {
        public void Configure(EntityTypeBuilder<Viaje> builder)
        {
            builder.ToTable("Viajes", t =>
                t.HasCheckConstraint("CK_Viajes_Horario", "[FechaHoraLlegada] > [FechaHoraSalida]"));

            builder.HasKey(v => v.Id);

            builder.Property(v => v.Turno).HasConversion<string>().HasMaxLength(20);
            builder.Property(v => v.Estado).HasConversion<string>().HasMaxLength(20);

            // Restrict: no se puede borrar físicamente una ruta, vehículo o conductor con viajes
            builder.HasOne(v => v.Ruta)
                   .WithMany(r => r.Viajes)
                   .HasForeignKey(v => v.RutaId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(v => v.Vehiculo)
                   .WithMany(ve => ve.Viajes)
                   .HasForeignKey(v => v.VehiculoId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(v => v.Conductor)
                   .WithMany(e => e.ViajesComoConductor)
                   .HasForeignKey(v => v.ConductorId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Índices para buscar rápido los cruces de horario
            builder.HasIndex(v => new { v.VehiculoId, v.FechaHoraSalida });
            builder.HasIndex(v => new { v.ConductorId, v.FechaHoraSalida });
        }
    }
}
