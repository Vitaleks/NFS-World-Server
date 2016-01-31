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
    class Tracks
    {
        public static void singleRace(int raceId, MySqlConnection connection, out string outPacket, out bool isGoodResponse)
        {
            string packetBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/matchmaking/launchevent/template.xml");
            
            string session = StringGen.Generate(20);

            string sql = String.Format("INSERT INTO EventSession (trackId, sessionType, challangeId) VALUES ({0},'{1}','{2}')", raceId, "singleplayer", session);
            
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.ExecuteNonQuery();
            }

            sql = String.Format("SELECT sessionId FROM EventSession WHERE challangeId = '{0}' ORDER BY sessionId DESC LIMIT 1", session);
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    string sessionId = reader["sessionId"].ToString();

                    outPacket = string.Format(packetBody, session, raceId, sessionId);
                    isGoodResponse = true;
                }
                else
                {
                    outPacket = "";
                    isGoodResponse = false;
                }

            }
        }
        public static void completedRace(String packet, int userId, MySqlConnection connection, out string outPacket, out bool isGoodResponse)
        {
            XmlDocument packetParsing = new XmlDocument();
            packetParsing.LoadXml(packet);
            int personaId = 0;
            int sessionId = 0;
            int trackId = 0;
            string heat = "1";
            int time = Time.getSeconds();

            string challangeId = packetParsing.ChildNodes[0].ChildNodes[8].ChildNodes[0].InnerText;

            string raceType = packetParsing.ChildNodes[0].Name.Replace("ArbitrationPacket", "");
            int carId = Convert.ToInt32(packetParsing.ChildNodes[0].ChildNodes[1].InnerText);


            string sql = String.Format("SELECT personaNumber FROM Users WHERE userId='{0}'", userId);
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                reader.Read();
                personaId = Convert.ToInt32(reader["personaNumber"]);
            }

            sql = String.Format("SELECT personaId FROM Persona WHERE userId='{0}' ORDER BY personaId ASC", userId);
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                int index = 0;
                while (reader.Read())
                {
                    if (personaId == index)
                    {
                        personaId = Convert.ToInt32(reader["personaId"]);
                        break;
                    }
                    index++;
                }
            }

            sql = String.Format("SELECT sessionId, trackId FROM EventSession WHERE challangeId='{0}'", challangeId);
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    sessionId = Convert.ToInt32(reader["sessionId"]);
                    trackId = Convert.ToInt32(reader["trackId"]);
                }
            }

            if (raceType == "Route")
            {
                Object[] data = new Object[21];
                data[0] = packetParsing.ChildNodes[0].ChildNodes[0].InnerText;
                data[1] = packetParsing.ChildNodes[0].ChildNodes[1].InnerText;
                data[2] = packetParsing.ChildNodes[0].ChildNodes[2].InnerText;
                data[3] = packetParsing.ChildNodes[0].ChildNodes[3].InnerText;

                data[4] = packetParsing.ChildNodes[0].ChildNodes[7].InnerText;

                data[5] = packetParsing.ChildNodes[0].ChildNodes[9].InnerText;
                data[6] = packetParsing.ChildNodes[0].ChildNodes[10].InnerText;
                data[7] = packetParsing.ChildNodes[0].ChildNodes[11].InnerText;
                data[8] = packetParsing.ChildNodes[0].ChildNodes[12].InnerText;
                data[9] = packetParsing.ChildNodes[0].ChildNodes[13].InnerText;
                data[10] = packetParsing.ChildNodes[0].ChildNodes[14].InnerText;
                data[11] = packetParsing.ChildNodes[0].ChildNodes[15].InnerText;

                data[12] = packetParsing.ChildNodes[0].ChildNodes[6].ChildNodes[0].InnerText;
                data[13] = packetParsing.ChildNodes[0].ChildNodes[6].ChildNodes[1].InnerText;
                data[14] = packetParsing.ChildNodes[0].ChildNodes[6].ChildNodes[2].InnerText;
                data[15] = packetParsing.ChildNodes[0].ChildNodes[6].ChildNodes[3].InnerText;
                data[16] = packetParsing.ChildNodes[0].ChildNodes[6].ChildNodes[4].InnerText;
                data[17] = packetParsing.ChildNodes[0].ChildNodes[6].ChildNodes[5].InnerText;

                data[18] = personaId;
                data[19] = sessionId;
                data[20] = time;

                sql = String.Format(@"INSERT INTO EventSessionResults (
                                                                            alternateEventDurationInMilliseconds,
                                                                            carId, 
                                                                            eventDurationInMilliseconds, 
                                                                            finishReason,

                                                                            rank,

                                                                            bestLapDurationInMilliseconds,
                                                                            fractionCompleted, 
                                                                            longestJumpDurationInMilliseconds, 
                                                                            numberOfCollisions,
                                                                            perfectStart,
                                                                            sumOfJumpsDurationInMilliseconds,
                                                                            topSpeed, 

                                                                            accelerationAverge,
                                                                            accelerationMaximum,
                                                                            accelerationMedian,
                                                                            speedAverage,
                                                                            speedMaximum,
                                                                            speedMedian,

                                                                            personaId, 
                                                                            sessionId,
                                                                            finishTime
                                                                            ) VALUES ({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20})", data);

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
            else if (raceType == "Drag")
            {
                Object[] data = new Object[20];
                data[0] = packetParsing.ChildNodes[0].ChildNodes[0].InnerText;
                data[1] = packetParsing.ChildNodes[0].ChildNodes[1].InnerText;
                data[2] = packetParsing.ChildNodes[0].ChildNodes[2].InnerText;
                data[3] = packetParsing.ChildNodes[0].ChildNodes[3].InnerText;

                data[4] = packetParsing.ChildNodes[0].ChildNodes[7].InnerText;

                data[5] = packetParsing.ChildNodes[0].ChildNodes[9].InnerText;
                data[6] = packetParsing.ChildNodes[0].ChildNodes[10].InnerText;
                data[7] = packetParsing.ChildNodes[0].ChildNodes[11].InnerText;
                data[8] = packetParsing.ChildNodes[0].ChildNodes[12].InnerText;
                data[9] = packetParsing.ChildNodes[0].ChildNodes[13].InnerText;
                data[10] = packetParsing.ChildNodes[0].ChildNodes[14].InnerText;

                data[11] = packetParsing.ChildNodes[0].ChildNodes[6].ChildNodes[0].InnerText;
                data[12] = packetParsing.ChildNodes[0].ChildNodes[6].ChildNodes[1].InnerText;
                data[13] = packetParsing.ChildNodes[0].ChildNodes[6].ChildNodes[2].InnerText;
                data[14] = packetParsing.ChildNodes[0].ChildNodes[6].ChildNodes[3].InnerText;
                data[15] = packetParsing.ChildNodes[0].ChildNodes[6].ChildNodes[4].InnerText;
                data[16] = packetParsing.ChildNodes[0].ChildNodes[6].ChildNodes[5].InnerText;

                data[17] = personaId;
                data[18] = sessionId;
                data[19] = time;

                sql = String.Format(@"INSERT INTO EventSessionResults (
                                                                            alternateEventDurationInMilliseconds,
                                                                            carId, 
                                                                            eventDurationInMilliseconds, 
                                                                            finishReason,

                                                                            rank,

                                                                            fractionCompleted, 
                                                                            longestJumpDurationInMilliseconds, 
                                                                            numberOfCollisions,
                                                                            perfectStart,
                                                                            sumOfJumpsDurationInMilliseconds,
                                                                            topSpeed,
 
                                                                            accelerationAverge,
                                                                            accelerationMaximum,
                                                                            accelerationMedian,
                                                                            speedAverage,
                                                                            speedMaximum,
                                                                            speedMedian,
                                                                            personaId, 
                                                                            sessionId,
                                                                            finishTime 
                                                                            ) VALUES ({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19})", data);

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
            else if (raceType == "Pursuit")
            {
                Object[] data = new Object[24];
                data[0] = packetParsing.ChildNodes[0].ChildNodes[0].InnerText;
                data[1] = packetParsing.ChildNodes[0].ChildNodes[1].InnerText;
                data[2] = packetParsing.ChildNodes[0].ChildNodes[2].InnerText;
                data[3] = packetParsing.ChildNodes[0].ChildNodes[3].InnerText;

                data[4] = packetParsing.ChildNodes[0].ChildNodes[7].InnerText;

                data[5] = packetParsing.ChildNodes[0].ChildNodes[9].InnerText;
                data[6] = packetParsing.ChildNodes[0].ChildNodes[10].InnerText;
                data[7] = packetParsing.ChildNodes[0].ChildNodes[11].InnerText;
                data[8] = packetParsing.ChildNodes[0].ChildNodes[12].InnerText;
                data[9] = packetParsing.ChildNodes[0].ChildNodes[14].InnerText;
                data[10] = packetParsing.ChildNodes[0].ChildNodes[15].InnerText;
                data[11] = packetParsing.ChildNodes[0].ChildNodes[16].InnerText;
                data[12] = packetParsing.ChildNodes[0].ChildNodes[17].InnerText;
                data[13] = packetParsing.ChildNodes[0].ChildNodes[18].InnerText;
                data[14] = packetParsing.ChildNodes[0].ChildNodes[19].InnerText;

                data[15] = packetParsing.ChildNodes[0].ChildNodes[6].ChildNodes[0].InnerText;
                data[16] = packetParsing.ChildNodes[0].ChildNodes[6].ChildNodes[1].InnerText;
                data[17] = packetParsing.ChildNodes[0].ChildNodes[6].ChildNodes[2].InnerText;
                data[18] = packetParsing.ChildNodes[0].ChildNodes[6].ChildNodes[3].InnerText;
                data[19] = packetParsing.ChildNodes[0].ChildNodes[6].ChildNodes[4].InnerText;
                data[20] = packetParsing.ChildNodes[0].ChildNodes[6].ChildNodes[5].InnerText;

                data[21] = personaId;
                data[22] = sessionId;
                data[23] = time;

                sql = String.Format(@"INSERT INTO EventSessionResults (
                                                                            alternateEventDurationInMilliseconds,
                                                                            carId, 
                                                                            eventDurationInMilliseconds, 
                                                                            finishReason,

                                                                            rank,
                                                                            copsDeployed,
                                                                            copsDisabled,
                                                                            copsRammed,
                                                                            costToState,
                                                                            infractions,
                                                                            longestJumpDurationInMilliseconds, 
                                                                            roadBlocksDodged,
                                                                            spikeStripsDodged,
                                                                            sumOfJumpsDurationInMilliseconds,
                                                                            topSpeed,
 
                                                                            accelerationAverge,
                                                                            accelerationMaximum,
                                                                            accelerationMedian,
                                                                            speedAverage,
                                                                            speedMaximum,
                                                                            speedMedian,
                                                                            personaId, 
                                                                            sessionId,
                                                                            finishTime 
                                                                            ) VALUES ({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23})", data);

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }

                heat = packetParsing.ChildNodes[0].ChildNodes[13].InnerText;

                if (data[3].ToString() == "266")
                    heat = "1";

                sql = String.Format("UPDATE PersonaCar SET heat = {0} WHERE carId = {1} AND personaId = {2}", heat, carId, personaId);
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }

            int durability = 100;
            sql = String.Format("SELECT durability FROM PersonaCar WHERE personaId='{0}' AND carId = {1}", personaId, carId);
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                reader.Read();
                durability = Convert.ToInt32(reader["durability"]);
            }

            durability = durability - 10;
            if (durability < 0) durability = 0;

            sql = String.Format("UPDATE PersonaCar SET durability = {2} WHERE carId = {0} AND personaId = {1}", carId, personaId, durability);
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.ExecuteNonQuery();
            }

            Object[] arguments = new Object[6];
            arguments[0] = raceType;
            arguments[1] = durability;
            arguments[2] = trackId;
            arguments[3] = (raceType == "Pursuit" ? sessionId : sessionId + 1);
            arguments[4] = personaId;
            arguments[5] = (raceType == "Pursuit" ? "<Heat>" + heat + "</Heat>" : "");
            string arbitrationBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/event/arbitration.xml");

            outPacket = String.Format(arbitrationBody, arguments);
            isGoodResponse = true;

            File.WriteAllText("Race.xml", outPacket);
        }
    }
}
