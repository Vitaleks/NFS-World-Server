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
    public struct Paints
    {
        public int group;
        public int slot;
        public int hue;
        public int var;
        public int sat;
    }

    public struct PerformanceParts
    {
        public int slot;
        public int partHash;
    }

    public struct Skills
    {
        public int slot;
        public int skillHash;
    }

    public struct Vinyls
    {
        public int layer;
        public int vinylHash;
        public int hue1;
        public int hue2;
        public int hue3;
        public int hue4;
        public bool mir;
        public int rot;
        public int sat1;
        public int sat2;
        public int sat3;
        public int sat4;
        public int scaleX;
        public int scaleY;
        public int shear;
        public int tranX;
        public int tranY;
        public int var1;
        public int var2;
        public int var3;
        public int var4;
    }

    public struct VisualParts
    {
        public int slot;
        public int visualPartHash;
    }

    public struct PersonaCar
    {
        public int baseCar;
        public int carClassHash;
        public int physicsProfileHash;
        public int rating;
        public int resalePrice;
        public int durability;
        public int heat;
    }
}
