// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Threading.Tasks;
using KodeAid.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace KodeAid.EntityFrameworkCore
{
    public class EFUnitOfWork : EFUnitOfWork<DbContext>
    {
        public EFUnitOfWork(DbContext context)
            : base(context)
        {
        }
    }

    public class EFUnitOfWork<TContext> : IUnitOfWork, IUnitOfWorkAsync
        where TContext : DbContext
    {
        public EFUnitOfWork(TContext context)
        {
            ArgCheck.NotNull(nameof(context), context);

            Context = context;
        }

        protected TContext Context { get; }

        public void Complete()
        {
            Context.SaveChanges();
        }

        public Task CompleteAsync()
        {
            return Context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Context.Dispose();
        }

        protected EFRepository<TEntity, TContext> GetRepository<TEntity>()
            where TEntity : class
        {
            return new EFRepository<TEntity, TContext>(Context);
        }

        protected TRepository GetRepository<TEntity, TRepository>()
            where TEntity : class
            where TRepository : EFRepository<TEntity, TContext>, new()
        {
            return new TRepository() { Context = Context };
        }

        protected TRepository GetRepository<TEntity, TRepository>(Func<TContext, TRepository> ctor)
            where TEntity : class
            where TRepository : EFRepository<TEntity, TContext>
        {
            ArgCheck.NotNull(nameof(ctor), ctor);

            return ctor(Context);
        }
    }
}
