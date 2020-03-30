// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace KodeAid.Repositories
{
    public interface ICrudTrackingRepositoryAsync<TEntity> : ICrudRepositoryAsync<TEntity>
        where TEntity : class
    {
        Task<TEntity> GetAsync(object id, bool trackChanges, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> GetAllAsync(bool trackChanges, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, bool trackChanges, CancellationToken cancellationToken = default);
        Task TrackAsync(TEntity entity, bool trackChanges = true, CancellationToken cancellationToken = default);
        Task TrackRangeAsync(IEnumerable<TEntity> entities, bool trackChanges = true, CancellationToken cancellationToken = default);
    }
}
