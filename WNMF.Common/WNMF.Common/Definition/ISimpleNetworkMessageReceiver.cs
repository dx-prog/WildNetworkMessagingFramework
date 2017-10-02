/***************************************************************
 * Notice:
 *       1) Do not remove copyright notice
 *       2) See License file (https://raw.githubusercontent.com/dx-prog/WildNetworkMessagingFramework/master/LICENSE) for more details 
 *       3) Copyright (c) 2017 David Garcia
 * ************************************************************/

namespace WNMF.Common.Definition {
    public interface ISimpleNetworkMessageReceiver : INServiceProvider {
        /// <summary>
        ///     Gets the name of the this agent
        /// </summary>
        string AgentName { get; }

        /// <summary>
        ///     Moves from the endpoint to local cache, and returns a stream that can be used to access
        ///     data from cache
        ///     the data
        /// </summary>
        /// <param name="result"></param>
        bool Stage(TryOperationResponse<INetworkMessageStream> result);
    }
}