// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace KodeAid.Data.Repositories
{
    public interface ICrudTrackingRepository<TEntity> : ICrudRepository<TEntity>
        where TEntity : class
    {
        TEntity Get(object id, bool trackChanges);
        IEnumerable<TEntity> GetAll(bool trackChanges);
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate, bool trackChanges);
        void TrackChanges(TEntity entity);
        void TrackChangesOnRange(IEnumerable<TEntity> entities);
    }
}
