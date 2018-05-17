// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


namespace KodeAid.Security.Cryptography
{
    public interface IDataProtector
    {
        byte[] EncryptData(byte[] dataToEncrypt, byte[] key);
        byte[] DecryptData(byte[] dataToDecrypt, byte[] key);
    }
}
