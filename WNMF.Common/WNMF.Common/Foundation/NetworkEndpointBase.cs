/***************************************************************
 * Notice:
 *       1) Do not remove copyright notice
 *       2) See License file (https://raw.githubusercontent.com/dx-prog/WildNetworkMessagingFramework/master/LICENSE) for more details 
 *       3) Copyright (c) 2017 David Garcia
 * ************************************************************/

using System;
using WNMF.Common.Culture;
using WNMF.Common.Definition;

namespace WNMF.Common.Foundation {
    public abstract class NetworkEndPointBase : NServiceProvider, INetworkEndpoint {
        protected NetworkEndPointBase(Uri uri) {
            Uri = uri;
        }

        public virtual string GlobalId =>
            string.Join(";", "Src=" + Environment.MachineName, "Type=" + GetType().FullName, "Uri=" + Uri);

        /// <summary>
        ///     Get a value indicating of Try Change should work
        /// </summary>
        public virtual bool CanChange { get; protected set; }

        /// <summary>
        ///     Get the Uri for this endpoint
        /// </summary>
        public virtual Uri Uri { get; }

        /// <summary>
        ///     Try to change the endpoint
        /// </summary>
        /// <param name="newEndpoint"></param>
        /// <param name="oldEndpoint"></param>
        /// <returns></returns>
        public virtual bool TryChange(Uri newEndpoint, out TryOperationResponse<Uri> oldEndpoint) {
            if (CanChange) {
                oldEndpoint = new TryOperationResponse<Uri>(LocalizationKeys.ForGeneralPurposes.Success, Uri);
                return true;
            }

            oldEndpoint = new TryOperationResponse<Uri>(LocalizationKeys.ForGeneralPurposes.ValueCannotBeChanged,
                Uri);
            return false;
        }

        ///// <summary>
        /////     Try to send from the stream provided to the endpoint
        ///// </summary>
        ///// <param name="input"></param>
        ///// <param name="responseCode"></param>
        ///// <returns></returns>
        //public abstract bool TrySend(INetworkMessageStream input, out TryOperationResponse<string> responseCode);
    }
}