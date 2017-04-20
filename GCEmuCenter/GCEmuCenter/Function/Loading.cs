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
using GCNet.PacketLib;
using GCEmuCenter.Session;

namespace GCEmuCenter.Function
{
    public class Loading
    {
        string[] InitLoading = new string[4];
        string[] MatchLoading = new string[3];
        string[] SquareLoading = new string[3];
        string[] HackList = new string[0];

        public string[] SHAFileList = new string[0];
        public string GuildMarkURL;

        public Loading()
        {
            InitLoading[0] = "Load1_1.dds";
            InitLoading[1] = "Load1_2.dds";
            InitLoading[2] = "Load1_3.dds";
            InitLoading[3] = "LoadGauge.dds";

            MatchLoading[0] = "ui_match_load1.dds";
            MatchLoading[1] = "ui_match_load2.dds";
            MatchLoading[2] = "ui_match_load3.dds";

            SquareLoading[0] = "Square.lua";
            SquareLoading[1] = "SquareObject.lua";
            SquareLoading[2] = "Square3DObject.lua";

            GuildMarkURL = "http://127.0.0.1/";

            AddHack("GCMaster.dll");
            AddHack("GCMasterUSA.dll");
            AddHack("GCTrainerDll.dll");
            AddHack("GrandChaseL.dll");
            AddHack("MachineCore2.dll");
            AddHack("PeneLoco.dll");
            AddHack("Pichula.dll");
            AddHack("Pichulon.dll");
            AddHack("main2.dll");
            AddHack("mamawevo.dll");
            AddHack("perro2.dll");

            AddCheckFile("ai.kom");
            AddCheckFile("main.exe");
            AddCheckFile("script.kom");
        }

        public void AddHack(string hack)
        {
            Array.Resize<string>(ref HackList, HackList.Length + 1);
            HackList[HackList.Length - 1] = hack;
        }

        public void AddCheckFile(string filename)
        {
            Array.Resize<string>(ref SHAFileList, SHAFileList.Length + 1);
            SHAFileList[SHAFileList.Length - 1] = filename;
        }

        public void NotifyContentInfo(ClientSession cs)
        {
            try
            {
                PayloadWriter pWriter = new PayloadWriter();

                byte[] plContent0 = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x05,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00 };
                pWriter.WriteData(plContent0);

                byte[] plContent1 = { 0x02, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00,
                    0x04, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00 };
                pWriter.WriteData(plContent1);

                pWriter.WriteData(InitLoading.Length);

                pWriter.WriteData(InitLoading[0].Length * 2);
                pWriter.WriteUnicodeString(InitLoading[0]);

                pWriter.WriteData(InitLoading[1].Length * 2);
                pWriter.WriteUnicodeString(InitLoading[1]);

                pWriter.WriteData(InitLoading[2].Length * 2);
                pWriter.WriteUnicodeString(InitLoading[2]);

                pWriter.WriteData(InitLoading[3].Length * 2);
                pWriter.WriteUnicodeString(InitLoading[3]);

                byte[] plContent2 = { 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00 };
                pWriter.WriteData(plContent2);

                pWriter.WriteData(MatchLoading.Length);

                pWriter.WriteData(MatchLoading[0].Length * 2);
                pWriter.WriteUnicodeString(MatchLoading[0]);

                pWriter.WriteData(MatchLoading[1].Length * 2);
                pWriter.WriteUnicodeString(MatchLoading[1]);

                pWriter.WriteData(MatchLoading[2].Length * 2);
                pWriter.WriteUnicodeString(MatchLoading[2]);
                pWriter.WriteData(0);

                pWriter.WriteData(SquareLoading.Length);
                pWriter.WriteData(0);

                pWriter.WriteData(SquareLoading[0].Length * 2);
                pWriter.WriteUnicodeString(SquareLoading[0]);

                pWriter.WriteData(1);
                pWriter.WriteData(SquareLoading[1].Length * 2);
                pWriter.WriteUnicodeString(SquareLoading[1]);

                pWriter.WriteData(2);
                pWriter.WriteData(SquareLoading[2].Length * 2);
                pWriter.WriteUnicodeString(SquareLoading[2]);

                byte[] plContent3 = { 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x01, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00 };
                pWriter.WriteData(plContent3);

                // HackList
                pWriter.WriteData(HackList.Length);
                for (int i = 0; i <= HackList.Length - 1; i++)
                {
                    pWriter.WriteData(HackList[i].Length * 2);
                    pWriter.WriteUnicodeString(HackList[i]);
                }

                OutPacket oPacket = new OutPacket(pWriter.GetCompressedPayload(CenterOpcodes.ENU_CLIENT_CONTENTS_FIRST_INIT_INFO_ACK),
                    cs._CryptoHandler, cs._AuthHandler, cs.Prefix, ++cs.Count);

                cs.Send(oPacket);
            }
            catch
            {

            }
        }

        public void NotifySHAFile(ClientSession cs)
        {
            try
            {
                PayloadWriter pWriter = new PayloadWriter();
                pWriter.WriteData(0);
                pWriter.WriteData(SHAFileList.Length);

                for (int i = 0; i <= SHAFileList.Length - 1; i++)
                {
                    pWriter.WriteData(SHAFileList[i].Length * 2);
                    pWriter.WriteData(SHAFileList[i]);
                }

                OutPacket oPacket = new OutPacket(pWriter.GetPayload(CenterOpcodes.ENU_SHAFILENAME_LIST_ACK), cs._CryptoHandler,
                    cs._AuthHandler, cs.Prefix, ++cs.Count);

                cs.Send(oPacket);
            }
            catch
            {

            }
        }
    }
}
