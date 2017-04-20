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
using GCNet.CoreLib;
using GCNet.PacketLib;
using GCEmuCenter.Function;
using GCEmuCenter.IO;

namespace GCEmuCenter.Session
{
    public class ClientSession : Session
    {
        public byte[] CryptoKey { get; private set; }
        public byte[] AuthKey { get; private set; }
        public short Prefix { get; set; } = 0;
        public int Count;

        public CryptoHandler _CryptoHandler { get; private set; }
        public AuthHandler _AuthHandler { get; private set; }

        public CryptoHandler tempCryptoHandler = new CryptoHandler();
        AuthHandler tempAuthHandler = new AuthHandler();

        public Loading MyLoading = new Loading();
        public User MyUser = new User();

        public int LastHeartBeat { get; set; }
        public uint IP { get; set; }
        public ushort Port { get; set; }

        public ClientSession(Socket socket) : base(socket)
        {
            Log.Inform("Cliente conectado.");

            IP = BitConverter.ToUInt32(IPAddress.Parse(GetIP()).GetAddressBytes(), 0);

            InitiateReceive(2, true);

            // Generate new security keys
            CryptoKey = Generate.Key();
            _CryptoHandler = new CryptoHandler(CryptoKey);

            AuthKey = Generate.Key();
            _AuthHandler = new AuthHandler(AuthKey);

            short TempPrefix = Generate.Prefix();

            try
            {
                PayloadWriter pWriter = new PayloadWriter();

                pWriter.WriteData(TempPrefix);
                pWriter.WriteData((int)8);
                pWriter.WriteData(_AuthHandler.HmacKey);
                pWriter.WriteData((int)8);
                pWriter.WriteData(_CryptoHandler.Key);

                byte[] plContent = { 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x0, 0x00, 0x00, 0x00, 0x00, 0x00 };
                pWriter.WriteData(plContent);

                OutPacket oPacket = new OutPacket(pWriter.GetPayload(CenterOpcodes.SET_SECURITY_KEY_NOT), tempCryptoHandler,
                    tempAuthHandler, Prefix, ++Count);

                Send(oPacket);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Close();
            }

            Prefix = TempPrefix;

            try
            {
                PayloadWriter pWritter = new PayloadWriter();

                byte[] plContent = { 0x00, 0x00, 0x27, 0x10 };
                pWritter.WriteData(plContent);

                OutPacket oPacket = new OutPacket(pWritter.GetPayload(CenterOpcodes.ENU_WAIT_TIME_NOT), _CryptoHandler,
                    _AuthHandler, Prefix, ++Count);

                Send(oPacket);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Close();
            }
        }

        public string GetIP()
        {
            if (socket == null)
                return "0.0.0.0";

            IPEndPoint remoteIpEndPoint = socket.RemoteEndPoint as IPEndPoint;

            return remoteIpEndPoint.Address.ToString();
        }

        public override void OnPacket(byte[] packetData)
        {
            try
            {
                InPacket iPacket = new InPacket(packetData, _CryptoHandler);
                PayloadReader pReader = new PayloadReader(iPacket.Payload);

                CenterOpcodes opcode = (CenterOpcodes)pReader.Id;

                bool isCompressed = pReader.CompressionFlag;

                if (isCompressed)
                    Log.Packet("Pacote comprimido recebido: 0x{0:X2} ({1})", (int)opcode, opcode.ToString());
                else
                    Log.Packet("Opcode: 0x{0:X2} ({1})", (int)opcode, opcode.ToString());

                switch (opcode)
                {
                    case CenterOpcodes.HEART_BIT_NOT:
                        OnHeartBeatNot();
                        break;
                    case CenterOpcodes.ENU_VERIFY_ACCOUNT_REQ:
                        MyUser.OnLogin(this, iPacket);
                        break;
                    case CenterOpcodes.ENU_GUIDE_BOOK_LIST_REQ:
                        MyUser.OnGuideBookList(this);
                        break;
                    case CenterOpcodes.ENU_CLIENT_CONTENTS_FIRST_INIT_INFO_REQ:
                        MyLoading.NotifyContentInfo(this);
                        break;
                    case CenterOpcodes.ENU_CLIENT_PING_CONFIG_REQ:
                        OnClientPingConfig();
                        break;
                    case CenterOpcodes.ENU_SHAFILENAME_LIST_REQ:
                        MyLoading.NotifySHAFile(this);
                        break;
                    default:
                        Log.Warn("Pacote indefinido recebido. Opcode: {0:X} ({1})", (int)opcode, opcode.ToString());
                        Log.Hex("Payload:", iPacket.Payload);
                        break;

                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                Close();
            }
        }

        public override void OnDisconnect()
        {
            //throw new NotImplementedException();
        }

        public void OnHeartBeatNot()
        {
            LastHeartBeat = Environment.TickCount;
        }

        public void OnClientPingConfig()
        {
            try
            {
                PayloadWriter pWriter = new PayloadWriter();

                byte[] plContent = { 0x00, 0x00, 0x0F, 0xA0, 0x00, 0x00, 0x0F, 0xA0, 0x00, 0x00, 0x0F, 0xA0,
                    0x00, 0x00, 0x00, 0x01, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00 };
                pWriter.WriteData(plContent);

                OutPacket oPacket = new OutPacket(pWriter.GetPayload(CenterOpcodes.ENU_CLIENT_PING_CONFIG_ACK), _CryptoHandler,
                    _AuthHandler, Prefix, ++Count);

                Send(oPacket);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Close();
            }
        }
    }
}
