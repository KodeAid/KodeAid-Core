// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Threading.Tasks;

namespace KodeAid
{
    /// <summary>
    /// Provides a mechanism to await an asynchronous initialization task started in the constructor.
    /// </summary>
    public interface IAsyncInitializable
    {
        Task Initialization { get; }
    }
}
