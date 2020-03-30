// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;

namespace KodeAid
{
    public sealed class Disposable : IDisposable
    {
        private readonly Action _dispose;

        public Disposable(Action dispose)
        {
            _dispose = dispose;
        }

        private Disposable()
        {
        }

        public static IDisposable Nop { get; } = new Disposable();

        public void Dispose()
        {
            _dispose?.Invoke();
        }
    }
}
