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

namespace FinalServerSolution
{
    class Time
    {
        private static DateTime time19700101 = new DateTime(1970, 1, 1);

        public static int getSeconds()
        {
            return (int)(DateTime.Now - time19700101).TotalSeconds;
        }
       // int seconds = (int)(DateTime.Now - time19700101).TotalSeconds;
        public static bool sessionLife(int seconds)
        {
            int currentSeconds = getSeconds();
            //if (currentSeconds - 60 * 60 < seconds)
            if (currentSeconds - 2 * 60 < seconds)
                return true;
            else
                return false;
        }
    }
}
