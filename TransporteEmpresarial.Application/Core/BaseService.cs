using Microsoft.Extensions.Logging;
using TransporteEmpresarial.Application.Dtos;
using TransporteEmpresarial.Application.Exceptions;
using TransporteEmpresarial.Domain.Core;
using TransporteEmpresarial.Domain.Repository;
using TransporteEmpresarial.Infrastructure.Exceptions;

namespace TransporteEmpresarial.Application.Core
{
    /// <summary>
    /// Implementación común del CRUD de los servicios.
    /// Cada operación sigue los mismos pasos: normalizar el DTO, validar cada campo,
    /// convertirlo en entidad, llamar al repositorio y devolver un ServiceResult.
    /// Los servicios concretos solo definen sus validaciones y su mapeo.
    /// </summary>
    public abstract class BaseService<TEntity, TModel, TDtoAdd, TDtoUpdate> : IBaseService<TModel, TDtoAdd, TDtoUpdate>
        where TEntity : BaseEntity
        where TDtoAdd : DtoBase
        where TDtoUpdate : DtoBase
    {
        private readonly IBaseRepository<TEntity> _repositorio;
        protected readonly ILogger Logger;

        protected BaseService(IBaseRepository<TEntity> repositorio, ILogger logger)
        {
            _repositorio = repositorio;
            Logger = logger;
        }

        // ---------------- Lo que define cada servicio ----------------

        protected abstract NombreEntidad Entidad { get; }

        protected abstract Task<List<TModel>> ConsultarTodosAsync();
        protected abstract Task<TModel> ConsultarModeloAsync(TEntity entidad);

        // Validaciones de cada campo del DTO
        protected abstract void ValidarRegistro(TDtoAdd dto, Validador validador);
        protected abstract void ValidarActualizacion(TDtoUpdate dto, TEntity actual, Validador validador);

        protected abstract TEntity CrearEntidad(TDtoAdd dto);
        protected abstract void ActualizarEntidad(TEntity entidad, TDtoUpdate dto);

        // Excepción propia del servicio (RutaServiceException, VehiculoServiceException...)
        protected abstract ServiceException CrearExcepcion(string mensaje, ServiceErrorType tipo,
                                                           IReadOnlyDictionary<string, string[]>? errores = null);

        // ---------------- CRUD ----------------

        public virtual Task<ServiceResult<List<TModel>>> GetAllAsync() =>
            EjecutarAsync(async () =>
            {
                var lista = await ConsultarTodosAsync();
                return ServiceResult<List<TModel>>.Ok(lista, MensajeCantidad(lista.Count));
            }, $"consultar {Entidad.PluralConArticulo}");

        public virtual Task<ServiceResult<TModel>> GetByIdAsync(int id) =>
            EjecutarAsync(async () =>
            {
                var entidad = await ObtenerExistenteAsync(id);
                return ServiceResult<TModel>.Ok(await ConsultarModeloAsync(entidad),
                                                $"{Entidad.Titulo} {id} encontrad{Entidad.Terminacion}.");
            }, $"consultar {Entidad.ConArticulo}");

        public virtual Task<ServiceResult<TModel>> SaveAsync(TDtoAdd dto) =>
            EjecutarAsync(async () =>
            {
                VerificarQueExista(dto);
                dto.Normalizar();

                var validador = new Validador();
                ValidarRegistro(dto, validador);
                LanzarSiHayErrores(validador);

                var entidad = CrearEntidad(dto);
                await _repositorio.AddAsync(entidad);

                return ServiceResult<TModel>.Ok(await ConsultarModeloAsync(entidad),
                                                $"{Entidad.Titulo} registrad{Entidad.Terminacion} correctamente.");
            }, $"registrar {Entidad.ConArticulo}");

        public virtual Task<ServiceResult<TModel>> UpdateAsync(int id, TDtoUpdate dto) =>
            EjecutarAsync(async () =>
            {
                VerificarQueExista(dto);
                var entidad = await ObtenerExistenteAsync(id);
                dto.Normalizar();

                var validador = new Validador();
                ValidarActualizacion(dto, entidad, validador);
                LanzarSiHayErrores(validador);

                ActualizarEntidad(entidad, dto);
                await _repositorio.UpdateAsync(entidad);

                return ServiceResult<TModel>.Ok(await ConsultarModeloAsync(entidad),
                                                $"{Entidad.Titulo} {id} actualizad{Entidad.Terminacion} correctamente.");
            }, $"actualizar {Entidad.ConArticulo}");

        public virtual Task<ServiceResult> RemoveAsync(int id) =>
            EjecutarAsync(async () =>
            {
                var entidad = await ObtenerExistenteAsync(id);
                await _repositorio.RemoveAsync(entidad);

                return ServiceResult.Ok($"{Entidad.Titulo} {id} eliminad{Entidad.Terminacion} correctamente.");
            }, $"eliminar {Entidad.ConArticulo}");

        // ---------------- Ayudas para los servicios ----------------

        /// <summary>
        /// Busca el registro y lanza la excepción del servicio (404) si no existe.
        /// </summary>
        protected async Task<TEntity> ObtenerExistenteAsync(int id)
        {
            ValidarId(id);

            return await _repositorio.GetByIdAsync(id)
                ?? throw CrearExcepcion($"No existe {Entidad.ConArticulo} {id}.", ServiceErrorType.NotFound);
        }

        protected void ValidarId(int id, string campo = "Id")
        {
            if (id <= 0)
                LanzarSiHayErrores(new Validador().Agregar(campo, $"El campo '{campo}' debe ser un número mayor que cero."));
        }

        protected void LanzarSiHayErrores(Validador validador)
        {
            if (!validador.EsValido)
                throw CrearExcepcion($"Los datos {Entidad.DeArticulo} no son válidos. Revise los errores de cada campo.",
                                     ServiceErrorType.Validation, validador.Errores);
        }

        // Lanza un error de validación si no se enviaron datos (cuerpo de la solicitud vacío)
        protected void VerificarQueExista(DtoBase? dto)
        {
            if (dto is null)
                throw CrearExcepcion($"Debe enviar los datos {Entidad.DeArticulo}.", ServiceErrorType.Validation);
        }

        private string MensajeCantidad(int cantidad) => cantidad == 1
            ? $"Se encontró 1 {Entidad.Singular}."
            : $"Se encontraron {cantidad} {Entidad.Plural}.";

        /// <summary>
        /// Ejecuta una operación y convierte cualquier excepción en un ServiceResult fallido:
        /// - ServiceException: validaciones del servicio (400) o registro inexistente (404).
        /// - TransporteException: reglas de negocio que validan los repositorios (400/404/409).
        /// - Cualquier otra: error inesperado (500); se registra en el log con el detalle.
        /// </summary>
        protected async Task<ServiceResult<T>> EjecutarAsync<T>(Func<Task<ServiceResult<T>>> operacion, string accion)
        {
            try
            {
                return await operacion();
            }
            catch (Exception ex)
            {
                var (mensaje, tipo, errores) = TraducirExcepcion(ex, accion);
                return ServiceResult<T>.Fail(mensaje, tipo, errores);
            }
        }

        protected async Task<ServiceResult> EjecutarAsync(Func<Task<ServiceResult>> operacion, string accion)
        {
            try
            {
                return await operacion();
            }
            catch (Exception ex)
            {
                var (mensaje, tipo, errores) = TraducirExcepcion(ex, accion);
                return ServiceResult.Fail(mensaje, tipo, errores);
            }
        }

        private (string Mensaje, ServiceErrorType Tipo, IReadOnlyDictionary<string, string[]>? Errores)
            TraducirExcepcion(Exception ex, string accion)
        {
            switch (ex)
            {
                case ServiceException servicio:
                    return (servicio.Message, servicio.ErrorType, servicio.Errores);

                case TransporteException negocio:
                    var tipo = negocio.Tipo switch
                    {
                        TipoError.Validacion => ServiceErrorType.Validation,
                        TipoError.NoEncontrado => ServiceErrorType.NotFound,
                        _ => ServiceErrorType.Conflict
                    };
                    return (negocio.Message, tipo, null);

                default:
                    Logger.LogError(ex, "Error inesperado al {Accion}.", accion);
                    return ($"Ocurrió un error inesperado al {accion}. Intente de nuevo más tarde.",
                            ServiceErrorType.Internal, null);
            }
        }
    }
}
