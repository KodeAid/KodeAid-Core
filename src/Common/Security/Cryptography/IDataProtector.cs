// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Threading.Tasks;

namespace KodeAid.Security.Cryptography
{
    public interface IDataProtector
    {
        byte[] ProtectData(byte[] unprotectedData);
        byte[] UnprotectData(byte[] protectedData);
    }
}
