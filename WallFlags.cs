/*
 * Copyright 2014 Garrett Blankenberg
 * 
 * Copyright 2020 Harjit Singh
 * Permission is hereby granted, free of charge, to any person obtaining a copy 
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights 
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
 * copies of the Software, and to permit persons to whom the Software is 
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in 
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
 * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR 
 * ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE
 * OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

namespace MouseControls
{
    using System;

    /// <summary>
    /// Represents the wall flags state.
    /// </summary>
    [Flags]
    public enum WallFlags : byte
    {
        /// <summary>
        /// North edge has been mapped.
        /// </summary>
        MappedNorth = 0x80,
        
        /// <summary>
        /// East edge has been mapped.
        /// </summary>
        MappedEast = 0x40,

        /// <summary>
        /// South edge has been mapped.
        /// </summary>
        MappedSouth = 0x20,

        /// <summary>
        /// West edge has been mapped.
        /// </summary>
        MappedWest = 0x10,

        /// <summary>
        /// North edge is a wall.
        /// </summary>
        NorthWall = 0x08,

        /// <summary>
        /// East edge is a wall.
        /// </summary>
        EastWall = 0x04,

        /// <summary>
        /// South edge is a wall.
        /// </summary>
        SouthWall = 0x02,

        /// <summary>
        /// West edge is a wall.
        /// </summary>
        WestWall = 0x01
    }
}
