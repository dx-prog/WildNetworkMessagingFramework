/***************************************************************
 * Notice:
 *       1) Do not remove copyright notice
 *       2) See License file (https://raw.githubusercontent.com/dx-prog/WildNetworkMessagingFramework/master/LICENSE) for more details 
 *       3) Copyright (c) 2017 David Garcia
 * ************************************************************/

using System.Runtime.ConstrainedExecution;

namespace WNMF.Common.Definition {
    /// <summary>
    ///     Normal Service Provider
    /// </summary>
    public interface INServiceProvider {
        /// <summary>
        ///     Tries to get a service if it is available
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service"></param>
        /// <param name="name">the name or id of the service if required</param>
        /// <returns></returns>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        bool TryGetService<T>(out TryOperationResponse<T> service, string name = null);

        /// <summary>
        ///     Get all services
        /// </summary>
        /// <returns></returns>
        bool TryGetServices(out TryOperationResponse<ServiceRegistration[]> response);
    }
}