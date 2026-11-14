using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransporteEmpresarial.Domain.Entities;

namespace TransporteEmpresarial.Infrastructure.Context.Configurations
{
    public class ViajePasajeroConfiguration : IEntityTypeConfiguration<ViajePasajero>
    {
        public void Configure(EntityTypeBuilder<ViajePasajero> builder)
        {
            builder.ToTable("ViajePasajeros");

            // Un empleado solo puede estar inscrito una vez en el mismo viaje
            builder.HasKey(vp => new { vp.ViajeId, vp.EmpleadoId });

            builder.HasOne(vp => vp.Viaje)
                   .WithMany(v => v.Pasajeros)
                   .HasForeignKey(vp => vp.ViajeId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(vp => vp.Empleado)
                   .WithMany(e => e.Traslados)
                   .HasForeignKey(vp => vp.EmpleadoId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
