// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Duende.IdentityServer.Services.KeyManagement
{
    /// <summary>
    /// In-memory implmenetaion of ISigningKeyStoreCache based on static variables. This expects to be used as a singleton.
    /// </summary>
    class InMemoryKeyStoreCache : ISigningKeyStoreCache
    {
        private readonly ISystemClock _clock;

        private object _lock = new object();

        private DateTime _expires = DateTime.MinValue;
        private IEnumerable<RsaKeyContainer> _cache;

        /// <summary>
        /// Constructor for InMemoryKeyStoreCache.
        /// </summary>
        /// <param name="clock"></param>
        public InMemoryKeyStoreCache(ISystemClock clock)
        {
            _clock = clock;
        }

        /// <summary>
        /// Returns cached keys.
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<RsaKeyContainer>> GetKeysAsync()
        {
            DateTime expires;
            IEnumerable<RsaKeyContainer> keys;

            lock (_lock)
            {
                expires = _expires;
                keys = _cache;
            }

            if (keys != null && expires >= _clock.UtcNow.DateTime)
            {
                return Task.FromResult(keys);
            }

            return Task.FromResult<IEnumerable<RsaKeyContainer>>(null);
        }

        /// <summary>
        /// Caches keys for duration.
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public Task StoreKeysAsync(IEnumerable<RsaKeyContainer> keys, TimeSpan duration)
        {
            lock (_lock)
            {
                _expires = _clock.UtcNow.DateTime.Add(duration);
                _cache = keys;
            }

            return Task.CompletedTask;
        }
    }
}
