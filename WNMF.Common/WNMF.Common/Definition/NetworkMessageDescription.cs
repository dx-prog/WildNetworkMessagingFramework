/***************************************************************
 * Notice:
 *       1) Do not remove copyright notice
 *       2) See License file (https://raw.githubusercontent.com/dx-prog/WildNetworkMessagingFramework/master/LICENSE) for more details 
 *       3) Copyright (c) 2017 David Garcia
 * ************************************************************/

using System;
using System.IO;

namespace WNMF.Common.Definition {
    public class NetworkMessageDescription {
        /// <summary>
        ///     For other types of message descriptions
        /// </summary>
        protected NetworkMessageDescription() {
        }

        public NetworkMessageDescription(FileInfo fi, string messageType) {
            Id = fi.Name;
            IsReady = fi.Exists && fi.IsReadOnly;

            MessageType = messageType;
            if (!fi.Exists)
                return;

            CreationTimeUtc = fi.CreationTimeUtc;
            MessageSize = fi.Length;
            LastChangedUtc = fi.LastWriteTimeUtc;
        }

        /// <summary>
        ///     Get a value indicating the size of the message
        /// </summary>
        public long MessageSize { get; protected set; }

        /// <summary>
        ///     Gets a value indicating if the message can be sent
        /// </summary>
        public bool IsReady { get; protected set; }

        /// <summary>
        ///     Gets a value indicating the data when the message was created
        /// </summary>
        public DateTime CreationTimeUtc { get; protected set; }

        /// <summary>
        ///     Gets a value indicating the data when the message was created
        /// </summary>
        public DateTime LastChangedUtc { get; protected set; }

        /// <summary>
        ///     The type of message
        /// </summary>
        public string MessageType { get; }

        /// <summary>
        ///     Gets the unique ID for the message
        /// </summary>
        public string Id { get; protected set; }
    }
}