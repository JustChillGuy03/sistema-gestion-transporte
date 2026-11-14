using Microsoft.Extensions.Logging;
using TransporteEmpresarial.Application.Contract;
using TransporteEmpresarial.Application.Core;
using TransporteEmpresarial.Application.Dtos.Viaje;
using TransporteEmpresarial.Application.Exceptions;
using TransporteEmpresarial.Domain.Entities;
using TransporteEmpresarial.Domain.Enums;
using TransporteEmpresarial.Infrastructure.Interfaces;
using TransporteEmpresarial.Infrastructure.Models;

namespace TransporteEmpresarial.Application.Service
{
    /// <summary>
    /// Programación de viajes, inscripción de pasajeros y cambios de estado.
    /// Este servicio valida los datos de entrada; las reglas que dependen de la base de datos
    /// (cupo del vehículo, cruces de horario, disponibilidad) las aplica el repositorio.
    /// </summary>
    public class ViajeService : BaseService<Viaje, ViajeModel, ViajeDtoAdd, ViajeDtoUpdate>, IViajeService
    {
        // Un viaje se puede programar como máximo con un año de anticipación
        public const int MesesMaximosDeAnticipacion = 12;

        private readonly IViajeRepository _repositorio;

        public ViajeService(IViajeRepository repositorio, ILogger<ViajeService> logger) : base(repositorio, logger)
        {
            _repositorio = repositorio;
        }

        protected override NombreEntidad Entidad { get; } = new("viaje", "viajes", Femenino: false);

        public Task<ServiceResult<List<ViajeModel>>> GetViajesAsync(DateTime? fecha, EstadoViaje? estado) =>
            EjecutarAsync(async () =>
            {
                if (estado.HasValue)
                    LanzarSiHayErrores(new Validador().EnumValido("estado", estado, "estado"));

                var viajes = await _repositorio.GetViajesAsync(fecha, estado);
                return ServiceResult<List<ViajeModel>>.Ok(viajes, $"Se encontraron {viajes.Count} viaje(s).");
            }, "consultar los viajes");

        public Task<ServiceResult<List<PasajeroModel>>> GetPasajerosAsync(int viajeId) =>
            EjecutarAsync(async () =>
            {
                ValidarId(viajeId, "viajeId");
                var pasajeros = await _repositorio.GetPasajerosAsync(viajeId);
                return ServiceResult<List<PasajeroModel>>.Ok(pasajeros, $"El viaje {viajeId} tiene {pasajeros.Count} pasajero(s) inscrito(s).");
            }, "consultar los pasajeros del viaje");

        public Task<ServiceResult<ViajeModel>> InscribirPasajeroAsync(int viajeId, int empleadoId) =>
            EjecutarAsync(async () =>
            {
                ValidarId(viajeId, "viajeId");
                ValidarId(empleadoId, "empleadoId");

                // El repositorio valida el cupo dentro de una transacción serializable
                await _repositorio.InscribirPasajeroAsync(viajeId, empleadoId);

                var viaje = await ConsultarPorIdAsync(viajeId);
                return ServiceResult<ViajeModel>.Ok(viaje,
                    $"Empleado {empleadoId} inscrito en el viaje {viajeId}. Quedan {viaje.CuposDisponibles} cupo(s).");
            }, "inscribir al pasajero");

        public Task<ServiceResult> QuitarPasajeroAsync(int viajeId, int empleadoId) =>
            EjecutarAsync(async () =>
            {
                ValidarId(viajeId, "viajeId");
                ValidarId(empleadoId, "empleadoId");

                await _repositorio.QuitarPasajeroAsync(viajeId, empleadoId);
                return ServiceResult.Ok($"Empleado {empleadoId} retirado del viaje {viajeId}.");
            }, "quitar al pasajero");

