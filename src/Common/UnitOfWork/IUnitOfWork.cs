// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;

namespace KodeAid.UnitOfWork
{
  public interface IUnitOfWork : IDisposable
  {
    #region Methods
    void Complete();
    #endregion
  }
}
