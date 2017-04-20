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

using System.Linq;
using GCNet.Util;

namespace GCNet.CoreLib
{
    /// <summary>
    /// Represents a packet encryption handler.
    /// </summary>
    public sealed class CryptoHandler
    {
        /// <summary>
        /// Gets the current handler's encryption key.
        /// </summary>
        public byte[] Key { get; }

        /// <summary>
        /// Initializes a new instance of the CryptoHandler class using the default encryption key.
        /// </summary>
        public CryptoHandler()
        {
            Key = new byte[] { 0xC7, 0xD8, 0xC4, 0xBF, 0xB5, 0xE9, 0xC0, 0xFD };
        }

        /// <summary>
        /// Initializes a new instance of the CryptoHandler class using the given encryption key.
        /// </summary>
        /// <param name="key">The encryption key which will be used by the crypto handler.</param>
        public CryptoHandler(byte[] key)
        {
            Key = key;
        }

        /// <summary>
        /// Encrypts the given packet payload data.
        /// </summary>
        /// <param name="payload">The payload data to be encrypted.</param>
        /// <param name="iv">The initialization vector (IV).</param>
        /// <returns>The encrypted payload data.</returns>
        public byte[] EncryptPacket(byte[] payload, byte[] iv)
        {
            byte[] paddedData = PadData(payload);
            return DESEncryption.EncryptData(paddedData, iv, Key);
        }

        /// <summary>
        /// Decrypts the specified packet buffer.
        /// </summary>
        /// <param name="packetData">The packet the way it was received.</param>
        /// <returns>The decrypted packet data.</returns>
        public byte[] DecryptPacket(byte[] packetData)
        {
            byte[] iv = Sequence.ReadBlock(packetData, 8, 8);
            byte[] encryptedData = Sequence.ReadBlock(packetData, 16, packetData.Length - 10 - 16);

            byte[] decryptedData = DESEncryption.DecryptData(encryptedData, iv, Key);
            int paddingLength = (decryptedData.Last() + 2);
            //System.Console.WriteLine(decryptedData.Length);
            //System.Console.WriteLine("padding data length: {0}", paddingLength);
            return Sequence.ReadBlock(decryptedData, 0, decryptedData.Length - paddingLength);
        }

        /// <summary>
        /// Pads the specified data accordingly to the encryption padding of Grand Chase packets.
        /// </summary>
        /// <param name="data">The data to be padded.</param>
        /// <returns>The padded data.</returns>
        private byte[] PadData(byte[] data)
        {
            int distance = 8 - (data.Length % 8); // It's the distance from the length value to the next number divisible by the block size (8).
            int paddingLength = (distance >= 3) ? (distance) : (8 + distance);

            byte[] padding = new byte[paddingLength];

            for (byte i = 0; i < (paddingLength - 1); i++)
            {
                padding[i] = i;
            }
            padding[paddingLength - 1] = padding[paddingLength - 2]; // Equals the last to the penultimate byte.

            return Sequence.Concat(data, padding);
        }
    }
}
