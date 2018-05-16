// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace KodeAid.Data.Repositories
{
    public interface IReadRepository<TEntity>
        where TEntity : class
    {
        TEntity Get(object id);
        IEnumerable<TEntity> GetAll();
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
    }
}
