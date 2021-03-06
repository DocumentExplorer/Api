using System;
using DocumentExplorer.Infrastructure.DTO;
using Microsoft.Extensions.Caching.Memory;

namespace DocumentExplorer.Infrastructure.Extensions
{
    public static class CacheExtensions
    {
        public static void SetJwt(this IMemoryCache cache, Guid tokenId, JwtDto jwt)
            => cache.Set(GetJwtKey(tokenId), jwt, TimeSpan.FromSeconds(5));

        public static JwtDto GetJwt(this IMemoryCache cache, Guid tokenId)
            => cache.Get<JwtDto>(GetJwtKey(tokenId));

        private static string GetJwtKey(Guid tokenId)
            => $"jwt-{tokenId}";
        public static string GetString(this IMemoryCache cache, Guid cacheId)
        {
            var fromCache = cache.Get(cacheId);
            if(fromCache is string result)
            {
                return result;
            }
            throw new InvalidCastException();
        }
    }
}