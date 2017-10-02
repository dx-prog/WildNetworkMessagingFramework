/***************************************************************
 * Notice:
 *       1) Do not remove copyright notice
 *       2) See License file (https://raw.githubusercontent.com/dx-prog/WildNetworkMessagingFramework/master/LICENSE) for more details 
 *       3) Copyright (c) 2017 David Garcia
 * ************************************************************/

using System;
using WNMF.Common.Culture;

namespace WNMF.Common.Definition {
    public sealed class TryOperationResponse<T> : TryOperationResponseBase {
        public TryOperationResponse(Exception error, LocalizationKeys.LocalizationKey customMessageType) :
            base(error, customMessageType) {
            DataUnchecked = default;
        }

        public TryOperationResponse(LocalizationKeys.LocalizationKey messageType, T data) :
            base(messageType) {
            DataUnchecked = data;
        }

        public TryOperationResponse(string informationText, T data) : base(
            LocalizationKeys.ForGeneralPurposes.UnclassifiedError.Id,
            informationText,
            null
        ) {
            DataUnchecked = data;
        }


        /// <summary>
        ///     Get the data associated with a response; this will not check if there is an exception
        /// </summary>
        public T DataUnchecked { get; }

        /// <summary>
        ///     Get the data associated with a response, will throw exception if there is an
        ///     exception associated with this response
        /// </summary>
        public T Data {
            get {
                if (Exception != null)
                    throw LocalizationKeys.ExceptionMessages.NoData.GetException(Exception);

                return DataUnchecked;
            }
        }


        public override int GetHashCode() {
            var ax = MessageType != null ? MessageType.GetHashCode() : 0;
            if (Exception != null)
                return (ax << 7) | Exception.GetHashCode();
            if (DataUnchecked != null)
                return (ax << 7) | DataUnchecked.GetHashCode();

            return ax;
        }

        public Exception AsException() {
            throw new NotImplementedException();
        }
    }
}