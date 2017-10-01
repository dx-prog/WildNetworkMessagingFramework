/***************************************************************
 * Notice:
 *       1) Do not remove copyright notice
 *       2) See License file (https://raw.githubusercontent.com/dx-prog/WildNetworkMessagingFramework/master/LICENSE) for more details 
 *       3) Copyright (c) 2017 David Garcia
 * ************************************************************/

using System;
using System.Collections.Generic;
using WNMF.Common.Definition;

namespace WNMF.Common.Foundation {
    public class NetworkEndpointComparer : IComparer<INetworkEndpoint> {
        public int Compare(INetworkEndpoint a, INetworkEndpoint b) {
            if (a == null && b != null)
                return 1;
            if (a != null && b == null)
                return -1;
            if (a == b)
                return 0;

            // ReSharper disable once SuspiciousTypeConversion.Global
            if (a is IComparable<INetworkEndpoint> aComprable)
                return aComprable.CompareTo(b);
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (b is IComparable<INetworkEndpoint> bComprable)
                return bComprable.CompareTo(a);
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (a is IEquatable<INetworkEndpoint> aEquatable)
                return aEquatable.Equals(b) ? 0 : -1;
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (b is IEquatable<INetworkEndpoint> bEquatable)
                return bEquatable.Equals(a) ? 0 : 1;

            return -1;
        }
    }
}