using StackExchange.Redis;

namespace Hub.Infrastructure.Cache.Interfaces
{
    public interface IRedisService : IDisposable
    {
        object Get(string key, bool ignoreTenant = false);
        void Set(string key, string value, TimeSpan? expiry = null);
        void Delete(string key);
        void DeleteFromPattern(string pattern);
        ConnectionMultiplexer GetRedisConnection(string connectionName = "redis");
    }
}
