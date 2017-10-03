using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Threading;
using WNMF.Common.Culture;
using WNMF.Common.Definition;
using WNMF.Common.Foundation;

namespace WNMF.Common.Protcols.Http {
    public class JsonEndPoint : NetworkEndPointBase,
        INetworkSubscriberEndpoint {
        public JsonEndPoint(Uri uri) : base(uri) {
        }

        public bool TrySend(INetworkMessageStream input, out TryOperationResponse<string> responseCode) {
            try {
                var request = WebRequest.CreateHttp(Uri);
                Configure(request);

                var asyncResult = new ConcurrentBag<object>();
                var waiter = request.BeginGetRequestStream(cb => {
                        var stream = request.EndGetRequestStream(cb);
                        var sw = new StringWriter();
                        try {
                            using (input.BeginReadScope()) {
                                input.ChunkTo(stream);
                            }
                            using (var sr = new StreamReader(stream)) {
                                sw.Write(sr.ReadToEnd());
                            }
                            asyncResult.Add(sw);
                        }
                        catch (Exception ex) {
                            asyncResult.Add(ex);
                        }
                    },
                    null);

                HandleWait(waiter.AsyncWaitHandle);

                if (!asyncResult.TryTake(out var result)) {
                    responseCode =
                        new TryOperationResponse<string>(LocalizationKeys.ForGeneralPurposes.SendFailed, null);
                    return false;
                }

                switch (result) {
                    case Exception subEx:
                        responseCode =
                            new TryOperationResponse<string>(subEx, LocalizationKeys.ForGeneralPurposes.SendFailed);
                        return false;
                    case TextWriter writer:
                        responseCode =
                            new TryOperationResponse<string>(LocalizationKeys.ForGeneralPurposes.Success,
                                writer.ToString());
                        return true;
                    default:
                        throw new InvalidOperationException();
                }
            }
            catch (Exception ex) {
                responseCode = new TryOperationResponse<string>(ex, LocalizationKeys.ForGeneralPurposes.SendFailed);
                return false;
            }
        }

        protected virtual void HandleWait(WaitHandle waiterAsyncWaitHandle) {
            waiterAsyncWaitHandle.WaitOne(10000);
        }

        protected void Configure(HttpWebRequest request) {
            request.UserAgent = "Test Agent";
        }
    }
}