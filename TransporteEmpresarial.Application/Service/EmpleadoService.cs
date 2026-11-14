using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using TransporteEmpresarial.Application.Contract;
using TransporteEmpresarial.Application.Core;
using TransporteEmpresarial.Application.Dtos.Empleado;
using TransporteEmpresarial.Application.Exceptions;
using TransporteEmpresarial.Domain.Entities;
using TransporteEmpresarial.Domain.Enums;
using TransporteEmpresarial.Infrastructure.Extensions;
using TransporteEmpresarial.Infrastructure.Interfaces;
using TransporteEmpresarial.Infrastructure.Models;

namespace TransporteEmpresarial.Application.Service
{
    public class EmpleadoService : BaseService<Empleado, EmpleadoModel, EmpleadoDtoAdd, EmpleadoDtoUpdate>, IEmpleadoService
    {
        // Cédula dominicana: 000-0000000-0
        private static readonly Regex FormatoCedula = new(@"^\d{3}-\d{7}-\d$", RegexOptions.Compiled);

        // Letras (con tildes y ñ), espacios, apóstrofes, puntos y guiones: "María José", "O'Neil", "Pérez-Gómez"
        private static readonly Regex FormatoNombre = new(@"^\p{L}[\p{L} '.\-]*$", RegexOptions.Compiled);

        // Dígitos con espacios, guiones, paréntesis y + opcional: "809-555-0101", "+1 (829) 555-0202"
        private static readonly Regex FormatoTelefono = new(@"^\+?[\d\s()\-]+$", RegexOptions.Compiled);

        private static readonly Regex FormatoCorreo = new(@"^[^@\s]+@[^@\s]+\.[A-Za-z]{2,}$", RegexOptions.Compiled);

        // Letras, números y guiones: "LIC-0001"
        private static readonly Regex FormatoLicencia = new(@"^[A-Z0-9][A-Z0-9\-]*$", RegexOptions.Compiled);

        private readonly IEmpleadoRepository _repositorio;

        public EmpleadoService(IEmpleadoRepository repositorio, ILogger<EmpleadoService> logger) : base(repositorio, logger)
        {
            _repositorio = repositorio;
        }

        protected override NombreEntidad Entidad { get; } = new("empleado", "empleados", Femenino: false);

        public Task<ServiceResult<List<EmpleadoModel>>> GetPorTipoAsync(TipoEmpleado tipo) =>
            EjecutarAsync(async () =>
            {
                LanzarSiHayErrores(new Validador().EnumValido("tipo", (TipoEmpleado?)tipo, "tipo"));

                var empleados = await _repositorio.GetPorTipoAsync(tipo);
                return ServiceResult<List<EmpleadoModel>>.Ok(empleados, $"Se encontraron {empleados.Count} empleado(s) de tipo {tipo}.");
            }, "consultar los empleados por tipo");

        public Task<ServiceResult<List<TrasladoModel>>> GetHistorialAsync(int empleadoId) =>
            EjecutarAsync(async () =>
            {
                ValidarId(empleadoId);
                var historial = await _repositorio.GetHistorialAsync(empleadoId);
                return ServiceResult<List<TrasladoModel>>.Ok(historial, $"El empleado {empleadoId} tiene {historial.Count} viaje(s) en su historial.");
            }, "consultar el historial del empleado");

        // ---------------- Validaciones de cada campo ----------------

        protected override void ValidarRegistro(EmpleadoDtoAdd dto, Validador validador)
        {
            // Tipo: obligatorio, Pasajero o Conductor
            validador.EnumValido(nameof(dto.Tipo), dto.Tipo, "Tipo");

            ValidarCampos(dto, dto.Tipo, validador);
        }

        // El tipo no cambia al actualizar: se valida con el tipo que ya tiene el empleado
        protected override void ValidarActualizacion(EmpleadoDtoUpdate dto, Empleado actual, Validador validador) =>
            ValidarCampos(dto, actual.Tipo, validador);

