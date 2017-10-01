/***************************************************************
 * Notice:
 *       1) Do not remove copyright notice
 *       2) See License file (https://raw.githubusercontent.com/dx-prog/WildNetworkMessagingFramework/master/LICENSE) for more details 
 *       3) Copyright (c) 2017 David Garcia
 * ************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using WNMF.Common.Culture;
using WNMF.Common.Definition;

namespace WNMF.Common.Foundation {
    /// <summary>
    ///     This only tracks the send history in memory, it could be extended to record its state to the file system
    /// </summary>
    public class SimpleRamHistory : INetworkMessageHistory {
        private readonly Dictionary<string, DateTime> _sent = new Dictionary<string, DateTime>();

        public virtual IDisposable BeginTransaction(out Action commit, out Action rollback) {
            commit = () => {
                lock (_sent) {
                    Commit(_sent);
                }
            };
            rollback = () => {
                lock (_sent) {
                    Rollback(_sent);
                }
            };
            Monitor.Enter(_sent);
            Debug.Assert(_sent != null, nameof(_sent) + " != null");
            return new MonitorLock(_sent);
        }

        public virtual bool TryMarkAsSent(
            string distroAgentId,
            INetworkEndpoint endpoint,
            NetworkMessageDescription messageDescription,
            out TryOperationResponse<bool> reason) {
            lock (_sent) {
                if (messageDescription.IsReady) {
                    var key = CreateKey(distroAgentId, endpoint, messageDescription);
                    if (_sent.ContainsKey(key)) {
                        reason = new TryOperationResponse<bool>(LocalizationKeys.NetworkMessageHistory.Success, true);
                        return false;
                    }

                    _sent[key] = DateTime.UtcNow;
                    reason = new TryOperationResponse<bool>(LocalizationKeys.NetworkMessageHistory.Success, true);
                    return true;
                }

                reason = new TryOperationResponse<bool>(
                    LocalizationKeys.NetworkMessageHistory.CannotMarkSentNonReadyMessage,
                    false);
                return false;
            }
        }

        public virtual bool TryCheckIfSent(
            string distroAgentId,
            INetworkEndpoint endpoint,
            NetworkMessageDescription messageDescription,
            out TryOperationResponse<bool> reason) {
            lock (_sent) {
                var key = CreateKey(distroAgentId, endpoint, messageDescription);

                reason = new TryOperationResponse<bool>(LocalizationKeys.NetworkMessageHistory.Success,
                    _sent.ContainsKey(key));
                return true;
            }
        }

        protected void Commit(IDictionary<string, DateTime> status) {
        }

        protected void Rollback(Dictionary<string, DateTime> sent) {
        }

        private static string CreateKey(string distroId,
            INetworkEndpoint endpoint,
            NetworkMessageDescription messageDescription) {
            var key =
                string.Join("|",
                    "DistroAgent:" + (distroId ?? "DEFAULT"),
                    "EndPoint:" + endpoint.GlobalId,
                    "MessageId:" + messageDescription.Id
                );
            return key;
        }
    }
}