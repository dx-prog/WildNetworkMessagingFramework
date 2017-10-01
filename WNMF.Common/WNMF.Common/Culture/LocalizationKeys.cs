/***************************************************************
 * Notice:
 *       1) Do not remove copyright notice
 *       2) See License file (https://raw.githubusercontent.com/dx-prog/WildNetworkMessagingFramework/master/LICENSE) for more details 
 *       3) Copyright (c) 2017 David Garcia
 * ************************************************************/
using System;

namespace WNMF.Common.Culture {
    /// <summary>
    ///     Provides a way to track localization lookup keys and provide
    ///     a way to expose auto lookup/resolve operation
    /// </summary>
    public class LocalizationKeys {
        public class LocalizationKey {
            public LocalizationKey(string systemId) {
                Id = systemId;
            }

            public LocalizationKey(string owner, string messageId, Type errorType = null) {
                Id = owner + "." + messageId;
                ErrorType = errorType;
            }

            /// <summary>
            ///     If this localization key is associated with a message for an exception
            /// </summary>
            public Type ErrorType { get; }

            public string Id { get; }

            public string Resolve() {
                return Localization.CurrentManager.ResolveString(Id);
            }

            public static explicit operator string(LocalizationKey src) {
                return src.Resolve();
            }

            public Exception GetException(Exception nestedExcpetion = null) {
                if (ErrorType == null)
                    return null;

                Exception ex;
                if (nestedExcpetion == null)
                    ex = (Exception) Activator.CreateInstance(
                        ErrorType,
                        Resolve());

                else
                    ex = (Exception) Activator.CreateInstance(
                        ErrorType,
                        Resolve(),
                        nestedExcpetion);
                ex.Data["LocalizationKey"] = Id;
                return ex;
            }
        }


        public static class NetworkMessageHistory {
            /// <summary>
            ///     See ForGeneralPurposes.Success
            /// </summary>
            /// <see cref="ForGeneralPurposes.TimeOutOnWait" />
            public static readonly LocalizationKey Success = ForGeneralPurposes.Success;

            /// <summary>
            ///     The message requested to mark as sent is not in a ready state
            /// </summary>
            public static readonly LocalizationKey CannotMarkSentNonReadyMessage =
                new LocalizationKey(nameof(NetworkMessageHistory),
                    nameof(CannotMarkSentNonReadyMessage),
                    typeof(InvalidOperationException));
        }

        public static class ForGeneralPurposes {
            /// <summary>
            ///     An unclassified error
            /// </summary>
            public static readonly LocalizationKey UnclassifiedError =
                new LocalizationKey(nameof(ForGeneralPurposes),
                    nameof(UnclassifiedError),
                    typeof(InvalidOperationException));

            /// <summary>
            ///     Success
            /// </summary>
            public static readonly LocalizationKey Success =
                new LocalizationKey(nameof(ForGeneralPurposes), nameof(Success));

            public static readonly LocalizationKey TimeOutOnWait =
                new LocalizationKey(nameof(ForGeneralPurposes), nameof(TimeOutOnWait), typeof(TimeoutException));

            public static readonly LocalizationKey ValueAlreadyExists =
                new LocalizationKey(nameof(ForGeneralPurposes), nameof(ValueAlreadyExists), typeof(ArgumentException));

            public static readonly LocalizationKey ValueNotFound =
                new LocalizationKey(nameof(ForGeneralPurposes), nameof(ValueNotFound), typeof(ArgumentException));

            public static readonly LocalizationKey ValueCannotBeChanged =
                new LocalizationKey(nameof(ForGeneralPurposes),
                    nameof(ValueCannotBeChanged),
                    typeof(ArgumentException));

            public static readonly LocalizationKey ServiceResolveFailed =
                new LocalizationKey(nameof(ForGeneralPurposes), nameof(ServiceResolveFailed), typeof(ArgumentException));

            public static readonly LocalizationKey SendFailed =
                new LocalizationKey(nameof(ForGeneralPurposes), nameof(SendFailed), typeof(InvalidOperationException));

