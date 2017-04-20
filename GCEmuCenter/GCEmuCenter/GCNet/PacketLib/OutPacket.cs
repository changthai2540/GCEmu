//-----------------------------------------------------------------------
// GCNet - A Grand Chase Networking Library
// Copyright © 2016  SyntaxDev
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//-----------------------------------------------------------------------

using System;
using GCNet.CoreLib;
using GCNet.Util;
using GCNet.Util.Endianness;

namespace GCNet.PacketLib
{
    /// <summary>
    /// Represents an outgoing packet.
    /// </summary>
    public class OutPacket : IDisposable
    {
        private bool disposed;

        /// <summary>
        /// Gets the packet data (ready to be sent) of the current outgoing packet.
        /// </summary>
        public byte[] PacketData { get; }

        /// <summary>
        /// Initializes a new instance of OutPacket from the given payload data, crypto handler, auth handler, prefix and count.
        /// </summary>
        /// <param name="payload">The ready payload data.</param>
        /// <param name="crypto">The crypto handler to be used.</param>
        /// <param name="auth">The auth handler to be used.</param>
        /// <param name="prefix">The packet's prefix.</param>
        /// <param name="count">The packet's count.</param>
        // 
        public OutPacket(byte[] payload, CryptoHandler crypto, AuthHandler auth, short prefix, int count)
        {
            byte[] packetPrefix = LittleEndian.GetBytes(prefix);
            byte[] packetCount = LittleEndian.GetBytes(count);
            byte[] iv = Generate.IV();
            byte[] encryptedData = crypto.EncryptPacket(payload, iv);

            byte[] size = LittleEndian.GetBytes(Convert.ToInt16(16 + encryptedData.Length + 10));

            byte[] authData = Sequence.Concat(packetPrefix, packetCount, iv, encryptedData);
            byte[] hmac = auth.GetHmac(authData);

            PacketData = Sequence.Concat(size, authData, hmac);
        }

        ~OutPacket()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                }
            }

            disposed = true;
        }
    }
}
