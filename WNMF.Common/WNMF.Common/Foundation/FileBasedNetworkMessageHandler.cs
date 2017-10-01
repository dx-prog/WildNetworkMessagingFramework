/***************************************************************
 * Notice:
 *       1) Do not remove copyright notice
 *       2) See License file (https://raw.githubusercontent.com/dx-prog/WildNetworkMessagingFramework/master/LICENSE) for more details 
 *       3) Copyright (c) 2017 David Garcia
 * ************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using WNMF.Common.Culture;
using WNMF.Common.Definition;
using WNMF.Common.Protcols.File;

namespace WNMF.Common.Foundation {
    /// <summary>
    ///     This is a Network message handler that uses the local file system for caching
    /// </summary>
    public class FileBasedNetworkMessageHandler : NServiceProvider, INetworkMessageHandler {
        private const string TypeDefinition = ".TYPE";
        private const string IgnoreDefinition = ".IGNORE";
        private readonly Func<string, bool> _canHandle;
        private readonly string _fileDrop;

        public FileBasedNetworkMessageHandler(
            Func<string, bool> canHandle,
            string fileDrop) {
            _canHandle = canHandle ?? (s => true);
            _fileDrop = fileDrop;
        }

        public bool IsSupportedMessageType(string messageType) {
            return _canHandle(messageType);
        }

        public bool TryEnumerateStagedMessages(Expression<Func<NetworkMessageDescription, bool>> source,
            out TryOperationResponse<NetworkMessageDescription[]> messages) {
            var descriptions = new List<NetworkMessageDescription>();
            try {
                var callback = source.Compile();
                // NOTE: DO NOT CONVERT TO EXPRESSION
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var filePath in Directory.EnumerateFiles(_fileDrop, "*")) {
                    var extension = Path.GetExtension(filePath).ToUpperInvariant();
                    switch (extension) {
                        case IgnoreDefinition:
                            continue;
                        case TypeDefinition:
                            continue;
                    }

                    // a File can be marked as ignored
                    if (File.Exists(filePath + IgnoreDefinition))
                        continue;
                    // Files without a type definition are automatically ignored
                    if (!File.Exists(filePath + TypeDefinition))
                        continue;

                    var desc = CreateMessageDescription(filePath);
                    if (callback(desc))
                        descriptions.Add(desc);
                }

                messages = new TryOperationResponse<NetworkMessageDescription[]>(
                    LocalizationKeys.NetworkMessageHandler.Success,
                    descriptions.ToArray());
                return true;
            }
            catch (Exception ex) {
                messages = new TryOperationResponse<NetworkMessageDescription[]>(ex,
                    LocalizationKeys.NetworkMessageHandler.StaggingEnumerationFailed);
                return false;
            }
        }

        public bool TryGetMessageStream(NetworkMessageDescription source,
            out TryOperationResponse<INetworkMessageStream> stream) {
            if (!TryEnumerateStagedMessages(
                f => f.Id == source.Id,
                out var descripton)) {
                if (descripton.Exception != null)
                    stream = new TryOperationResponse<INetworkMessageStream>(descripton.Exception,
                        new LocalizationKeys.LocalizationKey(descripton.MessageType));
                else
                    stream = new TryOperationResponse<INetworkMessageStream>(
                        LocalizationKeys.NetworkMessageHandler.StaggingEnumerationFailed,
                        null);

                return false;
            }

            if (descripton.Data.Length == 0) {
                stream = new TryOperationResponse<INetworkMessageStream>(
                    LocalizationKeys.NetworkMessageHandler.MessageNotFound,
                    null);
                return false;
            }

            if (descripton.Data.Length > 1) {
                stream = new TryOperationResponse<INetworkMessageStream>(
                    LocalizationKeys.NetworkMessageHandler.StaggingEnumerationReturnedUnxpectedEntryCountKey,
                    null);
                return false;
            }

            if (descripton.Data[0].IsReady == false) {
                stream = new TryOperationResponse<INetworkMessageStream>(
                    LocalizationKeys.NetworkMessageHandler.MessageFoundNotReady,
                    null);
                return false;
            }

            stream = new TryOperationResponse<INetworkMessageStream>(
                LocalizationKeys.NetworkMessageHandler.Success,
                CreateReadStream(new FileInfo(Path.Combine(_fileDrop, descripton.Data[0].Id))));

            return true;
        }

        public bool TryStageMessage(INetworkMessageStream source,
            out TryOperationResponse<NetworkMessageDescription> messageId) {
            var fileName = FileEndPoint.GetRandomFileName();
            var filePath = Path.Combine(_fileDrop, fileName);
            try {
                using (var fs = File.Open(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.None)) {
                    source.ChunkTo(fs);
                    fs.Flush();
                }
                MarkFileAsComplete(source, filePath);

                messageId = new TryOperationResponse<NetworkMessageDescription>(
                    LocalizationKeys.NetworkMessageHandler.Success,
                    CreateMessageDescription(filePath));
                return true;
            }
            catch (Exception ex) {
                messageId = new TryOperationResponse<NetworkMessageDescription>(ex,
                    LocalizationKeys.NetworkMessageHandler.StaggingFailed);
                return false;
            }
        }

        public bool TryDispatchMessage(
            INetworkEndpoint endPoint,
            NetworkMessageDescription queuedMessageId,
            out TryOperationResponse<string> responseCode) {
            try {
                if (TryGetMessageStream(queuedMessageId, out var stream))
                    return endPoint.TrySend(stream.Data,
                        out responseCode);

                throw stream.AsException();
            }
            catch (Exception ex) {
                responseCode = new TryOperationResponse<string>(
                    ex,
                    LocalizationKeys.NetworkMessageHandler.CriticalFailureOnSend);
                return false;
            }
        }

        public static void MarkFileAsComplete(string filePath, NetworkMessageDescription description) {
            File.WriteAllText(filePath + TypeDefinition, description.MessageType);

            // Message Type must be made readonly in order to be ready
            var fi = new FileInfo(filePath + TypeDefinition) {IsReadOnly = true};
            fi.Refresh();
            // Destination File must be made readonly in order to be ready
            fi = new FileInfo(filePath) {IsReadOnly = true};
            fi.Refresh();

            if (fi.IsReadOnly == false)
                throw new InvalidOperationException((string) LocalizationKeys.ExceptionMessages
                    .ObjectNotInExpectedState);
        }

        private static NetworkMessageDescription CreateMessageDescription(string filePath) {
            var fi = new FileInfo(filePath);
            var desc = new NetworkMessageDescription(fi, File.ReadAllText(filePath + TypeDefinition));
            return desc;
        }

        /// <summary>
        ///     Marks a file that is stored in the folder drop as being complete using meta data files
        ///     and file state attributes
        /// </summary>
        /// <param name="source"></param>
        /// <param name="filePath"></param>
        private static void MarkFileAsComplete(INetworkMessageStream source, string filePath) {
            var description = source.Description;
            // The file message definition is stored in an sibling file
            MarkFileAsComplete(filePath, description);
        }

        private static SimpleThreadLocalStream CreateReadStream(FileInfo fi) {
            return new SimpleThreadLocalStream(d => fi.Open(FileMode.Open, FileAccess.Read, FileShare.Read),
                new NetworkMessageDescription(fi, File.ReadAllText(fi.FullName + TypeDefinition)));
        }
    }
}