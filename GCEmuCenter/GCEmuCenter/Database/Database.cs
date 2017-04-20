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

using MySql.Data.MySqlClient;
using GCEmuCenter.IO;
using GCEmuCenter.Misc;
using System;

namespace GCEmuCenter.DB
{
    public class Database
    {
        public MySqlConnection dbconn { get; private set; }
        public MySqlCommand dbcmd { get; private set; }

        public bool IsConnected { get; private set; }

        internal string ConnectionString
        {
            get
            {
                return string.Format("server={0}; port={1}; uid={2}; pwd={3}; database={4};",
                    Configs.MySqlHost, Configs.MySqlPort, Configs.MySqlUser, Configs.MySqlPass, Configs.MySqlDB);
            }
        }

        public bool OpenDB()
        {
            try
            {
                dbconn = new MySqlConnection();
                dbcmd = new MySqlCommand();

                dbconn.ConnectionString = ConnectionString;
                dbconn.Open();
                Log.Sql("Conexão com a database efetuada com sucesso.");

                dbcmd.Connection = dbconn;

                IsConnected = true;
            }
            catch (MySqlException ex)
            {
                Log.Sql(ex.Message);
                while (true) ;
            }

            return true;
        }

        public MySqlDataReader Query(string cmdString, params string[] args)
        {
            if (!IsConnected)
                return null;

            dbcmd.CommandText = cmdString;
            dbcmd.Prepare();

            string str;
            for (int x = 0; x < args.Length; x++)
            {
                str = String.Format("@{0}", x);
                dbcmd.Parameters.AddWithValue(str, args[x]);
            }

            dbcmd.ExecuteNonQuery();

            return dbcmd.ExecuteReader();
        }

        public bool Test()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();
                    connection.Close();
                    return true;
                    
                }
            }
            catch (MySqlException e)
            {
                Log.Error(e);
                return false;
            }
        }
    }
}
