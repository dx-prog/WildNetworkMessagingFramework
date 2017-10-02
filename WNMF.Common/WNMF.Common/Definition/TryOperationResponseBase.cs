/***************************************************************
 * Notice:
 *       1) Do not remove copyright notice
 *       2) See License file (https://raw.githubusercontent.com/dx-prog/WildNetworkMessagingFramework/master/LICENSE) for more details 
 *       3) Copyright (c) 2017 David Garcia
 * ************************************************************/

using System;
using WNMF.Common.Culture;

namespace WNMF.Common.Definition {
    /// <summary>
    ///     Ideally any operation that Outs TryOperation should not throw an exception
    /// </summary>
    public class TryOperationResponseBase {
        public TryOperationResponseBase(string messageType, string informationText, Exception ex) {
            MessageType = messageType;
            InformationText = informationText;
            Exception = ex;
        }

        public TryOperationResponseBase(Exception error, LocalizationKeys.LocalizationKey customMessageType) {
            InformationText = customMessageType.Resolve();
            MessageType = customMessageType.Id;

            Exception = customMessageType.ErrorType != null ? customMessageType.GetException(error) : error;
        }

        public TryOperationResponseBase(LocalizationKeys.LocalizationKey messageType) {
            InformationText = messageType.Resolve();
            MessageType = messageType.Id;
            if (messageType.ErrorType != null)
                Exception = messageType.GetException();
        }

        /// <summary>
        ///     The Type of message
        /// </summary>
        public string MessageType { get; }

        /// <summary>
        ///     The Type of message
        /// </summary>
        public string InformationText { get; }

        /// <summary>
        ///     If an Exception Occured (and there is no Data), this is set
        /// </summary>
        public Exception Exception { get; }
    }
}