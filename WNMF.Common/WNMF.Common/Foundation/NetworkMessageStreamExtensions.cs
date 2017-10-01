/***************************************************************
 * Notice:
 *       1) Do not remove copyright notice
 *       2) See License file (https://raw.githubusercontent.com/dx-prog/WildNetworkMessagingFramework/master/LICENSE) for more details 
 *       3) Copyright (c) 2017 David Garcia
 * ************************************************************/
using System;
using System.IO;
using System.Threading.Tasks;
using WNMF.Common.Culture;
using WNMF.Common.Definition;

namespace WNMF.Common.Foundation {
    public static class NetworkMessageStreamExtensions {
        /// <summary>
        ///     Name for a memory block not guaranteed to be zeroed at any given moment
        /// </summary>
        private const string BlockKeyName = "64K";

        public static readonly FlyWeight<byte[]> BlockBufferFlyWeight = new FlyWeight<byte[]>();

        public static void ChunkTo(this INetworkMessageStream input, Stream dst, string blockKey = BlockKeyName) {
            blockKey = blockKey ?? BlockKeyName;
            var block = BlockBufferFlyWeight.GetOrCreate(blockKey, k => new byte[1000 * 64]);
            Task worker = null;
            using (input.BeginReadScope()) {
                // trying to have overlapping read/writes
                while (input.ReadTo(block, 0, block.Length, out var fillAmount)) {
                    worker?.Wait();
                    var original = block;
                    if (worker?.Exception != null)
                        throw new Exception((string) LocalizationKeys.ForGeneralPurposes.SendFailed, worker.Exception);

                    worker = dst.WriteAsync(original, 0, fillAmount)
                        .ContinueWith(c => {
                            Array.Clear(original, 0, original.Length);
                            BlockBufferFlyWeight.Store(blockKey, original);
                        });
                    block = BlockBufferFlyWeight.GetOrCreate(blockKey, k => new byte[1000 * 64]);
                }
            }
        }
    }
}