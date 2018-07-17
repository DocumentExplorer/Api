using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace DocumentExplorer.Infrastructure.Services
{
    public class TokenManager : ITokenManager
    {
        private readonly IMemoryCache _cache;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TokenManager(IMemoryCache cache, IHttpContextAccessor httpContextAccessor)
        {
            _cache = cache;
            _httpContextAccessor = httpContextAccessor;
        }
        public void Deactivate(string token)
            => _cache.Set(GetKey(token),TimeSpan.FromMinutes(20));

        public void DeactivateCurrent()
            => Deactivate(GetCurrent());

        public bool IsActive(string token)
            => _cache.Get(GetKey(token)) == null;

        public bool IsCurrentActive()
            => IsActive(GetCurrent());

        private string GetCurrent()
        {
            var authorizationHeader = _httpContextAccessor.HttpContext.Request.Headers["authorization"];
            return authorizationHeader == StringValues.Empty ? 
                string.Empty : authorizationHeader.Single().Split(" ").Last();
        }
        private static string GetKey(string token)
            => $"tokens:{token}:deactivated";
    }
}