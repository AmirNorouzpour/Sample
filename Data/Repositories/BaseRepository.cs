using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Model.Entities;


namespace Data.Repositories
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity, IEntity
    {
        protected readonly DataContext DbContext;
        public DbSet<TEntity> Entities { get; }
        public virtual IQueryable<TEntity> Table => Entities;
        public virtual IQueryable<TEntity> TableNoTracking => Entities.AsNoTracking();

        public BaseRepository(DataContext dbContext)
        {
            DbContext = dbContext;
            Entities = DbContext.Set<TEntity>();
        }

        #region Async Method

        public TEntity GetById(object id)
        {
            return Entities.Find(id);
        }

        public int Run(string query, params object[] parameters)
        {
            return DbContext.Database.ExecuteSqlRaw(query, parameters);
        }

        public DbConnection GetDbConnection()
        {
            return DbContext.Database.GetDbConnection();
        }

        public EntityEntry<TEntity> Entry(TEntity entity)
        {
            return DbContext.Entry(entity);
        }

        public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken, bool saveNow = true)
        {
            entity.InsertDateTime = DateTime.Now;

            await Entities.AddAsync(entity, cancellationToken).ConfigureAwait(false);
            if (saveNow)
                await DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken, bool saveNow = true)
        {
            await Entities.AddRangeAsync(entities, cancellationToken).ConfigureAwait(false);
            if (saveNow)
                await DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken, bool saveNow = true)
        {
            entity.ModifiedDateTime = DateTime.Now;
            AttachEntity(entity);
            Entities.Update(entity);
            if (saveNow)
                await DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual async Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken, bool saveNow = true)
        {
            Entities.UpdateRange(entities);
            if (saveNow)
                await DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public int Count(Expression<Func<TEntity, bool>>? predicate)
        {
            return predicate != null ? TableNoTracking.Count(predicate) : TableNoTracking.Count();
        }

        public int Count()
        {
            return TableNoTracking.Count();
        }

        public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken, bool saveNow = true)
        {
            Entities.Remove(entity);
            if (saveNow)
                await DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual async Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken, bool saveNow = true)
        {
            Entities.RemoveRange(entities);
            if (saveNow)
                await DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        #endregion

        #region Sync Methods
        public virtual TEntity GetById(params object[] ids)
        {
            return Entities.Find(ids);
        }

        public virtual IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate)
        {
            return predicate != null ? TableNoTracking.Where(predicate) : TableNoTracking;
        }

        public virtual IQueryable<TEntity> GetAll()
        {
            return TableNoTracking;
        }

        public Task<TEntity> GetFirstAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        {
            return predicate != null ? TableNoTracking.FirstOrDefaultAsync(predicate, cancellationToken) : TableNoTracking.FirstOrDefaultAsync(cancellationToken);
        }

        public TEntity GetFirst(Expression<Func<TEntity, bool>> predicate)
        {
            return predicate != null ? TableNoTracking.FirstOrDefault(predicate) : TableNoTracking.FirstOrDefault();
        }

        public virtual void Add(TEntity entity, bool saveNow = true)
        {
        
            entity.InsertDateTime = DateTime.Now;
            Entities.Add(entity);
            if (saveNow)
                DbContext.SaveChanges();
        }

        public virtual void AddRange(IEnumerable<TEntity> entities, bool saveNow = true)
        {
            if (entities == null)
                return;

            Entities.AddRange(entities);
            if (saveNow)
                DbContext.SaveChanges();
        }

        public virtual void Update(TEntity entity, bool saveNow = true)
        {
            AttachEntity(entity);
            entity.ModifiedDateTime = DateTime.Now;
            Entities.Update(entity);
            if (saveNow)
                DbContext.SaveChanges();
        }



        private void AttachEntity(TEntity entity)
        {
            var local = Entities.Local.FirstOrDefault(c => c.Id == entity.Id);
            if (local != null)
            {
                DbContext.Entry(local).State = EntityState.Detached;
            }

            Entities.Attach(entity);
            DbContext.Entry(entity).State = EntityState.Modified;
        }

        public virtual void UpdateRange(IEnumerable<TEntity> entities, bool saveNow = true)
        {
            Entities.UpdateRange(entities);
            if (saveNow)
                DbContext.SaveChanges();
        }

        public virtual void Delete(TEntity entity, bool saveNow = true)
        {
            Entities.Remove(entity);
            if (saveNow)
                DbContext.SaveChanges();
        }

        public virtual void DeleteRange(IEnumerable<TEntity> entities, bool saveNow = true)
        {
            Entities.RemoveRange(entities);
            if (saveNow)
                DbContext.SaveChanges();
        }
        #endregion

        #region Attach & Detach
        public virtual void Detach(TEntity entity)
        {
            var entry = DbContext.Entry(entity);
            if (entry != null)
                entry.State = EntityState.Detached;
        }

        public virtual void Attach(TEntity entity)
        {
            if (DbContext.Entry(entity).State == EntityState.Detached)
                Entities.Attach(entity);
        }
        #endregion

        #region Explicit Loading
        public virtual async Task LoadCollectionAsync<TProperty>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> collectionProperty, CancellationToken cancellationToken)
            where TProperty : class
        {
            Attach(entity);

            var collection = DbContext.Entry(entity).Collection(collectionProperty);
            if (!collection.IsLoaded)
                await collection.LoadAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual void LoadCollection<TProperty>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> collectionProperty)
            where TProperty : class
        {
            Attach(entity);
            var collection = DbContext.Entry(entity).Collection(collectionProperty);
            if (!collection.IsLoaded)
                collection.Load();
        }

        public virtual async Task LoadReferenceAsync<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> referenceProperty, CancellationToken cancellationToken)
            where TProperty : class
        {
            Attach(entity);
            var reference = DbContext.Entry(entity).Reference(referenceProperty);
            if (!reference.IsLoaded)
                await reference.LoadAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual void LoadReference<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> referenceProperty)
            where TProperty : class
        {
            Attach(entity);
            var reference = DbContext.Entry(entity).Reference(referenceProperty);
            if (!reference.IsLoaded)
                reference.Load();
        }
        #endregion
    }
}
