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

using GCNet.CoreLib;
using GCNet.Util;
using GCNet.Util.Endianness;

namespace GCNet.PacketLib
{
    /// <summary>
    /// Represents an incoming packet. 
    /// </summary>

    public sealed class InPacket
    {
        /// <summary>
        /// Gets the current packet's data.
        /// </summary>
        private byte[] PacketData { get; }

        /// <summary>
        /// Gets the current packet's crypto handler.
        /// </summary>
        private CryptoHandler CryptoHandler { get; }

        /// <summary>
        /// Gets the size of the current packet.
        /// </summary>
        public short size
        {
            get { return LittleEndian.GetInt16(PacketData, 0); }
        }

        /// <summary>
        /// Gets the prefix of the current packet.
        /// </summary>
        public short Prefix
        {
            get { return LittleEndian.GetInt16(PacketData, 2); }
        }

        /// <summary>
        /// Gets the count in the current packet's header.
        /// </summary>
        public int Count
        {
            get { return LittleEndian.GetInt32(PacketData, 4); }
        }

        /// <summary>
        /// Gets the IV of the current packet.
        /// </summary>
        public byte[] IV
        {
            get { return Sequence.ReadBlock(PacketData, 8, 8); }
        }

        /// <summary>
        /// Gets the current packet's decrypted payload.
        /// </summary>
        public byte[] Payload
        {
            get { return CryptoHandler.DecryptPacket(PacketData); }
        }

        /// <summary>
        /// Initializes a new instance of InPacket from the given packet buffer and crypto handler.
        /// </summary>
        /// <param name="packetBuffer">The packet buffer the way it was received.</param>
        /// <param name="crypto">The current crypto handler.</param>
        public InPacket(byte[] packetBuffer, CryptoHandler crypto)
        {
            PacketData = packetBuffer;
            CryptoHandler = crypto;
        }
    }
}
