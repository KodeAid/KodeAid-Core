// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Threading.Tasks;

namespace KodeAid.UnitOfWork
{
  public interface IUnitOfWorkAsync : IDisposable
  {
    #region Methods
    Task CompleteAsync();
    #endregion
  }
}
