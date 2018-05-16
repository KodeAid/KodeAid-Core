// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


namespace KodeAid
{
    public interface IService
    {
        bool IsStarted { get; }
        void Start();
        void Stop();
    }
}
