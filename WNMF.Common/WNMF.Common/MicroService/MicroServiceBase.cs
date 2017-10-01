/***************************************************************
 * Notice:
 *       1) Do not remove copyright notice
 *       2) See License file (https://raw.githubusercontent.com/dx-prog/WildNetworkMessagingFramework/master/LICENSE) for more details 
 *       3) Copyright (c) 2017 David Garcia
 * ************************************************************/
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using WNMF.Common.Definition;

namespace WNMF.Common.MicroService {
    public class MicroServiceBase : IDisposable {
        private readonly ConcurrentBag<AgentState> _agents =
            new ConcurrentBag<AgentState>();

        private readonly CancellationTokenSource _cancelation;

        private readonly Thread _thread;


        public MicroServiceBase(
            params ISimpleNetworkMessagePublisher[]sendAgents) {
            foreach (var agent in sendAgents)
                _agents.Add(new AgentState(agent));

            _cancelation = CancellationTokenSource.CreateLinkedTokenSource(new CancellationToken());
            // do not want to use thread pool here
            _thread = new Thread(Worker) {
                IsBackground = true
            };
            _thread.Start();
        }

        public ThreadState State => _thread.ThreadState;

        public void Dispose() {
            _cancelation.Cancel();
        }

        public void Publish(INetworkMessageStream sourceData) {
            foreach (var agent in _agents)
                agent.Queue.Enqueue(sourceData);
        }

        /// <summary>
        /// </summary>
        /// <param name="state"></param>
        /// <param name="queuedStream"></param>
        /// <param name="stagingResult"></param>
        /// <returns>true to resend</returns>
        protected virtual bool NotifyStageState(AgentState state,
            INetworkMessageStream queuedStream,
            List<TryOperationResponseBase> stagingResult) {
            return true; // Retry 
        }

        protected virtual void NotifyPumpState(AgentState state,
            double successRate,
            List<TryOperationResponseBase> pumpResults) {
        }

        private void Worker(object obj) {
            while (_cancelation.IsCancellationRequested == false)
                foreach (var agentState in _agents) {
                    if (!agentState.Queue.TryDequeue(out var outBound))
                        continue;

                    var sendSuccess = agentState.Agent.Stage(outBound, out var message);
                    if (!sendSuccess && NotifyStageState(agentState, outBound, message))
                        agentState.Queue.Enqueue(outBound);

                    var sr = agentState.Agent.Pump(out var pumpResults);

                    NotifyPumpState(agentState, sr, pumpResults);
                }
        }

        public class AgentState {
            public AgentState(ISimpleNetworkMessagePublisher agent) {
                Agent = agent;
                Queue = new ConcurrentQueue<INetworkMessageStream>();
            }

            public ConcurrentQueue<INetworkMessageStream> Queue { get; }

            public ISimpleNetworkMessagePublisher Agent { get; }
        }
    }
}