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

using System.Text;
using GCNet.CoreLib;
using GCNet.Util;
using GCNet.Util.Endianness;

namespace GCNet.PacketLib
{
    /// <summary>
    /// Represents a packet reader for payload data.
    /// </summary>
    public sealed class PayloadReader
    {
        /// <summary>
        /// Gets the current payload's data.
        /// </summary>
        byte[] Data { get; }

        /// <summary>
        /// Gets the ID of the current payload's packet.
        /// </summary>
        public short Id
        {
            get { return BigEndian.GetInt16(Data, 0); }
        }

        /// <summary>
        /// Gets the size of the current payload's content.
        /// </summary>
        public int Size
        {
            get { return BigEndian.GetInt32(Data, 2); }
        }

        /// <summary>
        /// Gets the value of the current payload's compression flag;
        /// </summary>
        public bool CompressionFlag
        {
            get { return (Data[6] == 1); }
        }

        /// <summary>
        /// Gets or sets the current reading position.
        /// </summary>
        private int Position { get; set; } = 7;

        /// <summary>
        /// Initializes a new instance of PayloadReader using the specified payload data.
        /// If the payload is compressed, it is automatically decompressed.
        /// </summary>
        /// <param name="payload">The payload data to be read.</param>
        /// <remarks>
        /// The null bytes padding is ignored by the reader.
        /// </remarks>
        public PayloadReader(byte[] payload)
        {
            Data = payload;

            if (CompressionFlag)
            {
                byte[] firstPart = Sequence.ReadBlock(Data, 0, 11);
                byte[] decompressedContent = ZLib.DecompressData(Sequence.ReadBlock(Data, 11, Size - 4));
                byte[] nullBytesPadding = new byte[3];

                Data = Sequence.Concat(firstPart, decompressedContent, nullBytesPadding);
            }
        }

        /// <summary>
        /// Skips a specified amount of bytes.
        /// </summary>
        /// <param name="count">The number of bytes to be skipped.</param>
        public void Skip (int count)
        {
            Position += count;
        }

        /// <summary>
        /// Reads a byte from the payload's content and advances the current position by 1 byte.
        /// </summary>
        /// <returns>The next byte.</returns>
        public byte ReadByte()
        {
            return Data[Position++];
        }

        /// <summary>
        /// Reads a boolean from the payload's content and advances the current position by 1 byte.
        /// </summary>
        /// <returns>The next boolean.</returns>
        public bool ReadBool()
        {
            return (ReadByte() == 1);
        }

        /// <summary>
        /// Reads a 16-bit integer from the payload's content and advances the current position by 2 bytes.
        /// </summary>
        /// <returns>The next 16-bit integer.</returns>
        public short ReadInt16()
        {
            return BigEndian.GetInt16(Data, Position += sizeof(short));
        }

        /// <summary>
        /// Reads a 32-bit integer from the payload's content and advances the current position by 4 bytes.
        /// </summary>
        /// <returns>The next 32-bit integer.</returns>
        public int ReadInt32()
        {
            return BigEndian.GetInt32(Data, Position += sizeof(int));
        }

        /// <summary>
        /// Reads a 64-bit integer from the payload's content and advances the current position by 8 bytes.
        /// </summary>
        /// <returns>The next 64-bit integer.</returns>
        public long ReadInt64()
        {
            return BigEndian.GetInt64(Data, Position += sizeof(long));
        }

        /// <summary>
        /// Reads a string from the payload's content and advances the current position by the string length.
        /// </summary>
        /// <param name="length">The string length.</param>
        /// <returns>The next string.</returns>
        public string ReadString(int length)
        {
            return Encoding.ASCII.GetString(ReadBytes(length));
        }

        /// <summary>
        /// Reads an unicode string from the payload's content and advances the current position by the string length.
        /// </summary>
        /// <param name="length">The unicode string length.</param>
        /// <returns>The next unicode string.</returns>
        public string ReadUnicodeString(int length)
        {
            return Encoding.Unicode.GetString(ReadBytes(length));
        }

        /// <summary>
        /// Reads the specified number of bytes from 'Data' into a byte array and advances the current position by
        /// that number of bytes.
        /// </summary>
        /// <param name="count">The number of bytes to be read.</param>
        /// <returns>The read bytes.</returns>
        public byte[] ReadBytes(int count)
        {
            byte[] data = Sequence.ReadBlock(Data, Position, count);
            Position += count;
            return data;
        }
    }
}
