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
using System.Data;
using MySql.Data.MySqlClient;
using GCNet.PacketLib;
using GCEmuCenter.Session;
using GCEmuCenter.DB;
using GCEmuCenter.IO;
using GCEmuCenter.Misc;

namespace GCEmuCenter.Function
{
    public class User
    {
        private string UID { get; set; }
        private string UPW { get; set; }

        public void SendServerList(ClientSession cs)
        {
            try
            {
                Database db = new Database();
                db.OpenDB();

                MySqlDataReader sReader = db.Query("SELECT id, name, ip, port, description FROM servers");
                Log.SqlQuerys(db.dbcmd.CommandText);
                DataTable dt = new DataTable();

                dt.Load(sReader);
                sReader.Close();
                db.dbconn.Close();

                int nServers = dt.Rows.Count;

                PayloadWriter pWriter = new PayloadWriter();

                byte[] plContent = { 0x00, 0x00, 0x01, 0x43, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
                byte[] plContent1 = { 0x00, 0x00, 0x00, 0x00 };

                pWriter.WriteData(nServers); // Number of servers

                int serverNumber;
                string serverName;
                string serverIP;
                short serverPort;
                string serverDesc;

                foreach (DataRow row in dt.Rows)
                {
                    serverNumber = (int)row[0];
                    serverName = (string)row[1];
                    serverIP = (string)row[2];
                    serverPort = (short)row[3];
                    serverDesc = (string)row[4];

                    // Server info
                    pWriter.WriteData(serverNumber); // Server number
                    pWriter.WriteData(serverNumber); // Server number?
                    pWriter.WriteData(serverName.Length * 2);
                    pWriter.WriteUnicodeString(serverName);
                    pWriter.WriteData(serverIP.Length);
                    pWriter.WriteData(serverIP);
                    pWriter.WriteData(serverPort);
                    pWriter.WriteData(0); // Atualmente curtiu?
                    pWriter.WriteData(500); // Likes máximos?
                    pWriter.WriteData(plContent);
                    pWriter.WriteData(serverIP.Length); // Send IP again
                    pWriter.WriteData(serverIP);
                    pWriter.WriteData(serverDesc.Length * 2);
                    pWriter.WriteUnicodeString(serverDesc);
                    pWriter.WriteData(plContent1);
                }

                OutPacket oPacket = new OutPacket(pWriter.GetPayload(CenterOpcodes.ENU_SERVER_LIST_NOT),
                    cs._CryptoHandler, cs._AuthHandler, cs.Prefix, ++cs.Count);

                cs.Send(oPacket);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public void SendChannelNews(ClientSession cs)
        {
            try
            {
                PayloadWriter pWritter = new PayloadWriter();

                byte[] plContent = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01 };
                pWritter.WriteData(plContent);

                OutPacket oPacket = new OutPacket(pWritter.GetPayload(CenterOpcodes.ENU_CHANNEL_NEWS_NOT),
                    cs._CryptoHandler, cs._AuthHandler, cs.Prefix, ++cs.Count);

                cs.Send(oPacket);
            }
            catch (Exception ex)
            {
                Log.Error("ChannelNews: {0}", ex.Message);
                while (true) ;
            }
        }

        public void SendClientContentOpen(ClientSession cs)
        {
            try
            {
                PayloadWriter pWritter = new PayloadWriter();

                byte[] plContent = Util.StringToByteArray("00 00 00 00 00 00 00 03 00 00 00 00 00 00 00 01 00 00 00 02 00 00 00 09 00 00 00 00 00 00 00 01 00 00 00 02 00 00 00 09 00 00 00 01 00 00 00 01 00 00 00 0A 00 00 00 01 00 00 00 01 00 00 00 0E 00 00 00 01 00 00 00 01 00 00 00 12 00 00 00 01 00 00 00 01 00 00 00 14 00 00 00 01 00 00 00 01 00 00 00 02 00 00 00 01 00 00 00 03 00 00 00 03 00 00 00 01 00 00 00 00 00 00 00 15 00 00 00 01 00 00 00 0D 00 00 00 08 00 00 00 00 00 00 00 02 00 00 00 01 00 00 00 02 00 00 00 01 00 00 00 01 00 00 00 05 00 00 00 03 00 00 00 01 00 00 00 06 00 00 00 04 00 00 00 01 00 00 00 00 00 00 00 05 00 00 00 02 00 00 00 03 00 00 00 04 00 00 00 02 00 00 00 42 00 00 00 07 00 00 00 08 00 00 00 09 00 00 00 0A 00 00 00 0B 00 00 00 0C 00 00 00 0D 00 00 00 0E 00 00 00 0F 00 00 00 10 00 00 00 11 00 00 00 12 00 00 00 13 00 00 00 14 00 00 00 15 00 00 00 16 00 00 00 17 00 00 00 18 00 00 00 19 00 00 00 1A 00 00 00 1B 00 00 00 1E 00 00 00 24 00 00 00 27 00 00 00 28 00 00 00 29 00 00 00 2A 00 00 00 2B 00 00 00 2C 00 00 00 2D 00 00 00 2E 00 00 00 2F 00 00 00 30 00 00 00 31 00 00 00 32 00 00 00 37 00 00 00 38 00 00 00 39 00 00 00 3A 00 00 00 3B 00 00 00 3C 00 00 00 3D 00 00 00 3E 00 00 00 3F 00 00 00 40 00 00 00 46 00 00 00 49 00 00 00 4A 00 00 00 4C 00 00 00 4E 00 00 00 4F 00 00 00 50 00 00 00 51 00 00 00 53 00 00 00 54 00 00 00 55 00 00 00 56 00 00 00 57 00 00 00 58 00 00 00 59 00 00 00 5A 00 00 00 5B 00 00 00 5C 00 00 00 5D 00 00 00 5E 00 00 00 5F 00 00 00 0B 00 00 00 01 00 00 00 26 00 00 00 0D 00 00 00 01 00 00 00 42 00 00 00 06 00 00 00 04 00 00 00 00 01 00 00 00 63 00 00 00 00 01 00 00 00 10 00 00 00 63 00 00 00 64 00 00 00 00 00 00 00 01 00 00 00 02 00 00 00 03 00 00 00 04 00 00 00 05 00 00 00 06 00 00 00 07 00 00 00 08 00 00 00 09 00 00 00 0A 00 00 00 0B 00 00 00 2B 00 00 00 2F 00 00 00 01 01 00 00 00 0E 00 00 00 0A 00 00 00 63 00 00 00 64 00 00 00 00 00 00 00 01 00 00 00 02 00 00 00 03 00 00 00 09 00 00 00 04 00 00 00 06 00 00 00 05 00 00 00 08 00 00 00 07 00 00 00 0B 00 00 00 05 01 00 00 00 10 00 00 00 63 00 00 00 64 00 00 00 00 00 00 00 01 00 00 00 02 00 00 00 03 00 00 00 04 00 00 00 05 00 00 00 06 00 00 00 07 00 00 00 08 00 00 00 09 00 00 00 0A 00 00 00 0B 00 00 00 2B 00 00 00 2F 00 00 00 03 01 00 00 00 09 00 00 00 63 00 00 00 64 00 00 00 00 00 00 00 01 00 00 00 09 00 00 00 0B 00 00 00 02 00 00 00 0A 00 00 00 04 00 00 00 0B 00 00 00 00 10 00 00 00 63 00 00 00 64 00 00 00 00 00 00 00 01 00 00 00 02 00 00 00 03 00 00 00 04 00 00 00 05 00 00 00 06 00 00 00 07 00 00 00 08 00 00 00 09 00 00 00 0A 00 00 00 0B 00 00 00 2B 00 00 00 2F 00 00 00 06 00 00 00 00 00 00 00 01 00 00 00 02 00 00 00 03 00 00 00 04 00 00 00 05 00 00 00 55 00 00 00 13 00 00 00 00 00 00 00 04 00 01 02 03 00 00 00 01 00 00 00 04 00 01 02 03 00 00 00 02 00 00 00 04 00 01 02 03 00 00 00 03 00 00 00 04 00 01 02 03 00 00 00 04 00 00 00 04 00 01 02 03 00 00 00 05 00 00 00 04 00 01 02 03 00 00 00 06 00 00 00 04 00 01 02 03 00 00 00 07 00 00 00 04 00 01 02 03 00 00 00 08 00 00 00 04 00 01 02 03 00 00 00 09 00 00 00 04 00 01 02 03 00 00 00 0A 00 00 00 04 00 01 02 03 00 00 00 0B 00 00 00 04 00 01 02 03 00 00 00 0C 00 00 00 04 00 01 02 03 00 00 00 0D 00 00 00 04 00 01 02 03 00 00 00 0E 00 00 00 04 00 01 02 03 00 00 00 0F 00 00 00 02 00 01 00 00 00 10 00 00 00 02 00 01 00 00 00 11 00 00 00 02 00 01 00 00 00 12 00 00 00 01 00 00 00 00 13 00 00 00 00 00 00 00 01 00 00 00 02 00 00 00 03 00 00 00 04 00 00 00 05 00 00 00 06 00 00 00 07 00 00 00 08 00 00 00 09 00 00 00 0A 00 00 00 0B 00 00 00 0C 00 00 00 0D 00 00 00 0E 00 00 00 0F 00 00 00 10 00 00 00 11 00 00 00 12 00 00 00 13 00 00 00 00 00 00 00 02 00 00 00 01 00 00 00 02 00 00 00 02 00 00 00 02 00 00 00 03 00 00 00 01 00 00 00 04 00 00 00 01 00 00 00 05 00 00 00 01 00 00 00 06 00 00 00 01 00 00 00 07 00 00 00 01 00 00 00 08 00 00 00 01 00 00 00 09 00 00 00 01 00 00 00 0A 00 00 00 01 00 00 00 0B 00 00 00 01 00 00 00 0C 00 00 00 01 00 00 00 0D 00 00 00 01 00 00 00 0E 00 00 00 00 00 00 00 0F 00 00 00 01 00 00 00 10 00 00 00 01 00 00 00 11 00 00 00 00 00 00 00 12 00 00 00 00 00 00 00 12 00 00 00 00 00 01 83 3F 00 00 00 01 00 01 83 40 00 00 00 02 00 01 83 41 00 00 00 03 00 01 83 42 00 00 00 04 00 01 83 43 00 00 00 05 00 01 83 44 00 00 00 06 00 01 83 45 00 00 00 07 00 01 83 46 00 00 00 08 00 01 83 47 00 00 00 09 00 01 83 48 00 00 00 0A 00 01 83 49 00 00 00 0B 00 01 83 4A 00 00 00 0C 00 01 83 4B 00 00 00 0D 00 01 83 4C 00 00 00 0E 00 01 83 4D 00 00 00 0F 00 01 83 4E 00 00 00 10 00 01 83 4F 00 00 00 11 00 01 E3 32 00 00 00 43 00 00 00 00 00 00 00 01 00 00 00 02 00 00 00 03 00 00 00 04 00 00 00 05 00 00 00 06 00 00 00 07 00 00 00 08 00 00 00 09 00 00 00 0A 00 00 00 0B 00 00 00 0C 00 00 00 0D 00 00 00 0E 00 00 00 0F 00 00 00 10 00 00 00 11 00 00 00 12 00 00 00 13 00 00 00 14 00 00 00 15 00 00 00 16 00 00 00 17 00 00 00 18 00 00 00 19 00 00 00 1A 00 00 00 1B 00 00 00 1C 00 00 00 1D 00 00 00 1E 00 00 00 1F 00 00 00 20 00 00 00 21 00 00 00 22 00 00 00 23 00 00 00 24 00 00 00 26 00 00 00 27 00 00 00 28 00 00 00 29 00 00 00 2A 00 00 00 2B 00 00 00 2C 00 00 00 2D 00 00 00 2E 00 00 00 2F 00 00 00 30 00 00 00 31 00 00 00 32 00 00 00 33 00 00 00 34 00 00 00 39 00 00 00 3A 00 00 00 3B 00 00 00 3C 00 00 00 3D 00 00 00 3E 00 00 00 3F 00 00 00 40 00 00 00 41 00 00 00 42 00 00 00 43 00 00 00 44 00 00 00 45 00 00 00 46 00 00 00 47 00 00 00 02 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 02 00 00 00 13 00 00 00 18 00 00 00 03 00 00 22 56 00 00 00 8E 00 00 00 3C 74 00 65 00 78 00 5F 00 67 00 63 00 5F 00 6D 00 62 00 6F 00 78 00 5F 00 67 00 61 00 77 00 69 00 62 00 61 00 77 00 69 00 62 00 6F 00 5F 00 64 00 6C 00 67 00 2E 00 64 00 64 00 73 00 00 00 00 01 00 06 4E F6 00 00 25 40 00 00 00 B3 00 00 00 36 74 00 65 00 78 00 5F 00 67 00 63 00 5F 00 6D 00 62 00 6F 00 78 00 5F 00 66 00 72 00 69 00 65 00 6E 00 64 00 5F 00 67 00 69 00 66 00 74 00 2E 00 64 00 64 00 73 00 00 00 00 01 00 0C C5 D8 00 00 27 D2 00 00 00 BF 00 00 00 38 74 00 65 00 78 00 5F 00 67 00 63 00 5F 00 6D 00 62 00 6F 00 78 00 5F 00 73 00 6F 00 6E 00 67 00 6B 00 72 00 61 00 6E 00 5F 00 64 00 6C 00 67 00 2E 00 64 00 64 00 73 00 00 00 00 01 00 0D 5E 44");
                pWritter.WriteData(plContent);

                OutPacket oPacket = new OutPacket(pWritter.GetCompressedPayload(CenterOpcodes.ENU_NEW_CLIENT_CONTENTS_OPEN_NOT),
                    cs._CryptoHandler, cs._AuthHandler, cs.Prefix, ++cs.Count);

                cs.Send(oPacket);
            }
            catch (Exception ex)
            {
                Log.Error("SendClientContent: {0}", ex.Message);
                while (true) ;
            }
        }

        public void SendSocketTableInfo(ClientSession cs)
        {
            try
            {
                PayloadWriter pWritter = new PayloadWriter();

                byte[] plContent = Util.StringToByteArray("00 00 00 65 00 00 00 00 00 00 00 01 00 00 00 02 00 00 00 03 00 00 00 04 00 00 00 01 00 00 00 01 00 00 00 02 00 00 00 03 00 00 00 04 00 00 00 02 00 00 00 01 00 00 00 02 00 00 00 03 00 00 00 04 00 00 00 03 00 00 00 02 00 00 00 03 00 00 00 04 00 00 00 05 00 00 00 04 00 00 00 02 00 00 00 03 00 00 00 04 00 00 00 05 00 00 00 05 00 00 00 02 00 00 00 03 00 00 00 04 00 00 00 05 00 00 00 06 00 00 00 03 00 00 00 04 00 00 00 05 00 00 00 06 00 00 00 07 00 00 00 03 00 00 00 04 00 00 00 05 00 00 00 06 00 00 00 08 00 00 00 03 00 00 00 04 00 00 00 05 00 00 00 06 00 00 00 09 00 00 00 04 00 00 00 05 00 00 00 06 00 00 00 07 00 00 00 0A 00 00 00 04 00 00 00 05 00 00 00 06 00 00 00 07 00 00 00 0B 00 00 00 04 00 00 00 05 00 00 00 06 00 00 00 07 00 00 00 0C 00 00 00 05 00 00 00 06 00 00 00 07 00 00 00 08 00 00 00 0D 00 00 00 05 00 00 00 06 00 00 00 07 00 00 00 08 00 00 00 0E 00 00 00 05 00 00 00 06 00 00 00 07 00 00 00 08 00 00 00 0F 00 00 00 06 00 00 00 07 00 00 00 08 00 00 00 09 00 00 00 10 00 00 00 06 00 00 00 07 00 00 00 08 00 00 00 09 00 00 00 11 00 00 00 06 00 00 00 07 00 00 00 08 00 00 00 09 00 00 00 12 00 00 00 07 00 00 00 08 00 00 00 09 00 00 00 0A 00 00 00 13 00 00 00 07 00 00 00 08 00 00 00 09 00 00 00 0A 00 00 00 14 00 00 00 07 00 00 00 08 00 00 00 09 00 00 00 0A 00 00 00 15 00 00 00 08 00 00 00 09 00 00 00 0A 00 00 00 0B 00 00 00 16 00 00 00 08 00 00 00 09 00 00 00 0A 00 00 00 0B 00 00 00 17 00 00 00 08 00 00 00 09 00 00 00 0A 00 00 00 0B 00 00 00 18 00 00 00 09 00 00 00 0A 00 00 00 0B 00 00 00 0C 00 00 00 19 00 00 00 09 00 00 00 0A 00 00 00 0B 00 00 00 0C 00 00 00 1A 00 00 00 09 00 00 00 0A 00 00 00 0B 00 00 00 0C 00 00 00 1B 00 00 00 0A 00 00 00 0B 00 00 00 0C 00 00 00 0D 00 00 00 1C 00 00 00 0A 00 00 00 0B 00 00 00 0C 00 00 00 0D 00 00 00 1D 00 00 00 0A 00 00 00 0B 00 00 00 0C 00 00 00 0D 00 00 00 1E 00 00 00 0B 00 00 00 0C 00 00 00 0D 00 00 00 0E 00 00 00 1F 00 00 00 0B 00 00 00 0C 00 00 00 0D 00 00 00 0E 00 00 00 20 00 00 00 0B 00 00 00 0C 00 00 00 0D 00 00 00 0E 00 00 00 21 00 00 00 0C 00 00 00 0D 00 00 00 0E 00 00 00 0F 00 00 00 22 00 00 00 0C 00 00 00 0D 00 00 00 0E 00 00 00 0F 00 00 00 23 00 00 00 0C 00 00 00 0D 00 00 00 0E 00 00 00 0F 00 00 00 24 00 00 00 0D 00 00 00 0E 00 00 00 0F 00 00 00 10 00 00 00 25 00 00 00 0D 00 00 00 0E 00 00 00 0F 00 00 00 10 00 00 00 26 00 00 00 0D 00 00 00 0E 00 00 00 0F 00 00 00 10 00 00 00 27 00 00 00 0E 00 00 00 0F 00 00 00 10 00 00 00 11 00 00 00 28 00 00 00 0E 00 00 00 0F 00 00 00 10 00 00 00 11 00 00 00 29 00 00 00 0E 00 00 00 0F 00 00 00 10 00 00 00 11 00 00 00 2A 00 00 00 0F 00 00 00 10 00 00 00 11 00 00 00 12 00 00 00 2B 00 00 00 0F 00 00 00 10 00 00 00 11 00 00 00 12 00 00 00 2C 00 00 00 0F 00 00 00 10 00 00 00 11 00 00 00 12 00 00 00 2D 00 00 00 10 00 00 00 11 00 00 00 12 00 00 00 13 00 00 00 2E 00 00 00 10 00 00 00 11 00 00 00 12 00 00 00 13 00 00 00 2F 00 00 00 10 00 00 00 11 00 00 00 12 00 00 00 13 00 00 00 30 00 00 00 11 00 00 00 12 00 00 00 13 00 00 00 14 00 00 00 31 00 00 00 11 00 00 00 12 00 00 00 13 00 00 00 14 00 00 00 32 00 00 00 11 00 00 00 12 00 00 00 13 00 00 00 14 00 00 00 33 00 00 00 12 00 00 00 13 00 00 00 14 00 00 00 15 00 00 00 34 00 00 00 12 00 00 00 13 00 00 00 14 00 00 00 15 00 00 00 35 00 00 00 12 00 00 00 13 00 00 00 14 00 00 00 15 00 00 00 36 00 00 00 13 00 00 00 14 00 00 00 15 00 00 00 16 00 00 00 37 00 00 00 13 00 00 00 14 00 00 00 15 00 00 00 16 00 00 00 38 00 00 00 13 00 00 00 14 00 00 00 15 00 00 00 16 00 00 00 39 00 00 00 14 00 00 00 15 00 00 00 16 00 00 00 17 00 00 00 3A 00 00 00 14 00 00 00 15 00 00 00 16 00 00 00 17 00 00 00 3B 00 00 00 14 00 00 00 15 00 00 00 16 00 00 00 17 00 00 00 3C 00 00 00 15 00 00 00 16 00 00 00 17 00 00 00 18 00 00 00 3D 00 00 00 15 00 00 00 16 00 00 00 17 00 00 00 18 00 00 00 3E 00 00 00 15 00 00 00 16 00 00 00 17 00 00 00 18 00 00 00 3F 00 00 00 16 00 00 00 17 00 00 00 18 00 00 00 19 00 00 00 40 00 00 00 16 00 00 00 17 00 00 00 18 00 00 00 19 00 00 00 41 00 00 00 16 00 00 00 17 00 00 00 18 00 00 00 19 00 00 00 42 00 00 00 17 00 00 00 18 00 00 00 19 00 00 00 1A 00 00 00 43 00 00 00 17 00 00 00 18 00 00 00 19 00 00 00 1A 00 00 00 44 00 00 00 17 00 00 00 18 00 00 00 19 00 00 00 1A 00 00 00 45 00 00 00 18 00 00 00 19 00 00 00 1A 00 00 00 1B 00 00 00 46 00 00 00 18 00 00 00 19 00 00 00 1A 00 00 00 1B 00 00 00 47 00 00 00 18 00 00 00 19 00 00 00 1A 00 00 00 1B 00 00 00 48 00 00 00 19 00 00 00 1A 00 00 00 1B 00 00 00 1C 00 00 00 49 00 00 00 19 00 00 00 1A 00 00 00 1B 00 00 00 1C 00 00 00 4A 00 00 00 19 00 00 00 1A 00 00 00 1B 00 00 00 1C 00 00 00 4B 00 00 00 1A 00 00 00 1B 00 00 00 1C 00 00 00 1D 00 00 00 4C 00 00 00 1A 00 00 00 1B 00 00 00 1C 00 00 00 1D 00 00 00 4D 00 00 00 1A 00 00 00 1B 00 00 00 1C 00 00 00 1D 00 00 00 4E 00 00 00 1B 00 00 00 1C 00 00 00 1D 00 00 00 1E 00 00 00 4F 00 00 00 1B 00 00 00 1C 00 00 00 1D 00 00 00 1E 00 00 00 50 00 00 00 1B 00 00 00 1C 00 00 00 1D 00 00 00 1E 00 00 00 51 00 00 00 1C 00 00 00 1D 00 00 00 1E 00 00 00 1F 00 00 00 52 00 00 00 1C 00 00 00 1D 00 00 00 1E 00 00 00 1F 00 00 00 53 00 00 00 1C 00 00 00 1D 00 00 00 1E 00 00 00 1F 00 00 00 54 00 00 00 1D 00 00 00 1E 00 00 00 1F 00 00 00 20 00 00 00 55 00 00 00 1D 00 00 00 1E 00 00 00 1F 00 00 00 20 00 00 00 56 00 00 00 1D 00 00 00 1E 00 00 00 1F 00 00 00 20 00 00 00 57 00 00 00 1E 00 00 00 1F 00 00 00 20 00 00 00 21 00 00 00 58 00 00 00 1E 00 00 00 1F 00 00 00 20 00 00 00 21 00 00 00 59 00 00 00 1E 00 00 00 1F 00 00 00 20 00 00 00 21 00 00 00 5A 00 00 00 1F 00 00 00 20 00 00 00 21 00 00 00 22 00 00 00 5B 00 00 00 1F 00 00 00 20 00 00 00 21 00 00 00 22 00 00 00 5C 00 00 00 1F 00 00 00 20 00 00 00 21 00 00 00 22 00 00 00 5D 00 00 00 20 00 00 00 21 00 00 00 22 00 00 00 23 00 00 00 5E 00 00 00 20 00 00 00 21 00 00 00 22 00 00 00 23 00 00 00 5F 00 00 00 20 00 00 00 21 00 00 00 22 00 00 00 23 00 00 00 60 00 00 00 21 00 00 00 22 00 00 00 23 00 00 00 24 00 00 00 61 00 00 00 21 00 00 00 22 00 00 00 23 00 00 00 24 00 00 00 62 00 00 00 21 00 00 00 22 00 00 00 23 00 00 00 24 00 00 00 63 00 00 00 22 00 00 00 23 00 00 00 24 00 00 00 25 00 00 00 64 00 00 00 22 00 00 00 23 00 00 00 24 00 00 00 25 00 00 00 65 00 00 00 00 00 00 00 BE 00 00 00 FA 00 00 01 4A 00 00 01 D6 00 00 00 01 00 00 00 BE 00 00 00 FA 00 00 01 4A 00 00 01 D6 00 00 00 02 00 00 00 BE 00 00 00 FA 00 00 01 4A 00 00 01 D6 00 00 00 03 00 00 00 FA 00 00 01 4A 00 00 01 D6 00 00 03 52 00 00 00 04 00 00 00 FA 00 00 01 4A 00 00 01 D6 00 00 03 52 00 00 00 05 00 00 00 FA 00 00 01 4A 00 00 01 D6 00 00 03 52 00 00 00 06 00 00 01 4A 00 00 01 D6 00 00 03 52 00 00 08 98 00 00 00 07 00 00 01 4A 00 00 01 D6 00 00 03 52 00 00 08 98 00 00 00 08 00 00 01 4A 00 00 01 D6 00 00 03 52 00 00 08 98 00 00 00 09 00 00 01 D6 00 00 03 52 00 00 08 98 00 00 0E 74 00 00 00 0A 00 00 01 D6 00 00 03 52 00 00 08 98 00 00 0E 74 00 00 00 0B 00 00 01 D6 00 00 03 52 00 00 08 98 00 00 0E 74 00 00 00 0C 00 00 03 52 00 00 08 98 00 00 0E 74 00 00 17 0C 00 00 00 0D 00 00 03 52 00 00 08 98 00 00 0E 74 00 00 17 0C 00 00 00 0E 00 00 03 52 00 00 08 98 00 00 0E 74 00 00 17 0C 00 00 00 0F 00 00 08 98 00 00 0E 74 00 00 17 0C 00 00 22 C4 00 00 00 10 00 00 08 98 00 00 0E 74 00 00 17 0C 00 00 22 C4 00 00 00 11 00 00 08 98 00 00 0E 74 00 00 17 0C 00 00 22 C4 00 00 00 12 00 00 0E 74 00 00 17 0C 00 00 22 C4 00 00 31 9C 00 00 00 13 00 00 0E 74 00 00 17 0C 00 00 22 C4 00 00 31 9C 00 00 00 14 00 00 0E 74 00 00 17 0C 00 00 22 C4 00 00 31 9C 00 00 00 15 00 00 17 0C 00 00 22 C4 00 00 31 9C 00 00 55 F0 00 00 00 16 00 00 17 0C 00 00 22 C4 00 00 31 9C 00 00 55 F0 00 00 00 17 00 00 17 0C 00 00 22 C4 00 00 31 9C 00 00 55 F0 00 00 00 18 00 00 22 C4 00 00 31 9C 00 00 55 F0 00 00 6C 98 00 00 00 19 00 00 22 C4 00 00 31 9C 00 00 55 F0 00 00 6C 98 00 00 00 1A 00 00 22 C4 00 00 31 9C 00 00 55 F0 00 00 6C 98 00 00 00 1B 00 00 31 9C 00 00 55 F0 00 00 6C 98 00 00 84 08 00 00 00 1C 00 00 31 9C 00 00 55 F0 00 00 6C 98 00 00 84 08 00 00 00 1D 00 00 31 9C 00 00 55 F0 00 00 6C 98 00 00 84 08 00 00 00 1E 00 00 55 F0 00 00 6C 98 00 00 84 08 00 00 9C 40 00 00 00 1F 00 00 55 F0 00 00 6C 98 00 00 84 08 00 00 9C 40 00 00 00 20 00 00 55 F0 00 00 6C 98 00 00 84 08 00 00 9C 40 00 00 00 21 00 00 6C 98 00 00 84 08 00 00 9C 40 00 00 B9 28 00 00 00 22 00 00 6C 98 00 00 84 08 00 00 9C 40 00 00 B9 28 00 00 00 23 00 00 6C 98 00 00 84 08 00 00 9C 40 00 00 B9 28 00 00 00 24 00 00 84 08 00 00 9C 40 00 00 B9 28 00 00 D8 CC 00 00 00 25 00 00 84 08 00 00 9C 40 00 00 B9 28 00 00 D8 CC 00 00 00 26 00 00 84 08 00 00 9C 40 00 00 B9 28 00 00 D8 CC 00 00 00 27 00 00 9C 40 00 00 B9 28 00 00 D8 CC 00 00 F8 70 00 00 00 28 00 00 9C 40 00 00 B9 28 00 00 D8 CC 00 00 F8 70 00 00 00 29 00 00 9C 40 00 00 B9 28 00 00 D8 CC 00 00 F8 70 00 00 00 2A 00 00 B9 28 00 00 D8 CC 00 00 F8 70 00 01 1D 28 00 00 00 2B 00 00 B9 28 00 00 D8 CC 00 00 F8 70 00 01 1D 28 00 00 00 2C 00 00 B9 28 00 00 D8 CC 00 00 F8 70 00 01 1D 28 00 00 00 2D 00 00 D8 CC 00 00 F8 70 00 01 1D 28 00 01 47 58 00 00 00 2E 00 00 D8 CC 00 00 F8 70 00 01 1D 28 00 01 47 58 00 00 00 2F 00 00 D8 CC 00 00 F8 70 00 01 1D 28 00 01 47 58 00 00 00 30 00 00 F8 70 00 01 1D 28 00 01 47 58 00 01 7D 40 00 00 00 31 00 00 F8 70 00 01 1D 28 00 01 47 58 00 01 7D 40 00 00 00 32 00 00 F8 70 00 01 1D 28 00 01 47 58 00 01 7D 40 00 00 00 33 00 01 1D 28 00 01 47 58 00 01 7D 40 00 01 B7 74 00 00 00 34 00 01 1D 28 00 01 47 58 00 01 7D 40 00 01 B7 74 00 00 00 35 00 01 1D 28 00 01 47 58 00 01 7D 40 00 01 B7 74 00 00 00 36 00 01 47 58 00 01 7D 40 00 01 B7 74 00 02 00 E4 00 00 00 37 00 01 47 58 00 01 7D 40 00 01 B7 74 00 02 00 E4 00 00 00 38 00 01 47 58 00 01 7D 40 00 01 B7 74 00 02 00 E4 00 00 00 39 00 01 7D 40 00 01 B7 74 00 02 00 E4 00 02 50 F8 00 00 00 3A 00 01 7D 40 00 01 B7 74 00 02 00 E4 00 02 50 F8 00 00 00 3B 00 01 7D 40 00 01 B7 74 00 02 00 E4 00 02 50 F8 00 00 00 3C 00 01 B7 74 00 02 00 E4 00 02 50 F8 00 02 AC C4 00 00 00 3D 00 01 B7 74 00 02 00 E4 00 02 50 F8 00 02 AC C4 00 00 00 3E 00 01 B7 74 00 02 00 E4 00 02 50 F8 00 02 AC C4 00 00 00 3F 00 02 00 E4 00 02 50 F8 00 02 AC C4 00 03 15 10 00 00 00 40 00 02 00 E4 00 02 50 F8 00 02 AC C4 00 03 15 10 00 00 00 41 00 02 00 E4 00 02 50 F8 00 02 AC C4 00 03 15 10 00 00 00 42 00 02 50 F8 00 02 AC C4 00 03 15 10 00 03 BB 78 00 00 00 43 00 02 50 F8 00 02 AC C4 00 03 15 10 00 03 BB 78 00 00 00 44 00 02 50 F8 00 02 AC C4 00 03 15 10 00 03 BB 78 00 00 00 45 00 02 AC C4 00 03 15 10 00 03 BB 78 00 04 27 48 00 00 00 46 00 02 AC C4 00 03 15 10 00 03 BB 78 00 04 27 48 00 00 00 47 00 02 AC C4 00 03 15 10 00 03 BB 78 00 04 27 48 00 00 00 48 00 03 15 10 00 03 BB 78 00 04 27 48 00 04 AC E0 00 00 00 49 00 03 15 10 00 03 BB 78 00 04 27 48 00 04 AC E0 00 00 00 4A 00 03 15 10 00 03 BB 78 00 04 27 48 00 04 AC E0 00 00 00 4B 00 03 BB 78 00 04 27 48 00 04 AC E0 00 05 32 78 00 00 00 4C 00 03 BB 78 00 04 27 48 00 04 AC E0 00 05 32 78 00 00 00 4D 00 03 BB 78 00 04 27 48 00 04 AC E0 00 05 32 78 00 00 00 4E 00 04 27 48 00 04 AC E0 00 05 32 78 00 05 B8 10 00 00 00 4F 00 04 27 48 00 04 AC E0 00 05 32 78 00 05 B8 10 00 00 00 50 00 04 27 48 00 04 AC E0 00 05 32 78 00 05 B8 10 00 00 00 51 00 04 AC E0 00 05 32 78 00 05 B8 10 00 06 3D A8 00 00 00 52 00 04 AC E0 00 05 32 78 00 05 B8 10 00 06 3D A8 00 00 00 53 00 04 AC E0 00 05 32 78 00 05 B8 10 00 06 3D A8 00 00 00 54 00 05 32 78 00 05 B8 10 00 06 3D A8 00 06 C3 40 00 00 00 55 00 05 32 78 00 05 B8 10 00 06 3D A8 00 06 C3 40 00 00 00 56 00 05 32 78 00 05 B8 10 00 06 3D A8 00 06 C3 40 00 00 00 57 00 05 B8 10 00 06 3D A8 00 06 C3 40 00 07 48 74 00 00 00 58 00 05 B8 10 00 06 3D A8 00 06 C3 40 00 07 48 74 00 00 00 59 00 05 B8 10 00 06 3D A8 00 06 C3 40 00 07 48 74 00 00 00 5A 00 06 3D A8 00 06 C3 40 00 07 48 74 00 07 CE 0C 00 00 00 5B 00 06 3D A8 00 06 C3 40 00 07 48 74 00 07 CE 0C 00 00 00 5C 00 06 3D A8 00 06 C3 40 00 07 48 74 00 07 CE 0C 00 00 00 5D 00 06 C3 40 00 07 48 74 00 07 CE 0C 00 08 53 A4 00 00 00 5E 00 06 C3 40 00 07 48 74 00 07 CE 0C 00 08 53 A4 00 00 00 5F 00 06 C3 40 00 07 48 74 00 07 CE 0C 00 08 53 A4 00 00 00 60 00 07 48 74 00 07 CE 0C 00 08 53 A4 00 08 D9 3C 00 00 00 61 00 07 48 74 00 07 CE 0C 00 08 53 A4 00 08 D9 3C 00 00 00 62 00 07 48 74 00 07 CE 0C 00 08 53 A4 00 08 D9 3C 00 00 00 63 00 07 CE 0C 00 08 53 A4 00 08 D9 3C 00 09 5E D4 00 00 00 64 00 07 CE 0C 00 08 53 A4 00 08 D9 3C 00 09 5E D4 00 04 61 54");
                pWritter.WriteData(plContent);

                OutPacket oPacket = new OutPacket(pWritter.GetCompressedPayload(CenterOpcodes.ENU_SOCKET_TABLE_INFO_NOT),
                    cs._CryptoHandler, cs._AuthHandler, cs.Prefix, ++cs.Count);

                cs.Send(oPacket);
            }
            catch (Exception ex)
            {
                Log.Error("SendSocketTableInfo: {0}", ex.Message);
                while (true) ;
            }
        }

        public void SendCashbackRatio(ClientSession cs)
        {
            try
            {
                PayloadWriter pWritter = new PayloadWriter();

                byte[] plContent = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                pWritter.WriteData(plContent);

                OutPacket oPacket = new OutPacket(pWritter.GetCompressedPayload(CenterOpcodes.ENU_CASHBACK_RATIO_INFO_NOT),
                    cs._CryptoHandler, cs._AuthHandler, cs.Prefix, ++cs.Count);

                cs.Send(oPacket);
            }
            catch (Exception ex)
            {
                Log.Error("SendCashbackRatio: {0}", ex.Message);
                while (true) ;
            }
        }

        public void SendShop(ClientSession cs)
        {
            Database db = new Database();

            if (db.OpenDB())
            {
                try
                {
                    MySqlDataReader sReader = db.Query("SELECT start, end FROM shop");
                    Log.SqlQuerys(db.dbcmd.CommandText);

                    DataTable dt = new DataTable();

                    dt.Load(sReader);
                    sReader.Close();
                    db.dbconn.Close();

                    PayloadWriter pWriter = new PayloadWriter();

                    byte[] plContent = { 0x00 };
                    pWriter.WriteData(plContent);
                    pWriter.WriteData(dt.Rows.Count);

                    foreach (DataRow row in dt.Rows)
                    {
                        pWriter.WriteData((int)row[0]);
                        pWriter.WriteData((int)row[1]);
                    }

                    byte[] plContent1 = { 0x00, 0x00, 0x00, 0x00 };
                    pWriter.WriteData(plContent1);

                    OutPacket oPacket = new OutPacket(pWriter.GetCompressedPayload(CenterOpcodes.ENU_ITEM_BUY_INFO_NOT),
                        cs._CryptoHandler, cs._AuthHandler, cs.Prefix, ++cs.Count);

                    cs.Send(oPacket);
                }
                catch (Exception ex)
                {
                    Log.Error("Send shop: {0}", ex.Message);
                    while (true) ;
                }
            }
        }

        public void OnLogin(ClientSession cs, InPacket ip)
        {
            PayloadReader pReader = new PayloadReader(ip.Payload);

            Database db = new Database();

            pReader.Skip(7);
            byte IDLength = pReader.ReadByte();

            UID = pReader.ReadString(IDLength);

            pReader.Skip(3);

            byte PWLength = pReader.ReadByte();

            UPW = pReader.ReadString(PWLength);

            if (db.OpenDB())
            {
                try
                {
                    MySqlDataReader sReader = db.Query("SELECT * FROM users WHERE LOGIN = @0 AND PASS = @1", UID, UPW);
                    Log.SqlQuerys(db.dbcmd.CommandText);

                    if (!sReader.HasRows)
                    {
                        Log.Inform("Falha no login. Usuário: {0}", UID);

                        try
                        {
                            PayloadWriter pWriter = new PayloadWriter();

                            byte[] plContent = { 0x00, 0x00, 0x00, 0x14 };
                            pWriter.WriteData(plContent);

                            pWriter.WriteData(IDLength * 2);
                            pWriter.WriteUnicodeString(UID);

                            byte[] plContent1 = { 0x00, 0x00, 0x00, 0x00, 0x00 };
                            pWriter.WriteData(plContent1);

                            OutPacket oPacket = new OutPacket(pWriter.GetCompressedPayload(CenterOpcodes.ENU_VERIFY_ACCOUNT_ACK),
                                cs._CryptoHandler, cs._AuthHandler, cs.Prefix, ++cs.Count);

                            cs.Send(oPacket);
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex.Message);
                            while (true) ;
                        }
                    }
                    else
                    {
                        // Successful login.
                        Log.Inform("Novo login. Usuário: {0}", UID);

                        // Send server list first.
                        Log.Inform("Enviando a lista de servidores.");
                        SendServerList(cs);
                        SendChannelNews(cs);
                        SendShop(cs);
                        SendClientContentOpen(cs);
                        SendSocketTableInfo(cs);
                        SendCashbackRatio(cs);

                        //Log.Inform("로그인 성공 메시지를 전송합니다.", ID);

                        PayloadWriter pWritter = new PayloadWriter();

                        byte[] plContent = { 0x00, 0x00, 0x00, 0x00 };
                        pWritter.WriteData(plContent);
                        pWritter.WriteData(IDLength * 2);
                        pWritter.WriteUnicodeString(UID);
                        pWritter.WriteData(PWLength);
                        pWritter.WriteData(UPW);

                        byte[] plContent2 = Util.StringToByteArray("00 00 00 00 14 00 8E A7 C5 01 00 00 00 00 00 00 02 4B 52 00 05 A3 BD 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 FF 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 03 00 00 00 00 00 00 00 00 00 00 00 00");
                        pWritter.WriteData(plContent2);

                        pWritter.WriteData(cs.MyLoading.GuildMarkURL.Length * 2);
                        pWritter.WriteUnicodeString(cs.MyLoading.GuildMarkURL);

                        byte[] plContent3 = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                        pWritter.WriteData(plContent3);

                        int SHAi = cs.MyLoading.SHAFileList.Length;
                        pWritter.WriteData(SHAi);

                        for (int i = 0; i <= cs.MyLoading.SHAFileList.Length - 1; i++)
                        {
                            pWritter.WriteData(cs.MyLoading.SHAFileList[i].Length * 2);
                            pWritter.WriteUnicodeString(cs.MyLoading.SHAFileList[i]);
                            pWritter.WriteData(true);
                        }

                        byte[] plContent4 = Util.StringToByteArray("00 00 00 01 00 00 00 03 00 00 00 00 00 00 00 00 64 01 00 00 00 00 00 00 00 64 02 00 00 00 00 00 00 00 64 01 BF 80 00 00 FC 04 97 FF 00 00 00 00 00 00 00 00 00 00 00 00");
                        pWritter.WriteData(plContent4);

                        OutPacket oPacket = new OutPacket(pWritter.GetCompressedPayload(CenterOpcodes.ENU_VERIFY_ACCOUNT_ACK),
                            cs._CryptoHandler, cs._AuthHandler, cs.Prefix, ++cs.Count);

                        cs.Send(oPacket);
                    }

                    sReader.Close();
                    db.dbconn.Close();
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    Log.Sql(ex.Message);
                    while (true) ;
                }
            }
        }

        public void OnGuideBookList(ClientSession cs)
        {
            try
            {
                PayloadWriter pWritter = new PayloadWriter();

                byte[] plContent = { 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00 };
                pWritter.WriteData(plContent);

                OutPacket oPacket = new OutPacket(pWritter.GetCompressedPayload(CenterOpcodes.ENU_GUIDE_BOOK_LIST_ACK),
                    cs._CryptoHandler, cs._AuthHandler, cs.Prefix, ++cs.Count);

                cs.Send(oPacket);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                while (true) ;
            }
        }
    }
}
