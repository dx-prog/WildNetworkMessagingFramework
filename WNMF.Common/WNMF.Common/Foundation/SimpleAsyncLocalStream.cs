/***************************************************************
 * Notice:
 *       1) Do not remove copyright notice
 *       2) See License file (https://raw.githubusercontent.com/dx-prog/WildNetworkMessagingFramework/master/LICENSE) for more details 
 *       3) Copyright (c) 2017 David Garcia
 * ************************************************************/

using System;
using System.IO;
using System.Threading;
using WNMF.Common.Definition;

namespace WNMF.Common.Foundation {
    /// <summary>
    ///     This class will isolate ReadTo operations per async context
    /// </summary>
    public class SimpleAsyncLocalStream : INetworkMessageStream {
        private readonly Func<NetworkMessageDescription, Stream> _open;
        private readonly AsyncLocal<Stream> _stream = new AsyncLocal<Stream>();

        public SimpleAsyncLocalStream(Func<NetworkMessageDescription, Stream> open,
            NetworkMessageDescription description) {
            _open = open;
            Description = description;
        }

        public NetworkMessageDescription Description { get; }


        public IDisposable BeginReadScope() {
            _stream.Value = _open(Description);
            return _stream.Value;
        }

        public bool ReadTo(byte[] targetBuffer, int offset, int length, out int fillLength) {
            return (fillLength = _stream.Value.Read(targetBuffer, offset, length)) > 0;
        }
    }
}