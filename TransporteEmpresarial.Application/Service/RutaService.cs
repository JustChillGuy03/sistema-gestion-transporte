using Microsoft.Extensions.Logging;
using TransporteEmpresarial.Application.Contract;
using TransporteEmpresarial.Application.Core;
using TransporteEmpresarial.Application.Dtos.Ruta;
using TransporteEmpresarial.Application.Exceptions;
using TransporteEmpresarial.Domain.Entities;
using TransporteEmpresarial.Infrastructure.Extensions;
using TransporteEmpresarial.Infrastructure.Interfaces;
using TransporteEmpresarial.Infrastructure.Models;

namespace TransporteEmpresarial.Application.Service
{
    public class RutaService : BaseService<Ruta, RutaModel, RutaDtoAdd, RutaDtoUpdate>, IRutaService
    {
        public const int MaximoParadas = 20;

        private readonly IRutaRepository _repositorio;

        public RutaService(IRutaRepository repositorio, ILogger<RutaService> logger) : base(repositorio, logger)
        {
            _repositorio = repositorio;
        }

        protected override NombreEntidad Entidad { get; } = new("ruta", "rutas", Femenino: true);

        public Task<ServiceResult<List<ViajeModel>>> GetViajesDeRutaAsync(int rutaId) =>
            EjecutarAsync(async () =>
            {
                ValidarId(rutaId);
                var viajes = await _repositorio.GetViajesDeRutaAsync(rutaId);
                return ServiceResult<List<ViajeModel>>.Ok(viajes, $"La ruta {rutaId} tiene {viajes.Count} viaje(s) registrados.");
            }, "consultar los viajes de la ruta");

        // ---------------- Validaciones de cada campo ----------------

        protected override void ValidarRegistro(RutaDtoAdd dto, Validador validador) => ValidarCampos(dto, validador);

        protected override void ValidarActualizacion(RutaDtoUpdate dto, Ruta actual, Validador validador) => ValidarCampos(dto, validador);

        private static void ValidarCampos(RutaDtoBase dto, Validador v)
        {
            // Nombre: obligatorio, 3 a 100 caracteres (único: lo verifica el repositorio)
            v.TextoObligatorio(nameof(dto.Nombre), dto.Nombre, 3, 100, "Nombre");

            // Origen y destino: obligatorios, 3 a 150 caracteres y distintos entre sí
            v.TextoObligatorio(nameof(dto.Origen), dto.Origen, 3, 150, "Origen");
            v.TextoObligatorio(nameof(dto.Destino), dto.Destino, 3, 150, "Destino");

            if (!v.TieneErrorEn(nameof(dto.Origen)) && !v.TieneErrorEn(nameof(dto.Destino)))
                v.Regla(!string.Equals(dto.Origen, dto.Destino, StringComparison.OrdinalIgnoreCase),
                        nameof(dto.Destino), "El destino debe ser diferente del origen.");

            // Paradas intermedias: opcionales; máximo 20, sin vacías, sin repetidas y distintas del origen y el destino
            var paradas = dto.ParadasIntermedias ?? new List<string>();
            var campo = nameof(dto.ParadasIntermedias);

            v.Regla(paradas.Count <= MaximoParadas, campo,
                    $"Una ruta puede tener como máximo {MaximoParadas} paradas intermedias.");

            for (var i = 0; i < paradas.Count; i++)
            {
                var parada = paradas[i];

                if (parada.Length == 0)
                    v.Agregar(campo, $"La parada {i + 1} está vacía.");
                else if (parada.Length < 3 || parada.Length > 150)
                    v.Agregar(campo, $"La parada {i + 1} debe tener entre 3 y 150 caracteres.");
                else if (string.Equals(parada, dto.Origen, StringComparison.OrdinalIgnoreCase) ||
                         string.Equals(parada, dto.Destino, StringComparison.OrdinalIgnoreCase))
                    v.Agregar(campo, $"La parada \"{parada}\" no puede ser el origen ni el destino de la ruta.");
            }

            var repetidas = paradas.Where(p => p.Length > 0)
                                   .GroupBy(p => p, StringComparer.OrdinalIgnoreCase)
                                   .Where(g => g.Count() > 1)
                                   .Select(g => g.Key);
            foreach (var parada in repetidas)
                v.Agregar(campo, $"La parada \"{parada}\" está repetida.");

            // Duración estimada: obligatoria, de 5 a 600 minutos (10 horas)
            v.Rango(nameof(dto.DuracionEstimadaMinutos), dto.DuracionEstimadaMinutos, 5, 600, "Duración estimada (minutos)");
        }

        // ---------------- Mapeo DTO <-> entidad ----------------

        protected override Ruta CrearEntidad(RutaDtoAdd dto)
        {
            var ruta = new Ruta();
            CopiarCampos(ruta, dto);
            return ruta;
        }

        protected override void ActualizarEntidad(Ruta ruta, RutaDtoUpdate dto) => CopiarCampos(ruta, dto);

        private static void CopiarCampos(Ruta ruta, RutaDtoBase dto)
        {
            ruta.Nombre = dto.Nombre!;
            ruta.Origen = dto.Origen!;
            ruta.Destino = dto.Destino!;
            ruta.ParadasIntermedias = dto.ParadasIntermedias?.ToList() ?? new List<string>();
            ruta.DuracionEstimadaMinutos = dto.DuracionEstimadaMinutos!.Value;
        }

        protected override async Task<List<RutaModel>> ConsultarTodosAsync() =>
            (await _repositorio.GetAllAsync()).Select(r => r.ToModel()).ToList();

        protected override Task<RutaModel> ConsultarModeloAsync(Ruta ruta) => Task.FromResult(ruta.ToModel());

        protected override ServiceException CrearExcepcion(string mensaje, ServiceErrorType tipo,
                                                           IReadOnlyDictionary<string, string[]>? errores = null) =>
            new RutaServiceException(mensaje, tipo, errores);
    }
}
