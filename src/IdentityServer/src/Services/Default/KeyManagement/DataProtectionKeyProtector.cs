// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.DataProtection;

namespace Duende.IdentityServer.Services.KeyManagement
{
    /// <summary>
    /// Implementation of IKeyProtector based on ASP.NET Core's data protection feature.
    /// </summary>
    public class DataProtectionKeyProtector : ISigningKeyProtector
    {
        private readonly IDataProtector _dataProtectionProvider;

        /// <summary>
        /// Constructor for DataProtectionKeyProtector.
        /// </summary>
        /// <param name="dataProtectionProvider"></param>
        public DataProtectionKeyProtector(IDataProtectionProvider dataProtectionProvider)
        {
            _dataProtectionProvider = dataProtectionProvider.CreateProtector(nameof(DataProtectionKeyProtector));
        }

        /// <summary>
        /// Protects RsaKeyContainer.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public SerializedKey Protect(RsaKeyContainer key)
        {
            var data = KeySerializer.Serialize(key);
            data = _dataProtectionProvider.Protect(data);
            return new SerializedKey(key, key.KeyType, data);
        }

        /// <summary>
        /// Unprotects RsaKeyContainer.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public RsaKeyContainer Unprotect(SerializedKey key)
        {
            var data = _dataProtectionProvider.Unprotect(key.Data);
            var item = KeySerializer.Deserialize<RsaKeyContainer>(data);
            if (item.KeyType == KeyType.X509)
            {
                item = KeySerializer.Deserialize<X509KeyContainer>(data);
            }
            return item;
        }
    }
}
