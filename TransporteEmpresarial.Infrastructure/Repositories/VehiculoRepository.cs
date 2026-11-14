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
    public class VehiculoRepository : BaseRepository<Vehiculo>, IVehiculoRepository
    {
        public VehiculoRepository(TransporteContext context) : base(context) { }

        public override async Task AddAsync(Vehiculo vehiculo)
        {
            await ValidarPlacaUnicaAsync(vehiculo);
            await base.AddAsync(vehiculo);
        }

        public override async Task UpdateAsync(Vehiculo vehiculo)
        {
            await ValidarPlacaUnicaAsync(vehiculo);

            var viajesVigentes = _context.Viajes.Vigentes().Where(v => v.VehiculoId == vehiculo.Id);

            // Un vehículo con viajes pendientes no puede pasar a mantenimiento ni fuera de servicio
            if (vehiculo.Estado != EstadoVehiculo.Disponible && await viajesVigentes.AnyAsync())
                throw new VehiculoException(
                    $"El vehículo {vehiculo.Placa} tiene viajes programados o en curso; reasígnelos o cancélelos antes de cambiar su estado a {vehiculo.Estado}.");

            // La capacidad no puede quedar por debajo de los pasajeros ya inscritos
            var inscritosPorViaje = await viajesVigentes.Select(v => v.Pasajeros.Count).ToListAsync();
            var maximoInscritos = inscritosPorViaje.DefaultIfEmpty(0).Max();
            if (vehiculo.Capacidad < maximoInscritos)
                throw new VehiculoException(
                    $"No se puede reducir la capacidad a {vehiculo.Capacidad}: un viaje vigente de este vehículo ya tiene {maximoInscritos} pasajeros inscritos.");

            await base.UpdateAsync(vehiculo);
        }

        public override async Task RemoveAsync(Vehiculo vehiculo)
        {
            if (await _context.Viajes.Vigentes().AnyAsync(v => v.VehiculoId == vehiculo.Id))
                throw new VehiculoException($"No se puede eliminar el vehículo {vehiculo.Placa}: tiene viajes programados o en curso.");

            await base.RemoveAsync(vehiculo);
        }

        public async Task<List<VehiculoModel>> GetDisponiblesAsync(DateTime desde, DateTime hasta)
        {
            // Vehículos ocupados por algún viaje vigente en ese intervalo
            var ocupados = _context.Viajes.Vigentes()
                                          .QueSeCruzanCon(desde, hasta)
                                          .Select(v => v.VehiculoId);

            var vehiculos = await Activos().AsNoTracking()
                                           .Where(v => v.Estado == EstadoVehiculo.Disponible && !ocupados.Contains(v.Id))
                                           .OrderBy(v => v.Id)
                                           .ToListAsync();

            return vehiculos.Select(v => v.ToModel()).ToList();
        }

        private async Task ValidarPlacaUnicaAsync(Vehiculo vehiculo)
        {
            if (await ExistsAsync(v => v.Placa == vehiculo.Placa && v.Id != vehiculo.Id))
                throw new VehiculoException($"Ya existe un vehículo con la placa {vehiculo.Placa}.");
        }
    }
}
