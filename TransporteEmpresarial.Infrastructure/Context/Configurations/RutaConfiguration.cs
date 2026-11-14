using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransporteEmpresarial.Domain.Entities;

namespace TransporteEmpresarial.Infrastructure.Context.Configurations
{
    public class RutaConfiguration : IEntityTypeConfiguration<Ruta>
    {
        public void Configure(EntityTypeBuilder<Ruta> builder)
        {
            builder.ToTable("Rutas", t =>
                t.HasCheckConstraint("CK_Rutas_Duracion", "[DuracionEstimadaMinutos] > 0"));

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Nombre).IsRequired().HasMaxLength(100);
            builder.Property(r => r.Origen).IsRequired().HasMaxLength(150);
            builder.Property(r => r.Destino).IsRequired().HasMaxLength(150);

            // EF Core guarda la lista de paradas como un arreglo JSON en una sola columna
            builder.Property(r => r.ParadasIntermedias).IsRequired();

            builder.HasIndex(r => r.Nombre).IsUnique().HasFilter("[Eliminado] = 0");
        }
    }
}