        private static void ValidarCampos(EmpleadoDtoBase dto, TipoEmpleado? tipo, Validador v)
        {
            // Cédula: obligatoria con formato 000-0000000-0 (única: lo verifica el repositorio)
            v.Requerido(nameof(dto.Cedula), dto.Cedula, "Cédula");
            v.Formato(nameof(dto.Cedula), dto.Cedula, FormatoCedula,
                      "La cédula debe tener el formato 000-0000000-0 (11 dígitos).");

            // Nombre y apellido: obligatorios, 2 a 60 caracteres, solo letras
            v.TextoObligatorio(nameof(dto.Nombre), dto.Nombre, 2, 60, "Nombre");
            v.Formato(nameof(dto.Nombre), dto.Nombre, FormatoNombre,
                      "El nombre solo admite letras, espacios, apóstrofes, puntos y guiones.");

            v.TextoObligatorio(nameof(dto.Apellido), dto.Apellido, 2, 60, "Apellido");
            v.Formato(nameof(dto.Apellido), dto.Apellido, FormatoNombre,
                      "El apellido solo admite letras, espacios, apóstrofes, puntos y guiones.");

            // Teléfono: opcional; si viene, 7 a 20 caracteres y entre 7 y 15 dígitos
            v.TextoOpcional(nameof(dto.Telefono), dto.Telefono, 7, 20, "Teléfono");
            v.Formato(nameof(dto.Telefono), dto.Telefono, FormatoTelefono,
                      "El teléfono solo admite dígitos, espacios, guiones, paréntesis y el signo + al inicio (ejemplo: 809-555-0101).");
            if (dto.Telefono is not null && !v.TieneErrorEn(nameof(dto.Telefono)))
            {
                var digitos = dto.Telefono.Count(char.IsDigit);
                v.Regla(digitos is >= 7 and <= 15, nameof(dto.Telefono), "El teléfono debe tener entre 7 y 15 dígitos.");
            }

            // Correo: opcional; si viene, máximo 100 caracteres y formato usuario@dominio.com
            v.TextoOpcional(nameof(dto.Correo), dto.Correo, 6, 100, "Correo");
            v.Formato(nameof(dto.Correo), dto.Correo, FormatoCorreo,
                      "El correo no tiene un formato válido (ejemplo: usuario@empresa.com).");

            // Departamento: opcional, 2 a 60 caracteres
            v.TextoOpcional(nameof(dto.Departamento), dto.Departamento, 2, 60, "Departamento");

            // Punto de recogida y licencia dependen del tipo de empleado
            switch (tipo)
            {
                case TipoEmpleado.Pasajero:
                    // El transporte necesita saber dónde recoger al pasajero
                    v.TextoObligatorio(nameof(dto.PuntoRecogida), dto.PuntoRecogida, 3, 150, "Punto de recogida");
                    v.Vacio(nameof(dto.LicenciaConducir), dto.LicenciaConducir,
                            "La licencia de conducir solo se registra para empleados de tipo Conductor.");
                    break;

                case TipoEmpleado.Conductor:
                    v.TextoObligatorio(nameof(dto.LicenciaConducir), dto.LicenciaConducir, 5, 20, "Licencia de conducir");
                    v.Formato(nameof(dto.LicenciaConducir), dto.LicenciaConducir, FormatoLicencia,
                              "La licencia solo admite letras, números y guiones (ejemplo: LIC-0001).");
                    v.Vacio(nameof(dto.PuntoRecogida), dto.PuntoRecogida,
                            "El punto de recogida solo se registra para empleados de tipo Pasajero.");
                    break;
            }
        }

        // ---------------- Mapeo DTO <-> entidad ----------------

        protected override Empleado CrearEntidad(EmpleadoDtoAdd dto)
        {
            var empleado = new Empleado { Tipo = dto.Tipo!.Value };
            CopiarCampos(empleado, dto);
            return empleado;
        }

        protected override void ActualizarEntidad(Empleado empleado, EmpleadoDtoUpdate dto) => CopiarCampos(empleado, dto);

        private static void CopiarCampos(Empleado empleado, EmpleadoDtoBase dto)
        {
            empleado.Cedula = dto.Cedula!;
            empleado.Nombre = dto.Nombre!;
            empleado.Apellido = dto.Apellido!;
            empleado.Telefono = dto.Telefono;
            empleado.Correo = dto.Correo;
            empleado.Departamento = dto.Departamento;
            empleado.PuntoRecogida = dto.PuntoRecogida;
            empleado.LicenciaConducir = dto.LicenciaConducir;
        }

        protected override async Task<List<EmpleadoModel>> ConsultarTodosAsync() =>
            (await _repositorio.GetAllAsync()).Select(e => e.ToModel()).ToList();

        protected override Task<EmpleadoModel> ConsultarModeloAsync(Empleado empleado) => Task.FromResult(empleado.ToModel());

        protected override ServiceException CrearExcepcion(string mensaje, ServiceErrorType tipo,
                                                           IReadOnlyDictionary<string, string[]>? errores = null) =>
            new EmpleadoServiceException(mensaje, tipo, errores);
    }
}
