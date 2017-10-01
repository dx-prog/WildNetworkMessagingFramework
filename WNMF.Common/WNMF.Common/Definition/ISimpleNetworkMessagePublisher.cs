/***************************************************************
 * Notice:
 *       1) Do not remove copyright notice
 *       2) See License file (https://raw.githubusercontent.com/dx-prog/WildNetworkMessagingFramework/master/LICENSE) for more details 
 *       3) Copyright (c) 2017 David Garcia
 * ************************************************************/
using System.Collections.Generic;

namespace WNMF.Common.Definition {
    public interface ISimpleNetworkMessagePublisher : INServiceProvider {
        /// <summary>
        /// Gets the name of the this distributor agent
        /// </summary>
        string AgentName { get; }
        /// <summary>
        /// Puts a message into the staging queue
        /// </summary>
        /// <param name="src"></param>
        /// <param name="responses"></param>
        /// <returns></returns>
        bool Stage(INetworkMessageStream src,out List<TryOperationResponseBase> responses);
        /// <summary>
        /// Pumps all pending messages in teh staging queue to all
        ///  endpoints associated withi this distributor
        /// </summary>
        /// <param name="responses"></param>
        /// <returns></returns>
        double Pump(out List<TryOperationResponseBase> responses);
    }
}