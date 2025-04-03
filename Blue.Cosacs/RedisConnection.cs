using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs
{
    public sealed class RedisConnection
    {
        private static readonly ConnectionMultiplexer RedisConnector;
        private const string HostName = "localhost:6379,syncTimeout=10000";

        static RedisConnection()
        {
            RedisConnector = ConnectionMultiplexer.Connect(HostName);
        }

        public static IDatabase Database()
        {
            return RedisConnector.GetDatabase();
        }

        public static IServer Server()
        {
            return RedisConnector.GetServer(HostName);
        }
    }
}