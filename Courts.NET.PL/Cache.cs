using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace STL.PL.Cache
{
    public sealed class Cache
    {
        private static readonly Cache InstanceLocal = new Cache();
        private readonly Hashtable items = new Hashtable();

        static Cache()
        {
        }

        private Cache()
        {
        }

        public static Cache Instance
        {
            get
            {
                return InstanceLocal;
            }
        }

        public void Add(string key, CacheItem obj)
        {
            if (items.Contains(key))
            {
                items.Remove(key);
            }
            items.Add(key, obj);
        }

        public CacheItem Get(string key)
        {
            return (CacheItem)items[key];
        }
    }

    public class CacheItem
    {
        public DateTime CacheDate { get; set; }
        public object[] Value { get; set; }
    }
}
