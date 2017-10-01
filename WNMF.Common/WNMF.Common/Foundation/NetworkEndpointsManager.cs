/***************************************************************
 * Notice:
 *       1) Do not remove copyright notice
 *       2) See License file (https://raw.githubusercontent.com/dx-prog/WildNetworkMessagingFramework/master/LICENSE) for more details 
 *       3) Copyright (c) 2017 David Garcia
 * ************************************************************/

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using WNMF.Common.Culture;
using WNMF.Common.Definition;

namespace WNMF.Common.Foundation {
    public class NetworkEndpointsManager : NServiceProvider, INetworkEndpointsManager {
        private readonly IComparer<INetworkEndpoint> _comparer;
        private readonly List<INetworkEndpoint> _endPoints = new List<INetworkEndpoint>();

        public NetworkEndpointsManager(IComparer<INetworkEndpoint> comparer = null) {
            _comparer = comparer ?? new NetworkEndpointComparer();
        }

        public bool TryAddEndPoint(INetworkEndpoint endpoint, out TryOperationResponse<INetworkEndpoint> existing) {
            var lockTaken = false;
            try {
                Monitor.TryEnter(_endPoints, 100, ref lockTaken);
                if (lockTaken) {
                    var tmp = _endPoints.FirstOrDefault(x => _comparer.Compare(endpoint, x) == 0);
                    if (tmp != null) {
                        existing =
                            new TryOperationResponse<INetworkEndpoint>(LocalizationKeys.ForNetworkGraphManager
                                    .ValueAlreadyExists,
                                tmp);
                        return false;
                    }

                    _endPoints.Add(endpoint);
                    existing =
                        new TryOperationResponse<INetworkEndpoint>(LocalizationKeys.ForNetworkGraphManager.Success,
                            endpoint);

                    return true;
                }
                else {
                    existing =
                        new TryOperationResponse<INetworkEndpoint>(
                            LocalizationKeys.ForNetworkGraphManager.TimeOutOnWait,
                            null);
                    return false;
                }
            }
            finally {
                if (lockTaken)
                    Monitor.Exit(_endPoints);
            }
        }

        public bool TryRemoveEndPoint(INetworkEndpoint endpoint,
            out TryOperationResponse<INetworkEndpoint> oldEndpoint) {
            var lockTaken = false;
            try {
                Monitor.TryEnter(_endPoints, 100, ref lockTaken);
                if (lockTaken) {
                    var tmp = _endPoints.FirstOrDefault(x => _comparer.Compare(endpoint, x) == 0);
                    if (tmp != null) {
                        oldEndpoint =
                            new TryOperationResponse<INetworkEndpoint>(LocalizationKeys.ForNetworkGraphManager
                                    .Success,
                                tmp);

                        return true;
                    }


                    oldEndpoint =
                        new TryOperationResponse<INetworkEndpoint>(
                            LocalizationKeys.ForNetworkGraphManager.ValueNotFound,
                            endpoint);

                    return true;
                }
                else {
                    oldEndpoint =
                        new TryOperationResponse<INetworkEndpoint>(
                            LocalizationKeys.ForNetworkGraphManager.TimeOutOnWait,
                            null);
                    return false;
                }
            }
            finally {
                if (lockTaken)
                    Monitor.Exit(_endPoints);
            }
        }

        public bool TryGetEndPoints(out TryOperationResponse<INetworkEndpoint[]> endPoints) {
            var lockTaken = false;
            try {
                Monitor.TryEnter(_endPoints, 100, ref lockTaken);
                if (lockTaken) {
                    if (_endPoints.Count == 0) {
                        endPoints =
                            new TryOperationResponse<INetworkEndpoint[]>(
                                LocalizationKeys.ForNetworkGraphManager.NoEndPoints,
                                _endPoints.ToArray());

                        return false;
                    }

                    endPoints =
                        new TryOperationResponse<INetworkEndpoint[]>(LocalizationKeys.ForNetworkGraphManager.Success,
                            _endPoints.ToArray());

                    return true;
                }
                else {
                    endPoints =
                        new TryOperationResponse<INetworkEndpoint[]>(
                            LocalizationKeys.ForNetworkGraphManager.TimeOutOnWait,
                            null);
                    return false;
                }
            }
            finally {
                if (lockTaken)
                    Monitor.Exit(_endPoints);
            }
        }
    }
}