using System;
using System.Threading.Tasks;

namespace KodeAid
{
  public interface IUnitOfWorkAsync : IDisposable
  {
    #region Methods
    Task CompleteAsync();
    #endregion
  }
}
