/***************************************************************
 * Notice:
 *       1) Do not remove copyright notice
 *       2) See License file (https://raw.githubusercontent.com/dx-prog/WildNetworkMessagingFramework/master/LICENSE) for more details 
 *       3) Copyright (c) 2017 David Garcia
 * ************************************************************/

using System;
using System.Threading;

namespace WNMF.Common.Definition {
    public class MonitorLock : IDisposable {
        private readonly object _syncRoot;

        public MonitorLock(object syncRoot) {
            _syncRoot = syncRoot;
        }

        public void Dispose() {
            Monitor.Exit(_syncRoot);
        }
    }
}