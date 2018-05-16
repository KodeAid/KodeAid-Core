using System;

namespace KodeAid
{
  public interface IUnitOfWork : IDisposable
  {
    #region Methods
    void Complete();
    #endregion
  }
}
