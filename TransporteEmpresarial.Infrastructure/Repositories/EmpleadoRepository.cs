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
    public class EmpleadoRepository : BaseRepository<Empleado>, IEmpleadoRepository
    {
        public EmpleadoRepository(TransporteContext context) : base(context) { }

        public override async Task AddAsync(Empleado empleado)
        {
            await ValidarAsync(empleado);
            await base.AddAsync(empleado);
        }

        public override async Task UpdateAsync(Empleado empleado)
        {
            await ValidarAsync(empleado);
            await base.UpdateAsync(empleado);
        }

        public override async Task RemoveAsync(Empleado empleado)
        {
            var viajesVigentes = _context.Viajes.Vigentes();

            if (await viajesVigentes.AnyAsync(v => v.ConductorId == empleado.Id))
                throw new EmpleadoException(
                    $"No se puede eliminar a {empleado.NombreCompleto}: es conductor de viajes programados o en curso.");

            if (await viajesVigentes.AnyAsync(v => v.Pasajeros.Any(p => p.EmpleadoId == empleado.Id)))
                throw new EmpleadoException(
                    $"No se puede eliminar a {empleado.NombreCompleto}: está inscrito en viajes programados o en curso.");

            await base.RemoveAsync(empleado);
        }

        public async Task<List<EmpleadoModel>> GetPorTipoAsync(TipoEmpleado tipo)
        {
            var empleados = await Activos().AsNoTracking()
                                           .Where(e => e.Tipo == tipo)
                                           .OrderBy(e => e.Id)
                                           .ToListAsync();

            return empleados.Select(e => e.ToModel()).ToList();
        }

        public async Task<List<TrasladoModel>> GetHistorialAsync(int empleadoId)
        {
            if (!await ExistsAsync(e => e.Id == empleadoId))
                throw new EmpleadoException($"No existe el empleado {empleadoId}.", TipoError.NoEncontrado);

            return await _context.Viajes.AsNoTracking()
                .Where(v => !v.Eliminado &&
                            (v.ConductorId == empleadoId || v.Pasajeros.Any(p => p.EmpleadoId == empleadoId)))
                .OrderByDescending(v => v.FechaHoraSalida)
                .Select(v => new TrasladoModel
                {
                    ViajeId = v.Id,
                    Rol = v.ConductorId == empleadoId ? TipoEmpleado.Conductor : TipoEmpleado.Pasajero,
                    Ruta = v.Ruta!.Nombre,
                    Origen = v.Ruta.Origen,
                    Destino = v.Ruta.Destino,
                    FechaHoraSalida = v.FechaHoraSalida,
                    FechaHoraLlegada = v.FechaHoraLlegada,
                    Estado = v.Estado,
                    Placa = v.Vehiculo!.Placa,
                    Conductor = v.Conductor!.Nombre + " " + v.Conductor.Apellido
                })
                .ToListAsync();
        }

        private async Task ValidarAsync(Empleado empleado)
        {
            if (empleado.Tipo == TipoEmpleado.Conductor && string.IsNullOrWhiteSpace(empleado.LicenciaConducir))
                throw new EmpleadoException("Un conductor debe tener licencia de conducir.", TipoError.Validacion);

            if (await ExistsAsync(e => e.Cedula == empleado.Cedula && e.Id != empleado.Id))
                throw new EmpleadoException($"Ya existe un empleado con la cédula {empleado.Cedula}.");
        }
    }
}
