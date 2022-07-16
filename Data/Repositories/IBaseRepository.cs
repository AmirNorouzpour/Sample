using System.Data.Common;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Model.Entities;

namespace Data.Repositories
{
    public interface IBaseRepository<TEntity> where TEntity : BaseEntity, IEntity
    {
        DbSet<TEntity> Entities { get; }
        IQueryable<TEntity> Table { get; }
        IQueryable<TEntity?> TableNoTracking { get; }
        void Add(TEntity entity, bool saveNow = true);
        Task AddAsync(TEntity entity, CancellationToken cancellationToken, bool saveNow = true);
        void AddRange(IEnumerable<TEntity> entities, bool saveNow = true);
        Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken, bool saveNow = true);
        void Attach(TEntity entity);
        void Delete(TEntity entity, bool saveNow = true);
        Task DeleteAsync(TEntity entity, CancellationToken cancellationToken, bool saveNow = true);
        void DeleteRange(IEnumerable<TEntity> entities, bool saveNow = true);
        Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken, bool saveNow = true);
        void Detach(TEntity entity);
        TEntity GetById(params object[] ids);
        TEntity GetById(object id);
        void LoadCollection<TProperty>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> collectionProperty) where TProperty : class;
        Task LoadCollectionAsync<TProperty>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> collectionProperty, CancellationToken cancellationToken) where TProperty : class;
        void LoadReference<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> referenceProperty) where TProperty : class;
        Task LoadReferenceAsync<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> referenceProperty, CancellationToken cancellationToken) where TProperty : class;
        void Update(TEntity entity, bool saveNow = true);
        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken, bool saveNow = true);
        void UpdateRange(IEnumerable<TEntity> entities, bool saveNow = true);
        Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken, bool saveNow = true);
        int Count(Expression<Func<TEntity, bool>>? predicate);
        int Count();
        IQueryable<TEntity?> GetAll(Expression<Func<TEntity?, bool>> predicate);
        IQueryable<TEntity?> GetAll();
        Task<TEntity> GetFirstAsync(Expression<Func<TEntity?, bool>> predicate, CancellationToken cancellationToken);
        TEntity? GetFirst(Expression<Func<TEntity?, bool>> predicate);
        int Run(string query, params object[] parameters);
        DbConnection GetDbConnection();
        EntityEntry<TEntity> Entry(TEntity entity);
    }
}