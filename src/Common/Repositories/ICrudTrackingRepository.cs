// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace KodeAid.Repositories
{
    public interface ICrudTrackingRepository<TEntity> : ICrudRepository<TEntity>
        where TEntity : class
    {
        TEntity Get(object id, bool trackChanges);
        IEnumerable<TEntity> GetAll(bool trackChanges);
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate, bool trackChanges);
        void Track(TEntity entity, bool trackChanges = true);
        void TrackRange(IEnumerable<TEntity> entities, bool trackChanges = true);
    }
}
