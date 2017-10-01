﻿/***************************************************************
 * Notice:
 *       1) Do not remove copyright notice
 *       2) See License file (https://raw.githubusercontent.com/dx-prog/WildNetworkMessagingFramework/master/LICENSE) for more details 
 *       3) Copyright (c) 2017 David Garcia
 * ************************************************************/
using System;

namespace WNMF.Common.Definition {
    public interface INetworkMessageHistory {
        /// <summary>
        /// Mark a message as being sent for a particular endpoint
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="messageDescription"></param>
        /// <param name="reason">data is true if the message is marked</param>
        /// <returns>false if there is was a critical failure</returns>
        bool TryMarkAsSent(INetworkEndpoint endpoint,NetworkMessageDescription messageDescription, out TryOperationResponse<bool> reason);
        /// <summary>
        /// Check if an endpoint had previously sent a message
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="messageDescription"></param>
        /// <param name="reason">data is same as returned value</param>
        /// <returns></returns>
        bool TryCheckIfSent(INetworkEndpoint endpoint, NetworkMessageDescription messageDescription, out TryOperationResponse<bool> reason);

        /// <summary>
        /// Begin whatever transaction or syncronization operation required
        /// </summary>
        /// <param name="commit">never null</param>
        /// <param name="rollback">never null</param>
        /// <returns></returns>
        IDisposable BeginTransaction(out Action commit, out Action rollback);
    }
}