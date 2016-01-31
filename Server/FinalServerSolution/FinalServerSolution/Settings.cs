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
using System.IO;
using MySql.Data.MySqlClient;
using System.Xml;

namespace FinalServerSolution
{
    class Settings
    {
        public static void userSettings(int userId, MySqlConnection connection, out string outPacket, out bool isGoodResponse){
            string packet = File.ReadAllText(Program.serverPrefix + "/Engine.svc/getusersettings.xml");

            outPacket = String.Format(packet,  userId);
            isGoodResponse = true;
        }
        public static void systemInfo(MySqlConnection connection, out string outPacket, out bool isGoodResponse)
        {
            string packet = File.ReadAllText(Program.serverPrefix + "/Engine.svc/systeminfo.xml");

            outPacket = String.Format(packet, DateTime.Now);
            isGoodResponse = true;
        }
        public static void fraudConfig(int userId, MySqlConnection connection, out string outPacket, out bool isGoodResponse)
        {
            string packet = File.ReadAllText(Program.serverPrefix + "/Engine.svc/security/fraudConfig.xml");

            outPacket = String.Format(packet, userId);
            isGoodResponse = true;
        }
    }
}
