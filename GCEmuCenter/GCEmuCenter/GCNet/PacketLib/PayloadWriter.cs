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
using System.Text;
using GCNet.CoreLib;
using GCNet.Util;
using GCNet.Util.Endianness;
using GCEmuCenter;

namespace GCNet.PacketLib
{
    /// <summary>
    /// Represents a packet writer for payload data.
    /// </summary>
    class PayloadWriter : IDisposable
    {
        private bool disposed; 
        /// <summary>
        /// Gets or sets the current content's data.
        /// </summary>
        public byte[] Data { get; set; } = new byte[0];

        /// <summary>
        /// Builds a payload from the current content's data.
        /// </summary>
        /// <param name="id">The packet's id.</param>
        /// <returns>A new payload.</returns>
        public byte[] GetPayload(CenterOpcodes oId)
        {
            short id = (short)oId;
            byte[] packetId = BigEndian.GetBytes(id);
            byte[] size = BigEndian.GetBytes(Data.Length);
            byte[] compressionFlag = { 0 }; // false
            byte[] padding = { 0, 0, 0 };

            return Sequence.Concat(packetId, size, compressionFlag, Data, padding);
        }

        /// <summary>
        /// Builds a compressed payload from the current content's data.
        /// </summary>
        /// <param name="id">The packet's id.</param>
        /// <returns>A new compressed payload.</returns>
        public byte[] GetCompressedPayload(CenterOpcodes oId)
        {
            short id = (short)oId;
            byte[] compressedData = ZLib.CompressData(Data);

            byte[] packetId = BigEndian.GetBytes(id);
            byte[] size = BigEndian.GetBytes(compressedData.Length + 4);
            byte[] compressionFlag = { 1 }; // true
            byte[] decompressedSize = LittleEndian.GetBytes(Data.Length);
            byte[] padding = { 0, 0, 0 };

            return Sequence.Concat(packetId, size, compressionFlag, decompressedSize, compressedData, padding);
        }

        /// <summary>
        /// Writes the specified byte to the current payload's content.
        /// </summary>
        /// <param name="value">The byte to be written.</param>
        public void WriteData(byte value)
        {
            WriteData(new byte[] { value });
        }

        /// <summary>
        /// Writes the specified boolean to the current payload's content.
        /// </summary>
        /// <param name="boolean">The boolean to be written.</param>
        public void WriteData(bool boolean)
        {
            WriteData(new byte[] { Convert.ToByte(boolean) });
        }

        /// <summary>
        /// Writes the specified 16-bit integer to the current payload's content.
        /// </summary>
        /// <param name="int16">The 16-bit integer to be written.</param>
        public void WriteData(short int16)
        {
            WriteData(BigEndian.GetBytes(int16));
        }

        /// <summary>
        /// Writes the specified 32-bit integer to the current payload's content.
        /// </summary>
        /// <param name="int32">The 32-bit integer to be written.</param>
        public void WriteData(int int32)
        {
            WriteData(BigEndian.GetBytes(int32));
        }

        /// <summary>
        /// Writes the specified 64-bit integer to the current payload's content.
        /// </summary>
        /// <param name="int64">The 64-bit integer to be written.</param>
        public void WriteData(long int64)
        {
            WriteData(BigEndian.GetBytes(int64));
        }

        /// <summary>
        /// Writes the specified string to the current payload's content.
        /// </summary>
        /// <param name="str">The string to be written.</param>
        public void WriteData(string str)
        {
            WriteData(Encoding.ASCII.GetBytes(str));
        }

        /// <summary>
        /// Writes the specified unicode string to the current payload's content.
        /// </summary>
        /// <param name="ustr">The unicode string to be written.</param>
        public void WriteUnicodeString(string ustr)
        {
            WriteData(Encoding.Unicode.GetBytes(ustr));
        }

        /// <summary>
        /// Writes the specified bytes to the current content's data.
        /// </summary>
        /// <param name="bytes">The bytes to be written.</param>
        public void WriteData(byte[] bytes)
        {
            Data = Sequence.Concat(Data, bytes);
        }

        ~PayloadWriter()
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
