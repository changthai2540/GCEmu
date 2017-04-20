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

using System.Configuration;

namespace GCEmuCenter.Misc
{
    public class Configs
    {
        /// <summary>
        /// Database parameters
        /// </summary>
        public static string MySqlHost
        {
            get
            {
                return ConfigurationManager.AppSettings["dbHost"].ToString();
            }
        }
        public static string MySqlPort
        {
            get
            {
                return ConfigurationManager.AppSettings["dbPort"].ToString();
            }
        }
        public static string MySqlUser
        {
            get
            {
                return ConfigurationManager.AppSettings["dbUser"].ToString();
            }
        }
        public static string MySqlPass
        {
            get
            {
                return ConfigurationManager.AppSettings["dbPass"].ToString();
            }
        }
        public static string MySqlDB
        {
            get
            {
                return ConfigurationManager.AppSettings["dbDB"].ToString();
            }
        }

        /// <summary>
        /// Server parameters
        /// </summary>
        public static short ServerPort
        {
            get
            {
                short sPort;
                short.TryParse(ConfigurationManager.AppSettings["serverPort"], out sPort);

                return sPort;
            }
        }

        public static bool LogPackets
        {
            get
            {
                bool logPackets;
                bool.TryParse(ConfigurationManager.AppSettings["logPackets"], out logPackets);

                return logPackets;
            }
        }

        public static bool LogSQL
        {
            get
            {
                bool logSQL;
                bool.TryParse(ConfigurationManager.AppSettings["logSQL"], out logSQL);

                return logSQL;
            }
        }

        public static bool LogSQLQuerys
        {
            get
            {
                bool logSQLQuerys;
                bool.TryParse(ConfigurationManager.AppSettings["logSQLQuerys"], out logSQLQuerys);

                return logSQLQuerys;
            }
        }
    }
}
