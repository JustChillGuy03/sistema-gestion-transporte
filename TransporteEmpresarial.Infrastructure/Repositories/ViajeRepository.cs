using System.Data;
using Microsoft.EntityFrameworkCore;
using TransporteEmpresarial.Domain.Entities;
using TransporteEmpresarial.Domain.Enums;
using TransporteEmpresarial.Infrastructure.Context;
using TransporteEmpresarial.Infrastructure.Core;
using TransporteEmpresarial.Infrastructure.Exceptions;
using TransporteEmpresarial.Infrastructure.Extensions;
using TransporteEmpresarial.Infrastructure.Interfaces;
using TransporteEmpresarial.Infrastructure.Models;

namespace TransporteEmpresarial.Infrastructure.Repositories
{
    public class ViajeRepository : BaseRepository<Viaje>, IViajeRepository
    {
        public ViajeRepository(TransporteContext context) : base(context) { }

        public override async Task AddAsync(Viaje viaje)
        {
            viaje.Estado = EstadoViaje.Programado;
            await ValidarProgramacionAsync(viaje);
            await base.AddAsync(viaje);
        }

        public override async Task UpdateAsync(Viaje viaje)
        {
            if (viaje.Estado != EstadoViaje.Programado)
                throw new ViajeException(
                    $"Solo se pueden reprogramar viajes en estado Programado; el viaje {viaje.Id} está {viaje.Estado}.");

            await ValidarProgramacionAsync(viaje);
            await base.UpdateAsync(viaje);
        }

        public override async Task RemoveAsync(Viaje viaje)
        {
            // Los viajes en curso o completados forman parte del historial de traslados
            if (viaje.Estado is EstadoViaje.EnCurso or EstadoViaje.Completado)
                throw new ViajeException(
                    $"No se puede eliminar el viaje {viaje.Id} porque está {viaje.Estado}; solo se eliminan viajes programados o cancelados.");

            await base.RemoveAsync(viaje);
        }

        public async Task<List<ViajeModel>> GetViajesAsync(DateTime? fecha, EstadoViaje? estado)
        {
            var consulta = Activos().AsNoTracking();

            if (fecha.HasValue)
            {
                var inicioDia = fecha.Value.Date;
                var finDia = inicioDia.AddDays(1);
                consulta = consulta.Where(v => v.FechaHoraSalida >= inicioDia && v.FechaHoraSalida < finDia);
            }

            if (estado.HasValue)
                consulta = consulta.Where(v => v.Estado == estado.Value);

            return await consulta.OrderBy(v => v.FechaHoraSalida)
                                 .ProyectarAModelo()
                                 .ToListAsync();
        }

        public async Task<ViajeModel?> GetViajeAsync(int id)
        {
            return await Activos().AsNoTracking()
                                  .Where(v => v.Id == id)
                                  .ProyectarAModelo()
                                  .FirstOrDefaultAsync();
        }

        public async Task<List<PasajeroModel>> GetPasajerosAsync(int viajeId)
        {
            if (!await ExistsAsync(v => v.Id == viajeId))
                throw new ViajeException($"No existe el viaje {viajeId}.", TipoError.NoEncontrado);

            return await _context.ViajePasajeros.AsNoTracking()
                .Where(p => p.ViajeId == viajeId)
                .OrderBy(p => p.Empleado!.Apellido)
                .Select(p => new PasajeroModel
                {
                    EmpleadoId = p.EmpleadoId,
                    Cedula = p.Empleado!.Cedula,
                    NombreCompleto = p.Empleado.Nombre + " " + p.Empleado.Apellido,
                    Departamento = p.Empleado.Departamento,
                    PuntoRecogida = p.Empleado.PuntoRecogida,
                    FechaInscripcion = p.FechaInscripcion
                })
                .ToListAsync();
        }

