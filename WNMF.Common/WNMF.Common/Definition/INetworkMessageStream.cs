/***************************************************************
 * Notice:
 *       1) Do not remove copyright notice
 *       2) See License file (https://raw.githubusercontent.com/dx-prog/WildNetworkMessagingFramework/master/LICENSE) for more details 
 *       3) Copyright (c) 2017 David Garcia
 * ************************************************************/
using System;

namespace WNMF.Common.Definition {
    public interface INetworkMessageStream {


        NetworkMessageDescription Description { get; }

        /// <summary>
        /// Will be used during the read scope
        /// </summary>
        /// <returns></returns>
        IDisposable BeginReadScope();

        /// <summary>
        ///     Reads data into the specific buffer
        /// </summary>
        /// <param name="targetBuffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="fillLength">the amount of data actually copied to targetBuffer</param>
        /// <returns></returns>
        bool ReadTo(byte[] targetBuffer, int offset, int length, out int fillLength);
    }
}