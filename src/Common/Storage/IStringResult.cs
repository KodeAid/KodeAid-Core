// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.



namespace KodeAid.Storage
{
    public interface IStringResult : IStoreResult
    {
        string Value { get; }
    }
}
