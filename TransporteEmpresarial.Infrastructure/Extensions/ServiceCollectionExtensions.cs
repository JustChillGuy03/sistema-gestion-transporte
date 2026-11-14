using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TransporteEmpresarial.Infrastructure.Context;
using TransporteEmpresarial.Infrastructure.Interfaces;
using TransporteEmpresarial.Infrastructure.Repositories;

namespace TransporteEmpresarial.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registra el contexto de SQL Server y los repositorios en el contenedor de dependencias.
        /// </summary>
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, string cadenaConexion)
        {
            services.AddDbContext<TransporteContext>(options => options.UseSqlServer(cadenaConexion));

            services.AddScoped<IRutaRepository, RutaRepository>();
            services.AddScoped<IEmpleadoRepository, EmpleadoRepository>();
            services.AddScoped<IVehiculoRepository, VehiculoRepository>();
            services.AddScoped<IViajeRepository, ViajeRepository>();

            return services;
        }
    }
}
