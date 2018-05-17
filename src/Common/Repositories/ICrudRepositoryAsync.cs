// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.Threading.Tasks;

namespace KodeAid.Data.Repositories
{
    public interface ICrudRepositoryAsync<TEntity> : IReadRepositoryAsync<TEntity>
        where TEntity : class
    {
        Task AddAsync(TEntity entity);
        Task AddRangeAsync(IEnumerable<TEntity> entities);
        Task RemoveAsync(TEntity entity);
        Task RemoveRangeAsync(IEnumerable<TEntity> entities);
        Task UpdateAsync(TEntity entity);
        Task UpdateRangeAsync(IEnumerable<TEntity> entities);
    }
}
