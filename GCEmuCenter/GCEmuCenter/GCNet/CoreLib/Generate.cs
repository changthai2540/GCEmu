//-----------------------------------------------------------------------
// GCNet - A Grand Chase Networking Library
// Copyright © 2016  SyntaxDev
// Copyright © 2017  Roverde
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
using System.Security.Cryptography;

namespace GCNet.CoreLib
{
    /// <summary>
    /// Generates a key or IV.
    /// </summary>
    public static class Generate
    {
        /// <summary>
        /// Generates a new 8-byte key which can be used in both encryption and authentication.
        /// </summary>
        /// <returns>The new generated key.</returns>
        public static byte[] Key()
        {
            byte[] outputKey = new byte[8];

            using (var rngProvider = new RNGCryptoServiceProvider())
            {
                rngProvider.GetBytes(outputKey);
            }
            return outputKey;
        }

        /// <summary>
        /// Generates a new initialization vector (IV).
        /// </summary>
        /// <returns>The new generated IV.</returns>
        public static byte[] IV()
        {
            byte[] outputIV = new byte[8];
            byte ivByte;

            Random random = new Random();
            ivByte = (byte)random.Next(0x00, 0xFF);

            for (int i = 00; i < outputIV.Length; i++)
            {
                outputIV[i] = ivByte;
            }
            return outputIV;
        }

        /// <summary>
        /// Generates a new short prefix.
        /// </summary>
        /// <returns>The new generated prefix.</returns>
        public static short Prefix()
        {
            short outputPrefix;

            Random random = new Random();
            outputPrefix = (short)random.Next(0x00, 0xFF);

            return outputPrefix;
        }
    }
}
