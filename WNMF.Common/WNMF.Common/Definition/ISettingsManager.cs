/***************************************************************
 * Notice:
 *       1) Do not remove copyright notice
 *       2) See License file (https://raw.githubusercontent.com/dx-prog/WildNetworkMessagingFramework/master/LICENSE) for more details 
 *       3) Copyright (c) 2017 David Garcia
 * ************************************************************/

using System.Collections.Generic;
using WNMF.Common.Culture;

namespace WNMF.Common.Definition {
    public interface ISettingsManager {
        /// <summary>
        ///     Tries to get all available settings
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        bool TryGetSettings(
            out TryOperationResponse<IReadOnlyDictionary<LocalizationKeys.LocalizationKey, object>> settings);

        /// <summary>
        ///     Tries to update the settings provided; those not specified are should remain unchanged
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        bool TryUpdateSettings(IDictionary<LocalizationKeys.LocalizationKey, object> settings);
    }
}