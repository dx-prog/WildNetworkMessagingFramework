/***************************************************************
 * Notice:
 *       1) Do not remove copyright notice
 *       2) See License file (https://raw.githubusercontent.com/dx-prog/WildNetworkMessagingFramework/master/LICENSE) for more details 
 *       3) Copyright (c) 2017 David Garcia
 * ************************************************************/
using System;

namespace WNMF.Common.Definition {
    /// <summary>
    ///     The Endpoint is only concerned with getting data to some endpoint
    /// </summary>
    public interface INetworkEndpoint : INServiceProvider {

        /// <summary>
        /// All end points should have a unique ID
        /// </summary>
        string GlobalId { get; }

        /// <summary>
        ///     Get value indicating if the EndPoint can be changed
        /// </summary>
        bool CanChange { get; }

        /// <summary>
        ///     This EndPoint may be FTP, TCP, HTTP, or anything else
        /// </summary>
        Uri Uri { get; }

        /// <summary>
        ///     Try to change the current endpoint
        /// </summary>
        /// <param name="newEndpoint"></param>
        /// <param name="oldEndpoint"></param>
        /// <returns></returns>
        bool TryChange(Uri newEndpoint, out TryOperationResponse<Uri> oldEndpoint);

        /// <summary>
        ///     Attempts to send data to some endpint
        /// </summary>
        /// <param name="input"></param>
        /// <param name="responseCode">the tracking id of the object sent as reported by the remote; or error information</param>
        bool TrySend(INetworkMessageStream input, out TryOperationResponse<string> responseCode);
    }
}