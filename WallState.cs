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
    /// <summary>
    /// Represents the different wall states.
    /// </summary>
    public enum WallState
    {
        /// <summary>
        /// Indicates that the edge has not been mapped yet.
        /// </summary>
        NotMapped = 0,

        /// <summary>
        /// Indicates that the edge has not been mapped yet but in the maze map it is no wall
        /// </summary>
        NotMappedNoWall = 1,

        /// <summary>
        /// Indicates that the edge has not been mapped yet but in the maze map it is a wall
        /// </summary>
        NotMappedWall = 2,

        /// <summary>
        /// Indicates that the edge has been mapped, and there is no wall.
        /// </summary>
        NoWall = 3,

        /// <summary>
        /// Indicates that the edge has been mapped, and there is a wall.
        /// </summary>
        Wall = 4,

        /// <summary>
        /// Indicates that the edge has been mapped at least twice, and the wall state has been corrected to no wall.
        /// </summary>
        CorrectedNoWall = 5,

        /// <summary>
        /// Indicates that the edge has been mapped at least twice, and the wall state has been corrected to wall.
        /// </summary>
        CorrectedWall = 6,
    }
}
