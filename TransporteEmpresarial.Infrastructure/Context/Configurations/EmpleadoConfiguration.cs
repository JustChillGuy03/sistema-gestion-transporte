using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransporteEmpresarial.Domain.Entities;

namespace TransporteEmpresarial.Infrastructure.Context.Configurations
{
    public class EmpleadoConfiguration : IEntityTypeConfiguration<Empleado>
    {
        public void Configure(EntityTypeBuilder<Empleado> builder)
        {
            builder.ToTable("Empleados");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Cedula).IsRequired().HasMaxLength(13);
            builder.Property(e => e.Nombre).IsRequired().HasMaxLength(60);
            builder.Property(e => e.Apellido).IsRequired().HasMaxLength(60);
            builder.Property(e => e.Telefono).HasMaxLength(20);
            builder.Property(e => e.Correo).HasMaxLength(100);
            builder.Property(e => e.Tipo).HasConversion<string>().HasMaxLength(20);
            builder.Property(e => e.Departamento).HasMaxLength(60);
            builder.Property(e => e.PuntoRecogida).HasMaxLength(150);
            builder.Property(e => e.LicenciaConducir).HasMaxLength(20);

            builder.HasIndex(e => e.Cedula).IsUnique().HasFilter("[Eliminado] = 0");
        }
    }
}
