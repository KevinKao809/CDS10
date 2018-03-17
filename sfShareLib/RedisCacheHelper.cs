using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sfShareLib
{
    public class RedisCacheHelper
    {
        private static readonly Lazy<ConnectionMultiplexer> LazyConnection;
        static RedisCacheHelper()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["RedisCache"].ConnectionString;
            LazyConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(connectionString));
        }
        public static ConnectionMultiplexer _Connection => LazyConnection.Value;
        public static IDatabase _RedisCache => _Connection.GetDatabase();
        public static string GetValueByKey(RedisKey cacheKey)
        {
            string keyValue = _RedisCache.StringGet(cacheKey);
            if (string.IsNullOrEmpty(keyValue))
                return null;
            else
                return keyValue;
        }
        public static void SetKeyValue(RedisKey cacheKey, string cacheValue)
        {
            _RedisCache.StringSet(cacheKey, cacheValue);
        }

        public static void SetKeyValue(RedisKey cacheKey, string cacheValue, TimeSpan timeSpan)
        {
            _RedisCache.StringSet(cacheKey, cacheValue, timeSpan);
        }

        public static void DeleteEmployeeCache(int id)
        {
            try
            {
                _RedisCache.KeyDelete("employee_" + id, CommandFlags.FireAndForget);
                _RedisCache.KeyDelete("employee_" + id + "_Permission", CommandFlags.FireAndForget);
                _RedisCache.KeyDelete("employee_" + id + "_Role", CommandFlags.FireAndForget);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void DeleteCompanyCache(int id)
        {
            try
            {
                _RedisCache.KeyDelete("company_" + id, CommandFlags.FireAndForget);
                _RedisCache.KeyDelete("external_company_" + id, CommandFlags.FireAndForget);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