        public async Task InscribirPasajeroAsync(int viajeId, int empleadoId)
        {
            // Serializable: si dos personas se inscriben al mismo tiempo en el último asiento,
            // una de las dos espera y luego ve el cupo lleno (no se sobrepasa la capacidad).
            await using var transaccion = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            var viaje = await Activos().Include(v => v.Vehiculo).FirstOrDefaultAsync(v => v.Id == viajeId)
                ?? throw new ViajeException($"No existe el viaje {viajeId}.", TipoError.NoEncontrado);

            var empleado = await _context.Empleados.FirstOrDefaultAsync(e => e.Id == empleadoId && !e.Eliminado)
                ?? throw new ViajeException($"No existe el empleado {empleadoId}.", TipoError.NoEncontrado);

            if (viaje.Estado != EstadoViaje.Programado)
                throw new ViajeException(
                    $"Solo se pueden inscribir pasajeros en viajes programados; el viaje {viajeId} está {viaje.Estado}.");

            if (empleado.Tipo != TipoEmpleado.Pasajero)
                throw new ViajeException(
                    $"{empleado.NombreCompleto} es conductor; solo los empleados de tipo Pasajero se inscriben en un viaje.",
                    TipoError.Validacion);

            if (await _context.ViajePasajeros.AnyAsync(p => p.ViajeId == viajeId && p.EmpleadoId == empleadoId))
                throw new ViajeException($"{empleado.NombreCompleto} ya está inscrito en el viaje {viajeId}.");

            // Gestión de capacidad
            var inscritos = await _context.ViajePasajeros.CountAsync(p => p.ViajeId == viajeId);
            var capacidad = viaje.Vehiculo!.Capacidad;
            if (inscritos >= capacidad)
                throw new ViajeException($"El viaje {viajeId} está lleno: {inscritos} de {capacidad} asientos ocupados.");

            // Un pasajero no puede estar en dos viajes que se cruzan
            var cruce = await _context.Viajes.Vigentes()
                .Where(v => v.Id != viajeId && v.Pasajeros.Any(p => p.EmpleadoId == empleadoId))
                .QueSeCruzanCon(viaje.FechaHoraSalida, viaje.FechaHoraLlegada)
                .FirstOrDefaultAsync();
            if (cruce != null)
                throw new ViajeException(
                    $"{empleado.NombreCompleto} ya está inscrito en el viaje {cruce.Id} ({Horario(cruce)}), que se cruza con este.");

            _context.ViajePasajeros.Add(new ViajePasajero
            {
                ViajeId = viajeId,
                EmpleadoId = empleadoId,
                FechaInscripcion = DateTime.Now
            });
            await _context.SaveChangesAsync();
            await transaccion.CommitAsync();
        }

        public async Task QuitarPasajeroAsync(int viajeId, int empleadoId)
        {
            var viaje = await Activos().AsNoTracking().FirstOrDefaultAsync(v => v.Id == viajeId)
                ?? throw new ViajeException($"No existe el viaje {viajeId}.", TipoError.NoEncontrado);

            if (viaje.Estado != EstadoViaje.Programado)
                throw new ViajeException(
                    $"Solo se pueden quitar pasajeros de viajes programados; el viaje {viajeId} está {viaje.Estado}.");

            var inscripcion = await _context.ViajePasajeros
                .FirstOrDefaultAsync(p => p.ViajeId == viajeId && p.EmpleadoId == empleadoId)
                ?? throw new ViajeException(
                    $"El empleado {empleadoId} no está inscrito en el viaje {viajeId}.", TipoError.NoEncontrado);

            _context.ViajePasajeros.Remove(inscripcion);
            await _context.SaveChangesAsync();
        }

