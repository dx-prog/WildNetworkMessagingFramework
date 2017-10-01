/***************************************************************
 * Notice:
 *       1) Do not remove copyright notice
 *       2) See License file (https://raw.githubusercontent.com/dx-prog/WildNetworkMessagingFramework/master/LICENSE) for more details 
 *       3) Copyright (c) 2017 David Garcia
 * ************************************************************/
using System;
using System.Collections.Concurrent;

namespace WNMF.Common.Foundation {
    public class FlyWeight<T> {
        private readonly ConcurrentDictionary<object, ConcurrentBag<object>> _instances =
            new ConcurrentDictionary<object, ConcurrentBag<object>>();

        public T GetOrCreate(
            string key,
            Func<string, T> ret) {
            key = typeof(T).Name + ":" + key;
            var instances = _instances.GetOrAdd(key, k => new ConcurrentBag<object>());

            if (instances.TryTake(out var instance))
                return (T) instance;

            return ret(key);
        }

        public void Store(string key, T value) {
            key = typeof(T).Name + ":" + key;
            var instances = _instances.GetOrAdd(key, k => new ConcurrentBag<object>());
            instances.Add(value);
        }
    }
}