// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KodeAid;
using KodeAid.Data;

namespace Microsoft.EntityFrameworkCore
{
    public static class DbContextExtensions
    {
        public static void AddOrUpdate<TEntity>(this DbContext context, TEntity entity)
            where TEntity : class
        {
            ArgCheck.NotNull(nameof(context), context);
            ArgCheck.NotNull(nameof(entity), entity);

            var existing = context.Set<TEntity>().Find(context.Model.FindEntityType(typeof(TEntity))
                .FindPrimaryKey().Properties.Select(p => p.PropertyInfo.GetValue(entity)).ToArray());

            AddOrUpdate(context, entity, existing);
        }

        public static async Task AddOrUpdateAsync<TEntity>(this DbContext context, TEntity entity, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            ArgCheck.NotNull(nameof(context), context);
            ArgCheck.NotNull(nameof(entity), entity);

            var existing = await context.Set<TEntity>().FindAsync(context.Model.FindEntityType(typeof(TEntity))
                .FindPrimaryKey().Properties.Select(p => p.PropertyInfo.GetValue(entity)).ToArray(), cancellationToken).ConfigureAwait(false);

            AddOrUpdate(context, entity, existing);
        }

        public static void AddOrUpdateRange<TEntity>(this DbContext context, IEnumerable<TEntity> entities)
            where TEntity : class
        {
            ArgCheck.NotNull(nameof(context), context);
            ArgCheck.NotNull(nameof(entities), entities);

            foreach (var entity in entities)
            {
                AddOrUpdate(context, entity);
            }
        }

        public static void AddOrUpdateRange<TEntity>(this DbContext context, params TEntity[] entities)
            where TEntity : class
        {
            AddOrUpdateRange(context, (IEnumerable<TEntity>)entities);
        }

        public static async Task AddOrUpdateRangeAsync<TEntity>(this DbContext context, IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            ArgCheck.NotNull(nameof(context), context);
            ArgCheck.NotNull(nameof(entities), entities);

            foreach (var entity in entities)
            {
                await AddOrUpdateAsync(context, entity, cancellationToken).ConfigureAwait(false);
            }
        }

        public static Task AddOrUpdateRangeAsync<TEntity>(this DbContext context, params TEntity[] entities)
            where TEntity : class
        {
            return AddOrUpdateRangeAsync(context, CancellationToken.None, entities);
        }

        public static Task AddOrUpdateRangeAsync<TEntity>(this DbContext context, CancellationToken cancellationToken, params TEntity[] entities)
            where TEntity : class
        {
            return AddOrUpdateRangeAsync(context, (IEnumerable<TEntity>)entities, cancellationToken);
        }

        private static void AddOrUpdate<TEntity>(DbContext context, TEntity entity, TEntity existing)
            where TEntity : class
        {
            var timestamp = DateTimeOffset.UtcNow;

            if (existing != null)
            {
                if (typeof(ICreatedTime).IsAssignableFrom(entity.GetType()))
                {
                    ((ICreatedTime)entity).CreatedAt = ((ICreatedTime)existing).CreatedAt;
                }

                if (typeof(IUpdatedTime).IsAssignableFrom(entity.GetType()))
                {
                    ((IUpdatedTime)entity).UpdatedAt = timestamp;
                }

                if (typeof(IOptimisticConcurrency).IsAssignableFrom(entity.GetType()))
                {
                    ((IOptimisticConcurrency)entity).ConcurrencyStamp = ((IOptimisticConcurrency)existing).ConcurrencyStamp;
                }

                context.Update(entity);
            }
            else
            {
                if (typeof(ICreatedTime).IsAssignableFrom(entity.GetType()))
                {
                    ((ICreatedTime)entity).CreatedAt = timestamp;
                }

                if (typeof(IUpdatedTime).IsAssignableFrom(entity.GetType()))
                {
                    ((IUpdatedTime)entity).UpdatedAt = timestamp;
                }

                context.Add(entity);
            }
        }
    }
}
