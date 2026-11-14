using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using TransporteEmpresarial.Application.Contract;
using TransporteEmpresarial.Application.Core;
using TransporteEmpresarial.Application.Dtos.Vehiculo;
using TransporteEmpresarial.Application.Exceptions;
using TransporteEmpresarial.Domain.Entities;
using TransporteEmpresarial.Domain.Enums;
using TransporteEmpresarial.Infrastructure.Extensions;
using TransporteEmpresarial.Infrastructure.Interfaces;
using TransporteEmpresarial.Infrastructure.Models;

namespace TransporteEmpresarial.Application.Service
{
    public class VehiculoService : BaseService<Vehiculo, VehiculoModel, VehiculoDtoAdd, VehiculoDtoUpdate>, IVehiculoService
    {
        public const int AnioMinimo = 1990;
        public const int CapacidadMaxima = 60;

        // Letras, números y guiones; debe tener al menos una letra y un número (ej.: A123456, I000101)
        private static readonly Regex FormatoPlaca = new(@"^(?=.*[A-Z])(?=.*\d)[A-Z0-9-]{5,10}$", RegexOptions.Compiled);

        // Letras (con tildes), números, espacios y algunos signos: "Hiace", "Coaster 4x4", "H-1"
        private static readonly Regex FormatoMarcaModelo = new(@"^[\p{L}\d][\p{L}\d .\-/]*$", RegexOptions.Compiled);

        private readonly IVehiculoRepository _repositorio;

        public VehiculoService(IVehiculoRepository repositorio, ILogger<VehiculoService> logger) : base(repositorio, logger)
        {
            _repositorio = repositorio;
        }

        protected override NombreEntidad Entidad { get; } = new("vehículo", "vehículos", Femenino: false);

        public Task<ServiceResult<List<VehiculoModel>>> GetDisponiblesAsync(DateTime? desde, DateTime? hasta) =>
            EjecutarAsync(async () =>
            {
                var v = new Validador()
                    .Obligatorio("desde", desde, "desde")
                    .Obligatorio("hasta", hasta, "hasta");

                if (desde.HasValue && hasta.HasValue)
                    v.Regla(hasta > desde, "hasta", "El parámetro 'hasta' debe ser posterior a 'desde'.");

                LanzarSiHayErrores(v);

                var vehiculos = await _repositorio.GetDisponiblesAsync(desde!.Value, hasta!.Value);
                return ServiceResult<List<VehiculoModel>>.Ok(vehiculos, $"{vehiculos.Count} vehículo(s) disponible(s) en ese horario.");
            }, "consultar los vehículos disponibles");

        // ---------------- Validaciones de cada campo ----------------

        protected override void ValidarRegistro(VehiculoDtoAdd dto, Validador validador) => ValidarCampos(dto, validador);

        protected override void ValidarActualizacion(VehiculoDtoUpdate dto, Vehiculo actual, Validador validador)
        {
            ValidarCampos(dto, validador);

            // Estado: obligatorio y debe ser Disponible, EnMantenimiento o FueraDeServicio
            validador.EnumValido(nameof(dto.Estado), dto.Estado, "Estado");
        }

        private static void ValidarCampos(VehiculoDtoBase dto, Validador v)
        {
            // Placa: obligatoria, 5 a 10 caracteres, letras y números (única: lo verifica el repositorio)
            v.TextoObligatorio(nameof(dto.Placa), dto.Placa, 5, 10, "Placa");
            v.Formato(nameof(dto.Placa), dto.Placa, FormatoPlaca,
                      "La placa solo admite letras, números y guiones, y debe tener al menos una letra y un número (ejemplo: A123456).");

            // Marca: obligatoria, 2 a 50 caracteres
            v.TextoObligatorio(nameof(dto.Marca), dto.Marca, 2, 50, "Marca");
            v.Formato(nameof(dto.Marca), dto.Marca, FormatoMarcaModelo,
                      "La marca solo admite letras, números, espacios, puntos, guiones y barras.");

            // Modelo: obligatorio, 1 a 50 caracteres
            v.TextoObligatorio(nameof(dto.Modelo), dto.Modelo, 1, 50, "Modelo");
            v.Formato(nameof(dto.Modelo), dto.Modelo, FormatoMarcaModelo,
                      "El modelo solo admite letras, números, espacios, puntos, guiones y barras.");

            // Año: obligatorio, desde 1990 hasta el año próximo (modelos del año siguiente)
            v.Rango(nameof(dto.Anio), dto.Anio, AnioMinimo, DateTime.Today.Year + 1, "Año");

            // Capacidad: obligatoria, de 1 a 60 asientos
            v.Rango(nameof(dto.Capacidad), dto.Capacidad, 1, CapacidadMaxima, "Capacidad");
        }

        // ---------------- Mapeo DTO <-> entidad ----------------

        protected override Vehiculo CrearEntidad(VehiculoDtoAdd dto)
        {
            var vehiculo = new Vehiculo { Estado = EstadoVehiculo.Disponible };
            CopiarCampos(vehiculo, dto);
            return vehiculo;
        }

        protected override void ActualizarEntidad(Vehiculo vehiculo, VehiculoDtoUpdate dto)
        {
            CopiarCampos(vehiculo, dto);
            vehiculo.Estado = dto.Estado!.Value;
        }

        private static void CopiarCampos(Vehiculo vehiculo, VehiculoDtoBase dto)
        {
            vehiculo.Placa = dto.Placa!;
            vehiculo.Marca = dto.Marca!;
            vehiculo.Modelo = dto.Modelo!;
            vehiculo.Anio = dto.Anio!.Value;
            vehiculo.Capacidad = dto.Capacidad!.Value;
        }

        protected override async Task<List<VehiculoModel>> ConsultarTodosAsync() =>
            (await _repositorio.GetAllAsync()).Select(v => v.ToModel()).ToList();

        protected override Task<VehiculoModel> ConsultarModeloAsync(Vehiculo vehiculo) => Task.FromResult(vehiculo.ToModel());

        protected override ServiceException CrearExcepcion(string mensaje, ServiceErrorType tipo,
                                                           IReadOnlyDictionary<string, string[]>? errores = null) =>
            new VehiculoServiceException(mensaje, tipo, errores);
    }
}
