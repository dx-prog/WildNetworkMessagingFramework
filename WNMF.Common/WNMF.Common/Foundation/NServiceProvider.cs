/***************************************************************
 * Notice:
 *       1) Do not remove copyright notice
 *       2) See License file (https://raw.githubusercontent.com/dx-prog/WildNetworkMessagingFramework/master/LICENSE) for more details 
 *       3) Copyright (c) 2017 David Garcia
 * ************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WNMF.Common.Culture;
using WNMF.Common.Definition;

namespace WNMF.Common.Foundation {
    /// <summary>
    ///     An Named Service provider which may be extended as needed to expose named or identified
    ///     services depending on the scenario.
    /// </summary>
    public class NServiceProvider : INServiceProvider {
        private readonly List<ServiceRegistration> _services = new List<ServiceRegistration>();

        public NServiceProvider() {
            // ReSharper disable once VirtualMemberCallInConstructor
            var m = GetType().GetMethod(nameof(RegisterService), BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var interfaceType in GetType().GetInterfaces())
                m.MakeGenericMethod(interfaceType)
                    .Invoke(this,
                        new object[] {
                            this,
                            null
                        });
        }

        /// <summary>
        ///     Tries to get a service of a particular type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual bool TryGetService<T>(out TryOperationResponse<T> service, string name = null) {
            try {
                return (service = GetService<T>(name)).Exception == null;
            }
            catch (Exception ex) {
                service = new TryOperationResponse<T>(ex, LocalizationKeys.ForGeneralPurposes.ServiceResolveFailed);
                return false;
            }
        }

        /// <summary>
        ///     Tries to the services associated with this provider. They type and count may change depending on the implementation
        ///     or use case.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public virtual bool TryGetServices(out TryOperationResponse<ServiceRegistration[]> response) {
            lock (_services) {
                response = new TryOperationResponse<ServiceRegistration[]>(
                    LocalizationKeys.ForGeneralPurposes.Success,
                    _services.ToArray());
            }

            return true;
        }

        /// <summary>
        ///     Register a new service
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        protected internal virtual NServiceProvider RegisterService<T>(T obj, string name = null) {
            lock (_services) {
                var predicate = new Predicate<Type>(x => typeof(T).IsAssignableFrom(x));
                _services.Add(new ServiceRegistration(name, predicate, obj));
            }
            return this;
        }

        private TryOperationResponse<T> GetService<T>(string name) {
            lock (_services) {
                var match = _services.FirstOrDefault(x => x.TypeCheck(typeof(T)) && x.Name == name);
                if (null != match)
                    return new TryOperationResponse<T>(LocalizationKeys.ForGeneralPurposes.Success,
                        (T) match.Instance);


                return new TryOperationResponse<T>(LocalizationKeys.ForGeneralPurposes.ServiceResolveFailed, default);
            }
        }
    }
}