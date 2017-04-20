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
using System.Text;
using GCEmuCenter.Misc;

namespace GCEmuCenter.IO
{
    public static class Log
    {
        private const byte LabelWidth = 10;
        private static bool Entitled = false;

        public static bool Running { get; private set; }

        public static string Margin
        {
            get { return new string(' ', Log.LabelWidth); }
        }

        public static string MaskString(string input, char mask = '*')
        {
            return new string(mask, input.Length);
        }

        public static bool ShowStackTrace { get; set; }

        static Log()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();

            Log.Running = true;
        }

        public static string Input(string label)
        {
            lock (typeof(Log))
            {
                Log.WriteItem("INPUT", ConsoleColor.Cyan, string.Empty);
                Console.Write(label);
                return Console.ReadLine();
            }
        }

        public static string Input(string label, string defaultValue)
        {
            lock (typeof(Log))
            {
                Log.WriteItem("INPUT", ConsoleColor.Cyan, string.Empty);
                Console.Write(label);
                string result = Console.ReadLine();

                if (result == string.Empty)
                {
                    result = defaultValue;
                    Console.CursorTop--;
                    Console.CursorLeft = Log.Margin.Length + label.Length;

                    Console.WriteLine(defaultValue == string.Empty ? "(None)" : result);
                }

                return result;
            }
        }

        /// <summary>
        /// Writes a labeled item to the output.
        /// </summary>
        /// <param name="label">The label</param>
        /// <param name="labelColor">The label's color</param>
        /// <param name="value">The text</param>
        /// <param name="args">Arguments</param>
        private static void WriteItem(string label, ConsoleColor labelColor, string value, params object[] args)
        {
            lock (typeof(Log))
            {
                StringBuilder sb = new StringBuilder();

                sb.Append(' ', Log.LabelWidth - label.Length - 3);
                sb.Append("[");
                sb.Append(label);
                sb.Append("]");
                sb.Append(" ");

                label = sb.ToString();
                value = string.Format(value, args);

                Console.ForegroundColor = labelColor;
                Console.Write(label);
                Console.ForegroundColor = ConsoleColor.Gray;

                bool first = true;

                foreach (string s in value.Split('\n'))
                {
                    string[] lines = new string[(int)Math.Ceiling((float)s.Length /
                        (float)(Console.BufferWidth -Log.LabelWidth))];

                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (i == lines.Length - 1)
                            lines[i] = s.Substring((Console.BufferWidth - Log.LabelWidth) * i);
                        else
                            lines[i] = s.Substring((Console.BufferWidth - Log.LabelWidth) * i,
                                (Console.BufferWidth - Log.LabelWidth));
                    }

                    foreach (string line in lines)
                    {
                        if (!first)
                            Console.Write(Log.Margin);

                        if ((line.Length + Log.LabelWidth) < Console.BufferWidth)
                            Console.WriteLine(line);
                        else if ((line.Length + Log.LabelWidth) == Console.BufferWidth)
                            Console.Write(line);

                        first = false;
                    }
                }
            }
        }

        public static void SkipLine()
        {
            Console.WriteLine();
        }

        public static void Entitle(string value, params object[] args)
        {
            lock (typeof(Log))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;

                StringBuilder sb = new StringBuilder();

                sb.Append("\n");
                sb.Append("  ");

                sb.Append((Log.Entitled ? "" : "") + string.Format(value, args) + '\n');
                Console.WriteLine(sb.ToString());
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Title = string.Format(value, args);

                Log.Entitled = true;
            }
        }

        public static void Inform(string value, params object[] args)
        {
            Log.WriteItem("INFO", ConsoleColor.White, value, args);
        }

        public static void Inform(object value)
        {
            Log.Inform(value.ToString());
        }

        public static void Warn(string value, params object[] args)
        {
            Log.WriteItem("AVISO", ConsoleColor.Yellow, value, args);
        }

        public static void Warn(object value)
        {
            Log.Warn(value.ToString());
        }

        public static void Error(string value, params object[] args)
        {
            Log.WriteItem("ERRO", ConsoleColor.Red, value, args);
        }

        public static void Error(object value)
        {
            Log.Error(value.ToString());
        }

        public static void Error(Exception exception)
        {
            Log.WriteItem("ERRO", ConsoleColor.Red, Log.ShowStackTrace ? exception.ToString() : exception.Message);
        }

        public static void Error(string label, Exception exception, params object[] args)
        {
            Log.WriteItem("EROO", ConsoleColor.Red, "{0}\n{1}", string.Format(label, args),
                Log.ShowStackTrace ? exception.ToString() : exception.Message);
        }

        public static void Success(string value, params object[] args)
        {
            Log.WriteItem("SUCESSO", ConsoleColor.Green, value, args);
        }

        public static void Success(object value)
        {
            Log.Inform(value.ToString());
        }

        public static void Packet(string value, params object[] args)
        {
            if (Configs.LogPackets)
                Log.WriteItem("PACOTE", ConsoleColor.Blue, value, args);
        }

        public static void Packet(object value)
        {
            Log.Packet(value.ToString());
        }

        public static void Sql(string value, params object[] args)
        {
            if (Configs.LogSQL)
                Log.WriteItem("SQL", ConsoleColor.Magenta, value, args);
        }

        public static void Sql(object value)
        {
            Log.Sql(value.ToString());
        }

        public static void SqlQuerys(string value, params object[] args)
        {
            if (Configs.LogSQLQuerys)
                Log.WriteItem("QUERY", ConsoleColor.Magenta, value, args);
        }

        public static void SqlQuerys(object value)
        {
            Log.SqlQuerys(value.ToString());
        }

        public static void Hex(string label, byte[] value, params object[] args)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(string.Format(label, args));
            sb.Append('\n');

            if (value == null || value.Length == 0)
                sb.Append("(VAZIO)");
            else
            {
                int lineSeparation = 0;

                foreach (byte b in value)
                {
                    if (lineSeparation == 16)
                    {
                        sb.Append('\n');
                        lineSeparation = 0;
                    }

                    sb.AppendFormat("{0:X2} ", b);
                    lineSeparation++;
                }
            }

            Log.WriteItem("HEX", ConsoleColor.Magenta, sb.ToString());
        }

        public static void Hex(string label, byte b, params object[] args)
        {
            Log.Hex(label, new byte[] { b }, args);
        }
    }
}
