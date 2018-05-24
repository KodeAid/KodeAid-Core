// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Security.Cryptography;

namespace KodeAid.Security.Cryptography
{
    public class MachineKeyProtector : IDataProtector
    {
        private readonly DataProtectionScope _scope;
        private readonly byte[] _entropy;

        public MachineKeyProtector()
            : this(null)
        {
        }

        public MachineKeyProtector(DataProtectionScope scope)
            : this(scope, null)
        {
        }

        public MachineKeyProtector(byte[] entropy)
            : this(DataProtectionScope.LocalMachine, entropy)
        {
        }

        public MachineKeyProtector(DataProtectionScope scope, byte[] entropy)
        {
            _scope = scope;
            _entropy = entropy;
        }

        public byte[] ProtectData(byte[] unprotectedData)
        {
            ArgCheck.NotNull(nameof(unprotectedData), unprotectedData);
            return ProtectedData.Protect(unprotectedData, _entropy, _scope);
        }

        public byte[] UnprotectData(byte[] protectedData)
        {
            ArgCheck.NotNull(nameof(protectedData), protectedData);
            return ProtectedData.Unprotect(protectedData, _entropy, _scope);
        }
    }
}
