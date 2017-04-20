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
    /// Represents a packet authentication handler.
    /// </summary>
    public sealed class AuthHandler
    {
        /// <summary>
        /// Gets the current handler's HMAC key.
        /// </summary>
        public byte[] HmacKey { get; }

        /// <summary>
        /// Initializes a new instance of the AuthHandler class using the default HMAC key.
        /// </summary>
        public AuthHandler()
        {
            HmacKey = new byte[] { 0xC0, 0xD3, 0xBD, 0xC3, 0xB7, 0xCE, 0xB8, 0xB8 };
        }

        /// <summary>
        /// Initializes a new instance of the AuthHandler class using the given HMAC key.
        /// </summary>
        /// <param name="hmacKey">The HMAC key which will be used by the auth handler.</param>
        public AuthHandler(byte[] hmacKey)
        {
            HmacKey = hmacKey;
        }

        /// <summary>
        /// Computes the HMAC for the specified packet data.
        /// </summary>
        /// <param name="authData"> The whole packet buffer, except for the size and the own HMAC.</param>
        /// <returns>The HMAC of the packet, which, in the case of Grand Chase, has the size of 10 bytes.</returns>
        public byte[] GetHmac(byte[] authData)
        {
            return MD5Hmac.ComputeHMAC(authData, HmacKey, 10);
        }

        /// <summary>
        /// Checks the validity of the stored HMAC in the packet buffer.
        /// </summary>
        /// <param name="packetData">The packet the way it was received.</param>
        /// <returns>A boolean that indicates if the stored HMAC is whether or not valid.</returns>
        public bool VerifyHmac(byte[] packetData)
        {
            byte[] storedHmac = Sequence.ReadBlock(packetData, packetData.Length - 10, 10);

            byte[] authData = Sequence.ReadBlock(packetData, 2, packetData.Length - 10 - 2);
            byte[] expectedHmac = MD5Hmac.ComputeHMAC(authData, HmacKey, 10);

            return storedHmac.SequenceEqual(expectedHmac);
        }
    }
}