        public Task<ServiceResult<ViajeModel>> CambiarEstadoAsync(int viajeId, ViajeDtoCambiarEstado dto) =>
            EjecutarAsync(async () =>
            {
                ValidarId(viajeId, "viajeId");
                VerificarQueExista(dto);

                // Estado: obligatorio, válido y distinto de Programado (un viaje no vuelve atrás)
                var v = new Validador().EnumValido(nameof(dto.Estado), dto.Estado, "Estado");
                if (!v.TieneErrorEn(nameof(dto.Estado)))
                    v.Regla(dto.Estado != EstadoViaje.Programado, nameof(dto.Estado),
                            "Un viaje no puede volver a Programado. Estados permitidos: EnCurso, Completado o Cancelado.");
                LanzarSiHayErrores(v);

                // Las transiciones permitidas según el estado actual las valida el repositorio
                await _repositorio.CambiarEstadoAsync(viajeId, dto.Estado!.Value);

                return ServiceResult<ViajeModel>.Ok(await ConsultarPorIdAsync(viajeId),
                                                    $"El viaje {viajeId} cambió a {dto.Estado}.");
            }, "cambiar el estado del viaje");

        // ---------------- Validaciones de cada campo ----------------

        protected override void ValidarRegistro(ViajeDtoAdd dto, Validador validador) => ValidarCampos(dto, validador);

        protected override void ValidarActualizacion(ViajeDtoUpdate dto, Viaje actual, Validador validador) => ValidarCampos(dto, validador);

        private static void ValidarCampos(ViajeDtoBase dto, Validador v)
        {
            // Ruta, vehículo y conductor: obligatorios y mayores que cero
            // (que existan, que el vehículo esté Disponible y que el empleado sea Conductor lo valida el repositorio)
            v.Identificador(nameof(dto.RutaId), dto.RutaId, "RutaId");
            v.Identificador(nameof(dto.VehiculoId), dto.VehiculoId, "VehiculoId");
            v.Identificador(nameof(dto.ConductorId), dto.ConductorId, "ConductorId");

            // Salida: obligatoria, futura y con máximo un año de anticipación
            v.Obligatorio(nameof(dto.FechaHoraSalida), dto.FechaHoraSalida, "Fecha y hora de salida");
            if (dto.FechaHoraSalida.HasValue)
            {
                var ahora = DateTime.Now;
                v.Regla(dto.FechaHoraSalida.Value > ahora, nameof(dto.FechaHoraSalida),
                        "La fecha y hora de salida debe ser futura.");
                v.Regla(dto.FechaHoraSalida.Value <= ahora.AddMonths(MesesMaximosDeAnticipacion), nameof(dto.FechaHoraSalida),
                        $"Un viaje se puede programar con un máximo de {MesesMaximosDeAnticipacion} meses de anticipación.");
            }
        }

        // ---------------- Mapeo DTO <-> entidad ----------------

        protected override Viaje CrearEntidad(ViajeDtoAdd dto)
        {
            var viaje = new Viaje { Estado = EstadoViaje.Programado };
            CopiarCampos(viaje, dto);
            return viaje;
        }

        protected override void ActualizarEntidad(Viaje viaje, ViajeDtoUpdate dto) => CopiarCampos(viaje, dto);

        // La llegada y el turno los calcula el repositorio al validar la programación
        private static void CopiarCampos(Viaje viaje, ViajeDtoBase dto)
        {
            viaje.RutaId = dto.RutaId!.Value;
            viaje.VehiculoId = dto.VehiculoId!.Value;
            viaje.ConductorId = dto.ConductorId!.Value;
            viaje.FechaHoraSalida = dto.FechaHoraSalida!.Value;
        }

        // El modelo de un viaje incluye el nombre de la ruta, la placa, el conductor y los cupos
        protected override Task<List<ViajeModel>> ConsultarTodosAsync() => _repositorio.GetViajesAsync(null, null);

        protected override Task<ViajeModel> ConsultarModeloAsync(Viaje viaje) => ConsultarPorIdAsync(viaje.Id);

        private async Task<ViajeModel> ConsultarPorIdAsync(int viajeId) =>
            await _repositorio.GetViajeAsync(viajeId)
            ?? throw CrearExcepcion($"No existe el viaje {viajeId}.", ServiceErrorType.NotFound);

        protected override ServiceException CrearExcepcion(string mensaje, ServiceErrorType tipo,
                                                           IReadOnlyDictionary<string, string[]>? errores = null) =>
            new ViajeServiceException(mensaje, tipo, errores);
    }
}
