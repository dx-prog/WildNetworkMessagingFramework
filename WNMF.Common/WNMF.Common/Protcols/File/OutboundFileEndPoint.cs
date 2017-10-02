/***************************************************************
 * Notice:
 *       1) Do not remove copyright notice
 *       2) See License file (https://raw.githubusercontent.com/dx-prog/WildNetworkMessagingFramework/master/LICENSE) for more details 
 *       3) Copyright (c) 2017 David Garcia
 * ************************************************************/

using System;
using System.IO;
using System.Threading;
using WNMF.Common.Culture;
using WNMF.Common.Definition;
using WNMF.Common.Foundation;

namespace WNMF.Common.Protcols.File {
    public class OutboundFileEndPoint : FileEndPointBase,
        INetworkSubscriberEndpoint {
        public OutboundFileEndPoint(Uri uri) : base(uri) {
        }

        public bool CreateDirectory { get; set; } = true;

        public virtual bool TrySend(INetworkMessageStream input, out TryOperationResponse<string> responseCode) {
            var fileName = GetRandomFileName();
            var stageName = Path.Combine(Uri.LocalPath, fileName);

            try {
                if (CreateDirectory)
                    Directory.CreateDirectory(Uri.LocalPath);

                try {
                    using (var dst = System.IO.File.Open(stageName,
                        FileMode.OpenOrCreate,
                        FileAccess.Write,
                        FileShare.None)) {
                        input.ChunkTo(dst);
                    }
                    // mark a file as readonly to indicate we are done with it

                    FileBasedNetworkMessageHandler.MarkFileAsComplete(stageName,
                        new NetworkMessageDescription(new FileInfo(stageName), input.Description.MessageType));

                    responseCode =
                        new TryOperationResponse<string>(LocalizationKeys.ForGeneralPurposes.Success, fileName);


                    return true;
                }
                catch (ThreadAbortException ex) {
                    Thread.ResetAbort();
                    responseCode =
                        new TryOperationResponse<string>(ex,
                            LocalizationKeys.ForGeneralPurposes.OutOfBandError);
                    return false;
                }
            }
            catch (Exception ex) {
                responseCode =
                    new TryOperationResponse<string>(ex, LocalizationKeys.ForGeneralPurposes.SendFailed);
                return false;
            }
        }
    }
}