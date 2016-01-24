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

namespace FinalServerSolution
{
    class Friends
    {
        public static void getFriendList(int userId, MySqlConnection connection, out string outPacket, out bool isGoodResponse)
        {
            string FriendPersonaBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/Friends/FriendPersona.xml");
            string FriendListBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/Friends/getfriendlistfromuserid.xml");
            string temp = "";

            string sql = String.Format("SELECT * FROM FriendList LEFT JOIN Persona ON FriendList.friendUserId = Persona.userId WHERE FriendList.userId = {0} ORDER BY Persona.name ASC", userId);
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    
                    while (reader.Read())
                    {
                        object[] friendData = new object[14];
                        friendData[0] = reader["iconIndex"];
                        friendData[1] = reader["level"];
                        friendData[2] = reader["name"];

                        friendData[3] = reader["personaId"];
                        friendData[4] = 0;
                        friendData[5] = 0;
                        friendData[6] = reader["userId"];

                        temp = temp + string.Format(FriendPersonaBody, friendData);
                    }
                }
            }

            outPacket = string.Format(FriendListBody, temp);

            isGoodResponse = true;

        }
        public static void addFriend(int personaId, int userId, string friendName, MySqlConnection connection, out string outPacket, out bool isGoodResponse)
        {
            int friendUserId = 0;
            string sql = String.Format("SELECT userId FROM Persona WHERE name = '{0}' ", friendName);
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    friendUserId = Convert.ToInt32(reader["userId"]);
                }
            }
            if (friendUserId != 0)
            {
                sql = String.Format("INSERT INTO FriendList (userId, friendUserId, friendStatus) VALUES ({0}, {1}, {2}) ", userId, friendUserId, 0);
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }

            string FriendPersonaBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/Friends/add.xml");
            //string FriendListBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/Friends/getfriendlistfromuserid.xml");
            string temp = "";

            sql = String.Format("SELECT * FROM FriendList LEFT JOIN Persona ON FriendList.friendUserId = Persona.userId WHERE FriendList.userId = {0} AND FriendList.friendUserId = {1} ORDER BY Persona.name ASC", userId, friendUserId);
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {

                    while (reader.Read())
                    {
                        object[] friendData = new object[14];
                        friendData[0] = reader["iconIndex"];
                        friendData[1] = reader["level"];
                        friendData[2] = reader["name"];

                        friendData[3] = reader["personaId"];
                        friendData[4] = 0;
                        friendData[5] = 0;
                        friendData[6] = reader["userId"];

                        temp = temp + string.Format(FriendPersonaBody, friendData);
                    }
                }
            }

            //outPacket = string.Format(FriendListBody, temp);
            outPacket = temp;
            isGoodResponse = true;
        }
        public static void removeFriend(int personaId, int userId, int friendPersonaId, MySqlConnection connection, out string outPacket, out bool isGoodResponse)
        {
            int friendUserId = 0;
            bool isYourPersona = false;

            string sql = String.Format("SELECT userId FROM Persona WHERE personaId = {0} AND userId = {1} ", personaId, userId);
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    isYourPersona = true; 
                }
            }
            if (isYourPersona)
            {
                sql = String.Format("SELECT userId FROM Persona WHERE personaId = {0}", friendPersonaId);
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        friendUserId = Convert.ToInt32(reader["userId"]);
                    }
                }

                if (friendUserId != 0)
                {
                    sql = String.Format("DELETE FROM FriendList WHERE userId = {0} AND friendUserId = {1}", userId, friendUserId);
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }

            string FriendPersonaBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/Friends/FriendPersona.xml");
            string FriendListBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/Friends/getfriendlistfromuserid.xml");
            string temp = "";

            sql = String.Format("SELECT * FROM FriendList LEFT JOIN Persona ON FriendList.friendUserId = Persona.userId WHERE FriendList.userId = {0} ORDER BY Persona.name ASC", userId);
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {

                    while (reader.Read())
                    {
                        object[] friendData = new object[14];
                        friendData[0] = reader["iconIndex"];
                        friendData[1] = reader["level"];
                        friendData[2] = reader["name"];

                        friendData[3] = reader["personaId"];
                        friendData[4] = 0;
                        friendData[5] = 0;
                        friendData[6] = reader["userId"];

                        temp = temp + string.Format(FriendPersonaBody, friendData);
                    }
                }
            }

            outPacket = string.Format(FriendListBody, temp);

            isGoodResponse = true;
        }
    }
}
