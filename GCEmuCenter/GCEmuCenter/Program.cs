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
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using GCEmuCenter.Session;
using GCEmuCenter.Misc;
using GCEmuCenter.IO;

namespace GCEmuCenter
{
    internal class Program
    {
        private static bool isAlive;
        private static bool IsAlive
        {
            get
            {
                return isAlive;
            }
            set
            {
                isAlive = value;

                if (!value)
                    _listenTask.Wait();
            }
        }

        public static TcpListener Listener { get; private set; }
        public static IPEndPoint RemoteEndPoint { get; private set; }

        private static Task _listenTask;

        static void Main(string[] args)
        {
            Log.Entitle("GCEmu v0.1 - Center");

            try
            {
                Listener = new TcpListener(IPAddress.Any, Configs.ServerPort);
                Listener.Start();

                Log.Success("Servidor inicializado em: {0}", Listener.LocalEndpoint);

                IsAlive = true;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            _listenTask = Task.Factory.StartNew(() => OnAcceptSocket());

            if (IsAlive)
                Log.Inform("Tarefa inicializada. ID: {0}", _listenTask.Id);
            else
                Log.Error("Tarefa não pode ser inicializada.");

            while (true) ;
        }

        /// <summary>
        /// Accepts the connection and creates a new Client Session.
        /// </summary>
        private static async void OnAcceptSocket()
        {
            while (IsAlive)
            {
                Log.Inform("Aceitando socket...");

                var cSocket = await Listener.AcceptSocketAsync();
                ClientSession session = new ClientSession(cSocket);

                Log.Inform("Conexão com socket iniciada. IP: {0}", session.Label);
            }
        }

        private static void Dispose()
        {
            if (Program.Listener != null)
                Program.Listener.Stop();
        }
    }
}
