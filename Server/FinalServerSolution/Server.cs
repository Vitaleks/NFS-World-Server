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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FinalServerSolution
{
    class Server
    {
        public static void IsOnline(out string outPacket, out bool isGoodResponse)
        {
            outPacket = "<status>ONLINE</status>";
            isGoodResponse = true;
        }

        public static void ContinueSession(string packet, MySqlConnection connection, out string outPacket, out bool isGoodResponse)
        {
            int sessionTime = 0;

            XmlDocument packetParsing = new XmlDocument();
            packetParsing.LoadXml(packet);
            string userId = "0";
            string session = "0";

            foreach (XmlNode key in packetParsing.FirstChild.ChildNodes)
            {
                if (key.Name == "Id")
                    userId = key.InnerText;
                if (key.Name == "Session")
                    session = key.InnerText;
            }

            string sql = String.Format("SELECT userSessionTime FROM Users WHERE userId='{0}' AND userLauncherSession='{1}'", userId, session);

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
                    sessionTime = 0;
                }
            }


            if (Time.sessionLife(sessionTime))
            {
                int currentTime = Time.getSeconds();

                sql = String.Format("UPDATE Users SET userSessionTime={0} WHERE userId = {1}", currentTime, userId);

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }

                outPacket = "<status>ONLINE</status>";
                isGoodResponse = true;
            }
            else
            {
                outPacket = "<status>NOSESSION</status>";
                isGoodResponse = true;
            }
        }
    }
}
