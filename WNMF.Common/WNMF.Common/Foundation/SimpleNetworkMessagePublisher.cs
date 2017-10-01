/***************************************************************
 * Notice:
 *       1) Do not remove copyright notice
 *       2) See License file (https://raw.githubusercontent.com/dx-prog/WildNetworkMessagingFramework/master/LICENSE) for more details 
 *       3) Copyright (c) 2017 David Garcia
 * ************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using WNMF.Common.Culture;
using WNMF.Common.Definition;

namespace WNMF.Common.Foundation {
    public class SimpleNetworkMessagePublisher : NServiceProvider, ISimpleNetworkMessagePublisher {
        private readonly INetworkEndpointsManager _endpoints;
        private readonly INetworkMessageHandler _handler;
        private readonly INetworkMessageHistory _historyLog;


        public SimpleNetworkMessagePublisher(
            string name,
            INetworkEndpointsManager endpoints,
            INetworkMessageHandler handler,
            INetworkMessageHistory historyLog) {
            AgentName = name;
            _endpoints = endpoints;
            _handler = handler;
            _historyLog = historyLog;

            // ReSharper disable once VirtualMemberCallInConstructor
            RegisterService(_endpoints).RegisterService(handler).RegisterService(historyLog);
        }

        public string AgentName { get; }

        public bool Stage(INetworkMessageStream src, out List<TryOperationResponseBase> responses) {
            responses = new List<TryOperationResponseBase>();
            if (false == _handler.IsSupportedMessageType(src.Description.MessageType)) {
                responses.Add(
                    new TryOperationResponseBase(LocalizationKeys.NetworkMessageHandler.UnsupportedMessageType));
                return false;
            }

            var success = _handler.TryStageMessage(src, out var response);
            responses.Add(response);
            return success;
        }

        public double Pump(out List<TryOperationResponseBase> responses) {
            responses = new List<TryOperationResponseBase>();
            var cannotContinue = !_endpoints.TryGetEndPoints(out var endPoints);
            responses.Add(endPoints);
            if (cannotContinue)
                return 0;

            if (endPoints.Data == null || endPoints.Data.Length == 0) {
                responses.Add(new TryOperationResponseBase(
                    LocalizationKeys.ForNetworkGraphManager.NoEndPoints));
                return 0;
            }

            cannotContinue = !_handler.TryEnumerateStagedMessages(x => CheckIfNotSent(x, endPoints.Data),
                out var pendingWork);
            responses.Add(pendingWork);
            if (cannotContinue)
                return 0;

            double sendTotalCount = 0;
            double sendSuccessCount = 0;

            var singleEndPoint = new INetworkEndpoint[1];
            foreach (var endpoint in endPoints.Data) {
                singleEndPoint[0] = endpoint;
                foreach (var message in pendingWork.Data)
                    try {
                        sendTotalCount++;
                        using (_historyLog.BeginTransaction(out var commit, out var rollback)) {
                            try {
                                if (!CheckIfNotSent(message, singleEndPoint))
                                    continue;

                                var sendSuccess = _handler.TryDispatchMessage(
                                    endpoint,
                                    message,
                                    out var responseCod
                                );
                                responses.Add(responseCod);
                                if (sendSuccess) {
                                    var markSentSuccess = _historyLog.TryMarkAsSent(AgentName,
                                        endpoint,
                                        message,
                                        out var markSentResult);
                                    responses.Add(markSentResult);
                                    if (!markSentSuccess || markSentResult.Exception != null) {
                                        rollback();
                                        continue;
                                    }
                                }

                                commit();
                                sendSuccessCount++;
                            }
                            catch (Exception) {
                                rollback();
                                throw;
                            }
                        }
                    }
                    catch (Exception ex) {
                        responses.Add(new TryOperationResponse<
                            Tuple<INetworkEndpoint, NetworkMessageDescription, Exception>>(
                            LocalizationKeys.SpecialOperationalResult.FailureTuple,
                            Tuple.Create(endpoint, message, ex)
                        ));
                    }
            }

            return sendSuccessCount / sendTotalCount;
        }

        protected virtual bool CheckIfNotSent(NetworkMessageDescription networkMessageDescription,
            INetworkEndpoint[] endPointsData) {
            if (endPointsData == null || endPointsData.Length == 0)
                throw LocalizationKeys.ExceptionMessages.ObjectNotInExpectedState.GetException();

            return endPointsData.Any(x => {
                // the history object should never fail
                // unless its backing store is down i.e. drive is out or server is down
                if (!_historyLog.TryCheckIfSent(AgentName,
                    x,
                    networkMessageDescription,
                    out var reason))
                    throw LocalizationKeys.ExceptionMessages.ObjectNotInExpectedState.GetException();

                return !reason.Data; // true if sent, we need to return true if not sent, so invert
            });
        }
    }
}