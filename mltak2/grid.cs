using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections;

namespace mltak2
{
    class Grid : IDisposable
    {
        [Flags]
        public enum BlockStatus { UNBLOCKED = 0x0, NORTH = 0x1, EAST = 0x2, SOUTH = 0x4, WEST = 0x8, BLOCKED = 0x10 }
        public Size Size { get; private set; }
        public Graphics Graphic { get; private set; }
        protected Hashtable Gridlines { get; set; }
        protected BlockStatus[,] __blockStatuses;
        public const int CellSize = 100;
        private bool __drawFromBlockStatuses = false;
        private System.Windows.Forms.PictureBox __pictureBox;
        public Point StartPoint { get; set; }
        public Point GoalPoint { get; set; }
        public Grid(BlockStatus[,] bs, System.Windows.Forms.PictureBox pb)
            : this(new Size(bs.GetLength(0), bs.GetLength(1)), pb.CreateGraphics())
        {
            this.__blockStatuses = bs;
            this.__drawFromBlockStatuses = true;
            this.__pictureBox = pb;
        }
        /// <summary>
        /// Construct a grid
        /// </summary>
        /// <param name="size"></param>
        /// <param name="graphic"></param>
        public Grid(Size size, Graphics graphic)
        {
            this.Size = size;
            this.Graphic = graphic;
            this.Gridlines = new Hashtable();
            __blockStatuses = new BlockStatus[(this.Size.Width), (this.Size.Height)];
            /**
             * Update the block status of borders of grid
             */
            for (int i = 0; i < this.Size.Width; i++)
            {
                this.__apply_block_status(i, 0, BlockStatus.NORTH);
                this.__apply_block_status(i, this.Size.Height - 1, BlockStatus.SOUTH);
            }
            for (int j = 0; j < this.Size.Width; j++)
            {
                this.__apply_block_status(0, j, BlockStatus.WEST);
                this.__apply_block_status(this.Size.Width - 1, j, BlockStatus.EAST);
            }
        }
        /// <summary>
        /// Draw a grid
        /// </summary>
        /// <returns></returns>
        public Size Draw()
        {
            for (int i = 0; i < this.Size.Width; i++)
            {
                for (int j = 0; j < this.Size.Height; j++)
                {
                    this.__drawBox(new Point(i, j));
                    /**
                     * Drawing related blockes to [i, j] cell based on loaded data
                     */
                    if (this.__drawFromBlockStatuses)
                    {
                        foreach (var block_type in Enum.GetValues(typeof(BlockStatus)))
                        {
                            if (this.IsBlocked(i, j, (BlockStatus)block_type))
                            {
                                switch ((BlockStatus)block_type)
                                {
                                    case BlockStatus.BLOCKED: /* Ingore the general block status */break;
                                    case BlockStatus.EAST:
                                        this.Block(this.__pictureBox, new Point((i + 1) * CellSize, j * CellSize + CellSize / 2), true);
                                        break;
                                    case BlockStatus.NORTH:
                                        {
                                            var b = this.__blockStatuses[i, j];
                                            this.Block(this.__pictureBox, new Point(i * CellSize + CellSize / 2, j * CellSize), true);
                                            this.__blockStatuses[i, j] = b;
                                        }
                                        break;
                                    case BlockStatus.SOUTH:
                                        this.Block(this.__pictureBox, new Point(i * CellSize + CellSize / 2, (j + 1) * CellSize), true);
                                        break;
                                    case BlockStatus.WEST:
                                        {
                                            var b = this.__blockStatuses[i, j];
                                            this.Block(this.__pictureBox, new Point(i * CellSize, j * CellSize + CellSize / 2), true);
                                            this.__blockStatuses[i, j] = b;
                                        }
                                        break;
                                    case BlockStatus.UNBLOCKED:
                                        this.UnBlock(this.__pictureBox, new Point(i * CellSize + CellSize / 2, j * CellSize + CellSize / 2), true);
                                        break;
                                    default:
                                        throw new InvalidProgramException("Undefined block-status");
                                }
                            }
                        }
                    }
                }
            }
            this.Dispose();
            return Grid.GetSizeOfGrid(this.Size);
        }
        /// <summary>
        /// 
        /// <param name="bp">The picture box which the changes will be applied to</param>
        /// </summary>
        public void BlockBorders(System.Windows.Forms.PictureBox pb)
        {
            for (int i = 0; i < this.Size.Width; i++)
            {
                this.Block(pb, new Point(0, i * CellSize + CellSize / 2), true, false);
                this.__apply_block_status(0, i, BlockStatus.WEST);
                this.Block(pb, new Point(i * CellSize + CellSize / 2, 0), true, false);
                this.__apply_block_status(i, 0, BlockStatus.NORTH);
            }
            for (int i = 0; i < this.Size.Height; i++)
            {
                this.Block(pb, new Point(this.Size.Width * CellSize, i * CellSize + CellSize / 2), true);
                this.Block(pb, new Point(i * CellSize + CellSize / 2, this.Size.Height * CellSize), true);
            }
        }
        /// <summary>
        /// Toggels UI blocks
        /// </summary>
        /// <param name="bp">The picture box which the changes will be applied to</param>
        /// <param name="p">A point in the bitmap to extract the line it has been linked</param>
        /// <param name="auto_update_status">Shoud it automatically update the grid's block status?</param>
        public void ToggleBlock(System.Windows.Forms.PictureBox pb, Point p, bool force = false, bool auto_update_status = true)
        {
            var bm = (Bitmap)pb.Image;
            if (!force && (p.X == bm.Width - 1 || p.Y == bm.Height - 1)) return;
            Color c = bm.GetPixel(p.X, p.Y);
            if (c.A != 255) return;
            foreach (var line in this.__get_line_location(bm, p))
            {
                if (this.Gridlines.Contains(line))
                {
                    this.UnBlock(pb, p, force, auto_update_status);
                }
                else
                {
                    this.Block(pb, p, force, auto_update_status);
                }
            }
        }
        /// <summary>
        /// Unblocks UI blocks
        /// </summary>
        /// <param name="bp">The picture box which the changes will be applied to</param>
        /// <param name="p">A point in the bitmap to extract the line it has been linked</param>
        /// <param name="auto_update_status">Shoud it automatically update the grid's block status?</param>
        public void UnBlock(System.Windows.Forms.PictureBox pb, Point p, bool force = false, bool auto_update_status = true)
        {
            var bm = (Bitmap)pb.Image;
            if (!force && (p.X == bm.Width - 1 || p.Y == bm.Height - 1)) return;
            Color c = bm.GetPixel(p.X, p.Y);
            if (c.A != 255) return;
            using (Graphics g = pb.CreateGraphics())
            {
                foreach (var line in this.__get_line_location(bm, p))
                {
                    if (this.Gridlines.Contains(line))
                    {
                        Pen pen = new Pen(Color.FromKnownColor(KnownColor.Control), 10);
                        g.DrawLine(pen, line.Key, line.Value);
                        pen = new Pen(Color.Black, 1);
                        g.DrawLine(pen, line.Key, line.Value);
                        this.Gridlines.Remove(line);
                        if (auto_update_status)
                            this.__update_block_status(line, BlockStatus.UNBLOCKED);
                    }
                }
            }
        }
        /// <summary>
        /// Blocks UI blocks
        /// </summary>
        /// <param name="bp">The picture box which the changes will be applied to</param>
        /// <param name="p">A point in the bitmap to extract the line it has been linked</param>
        /// <param name="auto_update_status">Shoud it automatically update the grid's block status?</param>
        public void Block(System.Windows.Forms.PictureBox pb, Point p, bool force = false, bool auto_update_status = true)
        {
            var bm = (Bitmap)pb.Image;
            if (!force && (p.X == bm.Width - 1 || p.Y == bm.Height - 1)) return;
            Color c = bm.GetPixel(p.X, p.Y);
            if (c.A != 255) return;
            using (Graphics g = pb.CreateGraphics())
            {
                foreach (var line in this.__get_line_location(bm, p))
                {
                    if (!this.Gridlines.Contains(line))
                    {
                        Pen pen = new Pen(Color.Black, 10);
                        this.Gridlines.Add(line, 1);
                        g.DrawLine(pen, line.Key, line.Value);
                        if (auto_update_status)
                            this.__update_block_status(line, BlockStatus.BLOCKED);
                    }
                }
            }
        }
        /// <summary>
        /// Get statuts of the grid
        /// </summary>
        /// <returns>Array of BlockStatus</returns>
        public BlockStatus[,] GetStatus() { return __blockStatuses; }
        /// <summary>
        /// Get graphical size of grid in bitmap scale
        /// </summary>
        /// <param name="Size">The abstract size of grid</param>
        /// <returns>The actual graphical size of grid</returns>
        public static Size GetSizeOfGrid(Size Size) { return new Size(Size.Height * CellSize, Size.Width * CellSize); }
        /// <summary>
        /// Checkes if a grid-cell is blocked
        /// </summary>
        /// <param name="x">The grid cell's X location</param>
        /// <param name="y">The grid cell's Y location</param>
        /// <param name="blockStatus">The type of block status to validate</param>
        /// <returns>Returns true if the cell is blocked by the gived status; Otherwise false</returns>
        private bool IsBlocked(int x, int y, BlockStatus blockStatus = BlockStatus.BLOCKED)
        {
            return __blockStatuses[x, y].HasFlag(blockStatus);
        }
        /// <summary>
        /// Dispose resources
        /// </summary>
        public void Dispose()
        {
            this.Graphic.Dispose();
        }
        /// <summary>
        /// Get the nearby line's info on a grid media
        /// </summary>
        /// <param name="grid">The grid media</param>
        /// <param name="e">The nearby point</param>
        /// <param name="margin">The investigate margin value</param>
        /// <returns>The Point's info in the nearest line to the point</returns>
        public KeyValuePair<Point, Color> GetNearByLineInfo(System.Windows.Forms.PictureBox grid, Point e, uint margin = 7)
        {
            Bitmap b;
            int x = e.X, y = e.Y;
            KeyValuePair<Point, Color> color = new KeyValuePair<Point, Color>(new Point(0, 0), new Color());
            try
            {
                b = (Bitmap)grid.Image;
                color = new KeyValuePair<Point, Color>(new Point(x, y), b.GetPixel(x, y));
                bool exit = false;
                if (x == 0) x = (int)margin;
                if (y == 0) y = (int)margin;
                for (int i = x - (int)margin; i < x; i++)
                {
                    for (int j = y - (int)margin; j < y; j++)
                    {
                        if (b.GetPixel(i, j).A != 0)
                        {
                            color = new KeyValuePair<Point, Color>(new Point(i, j), b.GetPixel(i, j));
                            exit = true;
                            break;
                        }
                    }
                    if (exit) break;
                }
            }
            catch (ArgumentException exp) { }
            return color;
        }
        /// <summary>
        /// Write text on the grid
        /// </summary>
        /// <param name="text">The target text</param>
        /// <param name="point">The location in the grid(The Actual Location)</param>
        /// <param name="graphics">The grid's graphics</param>
        public void Write(String text, Point point, Graphics graphics, Brush brush)
        {
            using (Font myFont = new Font("Arial", 14))
            {
                graphics.DrawString(text, myFont, brush, point);
            }
        }
        /// <summary>
        /// Converts an abstract point to its actual and cell-centered grid point
        /// </summary>
        /// <param name="p">The abstract location</param>
        /// <returns>The actual and cell-centered grid point</returns>
        public Point abs2grid(Point p)
        {
            return new Point(p.X * CellSize + CellSize / 2, p.Y * CellSize + CellSize / 2);
        }
        /// <summary>
        /// Converts a grid point to its abstract point
        /// </summary>
        /// <param name="p">The grid location</param>
        /// <returns>The abstract point</returns>
        public Point grid2abs(Point p)
        {
            return new Point(p.X / CellSize, p.Y / CellSize);
        }
        /// <summary>
        /// Updates a block's status
        /// </summary>
        /// <param name="line">The line the has been modified</param>
        /// <param name="b">The initial block status of line, whether it has been {BLOCK} or {UNBLOCKED}</param>
        protected void __update_block_status(KeyValuePair<Point, Point> line, BlockStatus b)
        {
            int x = ((line.Key.X == line.Value.X) ? line.Key.X - CellSize / 2 : (line.Key.X + line.Value.X) / 2) / CellSize;
            int y = ((line.Key.Y == line.Value.Y) ? line.Key.Y - CellSize / 2 : (line.Key.Y + line.Value.Y) / 2) / CellSize;
            if (b == BlockStatus.UNBLOCKED)
                __apply_block_status(x, y, BlockStatus.UNBLOCKED);
            else if (line.Key.X == line.Value.X)
                __apply_block_status(x, y, BlockStatus.EAST);
            else if (line.Key.Y == line.Value.Y)
                __apply_block_status(x, y, BlockStatus.SOUTH);
            else throw new InvalidOperationException("Unexpected line close-up detected!");
        }
        /// <summary>
        /// Properly applies a block status to cell
        /// </summary>
        /// <param name="x">The grid cell's X location</param>
        /// <param name="y">The grid cell's Y location</param>
        /// <param name="blockStatus">The block status</param>
        private void __apply_block_status(int x, int y, BlockStatus blockStatus)
        {
            if (blockStatus == BlockStatus.UNBLOCKED)
                __blockStatuses[x, y] = blockStatus;
            else
                __blockStatuses[x, y] = __blockStatuses[x, y] | blockStatus;
        }
        /// <summary>
        /// Get the point relared line's locations
        /// </summary>
        /// <param name="bm">The bitmap which the line has drawn</param>
        /// <param name="p">A point in the bitmap to extract the line it has been linked</param>
        /// <returns>The list of lines which the point is linked to</returns>
        protected List<KeyValuePair<Point, Point>> __get_line_location(Bitmap bm, Point p)
        {
            Point hl = p, hu = p, vl = p, vu = p;
            for (int i = hl.X; i >= 0 && i >= hl.X - CellSize; i--)
            {
                if (i >= bm.Height) { hu = new Point(p.X, bm.Height - 1); break; }
                if (bm.GetPixel(i, p.Y).A == 0) break;
                var t = 0;
                if (p.Y > 10)
                    t += bm.GetPixel(i, p.Y - 10).A;
                if (p.Y < bm.Height - 10)
                    t += bm.GetPixel(i, p.Y + 10).A;
                if (t > 0)
                {
                    hl = new Point(i, p.Y);
                    break;
                }
            }
            for (int i = hu.X; i <= hu.X + CellSize; i++)
            {
                if (i >= bm.Width) { hu = new Point(bm.Width - 1, p.Y); break; }
                if (bm.GetPixel(i, p.Y).A == 0) break;
                var t = 0;
                if (p.Y > 10)
                    t += bm.GetPixel(i, p.Y - 10).A;
                if (p.Y < bm.Height - 10)
                    t += bm.GetPixel(i, p.Y + 10).A;
                if (t > 0)
                {
                    hu = new Point(i, p.Y);
                    break;
                }
            }
            for (int i = vl.Y; i >= 0 && i >= vl.Y - CellSize; i--)
            {
                if (bm.GetPixel(p.X, i).A == 0) break;
                var t = 0;
                if (p.X > 10)
                    t += bm.GetPixel(p.X - 10, i).A;
                if (p.X < bm.Height - 10)
                    t += bm.GetPixel(p.X + 10, i).A;
                if (t > 0)
                {
                    vl = new Point(p.X, i);
                    break;
                }
            }
            for (int i = vu.Y; i <= vu.Y + CellSize; i++)
            {
                if (i >= bm.Height) { vu = new Point(p.X, bm.Height - 1); break; }
                if (bm.GetPixel(p.X, i).A == 0) break;
                var t = 0;
                if (p.X > 10)
                    t += bm.GetPixel(p.X - 10, i).A;
                if (p.X < bm.Height - 10)
                    t += bm.GetPixel(p.X + 10, i).A;
                if (t > 0)
                {
                    vu = new Point(p.X, i);
                    break;
                }
            }
            List<KeyValuePair<Point, Point>> l = new List<KeyValuePair<Point, Point>>();
            if (hl != hu)
                l.Add(new KeyValuePair<Point, Point>(hl, hu));
            if (vl != vu)
                l.Add(new KeyValuePair<Point, Point>(vl, vu));
            return l;
        }
        /// <summary>
        /// Draw an empty box
        /// </summary>
        /// <param name="p"></param>
        private void __drawBox(Point p)
        {
            Pen pen = new Pen(Color.Black);

            // NORTH
            Graphic.DrawLine(pen, p.X * CellSize, p.Y * CellSize, (p.X + 1) * CellSize, (p.Y) * CellSize);
            // WEST
            Graphic.DrawLine(pen, p.X * CellSize, p.Y * CellSize, p.X * CellSize, (p.Y + 1) * CellSize);
            // EAST
            Graphic.DrawLine(pen, (p.X + 1) * CellSize, p.Y * CellSize, (p.X + 1) * CellSize, (p.Y + 1) * CellSize);
            // SOUTH
            Graphic.DrawLine(pen, (p.X) * CellSize, (p.Y + 1) * CellSize, (p.X + 1) * CellSize, (p.Y + 1) * CellSize);
        }
    }
}