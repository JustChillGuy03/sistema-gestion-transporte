using System.Linq.Expressions;
using TransporteEmpresarial.Domain.Core;

namespace TransporteEmpresarial.Domain.Repository
{
    /// <summary>
    /// Operaciones comunes a todos los repositorios.
    /// Las consultas ignoran los registros con borrado lógico.
    /// </summary>
    public interface IBaseRepository<TEntity> where TEntity : BaseEntity
    {
        Task<List<TEntity>> GetAllAsync();
        Task<TEntity?> GetByIdAsync(int id);
        Task<List<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> filtro);
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> filtro);
        Task AddAsync(TEntity entidad);
        Task UpdateAsync(TEntity entidad);

        // Borrado lógico (marca el registro como eliminado)
        Task RemoveAsync(TEntity entidad);
    }
}
