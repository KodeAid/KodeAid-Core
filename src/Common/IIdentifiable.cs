// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


namespace KodeAid
{
    public interface IIdentifiable<T>
    {
        T Id { get; }
    }
}
