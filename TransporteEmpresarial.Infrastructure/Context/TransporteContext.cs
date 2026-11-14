using Microsoft.EntityFrameworkCore;
using TransporteEmpresarial.Domain.Core;
using TransporteEmpresarial.Domain.Entities;

namespace TransporteEmpresarial.Infrastructure.Context
{
    public class TransporteContext : DbContext
    {
        public TransporteContext(DbContextOptions<TransporteContext> options) : base(options) { }

        public DbSet<Ruta> Rutas { get; set; }
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Vehiculo> Vehiculos { get; set; }
        public DbSet<Viaje> Viajes { get; set; }
        public DbSet<ViajePasajero> ViajePasajeros { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Aplica las clases de la carpeta Configurations (una por entidad)
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TransporteContext).Assembly);

            DatosIniciales.Cargar(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            AsignarFechasDeAuditoria();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            AsignarFechasDeAuditoria();
            return base.SaveChanges();
        }

        private void AsignarFechasDeAuditoria()
        {
            var ahora = DateTime.Now;

            foreach (var entrada in ChangeTracker.Entries<BaseEntity>())
            {
                if (entrada.State == EntityState.Added)
                {
                    entrada.Entity.FechaCreacion = ahora;
                }
                else if (entrada.State == EntityState.Modified)
                {
                    // La fecha de creación nunca cambia después de insertar
                    entrada.Property(e => e.FechaCreacion).IsModified = false;
                    entrada.Entity.FechaModificacion = ahora;
                }
            }
        }
    }
}