            public static readonly LocalizationKey OutOfBandError =
                new LocalizationKey(nameof(ForGeneralPurposes), nameof(OutOfBandError), typeof(SystemException));


            public static readonly LocalizationKey ObjectNotInExpectedState =
                new LocalizationKey(nameof(ForGeneralPurposes),
                    nameof(ObjectNotInExpectedState),
                    typeof(InvalidOperationException));
        }

        public static class ForNetworkGraphManager {
            /// <summary>
            ///     See ForGeneralPurposes.Success
            /// </summary>
            /// <see cref="ForGeneralPurposes.TimeOutOnWait" />
            public static readonly LocalizationKey Success = ForGeneralPurposes.Success;

            /// <summary>
            ///     Value not found
            /// </summary>
            public static readonly LocalizationKey ValueNotFound = ForGeneralPurposes.ValueNotFound;

            /// <summary>
            ///     See ForGeneralPurposes.TimeOutOnWait
            /// </summary>
            /// <see cref="ForGeneralPurposes.TimeOutOnWait" />
            public static readonly LocalizationKey TimeOutOnWait =
                ForGeneralPurposes.TimeOutOnWait;

            /// <summary>
            ///     Value already exists
            /// </summary>
            public static readonly LocalizationKey ValueAlreadyExists
                = ForGeneralPurposes.ValueAlreadyExists;

            public static readonly LocalizationKey NoEndPoints =
                new LocalizationKey(nameof(ForNetworkGraphManager), nameof(NoEndPoints), typeof(ArgumentException));
        }

        public static class NetworkMessageHandler {
            /// <summary>
            ///     See ForGeneralPurposes.Success
            /// </summary>
            /// <see cref="ForGeneralPurposes.TimeOutOnWait" />
            public static readonly LocalizationKey Success = ForGeneralPurposes.Success;

            public static readonly LocalizationKey MessageNotFound =
                new LocalizationKey(nameof(NetworkMessageHandler), nameof(MessageNotFound), typeof(ArgumentException));

            public static readonly LocalizationKey StaggingFailed =
                new LocalizationKey(nameof(NetworkMessageHandler),
                    nameof(StaggingFailed),
                    typeof(InvalidOperationException));

            public static readonly LocalizationKey MessageFoundNotReady =
                new LocalizationKey(nameof(NetworkMessageHandler),
                    nameof(MessageFoundNotReady),
                    typeof(ArgumentException));

            public static readonly LocalizationKey CriticalFailureOnSend =
                new LocalizationKey(nameof(NetworkMessageHandler),
                    nameof(CriticalFailureOnSend),
                    typeof(InvalidOperationException));

            public static readonly LocalizationKey UnsupportedMessageType =
                new LocalizationKey(nameof(NetworkMessageHandler),
                    nameof(UnsupportedMessageType),
                    typeof(InvalidOperationException));

            public static readonly LocalizationKey StaggingEnumerationFailed =
                new LocalizationKey(nameof(NetworkMessageHandler),
                    nameof(StaggingEnumerationFailed),
                    typeof(InvalidOperationException));

            public static readonly LocalizationKey StaggingEnumerationReturnedUnxpectedEntryCountKey =
                new LocalizationKey(nameof(NetworkMessageHandler),
                    nameof(StaggingEnumerationReturnedUnxpectedEntryCountKey),
                    typeof(InvalidOperationException));
        }

        public static class SpecialOperationalResult {
            public static readonly LocalizationKey FailureTuple =
                new LocalizationKey(nameof(SpecialOperationalResult), nameof(FailureTuple));
        }

        public static class ExceptionMessages {
            public static readonly LocalizationKey NoData =
                new LocalizationKey(nameof(ExceptionMessages), nameof(NoData), typeof(InvalidOperationException));

            public static readonly LocalizationKey ObjectNotInExpectedState =
                new LocalizationKey(nameof(ExceptionMessages),
                    nameof(ObjectNotInExpectedState),
                    typeof(InvalidOperationException));
        }
    }
}