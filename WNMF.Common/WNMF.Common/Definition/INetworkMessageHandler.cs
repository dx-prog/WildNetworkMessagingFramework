/***************************************************************
 * Notice:
 *       1) Do not remove copyright notice
 *       2) See License file (https://raw.githubusercontent.com/dx-prog/WildNetworkMessagingFramework/master/LICENSE) for more details 
 *       3) Copyright (c) 2017 David Garcia
 * ************************************************************/
using System;
using System.Linq.Expressions;

namespace WNMF.Common.Definition {
    public interface INetworkMessageHandler {
        /// <summary>
        ///     Determines if a specific message type can be handled by this handler
        /// </summary>
        /// <param name="messageType"></param>
        /// <returns></returns>
        bool IsSupportedMessageType(string messageType);

        /// <summary>
        ///     Enumerates the staged messages
        /// </summary>
        /// <param name="source"></param>
        /// <param name="messages"></param>
        /// <returns></returns>
        bool TryEnumerateStagedMessages(
            Expression<Func<NetworkMessageDescription, bool>> source,
            out TryOperationResponse<NetworkMessageDescription[]> messages);

        /// <summary>
        ///     Tries to get a message stream
        /// </summary>
        /// <param name="source"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        bool TryGetMessageStream(NetworkMessageDescription source,
            out TryOperationResponse<INetworkMessageStream> stream);

        /// <summary>
        ///     Stages the message into the work queue for the handler
        /// </summary>
        /// <param name="source">The source data stream</param>
        /// <param name="messageId">the ID of the message in the queue or the error code</param>
        /// <returns>true if the message was queued, false if there was an error</returns>
        bool TryStageMessage(INetworkMessageStream source, out TryOperationResponse<NetworkMessageDescription> messageId);

        /// <summary>
        ///     Tries to move a particular message out of the staging queue into the network
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="queuedMessageId">The message Id</param>
        /// <param name="responseCode">If failure, the error code to look up, if success, the remote object id</param>
        /// <returns></returns>
        bool TryDispatchMessage(INetworkSubscriberEndpoint endPoint,
            NetworkMessageDescription queuedMessageId,
            out TryOperationResponse<string> responseCode);
    }
}