// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


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

        public static IDisposable NoOp { get; } = new Disposable();

        public void Dispose()
        {
            _dispose?.Invoke();
        }
    }
}
