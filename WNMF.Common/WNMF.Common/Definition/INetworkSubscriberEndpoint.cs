/***************************************************************
 * Notice:
 *       1) Do not remove copyright notice
 *       2) See License file (https://raw.githubusercontent.com/dx-prog/WildNetworkMessagingFramework/master/LICENSE) for more details 
 *       3) Copyright (c) 2017 David Garcia
 * ************************************************************/

namespace WNMF.Common.Definition {
    public interface INetworkSubscriberEndpoint : INetworkEndpoint {
        /// <summary>
        ///     Attempts to send data to some endpint
        /// </summary>
        /// <param name="input"></param>
        /// <param name="responseCode">the tracking id of the object sent as reported by the remote; or error information</param>
        bool TrySend(INetworkMessageStream input, out TryOperationResponse<string> responseCode);
    }
}