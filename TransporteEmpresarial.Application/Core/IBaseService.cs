using TransporteEmpresarial.Application.Dtos;

namespace TransporteEmpresarial.Application.Core
{
    /// <summary>
    /// Operaciones comunes de todos los servicios de negocio.
    /// TModel: DTO de respuesta. TDtoAdd / TDtoUpdate: DTOs para registrar y actualizar.
    /// </summary>
    public interface IBaseService<TModel, TDtoAdd, TDtoUpdate>
        where TDtoAdd : DtoBase
        where TDtoUpdate : DtoBase
    {
        Task<ServiceResult<List<TModel>>> GetAllAsync();
        Task<ServiceResult<TModel>> GetByIdAsync(int id);
        Task<ServiceResult<TModel>> SaveAsync(TDtoAdd dto);
        Task<ServiceResult<TModel>> UpdateAsync(int id, TDtoUpdate dto);

        // Borrado lógico
        Task<ServiceResult> RemoveAsync(int id);
    }
}
