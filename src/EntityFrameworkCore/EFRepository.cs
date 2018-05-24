// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using KodeAid.Repositories;
using Microsoft.EntityFrameworkCore;

namespace KodeAid.EntityFrameworkCore
{
    public class EFRepository<TEntity> : EFRepository<TEntity, DbContext>
        where TEntity : class
    {
        public EFRepository(DbContext context, bool autoSave = false)
            : base(context, autoSave)
        {
        }

        protected EFRepository()
        {
        }
    }

    public class EFRepository<TEntity, TContext> : ICrudTrackingRepository<TEntity>, ICrudTrackingRepositoryAsync<TEntity>
        where TEntity : class
        where TContext : DbContext
    {
        private readonly bool _autoSave;

        public EFRepository(TContext context, bool autoSave = false)
        {
            ArgCheck.NotNull(nameof(context), context);
            Context = context;
            _autoSave = autoSave;
        }

        protected EFRepository()
        {
        }

        protected internal TContext Context { get; set; }

        public TEntity Get(object id)
        {
            return Get(id, false);
        }

        public TEntity Get(object id, bool trackChanges)
        {
            // todo: make sure entity is not attached to context if not already attached and trackChanges is false
            // Find() will attach the entity for change tracking
            // we don't want that unless the entity was already being tracked
            return Context.Set<TEntity>().Find(id);
        }

        public Task<TEntity> GetAsync(object id, CancellationToken cancellationToken = default)
        {
            return GetAsync(id, false, cancellationToken);
        }

        public Task<TEntity> GetAsync(object id, bool trackChanges, CancellationToken cancellationToken = default)
        {
            // todo: make sure entity is not attached to context if not already attached and trackChanges is false
            // Find() will attach the entity for change tracking
            // we don't want that unless the entity was already being tracked
            return Context.Set<TEntity>().FindAsync(new[] { id }, cancellationToken);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return GetAll(false);
        }

        public IEnumerable<TEntity> GetAll(bool trackChanges)
        {
            if (trackChanges)
                return Context.Set<TEntity>().ToList();
            return Context.Set<TEntity>().AsNoTracking().ToList();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await Context.Set<TEntity>().AsNoTracking().ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(bool trackChanges, CancellationToken cancellationToken = default)
        {
            if (trackChanges)
                return await Context.Set<TEntity>().ToListAsync(cancellationToken).ConfigureAwait(false);
            return await GetAllAsync().ConfigureAwait(false);
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().AsNoTracking().Where(predicate).ToList();
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate, bool trackChanges)
        {
            if (trackChanges)
                return Context.Set<TEntity>().Where(predicate).ToList();
            return Find(predicate);
        }

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await Context.Set<TEntity>().AsNoTracking().Where(predicate).ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, bool trackChanges, CancellationToken cancellationToken = default)
        {
            if (trackChanges)
                return await Context.Set<TEntity>().Where(predicate).ToListAsync(cancellationToken).ConfigureAwait(false);
            return await FindAsync(predicate).ConfigureAwait(false);
        }

        public void Add(TEntity entity)
        {
            Context.Set<TEntity>().Add(entity);
            if (_autoSave)
                Context.SaveChanges(true);
        }

        public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            Context.Set<TEntity>().Add(entity);
            if (_autoSave)
                await Context.SaveChangesAsync(true).ConfigureAwait(false);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            Context.Set<TEntity>().AddRange(entities);
            if (_autoSave)
                Context.SaveChanges(true);
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            Context.Set<TEntity>().AddRange(entities);
            if (_autoSave)
                await Context.SaveChangesAsync(true).ConfigureAwait(false);
        }

        public void Remove(TEntity entity)
        {
            Context.Set<TEntity>().Remove(entity);
            if (_autoSave)
                Context.SaveChanges(true);
        }

        public async Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            Context.Set<TEntity>().Remove(entity);
            if (_autoSave)
                await Context.SaveChangesAsync(true).ConfigureAwait(false);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            Context.Set<TEntity>().RemoveRange(entities);
            if (_autoSave)
                Context.SaveChanges(true);
        }

        public async Task RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            Context.Set<TEntity>().RemoveRange(entities);
            if (_autoSave)
                await Context.SaveChangesAsync(true).ConfigureAwait(false);
        }

        public void Update(TEntity entity)
        {
            Context.Set<TEntity>().Update(entity);
            if (_autoSave)
                Context.SaveChanges(true);
        }

        public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            Context.Set<TEntity>().Update(entity);
            if (_autoSave)
                await Context.SaveChangesAsync(true).ConfigureAwait(false);
        }

        public void UpdateRange(IEnumerable<TEntity> entities)
        {
            Context.Set<TEntity>().UpdateRange(entities);
            if (_autoSave)
                Context.SaveChanges(true);
        }

        public async Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            Context.Set<TEntity>().UpdateRange(entities);
            if (_autoSave)
                await Context.SaveChangesAsync(true).ConfigureAwait(false);
        }

        public void TrackChanges(TEntity entity, bool trackChanges = true)
        {
            if (trackChanges)
                Context.Attach(entity);
            else
                Context.Entry(entity).State = EntityState.Detached;
        }

        public Task TrackChangesAsync(TEntity entity, bool trackChanges = true, CancellationToken cancellationToken = default)
        {
            TrackChanges(entity, trackChanges);
            return Task.CompletedTask;
        }

        public void TrackChangesForRange(IEnumerable<TEntity> entities, bool trackChanges = true)
        {
            if (trackChanges)
                Context.AttachRange(entities);
            else
                foreach (var entity in entities)
                    Context.Entry(entity).State = EntityState.Detached;
        }

        public Task TrackChangesForRangeAsync(IEnumerable<TEntity> entities, bool trackChanges = true, CancellationToken cancellationToken = default)
        {
            TrackChangesForRange(entities, trackChanges);
            return Task.CompletedTask;
        }
    }
}
