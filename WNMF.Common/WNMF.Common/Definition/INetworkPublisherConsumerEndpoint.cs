/***************************************************************
 * Notice:
 *       1) Do not remove copyright notice
 *       2) See License file (https://raw.githubusercontent.com/dx-prog/WildNetworkMessagingFramework/master/LICENSE) for more details 
 *       3) Copyright (c) 2017 David Garcia
 * ************************************************************/

using System;
using System.Threading;

namespace WNMF.Common.Definition {
    /// <summary>
    /// </summary>
    public interface INetworkPublisherPassiveConsumerEndpoint : INetworkEndpoint {
        /// <summary>
        ///     Accepts a stream or connection
        /// </summary>
        /// <param name="token"></param>
        /// <param name="filter"></param>
        /// <param name="drop"></param>
        /// <returns></returns>
        bool TryAccept(
            CancellationToken token,
            Func<NetworkMessageDescription, bool> filter,
            out TryOperationResponse<INetworkMessageStream> drop);
    }
}