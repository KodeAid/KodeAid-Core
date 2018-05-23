// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace KodeAid.Repositories
{
    public interface ICrudRepositoryAsync<TEntity> : IReadRepositoryAsync<TEntity>
        where TEntity : class
    {
        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
        Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    }
}
