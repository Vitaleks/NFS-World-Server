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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Xml;
using System.IO;

namespace FinalServerSolution
{
    class Authorization
    {
        public static void Login(string packet, MySqlConnection connection, out string outPacket, out bool isGoodResponse)
        {
            MySqlConnection connection2 = new MySqlConnection(MsSQL.connetionString);
            connection2.Open();

            XmlDocument packetParsing = new XmlDocument();
            packetParsing.LoadXml(packet);
            string email = "0";
            string password = "0";

            foreach (XmlNode key in packetParsing.FirstChild.ChildNodes)
            {
                if (key.Name == "Email")
                    email = key.InnerText;
                if (key.Name == "Password")
                    password = key.InnerText;
            }

            string sql = String.Format("SELECT userId FROM Users WHERE userEmail='{0}' AND userPassword='{1}'", email, password);
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            using (MySqlDataReader reader = command.ExecuteReader())
            {

                if (reader.HasRows)
                {
                    reader.Read();
                    int userId = Convert.ToInt32(reader["userId"].ToString());

                    string session = sessionGen();
                    int currentTime = Time.getSeconds();

                    sql = String.Format("UPDATE Users SET userSession = '{0}', userLauncherSession = '{0}', userSessionTime={2} WHERE userId = {1}", session, userId, currentTime);
                    using (MySqlCommand command2 = new MySqlCommand(sql, connection2))
                    {
                        command2.ExecuteNonQuery();
                    }

                    outPacket = String.Format("<success><ID>{0}</ID><session>{1}</session></success>", userId, session);
                    isGoodResponse = true;
                }
                else
                {
                    outPacket = String.Format("<fail><message>Wrong login or password</message></fail>");
                    isGoodResponse = true;
                }
            }

            connection2.Close();
        }
        public static void GetPermanentSession(int userId, string session, MySqlConnection connection, out string outPacket, out bool isGoodResponse )
        {
            MySqlConnection connection2 = new MySqlConnection(MsSQL.connetionString);
            connection2.Open();

            string profileData = File.ReadAllText(Program.serverPrefix + "/Engine.svc/User/ProfileData.xml");

            string profilePacket = "";

            string sql = String.Format("SELECT * FROM Persona WHERE userId = {0}", userId);
            if(userId == 1)
                sql = String.Format("SELECT * FROM Persona", userId);
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read()) {

                        object[] userData = new object[9];
                        userData[0] = 10000; //reader["boost"];
                        userData[1] = reader["cash"];
                        userData[2] = reader["iconIndex"];
                        userData[3] = reader["level"];
                        userData[4] = reader["motto"];
                        userData[5] = reader["name"];
                        userData[6] = reader["personaId"];
                        userData[7] = 1000;
                        userData[8] = reader["rep"];

                        profilePacket = profilePacket + string.Format(profileData, userData);
                    }
                }
            }

            sql = String.Format("SELECT * FROM Users WHERE Users.userId={0} AND Users.userSession='{1}'", userId, session);
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    session = sessionGen();
                    int currentTime = Time.getSeconds();

                    reader.Read();
                    object[] userData = new object[3];
                    userData[0] = profilePacket;
                    userData[2] = session;
                    userData[1] = userId;

                    sql = String.Format("UPDATE Users SET userSession = '{0}', userSessionTime={2} WHERE userId = {1}", session, userId, currentTime);
                    using (MySqlCommand command2 = new MySqlCommand(sql, connection2))
                    {
                        command2.ExecuteNonQuery();
                    }

                    string packet = File.ReadAllText(Program.serverPrefix + "/Engine.svc/User/GetPermanentSession_template.xml");

                    outPacket = String.Format(packet, userData);
                    isGoodResponse = true;
                }
                else
                {
                    outPacket = String.Format("fail");
                    isGoodResponse = true;
                }
            }

            connection2.Close();
        }
        public static bool IsAuthorizated(int userId, string session, MySqlConnection connection)
        {
            int sessionTime = 0;

            string sql = String.Format("SELECT userSessionTime FROM Users WHERE userId={0} AND userSession='{1}'", userId, session);

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    sessionTime = Convert.ToInt32(reader["userSessionTime"].ToString());
                }
                else
                {
                    return false;
                }
            }


            if (Time.sessionLife(sessionTime))
            {
                /*int currentTime = Time.getSeconds();

                sql = String.Format("UPDATE Users SET userSessionTime={0} WHERE userId = {1}", currentTime, userId);

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }*/

                return true;
            }
            else
            {
                return false;
            }

        }

        private static string sessionGen()
        {
            string session = StringGen.Generate(36);
            StringBuilder sessionArr = new StringBuilder(session);

            sessionArr[8] = '-';
            sessionArr[13] = '-';
            sessionArr[18] = '-';
            sessionArr[23] = '-';
            
            return sessionArr.ToString();
        }
    }
}
