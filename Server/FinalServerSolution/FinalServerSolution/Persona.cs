/*
 *  Copyright (c) 2015-2016 Vitaleks
 *  Copyright (c) 2015 Edmundas919
 *  Copyright (c) 2015 mc3dcm
 * 
 *  This file is part of NFS World Server.
 *
 *  NFS World Server is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *  
 *  NFS World Server is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with NFS World Server.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FinalServerSolution
{
    class Persona
    {
        public static void getpersonainfo(int personaId, MySqlConnection connection, out string outPacket, out bool isGoodResponse)
        {
            string sql = String.Format("SELECT * FROM Persona WHERE personaId = {0}", personaId);
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    object[] userData = new object[9];
                    userData[0] = reader["cash"];
                    userData[1] = reader["iconIndex"];
                    userData[2] = reader["level"];
                    userData[3] = reader["motto"];
                    userData[4] = reader["name"];
                    userData[5] = reader["personaId"];
                    userData[6] = 1000;
                    userData[7] = reader["rep"];
                    userData[8] = reader["score"];

                    string packet = File.ReadAllText(Program.serverPrefix + "/Engine.svc/DriverPersona/getpersonainfo.xml");

                    outPacket = String.Format(packet, userData);
                    isGoodResponse = true;
                }
                else
                {
                    outPacket = String.Format("");
                    isGoodResponse = false;
                }
            }
        }
        public static void getPersonaBaseFromList(string requestPacket, MySqlConnection connection, out string outPacket, out bool isGoodResponse)
        {
            XmlDocument packetParsing = new XmlDocument();
            packetParsing.LoadXml(requestPacket);

            string sql = String.Format("SELECT * FROM Persona LEFT JOIN Users ON Persona.userId = Users.userId WHERE personaId = {0}", packetParsing.FirstChild.FirstChild.FirstChild.InnerText);
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    object[] userData = new object[7];
                    userData[0] = reader["iconIndex"];
                    userData[1] = reader["level"];
                    userData[2] = reader["motto"];
                    userData[3] = reader["name"];
                    userData[4] = reader["personaId"];
                    userData[5] = reader["score"];
                    userData[6] = reader["userId"];

                    string packet = File.ReadAllText(Program.serverPrefix + "/Engine.svc/DriverPersona/GetPersonaBaseFromList.xml");

                    outPacket = String.Format(packet, userData);
                    isGoodResponse = true;
                }
                else
                {
                    outPacket = String.Format("");
                    isGoodResponse = false;
                }
            }
        }

        public static void getPersonaPresenceByName(string name, MySqlConnection connection, out string outPacket, out bool isGoodResponse)
        {
            string sql = String.Format("SELECT personaId, userId FROM Persona WHERE name = '{0}'", name);

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    object[] userData = new object[2];
                    userData[0] = reader["personaId"];
                    userData[1] = reader["userId"];

                    string packet = File.ReadAllText(Program.serverPrefix + "/Engine.svc/DriverPersona/GetPersonaPresenceByName.xml");

                    outPacket = String.Format(packet, userData);
                    isGoodResponse = true;
                }
                else
                {
                    outPacket = String.Format("");
                    isGoodResponse = true;
                }
            }
        }
    }
}
