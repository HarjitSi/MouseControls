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
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Represents a control used to maintain and display maze state.
    /// </summary>
    public partial class MazeControl : UserControl
    {
        /// <summary>
        /// The private storage for the path.
        /// </summary>
        private Point[] path;

        /// <summary>
        /// The private storage for the solver coordinates.
        /// </summary>
        private Point[] coord;

        /// <summary>
        /// The private storage for the ExpandPath coordinates.
        /// </summary>
        private int[] coordCopy = new int[100*100];

        /// <summary>
        /// The private storage for the cost.
        /// </summary>
        private int[] cost = new int[1024];

        /// <summary>
        /// Private storage for the number of rows.
        /// </summary>
        private int numberOfRows;

        /// <summary>
        /// Private storage for the number of columns.
        /// </summary>
        private int numberOfColumns;

        /// <summary>
        /// Private storage for the mouse coordinate.
        /// </summary>
        private Point mouseCoord;

        /// <summary>
        /// Private storage for the ExpandLine coordinates.
        /// </summary>
        private Point[] ExpandLine = new Point[2];

        /// <summary>
        /// Private storage for the EndPoint coordinates.
        /// </summary>
        private Point EndPoint;

        /// <summary>
        /// Private storage for the ExpandPoint coordinates.
        /// </summary>
        private List<Point> ExpandPoint = new List<Point>();

        /// <summary>
        /// Initializes a new instance of the MazeControl class.
        /// </summary>
        public MazeControl()
        {
            this.NumberOfRows = 16;
            this.NumberOfColumns = 16;
            this.BackColor = Color.Black;
            this.WallColor = Color.Red;
            this.WallFilled = true;
            this.NoWallColor = this.BackColor;
            this.NoWallFilled = false;
            this.CorrectedWallColor = Color.Green;
            this.CorrectedWallFilled = true;
            this.CorrectedNoWallColor = Color.Green;
            this.CorrectedNoWallFilled = false;
            this.NotMappedColor = Color.Red;
            this.NotMappedFilled = false;
            this.NotMappedNoWallColor = Color.Red;
            this.NotMappedNoWallFilled = false;
            this.NotMappedWallColor = Color.Red;
            this.NotMappedWallFilled = true;
            this.PegColor = Color.DarkRed;
            this.PathColor = Color.Yellow;
            this.PointEndColor = Color.Green;
            this.PointExpandColor = Color.Yellow;
            this.FontColor = Color.White;
            this.MouseColor = Color.AliceBlue;
            this.Path = new Point[0];
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets a two dimensional array for all the interior horizontal walls of the maze, with the origin being the bottom left corner of the maze.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public WallState[,] HorizontalWalls
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a two dimensional array for all the interior vertical walls of the maze, with the origin being the bottom left corner of the maze.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public WallState[,] VerticalWalls
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a two dimensional array for all the cells that have been visited in the maze, with the origin being the bottom left corner of the maze.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Boolean[,] VisitedCells
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the path for the maze as an array of points with the origin of the maze in the lower left corner.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Point[] Path
        {
            get
            {
                return this.path;
            }

            set
            {
                this.path = value;
                this.Refresh();
            }
        }

        /// <summary>
        /// Gets or sets the number of rows for this maze.
        /// </summary>
        [Category("Design")]
        [Description("The number of rows in the maze control.")]
        public int NumberOfRows
        {
            get
            {
                return this.numberOfRows;
            }

            set
            {
                this.numberOfRows = value;
                if (this.NumberOfRows > 0 && this.NumberOfColumns > 0)
                {
                    this.HorizontalWalls = new WallState[this.NumberOfRows + 1, this.NumberOfColumns];
                    this.VerticalWalls = new WallState[this.NumberOfRows, this.NumberOfColumns + 1];
                    this.VisitedCells = new Boolean[this.NumberOfRows, this.NumberOfColumns];

                    this.DrawBorderWalls();
                }
            }
        }

        /// <summary>
        /// Gets or sets the number of columns for this maze.
        /// </summary>
        [Category("Design")]
        [Description("The number of columns in the maze control.")]
        public int NumberOfColumns
        {
            get
            {
                return this.numberOfColumns;
            }

            set
            {
                this.numberOfColumns = value;
                if (this.NumberOfRows > 0 && this.NumberOfColumns > 0)
                {
                    this.HorizontalWalls = new WallState[this.NumberOfRows + 1, this.NumberOfColumns];
                    this.VerticalWalls = new WallState[this.NumberOfRows, this.NumberOfColumns + 1];
                    this.VisitedCells = new Boolean[this.NumberOfRows, this.NumberOfColumns];

                    this.DrawBorderWalls();
                }
            }
        }

        /// <summary>
        /// Gets or sets the color for the pegs.
        /// </summary>
        [Category("Appearance")]
        [Description("The color of the pegs.")]
        public Color PegColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the color for the mouse.
        /// </summary>
        [Category("Appearance")]
        [Description("The color of the mouse.")]
        public Color MouseColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the wall color.
        /// </summary>
        [Category("Appearance")]
        [Description("The color of the detected walls.")]
        public Color WallColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the wall is drawn as a filled rectangle.
        /// </summary>
        [Category("Appearance")]
        [Description("A value indicating whether the wall is drawn as a filled rectangle.")]
        public bool WallFilled
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the no-wall color.
        /// </summary>
        [Category("Appearance")]
        [Description("The color of the detected no-walls.")]
        public Color NoWallColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the no-wall rectangle is drawn as a filled rectangle.
        /// </summary>
        [Category("Appearance")]
        [Description("A value indicating whether the no-wall is drawn as a filled rectangle.")]
        public bool NoWallFilled
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the corrected wall color.
        /// </summary>
        [Category("Appearance")]
        [Description("The color of the corrected walls.")]
        public Color CorrectedWallColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the corrected wall is drawn as a filled rectangle.
        /// </summary>
        [Category("Appearance")]
        [Description("A value indicating whether the corrected wall is drawn as a filled rectangle.")]
        public bool CorrectedWallFilled
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the corrected no-wall color.
        /// </summary>
        [Category("Appearance")]
        [Description("The color of the corrected no-walls.")]
        public Color CorrectedNoWallColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the corrected no-wall rectangle is drawn as a filled rectangle.
        /// </summary>
        [Category("Appearance")]
        [Description("A value indicating whether the corrected no-wall is drawn as a filled rectangle.")]
        public bool CorrectedNoWallFilled
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the not mapped color.
        /// </summary>
        [Category("Appearance")]
        [Description("The color of the not-mapped walls.")]
        public Color NotMappedColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the not-mapped rectangle is drawn as a filled rectangle.
        /// </summary>
        [Category("Appearance")]
        [Description("A value indicating whether the not-mapped edge is drawn as a filled rectangle.")]
        public bool NotMappedFilled
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the not mapped no wall color.
        /// </summary>
        [Category("Appearance")]
        [Description("The color of the not-mapped no walls.")]
        public Color NotMappedNoWallColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the not-mapped no wall rectangle is drawn as a filled rectangle.
        /// </summary>
        [Category("Appearance")]
        [Description("A value indicating whether the not-mapped no wall is drawn as a filled rectangle.")]
        public bool NotMappedNoWallFilled
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the not mapped wall color.
        /// </summary>
        [Category("Appearance")]
        [Description("The color of the not-mapped wall.")]
        public Color NotMappedWallColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the not-mapped wall rectangle is drawn as a filled rectangle.
        /// </summary>
        [Category("Appearance")]
        [Description("A value indicating whether the not-mapped wall is drawn as a filled rectangle.")]
        public bool NotMappedWallFilled
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the path color.
        /// </summary>
        [Category("Appearance")]
        [Description("The color of the paths.")]
        public Color PathColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the solver expansion path end point color.
        /// </summary>
        [Category("Appearance")]
        [Description("The color of solver expansion path end point.")]
        public Color PointEndColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the solver expansion point color.
        /// </summary>
        [Category("Appearance")]
        [Description("The color of solver expansion point.")]
        public Color PointExpandColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the font color.
        /// </summary>
        [Category("Appearance")]
        [Description("The color of the fonts.")]
        public Color FontColor
        {
            get;
            set;
        }

        /// <summary>
        /// Adds cell to display and indicate that it has been visited. This is used when we load in a maze and then run it.
        /// </summary>
        /// <param name="cellCoordinate">The cell coordinate nibbles, in 0xYX format.</param>
        public void vAddCellVisited(byte cellCoordinate)
        {
            Point coordinate = new Point(cellCoordinate & 0x0F, (cellCoordinate >> 4) & 0x0F);

            this.VisitedCells[coordinate.Y, coordinate.X] = true;

            // change the cell's walls to indicate it has been mapped
            WallFlags walls = WallFlags.MappedNorth |
                                WallFlags.MappedEast |
                                WallFlags.MappedSouth |
                                WallFlags.MappedWest;

            UpdateWallsWithRefresh(cellCoordinate, walls);
        }

        /// <summary>
        /// Draws the mouse in the specified cell.
        /// </summary>
        /// <param name="cellCoordinate">The cell coordinate nibbles, in 0xYX format.</param>
        public void DrawMouse(byte cellCoordinate)
        {
            this.mouseCoord = new Point(cellCoordinate & 0x0F, (cellCoordinate >> 4) & 0x0F);

            this.Refresh();
        }

        /// <summary>
        /// Draws a line between the two points we are expanding.
        /// </summary>
        /// <param name="FromX, FromY">The X and Y coordinates of the cell we are expanding from.</param>
        /// <param name="ToX, ToY">The X and Y coordinates of the cell we are expanding to.</param>
        public void DrawExpandLine(int FromX, int FromY, int ToX, int ToY)
        {
            this.ExpandLine[0].X = FromX;
            this.ExpandLine[0].Y = FromY;
            this.ExpandLine[1].X = ToX;
            this.ExpandLine[1].Y = ToY;

            this.Refresh();
        }

        /// <summary>
        /// Draws the last cell in the path.
        /// </summary>
        /// <param name="X, Y">Even integers are the X coordinate and Odd integers are the corresponding Y coordinate. The coordinates include the cell and the pegs.</param>
        public void DrawEndPoint(int X, int Y)
        {
            this.EndPoint.X = X;
            this.EndPoint.Y = Y;

            this.Refresh();
        }

        /// <summary>
        /// Draws the solver path from an array of integers.
        /// The origin of the maze in the lower left corner.
        /// </summary>
        /// <param name="coordArray">The path coordinate integers. Even integers are the X coordinate and Odd integers are the corresponding Y coordinate. The coordinates include the cell and the pegs.</param>
        /// <param name="coordQty">The number of coordinate pairs.</param>
        public void DrawExpandPath(IntPtr coordArray, int coordQty)
        {
            Marshal.Copy(coordArray, this.coordCopy, 0, coordQty * 2);

            Point[] newCoord = new Point[coordQty];

            for (int i = 0, j = 0; j < coordQty; i += 2, j++)
            {
                newCoord[j].X = coordCopy[i];
                newCoord[j].Y = coordCopy[i + 1];
            }

            this.coord = newCoord;

            // update coordinate path on the display
            this.Refresh();
        }

        /// <summary>
        /// Draws the cell being expanded.
        /// </summary>
        /// <param name="X, Y">Even integers are the X coordinate and Odd integers are the corresponding Y coordinate. The coordinates include the cell and the pegs.</param>
        public void DrawExpandPoint(int X, int Y)
        {
            this.ExpandPoint.Add(new Point(X, Y));

            this.Refresh();
        }

        /// <summary>
        /// Remove a cell that has been expanded.
        /// </summary>
        /// <param name=""></param>
        public void RemoveExpandPoint()
        {
            if (this.ExpandPoint.Count != 0)
            {
                this.ExpandPoint.RemoveAt(0);
                this.Refresh();
            }
        }

        /// <summary>
        /// Sets the path from an array of coordinate nibbles in 0xYX format, with the origin of the maze in the lower left corner.
        /// </summary>
        /// <param name="pathCoordinates">The path coordinates nibbles, in 0xYX format.</param>
        /// <param name="pathLength">The path length.</param>
        public void SetPathFromCoordinateNibble(byte[] pathCoordinates, byte pathLength)
        {
            Point[] newPath = new Point[pathLength];
            for (int i = 0; i < pathLength; i++)
            {
                newPath[i] = new Point(pathCoordinates[i] & 0x0F, (pathCoordinates[i] >> 4) & 0x0F);
            }

            this.Path = newPath;
        }

        /// <summary>
        /// Displays the cost on the maze
        /// </summary>
        /// <param name="cost">The cost for each cell.</param>
        /// <param name="costQty">The number of cost elements.</param>
        public void SetCost(IntPtr costArray, int costQty)
        {

            Marshal.Copy(costArray, this.cost, 0, costQty);

            // update cost info. on maze display
            this.Refresh();
        }

        /// <summary>
        /// Reset the wall states to all unmapped but don't refresh the display.
        /// </summary>
        public void ResetWallsNoRefresh()
        {
            for (int row = 0; row < this.NumberOfRows; row++)
            {
                for (int column = 0; column < this.NumberOfColumns; column++)
                {
                    if (row != 0)
                    {
                        this.HorizontalWalls[row, column] = WallState.NotMapped;
                    }

                    if (column != 0)
                    {
                        this.VerticalWalls[row, column] = WallState.NotMapped;
                    }

                    this.VisitedCells[row, column] = false;
                }
            }

            return;
        }

        /// <summary>
        /// Reset the wall states to all unmapped.
        /// </summary>
        public void ResetWalls()
        {
            // update maze to empty state
            ResetWallsNoRefresh();

            // update the display
            this.Refresh();
        }

        /// <summary>
        /// Updates the wall states from a coordinate nibble and wall state but doesn't refresh the display
        /// I'm not sure what we use this routine for
        /// </summary>
        /// <param name="coordinateNibble">The coordinate nibble, in 0xYX format.</param>
        /// <param name="wallFlags">The wall flags.</param>
        public void UpdateNotMappedWallsNoRefresh(byte coordinateNibble, WallFlags wallFlags)
        {
            Point coordinate = new Point(coordinateNibble & 0x0F, (coordinateNibble >> 4) & 0x0F);

            // South wall
            this.HorizontalWalls[coordinate.Y, coordinate.X] = GetNotMappedWallState(
                this.HorizontalWalls[coordinate.Y, coordinate.X],
                (wallFlags & WallFlags.MappedSouth) != 0,
                (wallFlags & WallFlags.SouthWall) != 0);

            // North wall
            this.HorizontalWalls[coordinate.Y + 1, coordinate.X] = GetNotMappedWallState(
                this.HorizontalWalls[coordinate.Y + 1, coordinate.X],
                (wallFlags & WallFlags.MappedNorth) != 0,
                (wallFlags & WallFlags.NorthWall) != 0);

            // West wall
            this.VerticalWalls[coordinate.Y, coordinate.X] = GetNotMappedWallState(
                this.VerticalWalls[coordinate.Y, coordinate.X],
                (wallFlags & WallFlags.MappedWest) != 0,
                (wallFlags & WallFlags.WestWall) != 0);

            // East wall
            this.VerticalWalls[coordinate.Y, coordinate.X + 1] = GetNotMappedWallState(
                this.VerticalWalls[coordinate.Y, coordinate.X + 1],
                (wallFlags & WallFlags.MappedEast) != 0,
                (wallFlags & WallFlags.EastWall) != 0);
        }

        /// <summary>
        /// Updates the wall states from a coordinate nibble and wall state and refreshes the display
        /// </summary>
        /// <param name="coordinateNibble">The coordinate nibble, in 0xYX format.</param>
        /// <param name="wallFlags">The wall flags.</param>
        public void UpdateWallsNoRefresh(byte coordinateNibble, WallFlags wallFlags)
        {
            Point coordinate = new Point(coordinateNibble & 0x0F, (coordinateNibble >> 4) & 0x0F);

            // South wall
            this.HorizontalWalls[coordinate.Y, coordinate.X] = GetNewWallState(
                this.HorizontalWalls[coordinate.Y, coordinate.X],
                (wallFlags & WallFlags.MappedSouth) != 0,
                (wallFlags & WallFlags.SouthWall) != 0);

            // North wall
            this.HorizontalWalls[coordinate.Y + 1, coordinate.X] = GetNewWallState(
                this.HorizontalWalls[coordinate.Y + 1, coordinate.X],
                (wallFlags & WallFlags.MappedNorth) != 0,
                (wallFlags & WallFlags.NorthWall) != 0);

            // West wall
            this.VerticalWalls[coordinate.Y, coordinate.X] = GetNewWallState(
                this.VerticalWalls[coordinate.Y, coordinate.X],
                (wallFlags & WallFlags.MappedWest) != 0,
                (wallFlags & WallFlags.WestWall) != 0);

            // East wall
            this.VerticalWalls[coordinate.Y, coordinate.X + 1] = GetNewWallState(
                this.VerticalWalls[coordinate.Y, coordinate.X + 1],
                (wallFlags & WallFlags.MappedEast) != 0,
                (wallFlags & WallFlags.EastWall) != 0);

            return;
        }

        /// <summary>
        /// Updates the wall states from a coordinate nibble and wall state and refreshes the display
        /// </summary>
        /// <param name="coordinateNibble">The coordinate nibble, in 0xYX format.</param>
        /// <param name="wallFlags">The wall flags.</param>
        public void UpdateWallsWithRefresh(byte coordinateNibble, WallFlags wallFlags)
        {
            UpdateWallsNoRefresh(coordinateNibble, wallFlags);

            // make sure walls are displayed after they are added
            this.Refresh();

            return;
        }

        /// <summary>
        /// Draws the control surface.
        /// </summary>
        /// <param name="e">The OnPaint arguments.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            const float PegToWallRatio = 0.2F;
            int numberOfPegsWide = this.NumberOfRows + 1;
            int numberOfPegsTall = this.NumberOfColumns + 1;
            float horizontalBorderInPixels = (int)(this.Width * 0.04);
            float verticalBorderInPixels = (int)(this.Height * 0.04);
            float mazeWidth = this.Width - (horizontalBorderInPixels * 2);
            float mazeHeight = this.Height - (verticalBorderInPixels * 2);

            float pegWidth = (mazeWidth * PegToWallRatio) / numberOfPegsWide;
            float pegHeight = (mazeHeight * PegToWallRatio) / numberOfPegsTall;
            float horizontalWallWidth = (mazeWidth * (1 - PegToWallRatio)) / this.NumberOfRows;
            float verticalWallHeight = (mazeHeight * (1 - PegToWallRatio)) / this.NumberOfColumns;
            List<Rectangle> pegRectangles = new List<Rectangle>(numberOfPegsWide * numberOfPegsTall);
            List<Rectangle> wallRectangles = new List<Rectangle>(this.NumberOfRows * this.NumberOfColumns);
            List<Rectangle> notMappedRectangles = new List<Rectangle>(this.NumberOfRows * this.NumberOfColumns);
            List<Rectangle> notMappedNoWallRectangles = new List<Rectangle>(this.NumberOfRows * this.NumberOfColumns);
            List<Rectangle> notMappedWallRectangles = new List<Rectangle>(this.NumberOfRows * this.NumberOfColumns);
            List<Rectangle> noWallRectangles = new List<Rectangle>(this.NumberOfRows * this.NumberOfColumns);
            List<Rectangle> correctedWallRectangles = new List<Rectangle>(this.NumberOfRows * this.NumberOfColumns);
            List<Rectangle> correctedNoWallRectangles = new List<Rectangle>(this.NumberOfRows * this.NumberOfColumns);
            SizeF pegSize = new SizeF(pegWidth, pegHeight);
            SizeF horizontalRectangleSize = new SizeF(horizontalWallWidth, pegSize.Height);
            SizeF verticalRectangleSize = new SizeF(pegSize.Width, verticalWallHeight);
            PointF mazeBottomLeft = new PointF(horizontalBorderInPixels, this.Height - verticalBorderInPixels);
            PointF pegBottomLeft = mazeBottomLeft - new SizeF(0, pegSize.Height);
            PointF rowBottomLeft = pegBottomLeft;

            for (int row = 0; row < this.NumberOfRows + 1; row++)
            {
                for (int column = 0; column < this.NumberOfColumns + 1; column++)
                {
                    // Add the top left peg
                    pegRectangles.Add(new Rectangle(new Point((int)pegBottomLeft.X, (int)pegBottomLeft.Y), new Size((int)pegWidth, (int)pegHeight)));

                    if (column < this.NumberOfColumns)
                    {
                        Rectangle rectangle = new Rectangle((int)(pegBottomLeft.X + pegSize.Width), (int)pegBottomLeft.Y, (int)horizontalRectangleSize.Width, (int)horizontalRectangleSize.Height);
                        WallState wallstate = this.HorizontalWalls[row, column];
                        switch (wallstate)
                        {
                            case WallState.Wall:
                                wallRectangles.Add(rectangle);
                                break;
                            case WallState.NoWall:
                                noWallRectangles.Add(rectangle);
                                break;
                            case WallState.CorrectedWall:
                                correctedWallRectangles.Add(rectangle);
                                break;
                            case WallState.CorrectedNoWall:
                                correctedNoWallRectangles.Add(rectangle);
                                break;
                            case WallState.NotMapped:
                                notMappedRectangles.Add(rectangle);
                                break;
                            case WallState.NotMappedNoWall:
                                notMappedNoWallRectangles.Add(rectangle);
                                break;
                            case WallState.NotMappedWall:
                                notMappedWallRectangles.Add(rectangle);
                                break;
                            default:
                                throw new InvalidOperationException(string.Format("Unrecognzied value for wall state '{0}'.", wallstate));
                        }
                    }

                    if (row < this.NumberOfRows)
                    {
                        Rectangle rectangle = new Rectangle((int)pegBottomLeft.X, (int)(pegBottomLeft.Y - verticalRectangleSize.Height), (int)verticalRectangleSize.Width, (int)verticalRectangleSize.Height);
                        WallState wallstate = this.VerticalWalls[row, column];
                        switch (wallstate)
                        {
                            case WallState.Wall:
                                wallRectangles.Add(rectangle);
                                break;
                            case WallState.NoWall:
                                noWallRectangles.Add(rectangle);
                                break;
                            case WallState.CorrectedWall:
                                correctedWallRectangles.Add(rectangle);
                                break;
                            case WallState.CorrectedNoWall:
                                correctedNoWallRectangles.Add(rectangle);
                                break;
                            case WallState.NotMapped:
                                notMappedRectangles.Add(rectangle);
                                break;
                            case WallState.NotMappedNoWall:
                                notMappedNoWallRectangles.Add(rectangle);
                                break;
                            case WallState.NotMappedWall:
                                notMappedWallRectangles.Add(rectangle);
                                break;
                            default:
                                throw new InvalidOperationException(string.Format("Unrecognzied value for wall state '{0}'.", wallstate));
                        }
                    }

                    pegBottomLeft += new SizeF(pegSize.Width + horizontalRectangleSize.Width, 0);
                }

                pegBottomLeft = rowBottomLeft = rowBottomLeft - new SizeF(0, pegSize.Height + verticalRectangleSize.Height);
            }

            e.Graphics.Clear(this.BackColor);
            this.DrawRectangles(e.Graphics, this.WallFilled, this.WallColor, wallRectangles);
            this.DrawRectangles(e.Graphics, this.NoWallFilled, this.NoWallColor, noWallRectangles);
            this.DrawRectangles(e.Graphics, this.CorrectedWallFilled, this.CorrectedWallColor, correctedWallRectangles);
            this.DrawRectangles(e.Graphics, this.CorrectedNoWallFilled, this.CorrectedNoWallColor, correctedNoWallRectangles);
            this.DrawRectangles(e.Graphics, this.NotMappedFilled, this.NotMappedColor, notMappedRectangles);
            this.DrawRectangles(e.Graphics, this.NotMappedNoWallFilled, this.NotMappedNoWallColor, notMappedNoWallRectangles);
            this.DrawRectangles(e.Graphics, this.NotMappedWallFilled, this.NotMappedWallColor, notMappedWallRectangles);
            this.DrawRectangles(e.Graphics, true, this.PegColor, pegRectangles);

            if (this.Path != null)
            {
                List<PointF> pathLines = new List<PointF>(this.Path.Length);
                for (int i = 0; i < this.Path.Length; i++)
                {
                    pathLines.Add(
                        new PointF(
                            (((float)this.Path[i].X) * (horizontalRectangleSize.Width + pegSize.Width)) + horizontalBorderInPixels + pegSize.Width + (horizontalRectangleSize.Width / 2),
                            (((float)(this.NumberOfRows - this.Path[i].Y - 1)) * (verticalRectangleSize.Height + pegSize.Height)) + verticalBorderInPixels + pegSize.Height + (verticalRectangleSize.Height / 2)));
                }

                // must have two points to draw a line!
                if (pathLines.Count >= 2)
                {
                    e.Graphics.DrawLines(new Pen(new SolidBrush(this.PathColor), 4), pathLines.ToArray());
                }

                Font font = new Font(FontFamily.GenericMonospace, 10, FontStyle.Regular, GraphicsUnit.Pixel);
                for (int i = 0; i < pathLines.Count; i++)
                {
                    e.Graphics.DrawString(i.ToString(), font, new SolidBrush(this.FontColor), pathLines[i] + new SizeF(2, 2));
                }
            }

            if ((this.coord != null) && (this.coord.Length >= 2))
            {
                List<PointF> coordLines = new List<PointF>(this.coord.Length);
                for (int i = 0; i < this.coord.Length; i++)
                {
                    coordLines.Add(
                        new PointF(
                            (((((float)(this.coord[i].X - 1)) / 2) * (horizontalRectangleSize.Width + pegSize.Width)) + horizontalBorderInPixels + pegSize.Width + (horizontalRectangleSize.Width / 2)),
                            ((((float)(this.NumberOfRows * 2 - this.coord[i].Y - 1)) / 2)  * (verticalRectangleSize.Height + pegSize.Height)) + verticalBorderInPixels + pegSize.Height + (verticalRectangleSize.Height / 2)));
                }

                // e.Graphics.DrawLines(new Pen(new SolidBrush(this.PathColor), 4), coordLines.ToArray());
                e.Graphics.DrawCurve(new Pen(new SolidBrush(this.PathColor), 4), coordLines.ToArray(), 0.25F);
                // e.Graphics.DrawBeziers(new Pen(new SolidBrush(this.PathColor), 4), coordLines.ToArray());
#if DEBUG_DISPLAY_PATH_COORD
                Font font = new Font(FontFamily.GenericMonospace, 10, FontStyle.Regular, GraphicsUnit.Pixel);
                for (int i = 0; i < coordLines.Count; i++)
                {
                    e.Graphics.DrawString(i.ToString(), font, new SolidBrush(this.FontColor), coordLines[i] + new SizeF(2, 2));
                }
#endif
            }

            int mouseX, mouseY;

            mouseX = (int)((((float)mouseCoord.X) * (horizontalRectangleSize.Width + pegSize.Width)) + horizontalBorderInPixels + (pegSize.Width + horizontalRectangleSize.Width) / 2);
            mouseY = (int)((((float)(this.NumberOfRows - mouseCoord.Y - 1)) * (verticalRectangleSize.Height + pegSize.Height)) + verticalBorderInPixels + (pegSize.Height + verticalRectangleSize.Height) / 2);

            Rectangle mouseRectangle = new Rectangle(mouseX - (int)(pegWidth / 2), mouseY - (int)(pegHeight / 2), (int)(pegWidth * 2), (int)(pegHeight * 2));

            e.Graphics.FillRectangle(new SolidBrush(this.MouseColor), mouseRectangle);

            // draw end point and expansion points
            int pointX, pointY;

            pointX = (int)((((float)EndPoint.X - 1) / 2 * (horizontalRectangleSize.Width + pegSize.Width)) + horizontalBorderInPixels + (pegSize.Width + horizontalRectangleSize.Width) / 2);
            pointY = (int)((((float)(this.NumberOfRows * 2 - EndPoint.Y - 1)) / 2 * (verticalRectangleSize.Height + pegSize.Height)) + verticalBorderInPixels + (pegSize.Height + verticalRectangleSize.Height) / 2);

            Rectangle pointRectangle = new Rectangle(pointX, pointY, (int)pegWidth, (int)pegHeight);

            e.Graphics.FillRectangle(new SolidBrush(this.PointEndColor), pointRectangle);

            pointRectangle.Width = (int)(pegWidth / 2);
            pointRectangle.Height = (int)(pegHeight / 2);

            if (ExpandPoint.Count != 0)
            {
                foreach (Point point in ExpandPoint)
                {
                    // if this is a cell, then put it at the bottom of the cell; for a wall, leave it where it is
                    PointF Offset = new Point( 0, 0 );

                    if ((point.X & 0x01) == 0)
                    {
                        Offset.X = pegWidth / 2;
                    }
                    else
                    {
                        // Offset.X = horizontalWallWidth / 2;
                    }

                    if ((point.Y & 0x01) == 1)
                    {
                        Offset.Y = 0; // verticalWallHeight / 4;
                    }

                    pointRectangle.X = (int)(((float)point.X / 2 * (horizontalRectangleSize.Width + pegSize.Width)) + horizontalBorderInPixels + Offset.X);
                    pointRectangle.Y = (int)((((float)((NumberOfRows * 2) - point.Y - 1)) / 2 * (verticalRectangleSize.Height + pegSize.Height)) + verticalBorderInPixels + ((pegSize.Height + verticalRectangleSize.Height) / 2) + Offset.Y);

                    e.Graphics.FillRectangle(new SolidBrush(this.PointExpandColor), pointRectangle);
                }
            }

            int LineFromX, LineFromY, LineToX, LineToY;

            LineFromX = (int)((((float)ExpandLine[0].X) * (horizontalRectangleSize.Width + pegSize.Width)) + horizontalBorderInPixels + (pegSize.Width + horizontalRectangleSize.Width) / 2);
            LineFromY = (int)((((float)(this.NumberOfRows - ExpandLine[0].Y - 1)) * (verticalRectangleSize.Height + pegSize.Height)) + verticalBorderInPixels + (pegSize.Height + verticalRectangleSize.Height) / 2);
            LineToX = (int)((((float)ExpandLine[1].X) * (horizontalRectangleSize.Width + pegSize.Width)) + horizontalBorderInPixels + (pegSize.Width + horizontalRectangleSize.Width) / 2);
            LineToY = (int)((((float)(this.NumberOfRows - ExpandLine[1].Y - 1)) * (verticalRectangleSize.Height + pegSize.Height)) + verticalBorderInPixels + (pegSize.Height + verticalRectangleSize.Height) / 2);

            Point[] Line =
                        {
                            new Point(LineFromX, LineFromY),
                            new Point(LineToX, LineToY)
                        };

            e.Graphics.DrawLines(new Pen(new SolidBrush(this.PathColor), 2), Line);

            // code to show cost on maze

            if (cost != null)
            {
                int costQty = (this.NumberOfColumns * this.NumberOfRows);
                PointF costLocation = new PointF(0, 0);
                Font font = new Font(FontFamily.GenericMonospace, 10, FontStyle.Regular, GraphicsUnit.Pixel);
                SolidBrush brush = new SolidBrush(this.FontColor);

                for (int cellX = 0; cellX < this.NumberOfRows; cellX++)
                {
                    for (int cellY = 0; cellY < this.NumberOfColumns; cellY++)
                    {
                        // BUGBUG: Do loop above using the eqn. below
                        costLocation.X = ((((float)cellX) * (horizontalRectangleSize.Width + pegSize.Width)) + horizontalBorderInPixels + (pegSize.Width + horizontalRectangleSize.Width) / 2);
                        costLocation.Y = ((((float)(this.NumberOfRows - cellY - 1)) * (verticalRectangleSize.Height + pegSize.Height)) + verticalBorderInPixels + (pegSize.Height + verticalRectangleSize.Height) / 2);

                        costLocation.X -= 1.5F * pegSize.Width;
                        costLocation.Y += pegSize.Height;

                        // The solver code currently organizes the data in column major format
                        int i = cellY + cellX * this.NumberOfColumns;

                        e.Graphics.DrawString(Convert.ToString(cost[i], 16), font, brush, costLocation);
                    }
                }
            }

            // code to show cell coordinates on left and bottom of maze
            // only show if the maze is a square
            if (this.numberOfColumns == this.numberOfRows)
            {
                int nodeQty = this.NumberOfColumns;
                PointF nodeHorizLoc = new PointF(0, 0);
                PointF nodeVertLoc = new PointF(0, 0);
                Font font = new Font(FontFamily.GenericMonospace, 10, FontStyle.Regular, GraphicsUnit.Pixel);
                SolidBrush brush = new SolidBrush(this.FontColor);

                for (int node = 0; node < nodeQty; node++)
                {
                    nodeHorizLoc.X = (((float)node) * (horizontalRectangleSize.Width + pegSize.Width)) + horizontalBorderInPixels - pegSize.Width / 2;
                    nodeHorizLoc.Y = ((((float)(this.NumberOfRows)) * (verticalRectangleSize.Height + pegSize.Height)) + verticalBorderInPixels) + pegSize.Height; // + verticalRectangleSize.Height);

                    // show horizontal peg label
                    e.Graphics.DrawString(Convert.ToString(2 * node), font, brush, nodeHorizLoc);

                    // show horizontal wall label
                    nodeHorizLoc.X += pegSize.Width / 2 + horizontalRectangleSize.Width / 2;
                    e.Graphics.DrawString(Convert.ToString(2 * node + 1), font, brush, nodeHorizLoc);

                    nodeVertLoc.X = (float) horizontalBorderInPixels / 3;
                    nodeVertLoc.Y = (((float)this.NumberOfRows - node) * (verticalRectangleSize.Height + pegSize.Height)) + verticalBorderInPixels - pegSize.Height; // / 2;

                    // show vertical peg label
                    e.Graphics.DrawString(Convert.ToString(2 * node), font, brush, nodeVertLoc);

                    // show vertical wall label
                    nodeVertLoc.Y -= pegSize.Width / 2 + horizontalRectangleSize.Width / 2;
                    e.Graphics.DrawString(Convert.ToString(2 * node + 1), font, brush, nodeVertLoc);
                }
            }

        }
        /// <summary>
        /// Gets the new not mapped wall state.
        /// </summary>
        /// <param name="previousState">The previous state.</param>
        /// <param name="mapped">Whether the edge is currently mapped.</param>
        /// <param name="wall">Whether the wall is currently detected.</param>
        /// <returns>The not mapped wall state.</returns>
        private static WallState GetNotMappedWallState(WallState previousState, bool mapped, bool wall)
        {
            if (mapped)
            {
                if (wall)
                {
                    return WallState.NotMappedWall;
                }
                else
                {
                    return WallState.NotMappedNoWall;
                }
            }
            else
            {
                return previousState;
            }
        }

        /// <summary>
        /// Gets the new wall state based upon the current and previous state.
        /// </summary>
        /// <param name="previousState">The previous state.</param>
        /// <param name="mapped">Whether the edge is currently mapped.</param>
        /// <param name="wall">Whether the wall is currently detected.</param>
        /// <returns>The new wall state.</returns>
        private static WallState GetNewWallState(WallState previousState, bool mapped, bool wall)
        {
            if (mapped)
            {
                if (wall)
                {
                    return (previousState == WallState.NotMapped || previousState == WallState.NotMappedWall || previousState == WallState.Wall) ? WallState.Wall : WallState.CorrectedWall;
                }
                else
                {
                    return (previousState == WallState.NotMapped || previousState == WallState.NotMappedNoWall || previousState == WallState.NoWall) ? WallState.NoWall : WallState.CorrectedNoWall;
                }
            }
            else
            {
                return previousState;
            }
        }

        /// <summary>
        /// Draws the rectangles per the specified settings.
        /// </summary>
        /// <param name="graphics">The graphics object.</param>
        /// <param name="filled">A value indicating whether the rectangle is filled.</param>
        /// <param name="color">The rectangle color.</param>
        /// <param name="rectangles">The rectangles.</param>
        private void DrawRectangles(Graphics graphics, bool filled, Color color, List<Rectangle> rectangles)
        {
            if (rectangles.Count == 0)
            {
                return;
            }

            if (filled)
            {
                graphics.FillRectangles(new SolidBrush(color), rectangles.ToArray());
            }
            else
            {
                graphics.DrawRectangles(new Pen(new SolidBrush(color), 1), rectangles.ToArray());
            }
        }

        /// <summary>
        /// Draws the border walls.
        /// </summary>
        private void DrawBorderWalls()
        {
            for (int row = 0; row < this.NumberOfRows + 1; row++)
            {
                for (int column = 0; column < this.NumberOfColumns + 1; column++)
                {
                    if ((row == 0 || row == this.NumberOfRows) && column < this.NumberOfColumns)
                    {
                        this.HorizontalWalls[row, column] = WallState.Wall;
                    }

                    if ((column == 0 || column == this.NumberOfColumns) && row < this.NumberOfRows)
                    {
                        this.VerticalWalls[row, column] = WallState.Wall;
                    }

                    if ((row < this.NumberOfRows) && (column < NumberOfColumns))
                    {
                        this.VisitedCells[row, column] = false;
                    }
                }
            }
        }
    }
}
