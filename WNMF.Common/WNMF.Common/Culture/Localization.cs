/***************************************************************
 * Notice:
 *       1) Do not remove copyright notice
 *       2) See License file (https://raw.githubusercontent.com/dx-prog/WildNetworkMessagingFramework/master/LICENSE) for more details 
 *       3) Copyright (c) 2017 David Garcia
 * ************************************************************/
namespace WNMF.Common.Culture {
    /// <summary>
    /// This class provides a way to customize locatalization of messages throughout the application
    /// and framework
    /// </summary>
    public class Localization {
        public static readonly Localization DefaultManager = new Localization();
        private static Localization _current;

        /// <summary>
        ///     Gets or sets the current manager
        /// </summary>
        /// <remarks>If set to null, the current manager becomes the default manager</remarks>
        public static Localization CurrentManager {
            get => _current ?? DefaultManager;
            set => _current = value;
        }

        public virtual string ResolveString(string stringId) {
            return stringId;
        }
    }
}