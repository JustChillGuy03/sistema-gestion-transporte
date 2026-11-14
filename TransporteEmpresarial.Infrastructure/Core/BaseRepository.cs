using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TransporteEmpresarial.Domain.Core;
using TransporteEmpresarial.Domain.Repository;
using TransporteEmpresarial.Infrastructure.Context;

namespace TransporteEmpresarial.Infrastructure.Core
{
    /// <summary>
    /// Implementación genérica del CRUD. Los repositorios concretos sobrescriben
    /// AddAsync, UpdateAsync y RemoveAsync para validar sus reglas de negocio.
    /// </summary>
    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity
    {
        protected readonly TransporteContext _context;
        protected readonly DbSet<TEntity> _entidades;

        protected BaseRepository(TransporteContext context)
        {
            _context = context;
            _entidades = context.Set<TEntity>();
        }

        // Punto de partida de todas las consultas: excluye los eliminados
        protected IQueryable<TEntity> Activos() => _entidades.Where(e => !e.Eliminado);

        public virtual async Task<List<TEntity>> GetAllAsync()
        {
            return await Activos().AsNoTracking().OrderBy(e => e.Id).ToListAsync();
        }

        public virtual async Task<TEntity?> GetByIdAsync(int id)
        {
            return await Activos().FirstOrDefaultAsync(e => e.Id == id);
        }

        public virtual async Task<List<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> filtro)
        {
            return await Activos().AsNoTracking().Where(filtro).ToListAsync();
        }

        public virtual async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> filtro)
        {
            return await Activos().AnyAsync(filtro);
        }

        public virtual async Task AddAsync(TEntity entidad)
        {
            _entidades.Add(entidad);
            await _context.SaveChangesAsync();
        }

        public virtual async Task UpdateAsync(TEntity entidad)
        {
            _entidades.Update(entidad);
            await _context.SaveChangesAsync();
        }

        public virtual async Task RemoveAsync(TEntity entidad)
        {
            entidad.Eliminado = true;
            entidad.FechaEliminacion = DateTime.Now;
            _entidades.Update(entidad);
            await _context.SaveChangesAsync();
        }
    }
}