        public async Task CambiarEstadoAsync(int viajeId, EstadoViaje nuevoEstado)
        {
            var viaje = await Activos().FirstOrDefaultAsync(v => v.Id == viajeId)
                ?? throw new ViajeException($"No existe el viaje {viajeId}.", TipoError.NoEncontrado);

            var permitido = (viaje.Estado, nuevoEstado) switch
            {
                (EstadoViaje.Programado, EstadoViaje.EnCurso) => true,
                (EstadoViaje.Programado, EstadoViaje.Cancelado) => true,
                (EstadoViaje.EnCurso, EstadoViaje.Completado) => true,
                (EstadoViaje.EnCurso, EstadoViaje.Cancelado) => true,
                _ => false
            };

            if (!permitido)
                throw new ViajeException($"No se puede cambiar el viaje {viajeId} de {viaje.Estado} a {nuevoEstado}.");

            viaje.Estado = nuevoEstado;
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Valida ruta, vehículo, conductor y cruces de horario; calcula la llegada y el turno.
        /// Se usa al programar y al reprogramar un viaje.
        /// </summary>
        private async Task ValidarProgramacionAsync(Viaje viaje)
        {
            if (viaje.FechaHoraSalida <= DateTime.Now)
                throw new ViajeException("La fecha y hora de salida debe ser futura.", TipoError.Validacion);

            var ruta = await _context.Rutas.AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == viaje.RutaId && !r.Eliminado)
                ?? throw new ViajeException($"No existe la ruta {viaje.RutaId}.", TipoError.Validacion);

            var vehiculo = await _context.Vehiculos.AsNoTracking()
                .FirstOrDefaultAsync(v => v.Id == viaje.VehiculoId && !v.Eliminado)
                ?? throw new ViajeException($"No existe el vehículo {viaje.VehiculoId}.", TipoError.Validacion);

            var conductor = await _context.Empleados.AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == viaje.ConductorId && !e.Eliminado)
                ?? throw new ViajeException($"No existe el empleado {viaje.ConductorId}.", TipoError.Validacion);

            if (conductor.Tipo != TipoEmpleado.Conductor)
                throw new ViajeException(
                    $"{conductor.NombreCompleto} no es conductor; asigne un empleado de tipo Conductor.", TipoError.Validacion);

            if (vehiculo.Estado != EstadoVehiculo.Disponible)
                throw new ViajeException($"El vehículo {vehiculo.Placa} no está disponible (estado: {vehiculo.Estado}).");

            // La llegada se estima con la duración de la ruta y el turno con la hora de salida
            viaje.FechaHoraLlegada = viaje.FechaHoraSalida.AddMinutes(ruta.DuracionEstimadaMinutos);
            viaje.Turno = Viaje.TurnoSegunHora(viaje.FechaHoraSalida);

            // Control de horarios: otros viajes vigentes que se cruzan con este
            var viajesQueSeCruzan = _context.Viajes.AsNoTracking()
                .Vigentes()
                .Where(v => v.Id != viaje.Id)
                .QueSeCruzanCon(viaje.FechaHoraSalida, viaje.FechaHoraLlegada);

            var cruceVehiculo = await viajesQueSeCruzan
                .Where(v => v.VehiculoId == viaje.VehiculoId)
                .OrderBy(v => v.FechaHoraSalida)
                .FirstOrDefaultAsync();
            if (cruceVehiculo != null)
                throw new ViajeException(
                    $"El vehículo {vehiculo.Placa} ya está asignado al viaje {cruceVehiculo.Id} ({Horario(cruceVehiculo)}).");

            var cruceConductor = await viajesQueSeCruzan
                .Where(v => v.ConductorId == viaje.ConductorId)
                .OrderBy(v => v.FechaHoraSalida)
                .FirstOrDefaultAsync();
            if (cruceConductor != null)
                throw new ViajeException(
                    $"El conductor {conductor.NombreCompleto} ya está asignado al viaje {cruceConductor.Id} ({Horario(cruceConductor)}).");

            // Al reprogramar un viaje que ya tiene pasajeros
            if (viaje.Id != 0)
            {
                var inscritos = await _context.ViajePasajeros.CountAsync(p => p.ViajeId == viaje.Id);
                if (inscritos > vehiculo.Capacidad)
                    throw new ViajeException(
                        $"El vehículo {vehiculo.Placa} tiene {vehiculo.Capacidad} asientos y el viaje ya tiene {inscritos} pasajeros inscritos.");

                var pasajeroOcupado = await _context.ViajePasajeros.AsNoTracking()
                    .Where(p => p.ViajeId == viaje.Id &&
                                viajesQueSeCruzan.Any(o => o.Pasajeros.Any(op => op.EmpleadoId == p.EmpleadoId)))
                    .Select(p => p.Empleado!.Nombre + " " + p.Empleado.Apellido)
                    .FirstOrDefaultAsync();
                if (pasajeroOcupado != null)
                    throw new ViajeException($"El pasajero {pasajeroOcupado} tiene otro viaje en ese horario.");
            }
        }

        private static string Horario(Viaje v) => $"{v.FechaHoraSalida:dd/MM/yyyy HH:mm} - {v.FechaHoraLlegada:HH:mm}";
    }
}
