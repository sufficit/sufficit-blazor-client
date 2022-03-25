using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Models
{
    public class BlazorServerAuthStateCache
    {
        private readonly ILogger _logger;
        public BlazorServerAuthStateCache(ILogger<BlazorServerAuthStateCache> logger)
        {
            _logger = logger;
        }

        private ConcurrentDictionary<string, BlazorServerAuthData> Cache
            = new ConcurrentDictionary<string, BlazorServerAuthData>();

        public bool HasSubjectId(string subjectId)
            => Cache.ContainsKey(subjectId);

        public void Add(string subjectId, DateTimeOffset expiration, string accessToken, string refreshToken)
        {
            _logger.LogDebug($"caching sid minor: {subjectId}");

            var data = new BlazorServerAuthData
            {
                SubjectId = subjectId,
                Expiration = expiration,
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

            Cache.AddOrUpdate(subjectId, data, (k, v) => data);
        }

        public void Add(string subjectId, DateTimeOffset expiration, string idToken, string accessToken, string refreshToken, DateTimeOffset refreshAt)
        {
            _logger.LogDebug($"caching sid: {subjectId}");

            var data = new BlazorServerAuthData
            {
                SubjectId = subjectId,
                Expiration = expiration,
                IdToken = idToken,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                RefreshAt = refreshAt
            };

            Cache.AddOrUpdate(subjectId, data, (k, v) => data);
        }

        public BlazorServerAuthData Get(string subjectId)
        {
            Cache.TryGetValue(subjectId, out var data);
            return data;
        }

        public void Remove(string subjectId)
        {
            Cache.TryRemove(subjectId, out _);
        }
    }
}
