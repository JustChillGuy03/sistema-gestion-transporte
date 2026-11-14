using TransporteEmpresarial.Domain.Entities;
using TransporteEmpresarial.Domain.Enums;
using TransporteEmpresarial.Infrastructure.Models;

namespace TransporteEmpresarial.Infrastructure.Extensions
{
    /// <summary>
    /// Consultas sobre viajes que usan varios repositorios.
    /// </summary>
    public static class ViajeQueryExtensions
    {
        // Viajes vigentes: los que todavía ocupan un vehículo, un conductor o un pasajero
        // (no eliminados, ni completados, ni cancelados)
        public static IQueryable<Viaje> Vigentes(this IQueryable<Viaje> viajes) =>
            viajes.Where(v => !v.Eliminado && (v.Estado == EstadoViaje.Programado || v.Estado == EstadoViaje.EnCurso));

        // Dos intervalos se cruzan si uno empieza antes de que termine el otro
        public static IQueryable<Viaje> QueSeCruzanCon(this IQueryable<Viaje> viajes, DateTime salida, DateTime llegada) =>
            viajes.Where(v => v.FechaHoraSalida < llegada && salida < v.FechaHoraLlegada);

        public static IQueryable<ViajeModel> ProyectarAModelo(this IQueryable<Viaje> viajes) =>
            viajes.Select(v => new ViajeModel
            {
                Id = v.Id,
                RutaId = v.RutaId,
                Ruta = v.Ruta!.Nombre,
                VehiculoId = v.VehiculoId,
                Placa = v.Vehiculo!.Placa,
                ConductorId = v.ConductorId,
                Conductor = v.Conductor!.Nombre + " " + v.Conductor.Apellido,
                FechaHoraSalida = v.FechaHoraSalida,
                FechaHoraLlegada = v.FechaHoraLlegada,
                Turno = v.Turno,
                Estado = v.Estado,
                Capacidad = v.Vehiculo.Capacidad,
                PasajerosInscritos = v.Pasajeros.Count
            });
    }
}
