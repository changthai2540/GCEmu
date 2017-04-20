//-----------------------------------------------------------------------
// GCEmu - A Grand Chase Season 4 Eternal Emulator
// Copyright © 2017 Roverde
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
using System.Net;
using System.Net.Sockets;
using GCNet.PacketLib;
using GCNet.Util;
using GCNet.Util.Endianness;

namespace GCEmuCenter.Session
{
    public abstract class Session
    {
        protected Socket socket;

        private byte[] buffer;
        private int bufferIndex;
        private uint pHeader;

        private bool header;

        private uint riv;
        private uint siv;

        private object _lock;

        public string Label
        {
            get; private set;
        }

        private bool IsConnected
        {
            get; set;
        }

        public IPEndPoint RemoteEndPoint
        {
            get
            {
                try
                {
                    return (IPEndPoint)this.socket.RemoteEndPoint;
                }
                catch
                {
                    return new IPEndPoint(0, 0);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the Session class.
        /// </summary>
        /// <param name="sSocket"></param>
        public Session(Socket sSocket)
        {
            this.socket = sSocket;
            this.socket.NoDelay = true;

            try
            {
                this.Label = this.socket.RemoteEndPoint.ToString();
            }
            catch (SocketException)
            {
                this.Label = "Error";
            }

            this.IsConnected = true;
        }

        /// <summary>
        /// Initiates the receiving mechanism.
        /// </summary>
        /// <param name="rLength">The length of the data.</param>
        /// <param name="rHeader">Indicates if a header is received.</param>
        protected void InitiateReceive(uint rLength, bool rHeader = false)
        {
            if (!this.IsConnected)
                return;

            this.header = rHeader;
            this.buffer = new byte[rLength];
            this.bufferIndex = 0;

            this.BeginReceive();
        }

        /// <summary>
        /// Begins to asynchronously receive data from the socket.
        /// </summary>
        private void BeginReceive()
        {
            if (!this.IsConnected)
                return;

            var error = SocketError.Success;

            this.socket.BeginReceive(this.buffer, this.bufferIndex, this.buffer.Length - this.bufferIndex,
                SocketFlags.None, out error, EndReceive, null);

            if (error != SocketError.Success)
                this.Close();
        }

        /// <summary>
        /// Reads the data from the callback and handles it.
        /// </summary>
        /// <param name="iar"></param>
        // FIX: Remove the need to concat header and buffer
        private void EndReceive(IAsyncResult iar)
        {
            if (!this.IsConnected)
                return;

            var error = SocketError.Success;
            int received = this.socket.EndReceive(iar, out error);

            if (received == 0 || error != SocketError.Success)
            {
                this.Close();
                return;
            }

            this.bufferIndex += received;

            if (this.bufferIndex == this.buffer.Length)
            {
                if (this.header)
                 {
                     pHeader = BitConverter.ToUInt16(this.buffer, 0);
                     this.InitiateReceive(pHeader - 2, false);
                 }
                 else
                 {
                    byte[] nBuffer = new byte[buffer.Length];
                    nBuffer = Sequence.Concat<byte>(LittleEndian.GetBytes((short)pHeader), buffer);
                    this.OnPacket(nBuffer);
                    this.InitiateReceive(2, true);
               }
            }
            else
                this.BeginReceive();
        }

        /// <summary>
        /// Sends a GrandChase.IO.OutPacket array to the socket.
        /// </summary>
        /// <param name="outPacket"></param>
        public void Send(OutPacket outPacket)
        {
            this.Send(outPacket.PacketData);
        }

        /// <summary>
        /// Sends data to the socket.
        /// </summary>
        /// <param name="buffer"></param>
        public void Send(byte[] buffer)
        {
            //lock (_lock)
            //{
                if (!this.IsConnected)
                    return;

                this.SendRaw(buffer);
            //}
        }

        public void SendRaw(byte[] data)
        {
            //lock (_lock)
            //{
                if (!this.IsConnected)
                    return;

                int offset = 0;

                while (offset < data.Length)
                {
                    SocketError error = SocketError.Success;

                    int sent = this.socket.Send(data, offset, data.Length - offset, SocketFlags.None, out error);

                    if (sent == 0 || error != SocketError.Success)
                        throw new PacketSendException();

                    offset += sent;
                }
            //}
        }
        

        /// <summary>
        /// Closes the socket.
        /// </summary>
        public void Close()
        {
            //lock (_lock)
           // {
                if (!this.IsConnected)
                    return;

                this.IsConnected = false;
                this.socket.Shutdown(SocketShutdown.Both);
                this.socket.Close();

                this.OnDisconnect();
           // }
        }

        public abstract void OnDisconnect();
        public abstract void OnPacket(byte[] packetData);
    }
}
