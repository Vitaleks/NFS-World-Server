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
    class Cars
    {
        public static void getPersonasCars(int personaId, MySqlConnection connection, out string outPacket, out bool isGoodResponse)
        {
            MySqlConnection connection2 = new MySqlConnection(MsSQL.connetionString);
            connection2.Open();

            object[] arguments = new object[4];

            string packetBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/carslotsBody.xml");

            string cars = "";

            string sql = String.Format("SELECT * FROM PersonaCar WHERE personaId = {0} ORDER BY carId ASC", personaId);
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    string carBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/car.xml");
                    string vinylBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/vinyl.xml");
                    string visualPartBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/visualpart.xml");
                    string skillBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/skill.xml");
                    string performanceBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/performance.xml");
                    string paintBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/paint.xml");

                    while (reader.Read())
                    {
                        object[] carData = new object[14];
                        carData[0] = reader["baseCar"];
                        carData[1] = reader["carClassHash"];
                        carData[2] = reader["carId"];

                        carData[5] = reader["physicsProfileHash"];
                        carData[6] = reader["rating"];
                        carData[7] = reader["resalePrice"];

                        carData[11] = reader["durability"];
                        carData[12] = reader["heat"];
                        carData[13] = reader["carId"];

                        string vinylTemp = "";
                        string visualPartsTemp = "";
                        string skillTemp = "";
                        string paintTemp = "";
                        string performanceTemp = "";

                        string sql2 = "";

                        sql2 = String.Format("SELECT * FROM PersonaCarVinyls WHERE carId = {0} ORDER BY layer ASC", carData[2]);
                        using (MySqlCommand command2 = new MySqlCommand(sql2, connection2))
                        using (MySqlDataReader reader2 = command2.ExecuteReader())
                        {

                            if (reader2.HasRows)
                            {
                                while (reader2.Read())
                                {
                                    object[] vinylData = new object[21];
                                    vinylData[0] = reader2["vinylHash"];
                                    vinylData[1] = reader2["hue1"];
                                    vinylData[2] = reader2["hue2"];
                                    vinylData[3] = reader2["hue3"];
                                    vinylData[4] = reader2["hue4"];
                                    vinylData[5] = reader2["layer"];
                                    vinylData[6] = (Convert.ToInt32(reader2["mir"]) == 1 ? "true" : "false");
                                    vinylData[7] = reader2["rot"];
                                    vinylData[8] = reader2["sat1"];
                                    vinylData[9] = reader2["sat2"];
                                    vinylData[10] = reader2["sat3"];
                                    vinylData[11] = reader2["sat4"];
                                    vinylData[12] = reader2["scaleX"];
                                    vinylData[13] = reader2["scaleY"];
                                    vinylData[14] = reader2["shear"];
                                    vinylData[15] = reader2["tranX"];
                                    vinylData[16] = reader2["tranY"];
                                    vinylData[17] = reader2["var1"];
                                    vinylData[18] = reader2["var2"];
                                    vinylData[19] = reader2["var3"];
                                    vinylData[20] = reader2["var4"];

                                    vinylTemp = vinylTemp + String.Format(vinylBody, vinylData);
                                }
                            }
                        }

                        sql2 = String.Format("SELECT * FROM PersonaCarPaints WHERE carId = {0} ORDER BY slot ASC", carData[2]);
                        using (MySqlCommand command2 = new MySqlCommand(sql2, connection2))
                        using (MySqlDataReader reader2 = command2.ExecuteReader())
                        {

                            if (reader2.HasRows)
                            {
                                while (reader2.Read())
                                {
                                    object[] paintData = new object[5];
                                    paintData[0] = reader2["grp"]; //group
                                    paintData[1] = reader2["hue"];
                                    paintData[2] = reader2["sat"];
                                    paintData[3] = reader2["slot"];
                                    paintData[4] = reader2["var"];

                                    paintTemp = paintTemp + String.Format(paintBody, paintData);
                                }

                            }
                        }

                        sql2 = String.Format("SELECT * FROM PersonaCarPerformanceParts WHERE carId = {0} ORDER BY slot ASC", carData[2]);
                        using (MySqlCommand command2 = new MySqlCommand(sql2, connection2))
                        using (MySqlDataReader reader2 = command2.ExecuteReader())
                        {

                            if (reader2.HasRows)
                            {
                                while (reader2.Read())
                                {
                                    object[] performanceData = new object[1];
                                    performanceData[0] = reader2["partHash"];

                                    performanceTemp = performanceTemp + String.Format(performanceBody, performanceData);
                                }
                            }
                        }

                        sql2 = String.Format("SELECT * FROM PersonaCarSkills WHERE carId = {0} ORDER BY slot ASC", carData[2]);
                        using (MySqlCommand command2 = new MySqlCommand(sql2, connection2))
                        using (MySqlDataReader reader2 = command2.ExecuteReader())
                        {

                            if (reader2.HasRows)
                            {
                                while (reader2.Read())
                                {
                                    object[] skillData = new object[1];
                                    skillData[0] = reader2["skillHash"];

                                    skillTemp = skillTemp + String.Format(skillBody, skillData);
                                }

                            }
                        }

                        sql2 = String.Format("SELECT * FROM PersonaCarVisualParts WHERE carId = {0} ", carData[2]);
                        using (MySqlCommand command2 = new MySqlCommand(sql2, connection2))
                        using (MySqlDataReader reader2 = command2.ExecuteReader())
                        {

                            if (reader2.HasRows)
                            {
                                while (reader2.Read())
                                {
                                    object[] visualData = new object[2];
                                    visualData[0] = reader2["partHash"];
                                    visualData[1] = reader2["slotHash"];

                                    visualPartsTemp = visualPartsTemp + String.Format(visualPartBody, visualData);
                                }

                            }
                        }
                        carData[3] = paintTemp;
                        carData[4] = performanceTemp;
                        carData[8] = skillTemp;
                        carData[9] = vinylTemp;
                        carData[10] = visualPartsTemp;

                        cars = cars + String.Format(carBody, carData);
                    }
                }
            }

            string carIndex = "";
            {
                string sql2 = String.Format("SELECT defaultCarIndex FROM Persona WHERE personaId = {0} ", personaId);
                using (MySqlCommand command2 = new MySqlCommand(sql2, connection2))
                using (MySqlDataReader reader2 = command2.ExecuteReader())
                {
                    if (reader2.HasRows)
                    {
                        while (reader2.Read())
                        {
                            carIndex = reader2["defaultCarIndex"].ToString();
                        }
                    }
                }
            }
            arguments[0] = cars;
            arguments[1] = carIndex;

            outPacket = String.Format(packetBody, arguments);
            isGoodResponse = true;

            connection2.Close();
        }
        public static void getOtherPersonasCars(int personaId, MySqlConnection connection, out string outPacket, out bool isGoodResponse)
        {
            MySqlConnection connection2 = new MySqlConnection(MsSQL.connetionString);
            connection2.Open();

            object[] arguments = new object[4];

            string packetBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/carslotsBody.xml");

            string cars = "";

            string sql = String.Format("SELECT * FROM PersonaCar WHERE personaId = {0} ORDER BY carId ASC", personaId);
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    string carBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/car.xml");
                    string vinylBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/vinyl.xml");
                    string visualPartBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/visualpart.xml");
                    string skillBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/skill.xml");
                    string performanceBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/performance.xml");
                    string paintBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/paint.xml");

                    while (reader.Read())
                    {
                        object[] carData = new object[14];
                        carData[0] = reader["baseCar"];
                        carData[1] = reader["carClassHash"];
                        carData[2] = reader["carId"];

                        carData[5] = reader["physicsProfileHash"];
                        carData[6] = reader["rating"];
                        carData[7] = reader["resalePrice"];

                        carData[11] = reader["durability"];
                        carData[12] = reader["heat"];
                        carData[13] = reader["carId"];

                        string vinylTemp = "";
                        string visualPartsTemp = "";
                        string skillTemp = "";
                        string paintTemp = "";
                        string performanceTemp = "";

                        string sql2 = "";

                        sql2 = String.Format("SELECT * FROM PersonaCarVinyls WHERE carId = {0} ORDER BY layer ASC", carData[2]);
                        using (MySqlCommand command2 = new MySqlCommand(sql2, connection2))
                        using (MySqlDataReader reader2 = command2.ExecuteReader())
                        {

                            if (reader2.HasRows)
                            {
                                while (reader2.Read())
                                {
                                    object[] vinylData = new object[21];
                                    vinylData[0] = reader2["vinylHash"];
                                    vinylData[1] = reader2["hue1"];
                                    vinylData[2] = reader2["hue2"];
                                    vinylData[3] = reader2["hue3"];
                                    vinylData[4] = reader2["hue4"];
                                    vinylData[5] = reader2["layer"];
                                    vinylData[6] = (Convert.ToInt32(reader2["mir"]) == 1 ? "true" : "false");
                                    vinylData[7] = reader2["rot"];
                                    vinylData[8] = reader2["sat1"];
                                    vinylData[9] = reader2["sat2"];
                                    vinylData[10] = reader2["sat3"];
                                    vinylData[11] = reader2["sat4"];
                                    vinylData[12] = reader2["scaleX"];
                                    vinylData[13] = reader2["scaleY"];
                                    vinylData[14] = reader2["shear"];
                                    vinylData[15] = reader2["tranX"];
                                    vinylData[16] = reader2["tranY"];
                                    vinylData[17] = reader2["var1"];
                                    vinylData[18] = reader2["var2"];
                                    vinylData[19] = reader2["var3"];
                                    vinylData[20] = reader2["var4"];

                                    vinylTemp = vinylTemp + String.Format(vinylBody, vinylData);
                                }
                            }
                        }

                        sql2 = String.Format("SELECT * FROM PersonaCarPaints WHERE carId = {0} ORDER BY slot ASC", carData[2]);
                        using (MySqlCommand command2 = new MySqlCommand(sql2, connection2))
                        using (MySqlDataReader reader2 = command2.ExecuteReader())
                        {

                            if (reader2.HasRows)
                            {
                                while (reader2.Read())
                                {
                                    object[] paintData = new object[5];
                                    paintData[0] = reader2["grp"]; //group
                                    paintData[1] = reader2["hue"];
                                    paintData[2] = reader2["sat"];
                                    paintData[3] = reader2["slot"];
                                    paintData[4] = reader2["var"];

                                    paintTemp = paintTemp + String.Format(paintBody, paintData);
                                }

                            }
                        }

                        sql2 = String.Format("SELECT * FROM PersonaCarPerformanceParts WHERE carId = {0} ORDER BY slot ASC", carData[2]);
                        using (MySqlCommand command2 = new MySqlCommand(sql2, connection2))
                        using (MySqlDataReader reader2 = command2.ExecuteReader())
                        {

                            if (reader2.HasRows)
                            {
                                while (reader2.Read())
                                {
                                    object[] performanceData = new object[1];
                                    performanceData[0] = reader2["partHash"];

                                    performanceTemp = performanceTemp + String.Format(performanceBody, performanceData);
                                }
                            }
                        }

                        sql2 = String.Format("SELECT * FROM PersonaCarSkills WHERE carId = {0} ORDER BY slot ASC", carData[2]);
                        using (MySqlCommand command2 = new MySqlCommand(sql2, connection2))
                        using (MySqlDataReader reader2 = command2.ExecuteReader())
                        {

                            if (reader2.HasRows)
                            {
                                while (reader2.Read())
                                {
                                    object[] skillData = new object[1];
                                    skillData[0] = reader2["skillHash"];

                                    skillTemp = skillTemp + String.Format(skillBody, skillData);
                                }

                            }
                        }

                        sql2 = String.Format("SELECT * FROM PersonaCarVisualParts WHERE carId = {0} ", carData[2]);
                        using (MySqlCommand command2 = new MySqlCommand(sql2, connection2))
                        using (MySqlDataReader reader2 = command2.ExecuteReader())
                        {

                            if (reader2.HasRows)
                            {
                                while (reader2.Read())
                                {
                                    object[] visualData = new object[2];
                                    visualData[0] = reader2["partHash"];
                                    visualData[1] = reader2["slotHash"];

                                    visualPartsTemp = visualPartsTemp + String.Format(visualPartBody, visualData);
                                }

                            }
                        }
                        carData[3] = paintTemp;
                        carData[4] = performanceTemp;
                        carData[8] = skillTemp;
                        carData[9] = vinylTemp;
                        carData[10] = visualPartsTemp;

                        cars = cars + String.Format(carBody, carData);
                    }
                }
            }

            string carIndex = "";
            {
                string sql2 = String.Format("SELECT defaultCarIndex FROM Persona WHERE personaId = {0} ", personaId);
                using (MySqlCommand command2 = new MySqlCommand(sql2, connection2))
                using (MySqlDataReader reader2 = command2.ExecuteReader())
                {
                    if (reader2.HasRows)
                    {
                        while (reader2.Read())
                        {
                            carIndex = reader2["defaultCarIndex"].ToString();
                        }
                    }
                }
            }
            arguments[0] = cars;
            arguments[1] = carIndex;

            //outPacket = String.Format(packetBody, arguments);

            outPacket = cars;

            isGoodResponse = true;

            connection2.Close();
        }
        public static void updatePersonaCar(int personaId, int userId, string packet, MySqlConnection connection, out string outPacket, out bool isGoodResponse)
        {
            MySqlConnection connection2 = new MySqlConnection(MsSQL.connetionString);
            connection2.Open();

            XmlDocument packetParsing = new XmlDocument();
            packetParsing.LoadXml(packet);
            int carId = 0;

            var customCar = packetParsing.ChildNodes[0].ChildNodes[2].ChildNodes[0];

            carId = Convert.ToInt32(customCar.ChildNodes[2].InnerText);

            PersonaCar carOrigData = new PersonaCar();

            bool personaHasCar = false;

            string sql = String.Format("SELECT PersonaCar.carId, PersonaCar.baseCar, PersonaCar.physicsProfileHash, PersonaCar.carClassHash, PersonaCar.rating, PersonaCar.resalePrice, PersonaCar.durability, PersonaCar.heat FROM PersonaCar LEFT JOIN Persona ON PersonaCar.personaId = Persona.personaId LEFT JOIN Users ON Persona.userId = Users.userId WHERE Users.userId = {0} AND Persona.personaId = {1} AND PersonaCar.carId = {2}", userId, personaId, carId);

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    personaHasCar = true;
                    reader.Read();

                    carOrigData.baseCar = Convert.ToInt32(reader["baseCar"]);
                    carOrigData.physicsProfileHash = Convert.ToInt32(reader["physicsProfileHash"]);
                    carOrigData.carClassHash = Convert.ToInt32(reader["carClassHash"]);
                    carOrigData.rating = Convert.ToInt32(reader["rating"]);
                    carOrigData.resalePrice = Convert.ToInt32(reader["resalePrice"]);
                    carOrigData.durability = Convert.ToInt32(reader["durability"]);
                    carOrigData.heat = Convert.ToInt32(reader["heat"]);
                }
            }

            List<Paints> paints = new List<Paints>();
            List<PerformanceParts> performanceParts = new List<PerformanceParts>();
            List<Skills> skills = new List<Skills>();
            List<VisualParts> visualParts = new List<VisualParts>();
            List<Vinyls> vinyls = new List<Vinyls>();

            string car = "";

            if (personaHasCar)
            {
                //Paints
                foreach (XmlNode node in customCar.ChildNodes[6])
                {
                    Paints temp = new Paints();

                    temp.group = Convert.ToInt32(node.ChildNodes[0].InnerText);
                    temp.hue = Convert.ToInt32(node.ChildNodes[1].InnerText);
                    temp.sat = Convert.ToInt32(node.ChildNodes[2].InnerText);
                    temp.slot = Convert.ToInt32(node.ChildNodes[3].InnerText);
                    temp.var = Convert.ToInt32(node.ChildNodes[4].InnerText);

                    paints.Add(temp);
                }
                //PerformanceParts

                int slotIndex = 0;
                foreach (XmlNode node in customCar.ChildNodes[7])
                {
                    PerformanceParts temp = new PerformanceParts();

                    temp.slot = slotIndex;
                    temp.partHash = Convert.ToInt32(node.ChildNodes[0].InnerText);

                    slotIndex++;

                    performanceParts.Add(temp);
                }
                //Skills

                slotIndex = 0;
                foreach (XmlNode node in customCar.ChildNodes[12])
                {
                    Skills temp = new Skills();

                    temp.slot = slotIndex;
                    temp.skillHash = Convert.ToInt32(node.ChildNodes[1].InnerText);

                    slotIndex++;

                    skills.Add(temp);
                }
                //Vinyls

                foreach (XmlNode node in customCar.ChildNodes[15])
                {
                    Vinyls temp = new Vinyls();

                    temp.layer = Convert.ToInt32(node.ChildNodes[5].InnerText);

                    temp.vinylHash = Convert.ToInt32(node.ChildNodes[0].InnerText);
                    temp.hue1 = Convert.ToInt32(node.ChildNodes[1].InnerText);
                    temp.hue2 = Convert.ToInt32(node.ChildNodes[2].InnerText);
                    temp.hue3 = Convert.ToInt32(node.ChildNodes[3].InnerText);
                    temp.hue4 = Convert.ToInt32(node.ChildNodes[4].InnerText);

                    temp.mir = Convert.ToBoolean(node.ChildNodes[6].InnerText);
                    temp.rot = Convert.ToInt32(node.ChildNodes[7].InnerText);

                    temp.sat1 = Convert.ToInt32(node.ChildNodes[8].InnerText);
                    temp.sat2 = Convert.ToInt32(node.ChildNodes[9].InnerText);
                    temp.sat3 = Convert.ToInt32(node.ChildNodes[10].InnerText);
                    temp.sat4 = Convert.ToInt32(node.ChildNodes[11].InnerText);

                    temp.scaleX = Convert.ToInt32(node.ChildNodes[12].InnerText);
                    temp.scaleY = Convert.ToInt32(node.ChildNodes[13].InnerText);
                    temp.shear = Convert.ToInt32(node.ChildNodes[14].InnerText);
                    temp.tranX = Convert.ToInt32(node.ChildNodes[15].InnerText);
                    temp.tranY = Convert.ToInt32(node.ChildNodes[16].InnerText);

                    temp.var1 = Convert.ToInt32(node.ChildNodes[17].InnerText);
                    temp.var2 = Convert.ToInt32(node.ChildNodes[18].InnerText);
                    temp.var3 = Convert.ToInt32(node.ChildNodes[19].InnerText);
                    temp.var4 = Convert.ToInt32(node.ChildNodes[20].InnerText);

                    vinyls.Add(temp);
                }
                //VisualParts

                foreach (XmlNode node in customCar.ChildNodes[16])
                {
                    VisualParts temp = new VisualParts();

                    temp.slot = Convert.ToInt32(node.ChildNodes[1].InnerText);
                    temp.visualPartHash = Convert.ToInt32(node.ChildNodes[0].InnerText);

                    visualParts.Add(temp);
                }

                /////////////CheckingLogic

                /////////////Customization Save

                ///////////////Paints
                sql = String.Format("DELETE FROM PersonaCarPaints WHERE carId = {0}", carId);

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }

                sql = "INSERT INTO PersonaCarPaints (carId, sat, slot, var, hue, grp) VALUES ";

                bool isFirst = true;

                foreach (Paints paint in paints)
                {

                    object[] data = new object[6];
                    data[0] = carId;
                    data[1] = paint.sat;
                    data[2] = paint.slot;
                    data[3] = paint.var;
                    data[4] = paint.hue;
                    data[5] = paint.group;


                    if (!isFirst)
                        sql = sql + ", ";
                    sql = sql + String.Format("({0},{1},{2},{3},{4},{5})", data);

                    isFirst = false;
                }

                if (!isFirst)
                {
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                ///////////////performanceParts
                sql = String.Format("DELETE FROM PersonaCarPerformanceParts WHERE carId = {0}", carId);
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }

                sql = "INSERT INTO PersonaCarPerformanceParts (carId, slot, partHash) VALUES ";

                isFirst = true;

                foreach (PerformanceParts performancePart in performanceParts)
                {
                    object[] data = new object[3];
                    data[0] = carId;
                    data[1] = performancePart.slot;
                    data[2] = performancePart.partHash;

                    if (!isFirst)
                        sql = sql + ", ";
                    sql = sql + String.Format("({0},{1},{2})", data);

                    isFirst = false;
                }

                if (!isFirst)
                {
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                ///////////////Skills
                sql = String.Format("DELETE FROM PersonaCarSkills WHERE carId = {0}", carId);
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }

                sql = "INSERT INTO PersonaCarSkills (carId, slot, skillHash) VALUES ";

                isFirst = true;

                foreach (Skills skill in skills)
                {
                    object[] data = new object[3];
                    data[0] = carId;
                    data[1] = skill.slot;
                    data[2] = skill.skillHash;

                    if (!isFirst)
                        sql = sql + ", ";
                    sql = sql + String.Format("({0},{1},{2})", data);

                    isFirst = false;
                }

                if (!isFirst)
                {
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                ///////////////Vinyls
                sql = String.Format("DELETE FROM PersonaCarVinyls WHERE carId = {0}", carId);
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }

                sql = "INSERT INTO PersonaCarVinyls (carId, layer, vinylHash, hue1, hue2, hue3, hue4, mir, rot, sat1, sat2, sat3, sat4, scaleX, scaleY, shear, tranX, tranY, var1, var2, var3, var4) VALUES ";

                isFirst = true;

                foreach (Vinyls vinyl in vinyls)
                {
                    object[] data = new object[22];
                    data[0] = carId;
                    data[1] = vinyl.layer;
                    data[2] = vinyl.vinylHash;
                    data[3] = vinyl.hue1;
                    data[4] = vinyl.hue2;
                    data[5] = vinyl.hue3;
                    data[6] = vinyl.hue4;
                    data[7] = (vinyl.mir ? 1 : 0);
                    data[8] = vinyl.rot;
                    data[9] = vinyl.sat1;
                    data[10] = vinyl.sat2;
                    data[11] = vinyl.sat3;
                    data[12] = vinyl.sat4;
                    data[13] = vinyl.scaleX;
                    data[14] = vinyl.scaleY;
                    data[15] = vinyl.shear;
                    data[16] = vinyl.tranX;
                    data[17] = vinyl.tranY;
                    data[18] = vinyl.var1;
                    data[19] = vinyl.var2;
                    data[20] = vinyl.var3;
                    data[21] = vinyl.var4;

                    if (!isFirst)
                        sql = sql + ", ";
                    sql = sql + String.Format("({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21})", data);

                    isFirst = false;

                }

                if (!isFirst)
                {
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                ///////////////Visual parts
                sql = String.Format("DELETE FROM PersonaCarVisualParts WHERE carId = {0}", carId);
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }

                sql = "INSERT INTO PersonaCarVisualParts (carId, partHash, slotHash) VALUES ";

                isFirst = true;

                foreach (VisualParts visualPart in visualParts)
                {
                    object[] data = new object[3];
                    data[0] = carId;
                    data[1] = visualPart.visualPartHash;
                    data[2] = visualPart.slot;

                    if (!isFirst)
                        sql = sql + ", ";
                    sql = sql + String.Format("({0},{1},{2})", data);

                    isFirst = false;
                }

                if (!isFirst)
                {
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                string vinylTemp = "";
                string visualPartsTemp = "";
                string skillTemp = "";
                string paintTemp = "";
                string performanceTemp = "";

                string carBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/updateCar.xml");
                string vinylBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/vinyl.xml");
                string visualPartBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/visualpart.xml");
                string skillBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/skill.xml");
                string performanceBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/performance.xml");
                string paintBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/paint.xml");

                foreach (Paints paint in paints)
                {
                    object[] paintData = new object[6];
                    paintData[0] = paint.group;
                    paintData[1] = paint.hue;
                    paintData[2] = paint.sat;
                    paintData[3] = paint.slot;
                    paintData[4] = paint.var;
                    paintTemp = paintTemp + String.Format(paintBody, paintData);
                }

                foreach (PerformanceParts performancePart in performanceParts)
                {
                    object[] performanceData = new object[1];
                    performanceData[0] = performancePart.partHash;
                    performanceTemp = performanceTemp + String.Format(performanceBody, performanceData);
                }

                foreach (Skills skill in skills)
                {
                    object[] skillData = new object[1];
                    skillData[0] = skill.skillHash;
                    skillTemp = skillTemp + String.Format(skillBody, skillData);
                }

                foreach (Vinyls vinyl in vinyls)
                {
                    object[] vinylData = new object[21];
                    vinylData[0] = vinyl.vinylHash;
                    vinylData[1] = vinyl.hue1;
                    vinylData[2] = vinyl.hue2;
                    vinylData[3] = vinyl.hue3;
                    vinylData[4] = vinyl.hue4;
                    vinylData[5] = vinyl.layer;
                    vinylData[6] = (vinyl.mir ? "true" : "false");
                    vinylData[7] = vinyl.rot;
                    vinylData[8] = vinyl.sat1;
                    vinylData[9] = vinyl.sat2;
                    vinylData[10] = vinyl.sat3;
                    vinylData[11] = vinyl.sat4;
                    vinylData[12] = vinyl.scaleX;
                    vinylData[13] = vinyl.scaleY;
                    vinylData[14] = vinyl.shear;
                    vinylData[15] = vinyl.tranX;
                    vinylData[16] = vinyl.tranY;
                    vinylData[17] = vinyl.var1;
                    vinylData[18] = vinyl.var2;
                    vinylData[19] = vinyl.var3;
                    vinylData[20] = vinyl.var4;
                    vinylTemp = vinylTemp + String.Format(vinylBody, vinylData);
                }

                foreach (VisualParts visualPart in visualParts)
                {
                    object[] visualData = new object[2];
                    visualData[0] = visualPart.visualPartHash;
                    visualData[1] = visualPart.slot;

                    visualPartsTemp = visualPartsTemp + String.Format(visualPartBody, visualData);
                }

                object[] carData = new object[14];
                carData[0] = carOrigData.baseCar;
                carData[1] = carOrigData.carClassHash;
                carData[2] = carId;
                //carData[3] = reader["motto"]; 
                //carData[4] = reader["name"];
                carData[5] = carOrigData.physicsProfileHash;
                carData[6] = carOrigData.rating;
                carData[7] = carOrigData.resalePrice;
                //carData[8] = reader["score"];
                //carData[9] = reader["score"];
                //carData[10] = reader["score"];
                carData[11] = carOrigData.durability;
                carData[12] = carOrigData.heat;
                carData[13] = carId;

                carData[3] = paintTemp;
                carData[4] = performanceTemp;
                carData[8] = skillTemp;
                carData[9] = vinylTemp;
                carData[10] = visualPartsTemp;

                car = car + String.Format(carBody, carData);
            }

            //Result generation
            string packetBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/commerce.xml");

            outPacket = string.Format(packetBody, car);

            //File.WriteAllText("packet", outPacket);

            //outPacket = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/100/commerce.xml");
            isGoodResponse = true;

            connection2.Close();
        }
        public static void updatePersonaCar_car(int personaId, int userId, string packet, MySqlConnection connection, out string outPacket, out bool isGoodResponse)
        {
            XmlDocument packetParsing = new XmlDocument();
            packetParsing.LoadXml(packet);
            int carId = 0;

            var customCar = packetParsing.ChildNodes[0].ChildNodes[0];

            carId = Convert.ToInt32(customCar.ChildNodes[2].InnerText);

            PersonaCar carOrigData = new PersonaCar();

            bool personaHasCar = false;

            string sql = String.Format("SELECT PersonaCar.carId, PersonaCar.baseCar, PersonaCar.physicsProfileHash, PersonaCar.carClassHash, PersonaCar.rating, PersonaCar.resalePrice, PersonaCar.durability, PersonaCar.heat FROM PersonaCar LEFT JOIN Persona ON PersonaCar.personaId = Persona.personaId LEFT JOIN Users ON Persona.userId = Users.userId WHERE Users.userId = {0} AND Persona.personaId = {1} AND PersonaCar.carId = {2}", userId, personaId, carId);

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    personaHasCar = true;
                    reader.Read();

                    carOrigData.baseCar = Convert.ToInt32(reader["baseCar"]);
                    carOrigData.physicsProfileHash = Convert.ToInt32(reader["physicsProfileHash"]);
                    carOrigData.carClassHash = Convert.ToInt32(reader["carClassHash"]);
                    carOrigData.rating = Convert.ToInt32(reader["rating"]);
                    carOrigData.resalePrice = Convert.ToInt32(reader["resalePrice"]);
                    carOrigData.durability = Convert.ToInt32(reader["durability"]);
                    carOrigData.heat = Convert.ToInt32(reader["heat"]);
                }
            }

            List<Paints> paints = new List<Paints>();
            List<PerformanceParts> performanceParts = new List<PerformanceParts>();
            List<Skills> skills = new List<Skills>();
            List<VisualParts> visualParts = new List<VisualParts>();
            List<Vinyls> vinyls = new List<Vinyls>();

            string car = "";

            if (personaHasCar)
            {
                //Paints
                foreach (XmlNode node in customCar.ChildNodes[6])
                {
                    Paints temp = new Paints();

                    temp.group = Convert.ToInt32(node.ChildNodes[0].InnerText);
                    temp.hue = Convert.ToInt32(node.ChildNodes[1].InnerText);
                    temp.sat = Convert.ToInt32(node.ChildNodes[2].InnerText);
                    temp.slot = Convert.ToInt32(node.ChildNodes[3].InnerText);
                    temp.var = Convert.ToInt32(node.ChildNodes[4].InnerText);

                    paints.Add(temp);
                }
                //PerformanceParts

                int slotIndex = 0;
                foreach (XmlNode node in customCar.ChildNodes[7])
                {
                    PerformanceParts temp = new PerformanceParts();

                    temp.slot = slotIndex;
                    temp.partHash = Convert.ToInt32(node.ChildNodes[0].InnerText);

                    slotIndex++;

                    performanceParts.Add(temp);
                }
                //Skills

                slotIndex = 0;
                foreach (XmlNode node in customCar.ChildNodes[12])
                {
                    Skills temp = new Skills();

                    temp.slot = slotIndex;
                    temp.skillHash = Convert.ToInt32(node.ChildNodes[1].InnerText);

                    slotIndex++;

                    skills.Add(temp);
                }
                //Vinyls

                foreach (XmlNode node in customCar.ChildNodes[15])
                {
                    Vinyls temp = new Vinyls();

                    temp.layer = Convert.ToInt32(node.ChildNodes[5].InnerText);

                    temp.vinylHash = Convert.ToInt32(node.ChildNodes[0].InnerText);
                    temp.hue1 = Convert.ToInt32(node.ChildNodes[1].InnerText);
                    temp.hue2 = Convert.ToInt32(node.ChildNodes[2].InnerText);
                    temp.hue3 = Convert.ToInt32(node.ChildNodes[3].InnerText);
                    temp.hue4 = Convert.ToInt32(node.ChildNodes[4].InnerText);

                    temp.mir = Convert.ToBoolean(node.ChildNodes[6].InnerText);
                    temp.rot = Convert.ToInt32(node.ChildNodes[7].InnerText);

                    temp.sat1 = Convert.ToInt32(node.ChildNodes[8].InnerText);
                    temp.sat2 = Convert.ToInt32(node.ChildNodes[9].InnerText);
                    temp.sat3 = Convert.ToInt32(node.ChildNodes[10].InnerText);
                    temp.sat4 = Convert.ToInt32(node.ChildNodes[11].InnerText);

                    temp.scaleX = Convert.ToInt32(node.ChildNodes[12].InnerText);
                    temp.scaleY = Convert.ToInt32(node.ChildNodes[13].InnerText);
                    temp.shear = Convert.ToInt32(node.ChildNodes[14].InnerText);
                    temp.tranX = Convert.ToInt32(node.ChildNodes[15].InnerText);
                    temp.tranY = Convert.ToInt32(node.ChildNodes[16].InnerText);

                    temp.var1 = Convert.ToInt32(node.ChildNodes[17].InnerText);
                    temp.var2 = Convert.ToInt32(node.ChildNodes[18].InnerText);
                    temp.var3 = Convert.ToInt32(node.ChildNodes[19].InnerText);
                    temp.var4 = Convert.ToInt32(node.ChildNodes[20].InnerText);

                    vinyls.Add(temp);
                }
                //VisualParts

                foreach (XmlNode node in customCar.ChildNodes[16])
                {
                    VisualParts temp = new VisualParts();

                    temp.slot = Convert.ToInt32(node.ChildNodes[1].InnerText);
                    temp.visualPartHash = Convert.ToInt32(node.ChildNodes[0].InnerText);

                    visualParts.Add(temp);
                }

                /////////////CheckingLogic

                /////////////Customization Save

                ///////////////Paints
                sql = String.Format("DELETE FROM PersonaCarPaints WHERE carId = {0}", carId);

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }

                sql = "INSERT INTO PersonaCarPaints (carId, sat, slot, var, hue, grp) VALUES ";

                bool isFirst = true;

                foreach (Paints paint in paints)
                {

                    object[] data = new object[6];
                    data[0] = carId;
                    data[1] = paint.sat;
                    data[2] = paint.slot;
                    data[3] = paint.var;
                    data[4] = paint.hue;
                    data[5] = paint.group;


                    if (!isFirst)
                        sql = sql + ", ";
                    sql = sql + String.Format("({0},{1},{2},{3},{4},{5})", data);

                    isFirst = false;
                }

                if (!isFirst)
                {
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                ///////////////performanceParts
                sql = String.Format("DELETE FROM PersonaCarPerformanceParts WHERE carId = {0}", carId);
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }

                sql = "INSERT INTO PersonaCarPerformanceParts (carId, slot, partHash) VALUES ";

                isFirst = true;

                foreach (PerformanceParts performancePart in performanceParts)
                {
                    object[] data = new object[3];
                    data[0] = carId;
                    data[1] = performancePart.slot;
                    data[2] = performancePart.partHash;

                    if (!isFirst)
                        sql = sql + ", ";
                    sql = sql + String.Format("({0},{1},{2})", data);

                    isFirst = false;
                }

                if (!isFirst)
                {
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                ///////////////Skills
                sql = String.Format("DELETE FROM PersonaCarSkills WHERE carId = {0}", carId);
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }

                sql = "INSERT INTO PersonaCarSkills (carId, slot, skillHash) VALUES ";

                isFirst = true;

                foreach (Skills skill in skills)
                {
                    object[] data = new object[3];
                    data[0] = carId;
                    data[1] = skill.slot;
                    data[2] = skill.skillHash;

                    if (!isFirst)
                        sql = sql + ", ";
                    sql = sql + String.Format("({0},{1},{2})", data);

                    isFirst = false;
                }

                if (!isFirst)
                {
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                ///////////////Vinyls
                sql = String.Format("DELETE FROM PersonaCarVinyls WHERE carId = {0}", carId);
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }

                sql = "INSERT INTO PersonaCarVinyls (carId, layer, vinylHash, hue1, hue2, hue3, hue4, mir, rot, sat1, sat2, sat3, sat4, scaleX, scaleY, shear, tranX, tranY, var1, var2, var3, var4) VALUES ";

                isFirst = true;

                foreach (Vinyls vinyl in vinyls)
                {
                    object[] data = new object[22];
                    data[0] = carId;
                    data[1] = vinyl.layer;
                    data[2] = vinyl.vinylHash;
                    data[3] = vinyl.hue1;
                    data[4] = vinyl.hue2;
                    data[5] = vinyl.hue3;
                    data[6] = vinyl.hue4;
                    data[7] = (vinyl.mir ? 1 : 0);
                    data[8] = vinyl.rot;
                    data[9] = vinyl.sat1;
                    data[10] = vinyl.sat2;
                    data[11] = vinyl.sat3;
                    data[12] = vinyl.sat4;
                    data[13] = vinyl.scaleX;
                    data[14] = vinyl.scaleY;
                    data[15] = vinyl.shear;
                    data[16] = vinyl.tranX;
                    data[17] = vinyl.tranY;
                    data[18] = vinyl.var1;
                    data[19] = vinyl.var2;
                    data[20] = vinyl.var3;
                    data[21] = vinyl.var4;

                    if (!isFirst)
                        sql = sql + ", ";
                    sql = sql + String.Format("({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21})", data);

                    isFirst = false;

                }

                if (!isFirst)
                {
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                ///////////////Visual parts
                sql = String.Format("DELETE FROM PersonaCarVisualParts WHERE carId = {0}", carId);
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }

                sql = "INSERT INTO PersonaCarVisualParts (carId, partHash, slotHash) VALUES ";

                isFirst = true;

                foreach (VisualParts visualPart in visualParts)
                {
                    object[] data = new object[3];
                    data[0] = carId;
                    data[1] = visualPart.visualPartHash;
                    data[2] = visualPart.slot;

                    if (!isFirst)
                        sql = sql + ", ";
                    sql = sql + String.Format("({0},{1},{2})", data);

                    isFirst = false;
                }

                if (!isFirst)
                {
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                string vinylTemp = "";
                string visualPartsTemp = "";
                string skillTemp = "";
                string paintTemp = "";
                string performanceTemp = "";

                string carBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/car.xml");
                string vinylBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/vinyl.xml");
                string visualPartBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/visualpart.xml");
                string skillBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/skill.xml");
                string performanceBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/performance.xml");
                string paintBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/paint.xml");

                foreach (Paints paint in paints)
                {
                    object[] paintData = new object[6];
                    paintData[0] = paint.group;
                    paintData[1] = paint.hue;
                    paintData[2] = paint.sat;
                    paintData[3] = paint.slot;
                    paintData[4] = paint.var;
                    paintTemp = paintTemp + String.Format(paintBody, paintData);
                }

                foreach (PerformanceParts performancePart in performanceParts)
                {
                    object[] performanceData = new object[1];
                    performanceData[0] = performancePart.partHash;
                    performanceTemp = performanceTemp + String.Format(performanceBody, performanceData);
                }

                foreach (Skills skill in skills)
                {
                    object[] skillData = new object[1];
                    skillData[0] = skill.skillHash;
                    skillTemp = skillTemp + String.Format(skillBody, skillData);
                }

                foreach (Vinyls vinyl in vinyls)
                {
                    object[] vinylData = new object[21];
                    vinylData[0] = vinyl.vinylHash;
                    vinylData[1] = vinyl.hue1;
                    vinylData[2] = vinyl.hue2;
                    vinylData[3] = vinyl.hue3;
                    vinylData[4] = vinyl.hue4;
                    vinylData[5] = vinyl.layer;
                    vinylData[6] = (vinyl.mir ? "true" : "false");
                    vinylData[7] = vinyl.rot;
                    vinylData[8] = vinyl.sat1;
                    vinylData[9] = vinyl.sat2;
                    vinylData[10] = vinyl.sat3;
                    vinylData[11] = vinyl.sat4;
                    vinylData[12] = vinyl.scaleX;
                    vinylData[13] = vinyl.scaleY;
                    vinylData[14] = vinyl.shear;
                    vinylData[15] = vinyl.tranX;
                    vinylData[16] = vinyl.tranY;
                    vinylData[17] = vinyl.var1;
                    vinylData[18] = vinyl.var2;
                    vinylData[19] = vinyl.var3;
                    vinylData[20] = vinyl.var4;
                    vinylTemp = vinylTemp + String.Format(vinylBody, vinylData);
                }

                foreach (VisualParts visualPart in visualParts)
                {
                    object[] visualData = new object[2];
                    visualData[0] = visualPart.visualPartHash;
                    visualData[1] = visualPart.slot;

                    visualPartsTemp = visualPartsTemp + String.Format(visualPartBody, visualData);
                }

                object[] carData = new object[14];
                carData[0] = carOrigData.baseCar;
                carData[1] = carOrigData.carClassHash;
                carData[2] = carId;
                //carData[3] = reader["motto"]; 
                //carData[4] = reader["name"];
                carData[5] = carOrigData.physicsProfileHash;
                carData[6] = carOrigData.rating;
                carData[7] = carOrigData.resalePrice;
                //carData[8] = reader["score"];
                //carData[9] = reader["score"];
                //carData[10] = reader["score"];
                carData[11] = carOrigData.durability;
                carData[12] = carOrigData.heat;
                carData[13] = carId;

                carData[3] = paintTemp;
                carData[4] = performanceTemp;
                carData[8] = skillTemp;
                carData[9] = vinylTemp;
                carData[10] = visualPartsTemp;

                car = car + String.Format(carBody, carData);
            }

            //Result generation
            //string packetBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/commerce.xml");

            outPacket = car;

            //File.WriteAllText("packet", outPacket);

            //outPacket = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/100/commerce.xml");
            isGoodResponse = true;
        }
        public static void getPersonaDefaultCar(int personaId, int userId, MySqlConnection connection, out string outPacket, out bool isGoodResponse)
        {
            MySqlConnection connection2 = new MySqlConnection(MsSQL.connetionString);
            connection2.Open();

            int defaultCarIndex = 0;
            string sql = String.Format("SELECT defaultCarIndex FROM Persona WHERE personaId = {0}", personaId);
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    defaultCarIndex = Convert.ToInt32(reader["defaultCarIndex"]);
                }
            }

            string packetBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/defaultCar.xml");

            string cars = "";

            sql = String.Format("SELECT PersonaCar.carId, PersonaCar.baseCar, PersonaCar.physicsProfileHash, PersonaCar.carClassHash, PersonaCar.rating, PersonaCar.resalePrice, PersonaCar.durability, PersonaCar.heat FROM PersonaCar LEFT JOIN Persona ON PersonaCar.personaId = Persona.personaId LEFT JOIN Users ON Persona.userId = Users.userId WHERE Users.userId = {0} AND Persona.personaId = {1} ORDER BY PersonaCar.carId ASC", userId, personaId);
            if(userId == 1)
                sql = String.Format("SELECT PersonaCar.carId, PersonaCar.baseCar, PersonaCar.physicsProfileHash, PersonaCar.carClassHash, PersonaCar.rating, PersonaCar.resalePrice, PersonaCar.durability, PersonaCar.heat FROM PersonaCar LEFT JOIN Persona ON PersonaCar.personaId = Persona.personaId LEFT JOIN Users ON Persona.userId = Users.userId WHERE Persona.personaId = {0} ORDER BY PersonaCar.carId ASC", personaId);
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    int index = 0;
                    while (reader.Read())
                    {
                        if (defaultCarIndex == index)
                        {
                            string carBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/car.xml");
                            string vinylBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/vinyl.xml");
                            string visualPartBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/visualpart.xml");
                            string skillBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/skill.xml");
                            string performanceBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/performance.xml");
                            string paintBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/paint.xml");

                            object[] carData = new object[14];
                            carData[0] = reader["baseCar"];
                            carData[1] = reader["carClassHash"];
                            carData[2] = reader["carId"];
                            //carData[3] = reader["motto"]; 
                            //carData[4] = reader["name"];
                            carData[5] = reader["physicsProfileHash"];
                            carData[6] = reader["rating"];
                            carData[7] = reader["resalePrice"];
                            //carData[8] = reader["score"];
                            //carData[9] = reader["score"];
                            //carData[10] = reader["score"];
                            carData[11] = reader["durability"];
                            carData[12] = reader["heat"];
                            carData[13] = reader["carId"];

                            string vinylTemp = "";
                            string visualPartsTemp = "";
                            string skillTemp = "";
                            string paintTemp = "";
                            string performanceTemp = "";

                            string sql2 = "";

                            sql2 = String.Format("SELECT * FROM PersonaCarVinyls WHERE carId = {0} ORDER BY layer ASC", carData[2]);
                            using (MySqlCommand command2 = new MySqlCommand(sql2, connection2))
                            using (MySqlDataReader reader2 = command2.ExecuteReader())
                            {

                                if (reader2.HasRows)
                                {
                                    while (reader2.Read())
                                    {
                                        object[] vinylData = new object[21];
                                        vinylData[0] = reader2["vinylHash"];
                                        vinylData[1] = reader2["hue1"];
                                        vinylData[2] = reader2["hue2"];
                                        vinylData[3] = reader2["hue3"];
                                        vinylData[4] = reader2["hue4"];
                                        vinylData[5] = reader2["layer"];
                                        vinylData[6] = (Convert.ToInt32(reader2["mir"]) == 1 ? "true" : "false");
                                        vinylData[7] = reader2["rot"];
                                        vinylData[8] = reader2["sat1"];
                                        vinylData[9] = reader2["sat2"];
                                        vinylData[10] = reader2["sat3"];
                                        vinylData[11] = reader2["sat4"];
                                        vinylData[12] = reader2["scaleX"];
                                        vinylData[13] = reader2["scaleY"];
                                        vinylData[14] = reader2["shear"];
                                        vinylData[15] = reader2["tranX"];
                                        vinylData[16] = reader2["tranY"];
                                        vinylData[17] = reader2["var1"];
                                        vinylData[18] = reader2["var2"];
                                        vinylData[19] = reader2["var3"];
                                        vinylData[20] = reader2["var4"];

                                        vinylTemp = vinylTemp + String.Format(vinylBody, vinylData);
                                    }
                                }
                            }


                            sql2 = String.Format("SELECT * FROM PersonaCarPaints WHERE carId = {0} ORDER BY slot ASC", carData[2]);
                            using (MySqlCommand command2 = new MySqlCommand(sql2, connection2))
                            using (MySqlDataReader reader2 = command2.ExecuteReader())
                            {

                                if (reader2.HasRows)
                                {
                                    while (reader2.Read())
                                    {
                                        object[] paintData = new object[5];
                                        paintData[0] = reader2["grp"]; //group
                                        paintData[1] = reader2["hue"];
                                        paintData[2] = reader2["sat"];
                                        paintData[3] = reader2["slot"];
                                        paintData[4] = reader2["var"];

                                        paintTemp = paintTemp + String.Format(paintBody, paintData);
                                    }

                                }

                            }

                            sql2 = String.Format("SELECT * FROM PersonaCarPerformanceParts WHERE carId = {0} ORDER BY slot ASC", carData[2]);
                            using (MySqlCommand command2 = new MySqlCommand(sql2, connection2))
                            using (MySqlDataReader reader2 = command2.ExecuteReader())
                            {

                                if (reader2.HasRows)
                                {
                                    while (reader2.Read())
                                    {
                                        object[] performanceData = new object[1];
                                        performanceData[0] = reader2["partHash"];

                                        performanceTemp = performanceTemp + String.Format(performanceBody, performanceData);
                                    }

                                }
                            }

                            sql2 = String.Format("SELECT * FROM PersonaCarSkills WHERE carId = {0} ORDER BY slot ASC", carData[2]);
                            using (MySqlCommand command2 = new MySqlCommand(sql2, connection2))
                            using (MySqlDataReader reader2 = command2.ExecuteReader())
                            {

                                if (reader2.HasRows)
                                {
                                    while (reader2.Read())
                                    {
                                        object[] skillData = new object[1];
                                        skillData[0] = reader2["skillHash"];

                                        skillTemp = skillTemp + String.Format(skillBody, skillData);
                                    }

                                }

                            }

                            sql2 = String.Format("SELECT * FROM PersonaCarVisualParts WHERE carId = {0} ", carData[2]);
                            using (MySqlCommand command2 = new MySqlCommand(sql2, connection2))
                            using (MySqlDataReader reader2 = command2.ExecuteReader())
                            {

                                if (reader2.HasRows)
                                {
                                    while (reader2.Read())
                                    {
                                        object[] visualData = new object[2];
                                        visualData[0] = reader2["partHash"];
                                        visualData[1] = reader2["slotHash"];

                                        visualPartsTemp = visualPartsTemp + String.Format(visualPartBody, visualData);
                                    }

                                }
                            }
                            carData[3] = paintTemp;
                            carData[4] = performanceTemp;
                            carData[8] = skillTemp;
                            carData[9] = vinylTemp;
                            carData[10] = visualPartsTemp;

                            cars = cars + String.Format(carBody, carData);


                            break;
                        }

                        index++;
                    }
                }
            }

            //outPacket = String.Format(packetBody, cars);
            outPacket = cars;
            isGoodResponse = true;

            connection2.Close();
        }
        public static void updatePersonaDefaultCar(int carId, int personaId, int userId, MySqlConnection connection, out string outPacket, out bool isGoodResponse)
        {
            MySqlConnection connection2 = new MySqlConnection(MsSQL.connetionString);
            connection2.Open();

            bool personaHasCar = false;

            string sql = String.Format("SELECT PersonaCar.carId FROM PersonaCar LEFT JOIN Persona ON PersonaCar.personaId = Persona.personaId LEFT JOIN Users ON Persona.userId = Users.userId WHERE Users.userId = {0} AND Persona.personaId = {1} AND PersonaCar.carId = {2}", userId, personaId, carId);
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    personaHasCar = true;
                }
            }

            if (personaHasCar)
            {
                int index = 0;

                sql = String.Format("SELECT carId FROM PersonaCar WHERE personaId = {0} ORDER BY carId ASC", personaId);
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (Convert.ToInt32(reader["carId"]) == carId)
                            break;

                        index++;

                    }
                }

                sql = String.Format("UPDATE Persona SET defaultCarIndex = '{0}' WHERE personaId = {1}", index, personaId);
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }


            string packetBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/defaultCar.xml");

            string cars = "";

            sql = String.Format("SELECT * FROM PersonaCar WHERE carId = {0}", carId);
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    string carBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/car.xml");
                    string vinylBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/vinyl.xml");
                    string visualPartBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/visualpart.xml");
                    string skillBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/skill.xml");
                    string performanceBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/performance.xml");
                    string paintBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/paint.xml");

                    reader.Read();

                    object[] carData = new object[14];
                    carData[0] = reader["baseCar"];
                    carData[1] = reader["carClassHash"];
                    carData[2] = reader["carId"];
                    //carData[3] = reader["motto"]; 
                    //carData[4] = reader["name"];
                    carData[5] = reader["physicsProfileHash"];
                    carData[6] = reader["rating"];
                    carData[7] = reader["resalePrice"];
                    //carData[8] = reader["score"];
                    //carData[9] = reader["score"];
                    //carData[10] = reader["score"];
                    carData[11] = reader["durability"];
                    carData[12] = reader["heat"];
                    carData[13] = reader["carId"];

                    string vinylTemp = "";
                    string visualPartsTemp = "";
                    string skillTemp = "";
                    string paintTemp = "";
                    string performanceTemp = "";

                    string sql2 = "";

                    sql2 = String.Format("SELECT * FROM PersonaCarVinyls WHERE carId = {0} ORDER BY layer ASC", carData[2]);
                    using (MySqlCommand command2 = new MySqlCommand(sql2, connection2))
                    using (MySqlDataReader reader2 = command2.ExecuteReader())
                    {
                        if (reader2.HasRows)
                        {
                            while (reader2.Read())
                            {
                                object[] vinylData = new object[21];
                                vinylData[0] = reader2["vinylHash"];
                                vinylData[1] = reader2["hue1"];
                                vinylData[2] = reader2["hue2"];
                                vinylData[3] = reader2["hue3"];
                                vinylData[4] = reader2["hue4"];
                                vinylData[5] = reader2["layer"];
                                vinylData[6] = (Convert.ToInt32(reader2["mir"]) == 1 ? "true" : "false");
                                vinylData[7] = reader2["rot"];
                                vinylData[8] = reader2["sat1"];
                                vinylData[9] = reader2["sat2"];
                                vinylData[10] = reader2["sat3"];
                                vinylData[11] = reader2["sat4"];
                                vinylData[12] = reader2["scaleX"];
                                vinylData[13] = reader2["scaleY"];
                                vinylData[14] = reader2["shear"];
                                vinylData[15] = reader2["tranX"];
                                vinylData[16] = reader2["tranY"];
                                vinylData[17] = reader2["var1"];
                                vinylData[18] = reader2["var2"];
                                vinylData[19] = reader2["var3"];
                                vinylData[20] = reader2["var4"];

                                vinylTemp = vinylTemp + String.Format(vinylBody, vinylData);
                            }
                        }
                    }

                    sql2 = String.Format("SELECT * FROM PersonaCarPaints WHERE carId = {0} ORDER BY slot ASC", carData[2]);
                    using (MySqlCommand command2 = new MySqlCommand(sql2, connection2))
                    using (MySqlDataReader reader2 = command2.ExecuteReader())
                    {

                        if (reader2.HasRows)
                        {
                            while (reader2.Read())
                            {
                                object[] paintData = new object[5];
                                paintData[0] = reader2["grp"]; //group
                                paintData[1] = reader2["hue"];
                                paintData[2] = reader2["sat"];
                                paintData[3] = reader2["slot"];
                                paintData[4] = reader2["var"];

                                paintTemp = paintTemp + String.Format(paintBody, paintData);
                            }

                        }
                    }

                    sql2 = String.Format("SELECT * FROM PersonaCarPerformanceParts WHERE carId = {0} ORDER BY slot ASC", carData[2]);
                    using (MySqlCommand command2 = new MySqlCommand(sql2, connection2))
                    using (MySqlDataReader reader2 = command2.ExecuteReader())
                    {
                        if (reader2.HasRows)
                        {
                            while (reader2.Read())
                            {
                                object[] performanceData = new object[1];
                                performanceData[0] = reader2["partHash"];

                                performanceTemp = performanceTemp + String.Format(performanceBody, performanceData);
                            }


                        }

                    }

                    sql2 = String.Format("SELECT * FROM PersonaCarSkills WHERE carId = {0} ORDER BY slot ASC", carData[2]);
                    using (MySqlCommand command2 = new MySqlCommand(sql2, connection2))
                    using (MySqlDataReader reader2 = command2.ExecuteReader())
                    {

                        if (reader2.HasRows)
                        {
                            while (reader2.Read())
                            {
                                object[] skillData = new object[1];
                                skillData[0] = reader2["skillHash"];

                                skillTemp = skillTemp + String.Format(skillBody, skillData);
                            }


                        }

                    }

                    sql2 = String.Format("SELECT * FROM PersonaCarVisualParts WHERE carId = {0} ", carData[2]);
                    using (MySqlCommand command2 = new MySqlCommand(sql2, connection2))
                    using (MySqlDataReader reader2 = command2.ExecuteReader())
                    {

                        if (reader2.HasRows)
                        {
                            while (reader2.Read())
                            {
                                object[] visualData = new object[2];
                                visualData[0] = reader2["partHash"];
                                visualData[1] = reader2["slotHash"];

                                visualPartsTemp = visualPartsTemp + String.Format(visualPartBody, visualData);
                            }


                        }

                    }
                    carData[3] = paintTemp;
                    carData[4] = performanceTemp;
                    carData[8] = skillTemp;
                    carData[9] = vinylTemp;
                    carData[10] = visualPartsTemp;

                    cars = cars + String.Format(carBody, carData);


                }
                else
                {
                    /////////////////////
                }
            }

            outPacket = String.Format(packetBody, cars);
            isGoodResponse = true;

            connection2.Close();
        }
        public static void buyCar(string packet, int personaId, int userId, MySqlConnection connection, out string outPacket, out bool isGoodResponse)
        {
            MySqlConnection connection2 = new MySqlConnection(MsSQL.connetionString);
            connection2.Open();

            bool isYourPersona = false;
            string sql = String.Format("SELECT personaId FROM Persona WHERE userId = {0}", userId);
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (Convert.ToInt32(reader["personaId"]) == personaId)
                    {
                        isYourPersona = true;
                        break;
                    }
                }
            }

            XmlDocument packetParsing = new XmlDocument();
            packetParsing.LoadXml(packet);

            string productId = packetParsing.ChildNodes[0].ChildNodes[0].ChildNodes[0].ChildNodes[0].InnerText.Replace(":", "");

            int carId = 0;

            string car = "";

            //productId = "example";

            if (File.Exists(Program.serverPrefix + "/Engine.svc/personas/car/baskets/" + productId + ".xml") && isYourPersona)
            {
                sql = String.Format("UPDATE Persona SET defaultCarIndex = carCount, carCount = carCount + 1 WHERE personaId = {0}", personaId);
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }

                string basket = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/baskets/" + productId + ".xml").Replace(" i:nil=\"true\" ", "");
                packetParsing.LoadXml(basket);
                var customCar = packetParsing.ChildNodes[0].ChildNodes[0];

                PersonaCar carOrigData = new PersonaCar(); ;

                carOrigData.baseCar = Convert.ToInt32(customCar.ChildNodes[0].InnerText);
                carOrigData.physicsProfileHash = Convert.ToInt32(customCar.ChildNodes[8].InnerText);
                carOrigData.carClassHash = Convert.ToInt32(customCar.ChildNodes[1].InnerText);
                carOrigData.rating = Convert.ToInt32(customCar.ChildNodes[9].InnerText);
                carOrigData.resalePrice = Convert.ToInt32(customCar.ChildNodes[10].InnerText);
                carOrigData.durability = 100;
                carOrigData.heat = 1;

                List<Paints> paints = new List<Paints>();
                List<PerformanceParts> performanceParts = new List<PerformanceParts>();
                List<Skills> skills = new List<Skills>();
                List<VisualParts> visualParts = new List<VisualParts>();
                List<Vinyls> vinyls = new List<Vinyls>();

                //Paints
                foreach (XmlNode node in customCar.ChildNodes[6])
                {
                    Paints temp = new Paints();

                    temp.group = Convert.ToInt32(node.ChildNodes[0].InnerText);
                    temp.hue = Convert.ToInt32(node.ChildNodes[1].InnerText);
                    temp.sat = Convert.ToInt32(node.ChildNodes[2].InnerText);
                    temp.slot = Convert.ToInt32(node.ChildNodes[3].InnerText);
                    temp.var = Convert.ToInt32(node.ChildNodes[4].InnerText);

                    paints.Add(temp);
                }
                //PerformanceParts

                int slotIndex = 0;
                foreach (XmlNode node in customCar.ChildNodes[7])
                {
                    PerformanceParts temp = new PerformanceParts();

                    temp.slot = slotIndex;
                    temp.partHash = Convert.ToInt32(node.ChildNodes[0].InnerText);

                    slotIndex++;

                    performanceParts.Add(temp);
                }
                //Skills

                slotIndex = 0;
                foreach (XmlNode node in customCar.ChildNodes[12])
                {
                    Skills temp = new Skills();

                    temp.slot = slotIndex;
                    temp.skillHash = Convert.ToInt32(node.ChildNodes[1].InnerText);

                    slotIndex++;

                    skills.Add(temp);
                }
                //Vinyls

                foreach (XmlNode node in customCar.ChildNodes[15])
                {
                    Vinyls temp = new Vinyls();

                    temp.layer = Convert.ToInt32(node.ChildNodes[5].InnerText);

                    temp.vinylHash = Convert.ToInt32(node.ChildNodes[0].InnerText);
                    temp.hue1 = Convert.ToInt32(node.ChildNodes[1].InnerText);
                    temp.hue2 = Convert.ToInt32(node.ChildNodes[2].InnerText);
                    temp.hue3 = Convert.ToInt32(node.ChildNodes[3].InnerText);
                    temp.hue4 = Convert.ToInt32(node.ChildNodes[4].InnerText);

                    temp.mir = Convert.ToBoolean(node.ChildNodes[6].InnerText);
                    temp.rot = Convert.ToInt32(node.ChildNodes[7].InnerText);

                    temp.sat1 = Convert.ToInt32(node.ChildNodes[8].InnerText);
                    temp.sat2 = Convert.ToInt32(node.ChildNodes[9].InnerText);
                    temp.sat3 = Convert.ToInt32(node.ChildNodes[10].InnerText);
                    temp.sat4 = Convert.ToInt32(node.ChildNodes[11].InnerText);

                    temp.scaleX = Convert.ToInt32(node.ChildNodes[12].InnerText);
                    temp.scaleY = Convert.ToInt32(node.ChildNodes[13].InnerText);
                    temp.shear = Convert.ToInt32(node.ChildNodes[14].InnerText);
                    temp.tranX = Convert.ToInt32(node.ChildNodes[15].InnerText);
                    temp.tranY = Convert.ToInt32(node.ChildNodes[16].InnerText);

                    temp.var1 = Convert.ToInt32(node.ChildNodes[17].InnerText);
                    temp.var2 = Convert.ToInt32(node.ChildNodes[18].InnerText);
                    temp.var3 = Convert.ToInt32(node.ChildNodes[19].InnerText);
                    temp.var4 = Convert.ToInt32(node.ChildNodes[20].InnerText);

                    vinyls.Add(temp);
                }
                //VisualParts

                foreach (XmlNode node in customCar.ChildNodes[16])
                {
                    VisualParts temp = new VisualParts();

                    temp.slot = Convert.ToInt32(node.ChildNodes[1].InnerText);
                    temp.visualPartHash = Convert.ToInt32(node.ChildNodes[0].InnerText);

                    visualParts.Add(temp);
                }

                sql = "";

                {
                    object[] carData = new object[8];
                    carData[0] = personaId;
                    carData[1] = carOrigData.baseCar;
                    carData[2] = carOrigData.carClassHash;
                    carData[3] = carOrigData.physicsProfileHash;
                    carData[4] = carOrigData.rating;
                    carData[5] = carOrigData.resalePrice;
                    carData[6] = carOrigData.durability;
                    carData[7] = carOrigData.heat;

                    sql = string.Format("INSERT INTO PersonaCar (personaId, baseCar, carClassHash, physicsProfileHash, rating, resalePrice, durability, heat) VALUES ({0},{1},{2},{3},{4},{5},{6},{7})", carData);
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    sql = string.Format("SELECT carId FROM PersonaCar WHERE personaId = {0} ORDER BY carId DESC LIMIT 1", personaId);
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                        carId = Convert.ToInt32(reader["carId"]);
                    }


                }


                ///////////////Paints

                sql = "INSERT INTO PersonaCarPaints (carId, sat, slot, var, hue, grp) VALUES ";

                bool isFirst = true;

                foreach (Paints paint in paints)
                {

                    object[] data = new object[6];
                    data[0] = carId;
                    data[1] = paint.sat;
                    data[2] = paint.slot;
                    data[3] = paint.var;
                    data[4] = paint.hue;
                    data[5] = paint.group;


                    if (!isFirst)
                        sql = sql + ", ";
                    sql = sql + String.Format("({0},{1},{2},{3},{4},{5})", data);

                    isFirst = false;
                }

                if (!isFirst)
                {
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                ///////////////performanceParts

                sql = "INSERT INTO PersonaCarPerformanceParts (carId, slot, partHash) VALUES ";

                isFirst = true;

                foreach (PerformanceParts performancePart in performanceParts)
                {
                    object[] data = new object[3];
                    data[0] = carId;
                    data[1] = performancePart.slot;
                    data[2] = performancePart.partHash;

                    if (!isFirst)
                        sql = sql + ", ";
                    sql = sql + String.Format("({0},{1},{2})", data);

                    isFirst = false;
                }

                if (!isFirst)
                {
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                ///////////////Skills

                sql = "INSERT INTO PersonaCarSkills (carId, slot, skillHash) VALUES ";

                isFirst = true;

                foreach (Skills skill in skills)
                {
                    object[] data = new object[3];
                    data[0] = carId;
                    data[1] = skill.slot;
                    data[2] = skill.skillHash;

                    if (!isFirst)
                        sql = sql + ", ";
                    sql = sql + String.Format("({0},{1},{2})", data);

                    isFirst = false;
                }

                if (!isFirst)
                {
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                ///////////////Vinyls

                sql = "INSERT INTO PersonaCarVinyls (carId, layer, vinylHash, hue1, hue2, hue3, hue4, mir, rot, sat1, sat2, sat3, sat4, scaleX, scaleY, shear, tranX, tranY, var1, var2, var3, var4) VALUES ";

                isFirst = true;

                foreach (Vinyls vinyl in vinyls)
                {
                    object[] data = new object[22];
                    data[0] = carId;
                    data[1] = vinyl.layer;
                    data[2] = vinyl.vinylHash;
                    data[3] = vinyl.hue1;
                    data[4] = vinyl.hue2;
                    data[5] = vinyl.hue3;
                    data[6] = vinyl.hue4;
                    data[7] = (vinyl.mir ? 1 : 0);
                    data[8] = vinyl.rot;
                    data[9] = vinyl.sat1;
                    data[10] = vinyl.sat2;
                    data[11] = vinyl.sat3;
                    data[12] = vinyl.sat4;
                    data[13] = vinyl.scaleX;
                    data[14] = vinyl.scaleY;
                    data[15] = vinyl.shear;
                    data[16] = vinyl.tranX;
                    data[17] = vinyl.tranY;
                    data[18] = vinyl.var1;
                    data[19] = vinyl.var2;
                    data[20] = vinyl.var3;
                    data[21] = vinyl.var4;

                    if (!isFirst)
                        sql = sql + ", ";
                    sql = sql + String.Format("({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21})", data);

                    isFirst = false;

                }

                if (!isFirst)
                {
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                ///////////////Visual parts

                sql = "INSERT INTO PersonaCarVisualParts (carId, partHash, slotHash) VALUES ";

                isFirst = true;

                foreach (VisualParts visualPart in visualParts)
                {
                    object[] data = new object[3];
                    data[0] = carId;
                    data[1] = visualPart.visualPartHash;
                    data[2] = visualPart.slot;

                    if (!isFirst)
                        sql = sql + ", ";
                    sql = sql + String.Format("({0},{1},{2})", data);

                    isFirst = false;
                }

                if (!isFirst)
                {
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }


                string vinylTemp = "";
                string visualPartsTemp = "";
                string skillTemp = "";
                string paintTemp = "";
                string performanceTemp = "";

                string carBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/car.xml");
                string vinylBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/vinyl.xml");
                string visualPartBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/visualpart.xml");
                string skillBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/skill.xml");
                string performanceBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/performance.xml");
                string paintBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/paint.xml");

                foreach (Paints paint in paints)
                {
                    object[] paintData = new object[6];
                    paintData[0] = paint.group;
                    paintData[1] = paint.hue;
                    paintData[2] = paint.sat;
                    paintData[3] = paint.slot;
                    paintData[4] = paint.var;
                    paintTemp = paintTemp + String.Format(paintBody, paintData);
                }

                foreach (PerformanceParts performancePart in performanceParts)
                {
                    object[] performanceData = new object[1];
                    performanceData[0] = performancePart.partHash;
                    performanceTemp = performanceTemp + String.Format(performanceBody, performanceData);
                }

                foreach (Skills skill in skills)
                {
                    object[] skillData = new object[1];
                    skillData[0] = skill.skillHash;
                    skillTemp = skillTemp + String.Format(skillBody, skillData);
                }

                foreach (Vinyls vinyl in vinyls)
                {
                    object[] vinylData = new object[21];
                    vinylData[0] = vinyl.vinylHash;
                    vinylData[1] = vinyl.hue1;
                    vinylData[2] = vinyl.hue2;
                    vinylData[3] = vinyl.hue3;
                    vinylData[4] = vinyl.hue4;
                    vinylData[5] = vinyl.layer;
                    vinylData[6] = (vinyl.mir ? "true" : "false");
                    vinylData[7] = vinyl.rot;
                    vinylData[8] = vinyl.sat1;
                    vinylData[9] = vinyl.sat2;
                    vinylData[10] = vinyl.sat3;
                    vinylData[11] = vinyl.sat4;
                    vinylData[12] = vinyl.scaleX;
                    vinylData[13] = vinyl.scaleY;
                    vinylData[14] = vinyl.shear;
                    vinylData[15] = vinyl.tranX;
                    vinylData[16] = vinyl.tranY;
                    vinylData[17] = vinyl.var1;
                    vinylData[18] = vinyl.var2;
                    vinylData[19] = vinyl.var3;
                    vinylData[20] = vinyl.var4;
                    vinylTemp = vinylTemp + String.Format(vinylBody, vinylData);
                }

                foreach (VisualParts visualPart in visualParts)
                {
                    object[] visualData = new object[2];
                    visualData[0] = visualPart.visualPartHash;
                    visualData[1] = visualPart.slot;

                    visualPartsTemp = visualPartsTemp + String.Format(visualPartBody, visualData);
                }
                {
                    object[] carData = new object[14];
                    carData[0] = carOrigData.baseCar;
                    carData[1] = carOrigData.carClassHash;
                    carData[2] = carId;
                    //carData[3] = reader["motto"]; 
                    //carData[4] = reader["name"];
                    carData[5] = carOrigData.physicsProfileHash;
                    carData[6] = carOrigData.rating;
                    carData[7] = carOrigData.resalePrice;
                    //carData[8] = reader["score"];
                    //carData[9] = reader["score"];
                    //carData[10] = reader["score"];
                    carData[11] = carOrigData.durability;
                    carData[12] = carOrigData.heat;
                    carData[13] = carId;

                    carData[3] = paintTemp;
                    carData[4] = performanceTemp;
                    carData[8] = skillTemp;
                    carData[9] = vinylTemp;
                    carData[10] = visualPartsTemp;

                    car = car + String.Format(carBody, carData);
                }
            }

            //Result generation
            string packetBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/baskets.xml");

            outPacket = string.Format(packetBody, car);

            //File.WriteAllText("packet", outPacket);

            //outPacket = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/100/commerce.xml");
            isGoodResponse = true;

            connection2.Close();
        }
        public static void sellCar(int carId, int personaId, int userId, MySqlConnection connection, out string outPacket, out bool isGoodResponse)
        {
            MySqlConnection connection2 = new MySqlConnection(MsSQL.connetionString);
            connection2.Open();

            bool isYourPersona = false;
            int carCount = 0;
            int defaultCarIndex = 0;
            string sql = String.Format("SELECT PersonaCar.personaId, Persona.defaultCarIndex, Persona.carCount FROM PersonaCar LEFT JOIN Persona ON PersonaCar.personaId = Persona.personaId WHERE Persona.userId = {0} AND PersonaCar.carId = {1}", userId, carId);
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                    while (reader.Read())
                    {
                        if (Convert.ToInt32(reader["personaId"]) == personaId)
                        {
                            isYourPersona = true;
                            carCount = (int)reader["carCount"];
                            defaultCarIndex = (int)reader["defaultCarIndex"];
                            break;
                        }
                    }
            }

            string cars = "";

            if (isYourPersona && carCount > 1)
            {
                int sellCarIndex = 0;

                sql = String.Format("SELECT carId FROM PersonaCar WHERE personaId = {0} ORDER BY carId ASC", personaId);
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (Convert.ToInt32(reader["carId"]) == carId)
                            break;

                        sellCarIndex++;

                    }
                }
                if (sellCarIndex > defaultCarIndex || defaultCarIndex == 0)
                {
                    carCount--;
                    sql = String.Format("UPDATE Persona SET carCount = '{0}' WHERE personaId = {1}", carCount, personaId);
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                else
                {
                    carCount--;
                    defaultCarIndex--;
                    sql = String.Format("UPDATE Persona SET carCount = '{0}', defaultCarIndex = {1} WHERE personaId = {2}", carCount, defaultCarIndex, personaId);
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                }

                sql = String.Format("DELETE FROM PersonaCarPaints WHERE carId = {0}", carId);
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
                sql = String.Format("DELETE FROM PersonaCarPerformanceParts WHERE carId = {0}", carId);
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
                sql = String.Format("DELETE FROM PersonaCarSkills WHERE carId = {0}", carId);
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
                sql = String.Format("DELETE FROM PersonaCarVinyls WHERE carId = {0}", carId);
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
                sql = String.Format("DELETE FROM PersonaCarVisualParts WHERE carId = {0}", carId);
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
                sql = String.Format("DELETE FROM PersonaCar WHERE carId = {0}", carId);
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }

                sql = String.Format("SELECT PersonaCar.carId, PersonaCar.baseCar, PersonaCar.physicsProfileHash, PersonaCar.carClassHash, PersonaCar.rating, PersonaCar.resalePrice, PersonaCar.durability, PersonaCar.heat FROM PersonaCar LEFT JOIN Persona ON PersonaCar.personaId = Persona.personaId LEFT JOIN Users ON Persona.userId = Users.userId WHERE Users.userId = {0} AND Persona.personaId = {1} ORDER BY PersonaCar.carId ASC", userId, personaId);
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        int index = 0;
                        while (reader.Read())
                        {
                            if (defaultCarIndex == index)
                            {
                                string carBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/car.xml");
                                string vinylBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/vinyl.xml");
                                string visualPartBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/visualpart.xml");
                                string skillBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/skill.xml");
                                string performanceBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/performance.xml");
                                string paintBody = File.ReadAllText(Program.serverPrefix + "/Engine.svc/personas/car/paint.xml");

                                object[] carData = new object[14];
                                carData[0] = reader["baseCar"];
                                carData[1] = reader["carClassHash"];
                                carData[2] = reader["carId"];
                                //carData[3] = reader["motto"]; 
                                //carData[4] = reader["name"];
                                carData[5] = reader["physicsProfileHash"];
                                carData[6] = reader["rating"];
                                carData[7] = reader["resalePrice"];
                                //carData[8] = reader["score"];
                                //carData[9] = reader["score"];
                                //carData[10] = reader["score"];
                                carData[11] = reader["durability"];
                                carData[12] = reader["heat"];
                                carData[13] = reader["carId"];

                                string vinylTemp = "";
                                string visualPartsTemp = "";
                                string skillTemp = "";
                                string paintTemp = "";
                                string performanceTemp = "";

                                string sql2 = "";

                                sql2 = String.Format("SELECT * FROM PersonaCarVinyls WHERE carId = {0} ORDER BY layer ASC", carData[2]);
                                using (MySqlCommand command2 = new MySqlCommand(sql2, connection2))
                                using (MySqlDataReader reader2 = command2.ExecuteReader())
                                {

                                    if (reader2.HasRows)
                                    {
                                        while (reader2.Read())
                                        {
                                            object[] vinylData = new object[21];
                                            vinylData[0] = reader2["vinylHash"];
                                            vinylData[1] = reader2["hue1"];
                                            vinylData[2] = reader2["hue2"];
                                            vinylData[3] = reader2["hue3"];
                                            vinylData[4] = reader2["hue4"];
                                            vinylData[5] = reader2["layer"];
                                            vinylData[6] = (Convert.ToInt32(reader2["mir"]) == 1 ? "true" : "false");
                                            vinylData[7] = reader2["rot"];
                                            vinylData[8] = reader2["sat1"];
                                            vinylData[9] = reader2["sat2"];
                                            vinylData[10] = reader2["sat3"];
                                            vinylData[11] = reader2["sat4"];
                                            vinylData[12] = reader2["scaleX"];
                                            vinylData[13] = reader2["scaleY"];
                                            vinylData[14] = reader2["shear"];
                                            vinylData[15] = reader2["tranX"];
                                            vinylData[16] = reader2["tranY"];
                                            vinylData[17] = reader2["var1"];
                                            vinylData[18] = reader2["var2"];
                                            vinylData[19] = reader2["var3"];
                                            vinylData[20] = reader2["var4"];

                                            vinylTemp = vinylTemp + String.Format(vinylBody, vinylData);
                                        }
                                    }
                                }


                                sql2 = String.Format("SELECT * FROM PersonaCarPaints WHERE carId = {0} ORDER BY slot ASC", carData[2]);
                                using (MySqlCommand command2 = new MySqlCommand(sql2, connection2))
                                using (MySqlDataReader reader2 = command2.ExecuteReader())
                                {

                                    if (reader2.HasRows)
                                    {
                                        while (reader2.Read())
                                        {
                                            object[] paintData = new object[5];
                                            paintData[0] = reader2["grp"]; //group
                                            paintData[1] = reader2["hue"];
                                            paintData[2] = reader2["sat"];
                                            paintData[3] = reader2["slot"];
                                            paintData[4] = reader2["var"];

                                            paintTemp = paintTemp + String.Format(paintBody, paintData);
                                        }

                                    }

                                }

                                sql2 = String.Format("SELECT * FROM PersonaCarPerformanceParts WHERE carId = {0} ORDER BY slot ASC", carData[2]);
                                using (MySqlCommand command2 = new MySqlCommand(sql2, connection2))
                                using (MySqlDataReader reader2 = command2.ExecuteReader())
                                {

                                    if (reader2.HasRows)
                                    {
                                        while (reader2.Read())
                                        {
                                            object[] performanceData = new object[1];
                                            performanceData[0] = reader2["partHash"];

                                            performanceTemp = performanceTemp + String.Format(performanceBody, performanceData);
                                        }

                                    }
                                }

                                sql2 = String.Format("SELECT * FROM PersonaCarSkills WHERE carId = {0} ORDER BY slot ASC", carData[2]);
                                using (MySqlCommand command2 = new MySqlCommand(sql2, connection2))
                                using (MySqlDataReader reader2 = command2.ExecuteReader())
                                {

                                    if (reader2.HasRows)
                                    {
                                        while (reader2.Read())
                                        {
                                            object[] skillData = new object[1];
                                            skillData[0] = reader2["skillHash"];

                                            skillTemp = skillTemp + String.Format(skillBody, skillData);
                                        }

                                    }

                                }

                                sql2 = String.Format("SELECT * FROM PersonaCarVisualParts WHERE carId = {0} ", carData[2]);
                                using (MySqlCommand command2 = new MySqlCommand(sql2, connection2))
                                using (MySqlDataReader reader2 = command2.ExecuteReader())
                                {

                                    if (reader2.HasRows)
                                    {
                                        while (reader2.Read())
                                        {
                                            object[] visualData = new object[2];
                                            visualData[0] = reader2["partHash"];
                                            visualData[1] = reader2["slotHash"];

                                            visualPartsTemp = visualPartsTemp + String.Format(visualPartBody, visualData);
                                        }

                                    }
                                }
                                carData[3] = paintTemp;
                                carData[4] = performanceTemp;
                                carData[8] = skillTemp;
                                carData[9] = vinylTemp;
                                carData[10] = visualPartsTemp;

                                cars = cars + String.Format(carBody, carData);


                                break;
                            }

                            index++;
                        }
                    }
                }
            }
            connection2.Close();
            outPacket = cars;
            isGoodResponse = true;
        }
        public static void repair(int userId, int personaId, MySqlConnection connection, out string outPacket, out bool isGoodResponse)
        {
            MySqlConnection connection2 = new MySqlConnection(MsSQL.connetionString);
            connection2.Open();

            int defaultCarIndex = 0;
            string sql = String.Format("SELECT defaultCarIndex FROM Persona WHERE personaId = {0}", personaId);
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    defaultCarIndex = Convert.ToInt32(reader["defaultCarIndex"]);
                }
            }

            sql = String.Format("SELECT PersonaCar.carId FROM PersonaCar LEFT JOIN Persona ON PersonaCar.personaId = Persona.personaId LEFT JOIN Users ON Persona.userId = Users.userId WHERE Users.userId = {0} AND Persona.personaId = {1} ORDER BY PersonaCar.carId ASC", userId, personaId);
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    int index = 0;
                    while (reader.Read())
                    {
                        if (defaultCarIndex == index)
                        {
                            string carId = reader["carId"].ToString();
                            sql = String.Format("UPDATE PersonaCar SET durability = {0} WHERE carId = {1} AND personaId = {2}", 100, carId, personaId);
                            using (MySqlCommand command2 = new MySqlCommand(sql, connection2))
                            {
                                command2.ExecuteNonQuery();
                            }
                            break;
                        }
                        index++;
                    }
                }
            }

            outPacket = File.ReadAllText(Program.serverPrefix + "/Engine.svc/car/repair.xml");
            isGoodResponse = true;

            connection2.Close();
        } 
    }
}
