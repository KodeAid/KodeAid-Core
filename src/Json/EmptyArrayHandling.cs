// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;

namespace KodeAid.Serialization.Json.ContractResolvers
{
    //
    // Summary:
    //     Specifies default value handling options for the Newtonsoft.Json.JsonSerializer.
    /// <summary>
    /// Specifies empty array handling options for the KodeAid.Serialization.Json.ContractResolvers.
    /// </summary>
    [Flags]
    public enum EmptyArrayHandling
    {
        /// <summary>
        /// Include array based members where the array is empty when serializing objects.
        /// Included members are written to JSON. Has no effect when deserializing.
        /// </summary>
        Include = 0,
        
        /// <summary>
        /// Ignore array based members where the array is empty when serializing objects
        /// so that it is not written to JSON.
        /// </summary>
        Ignore = 1,
        
        /// <summary>
        /// Array based members with no JSON will be set to an empty array when deserializing.
        /// </summary>
        Populate = 2,

        /// <summary>
        /// Ignore array based members where the array is empty when serializing objects
        /// and set members to an empty array when deserializing.
        /// </summary>
        IgnoreAndPopulate = 3
    }
}
