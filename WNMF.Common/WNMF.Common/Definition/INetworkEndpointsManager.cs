/***************************************************************
 * Notice:
 *       1) Do not remove copyright notice
 *       2) See License file (https://raw.githubusercontent.com/dx-prog/WildNetworkMessagingFramework/master/LICENSE) for more details 
 *       3) Copyright (c) 2017 David Garcia
 * ************************************************************/

namespace WNMF.Common.Definition {
    /// <summary>
    ///     Provides a way for manipulationg a network graph
    /// </summary>
    public interface INetworkEndpointsManager {
        /// <summary>
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="existing"></param>
        /// <returns></returns>
        bool TryAddEndPoint(INetworkEndpoint endpoint, out TryOperationResponse<INetworkEndpoint> existing);

        /// <summary>
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="oldEndpoint"></param>
        /// <returns></returns>
        bool TryRemoveEndPoint(INetworkEndpoint endpoint, out TryOperationResponse<INetworkEndpoint> oldEndpoint);

        /// <summary>
        ///     Try to get the currently known endpoints
        /// </summary>
        /// <returns></returns>
        bool TryGetEndPoints<T>(out TryOperationResponse<T[]> endPoints) where T : INetworkEndpoint;
    }
}