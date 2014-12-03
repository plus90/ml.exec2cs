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
        public Size Size { get; private set; }
        public Graphics Graphic { get; private set; }
        protected static Hashtable Gridlines { get; set; }
        const int CellSize = 100;
        /// <summary>
        /// Construct a grid
        /// </summary>
        /// <param name="size"></param>
        /// <param name="graphic"></param>
        public Grid(Size size, Graphics graphic)
        {
            this.Size = size;
            this.Graphic = graphic;
        }
        static Grid()
        {
            Gridlines = new Hashtable();
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
                }
            }
            return Grid.GetSizeOfGrid(this.Size);
        }
        /// <summary>
        /// Toggels UI blocks
        /// </summary>
        /// <param name="pb"></param>
        /// <param name="p"></param>
        public static void ToggleBlock(System.Windows.Forms.PictureBox pb, Point p)
        {
            var bm = (Bitmap)pb.Image;
            Color c = bm.GetPixel(p.X, p.Y);
            if (c.A != 255) return;
            using (Graphics g = pb.CreateGraphics())
            {
                foreach (var line in Grid.getLineLocation(bm, p))
                {
                    var sig = String.Format("{0}{1}", line.Key.X / CellSize + line.Value.X / CellSize, (line.Value.Y / CellSize) + (line.Key.Y / CellSize));
                    Pen pen = new Pen(Color.Black, 10);
                    Color flag = Color.Black;
                    if (Gridlines.Contains(sig))
                    {
                        pen.Color = Color.FromKnownColor(KnownColor.Control);
                        g.DrawLine(pen, line.Key, line.Value);
                        flag = Color.FromArgb(0, 240, 240, 240);
                        pen = new Pen(Color.Black, 1);
                        Gridlines.Remove(sig);
                    }
                    else Gridlines.Add(sig, 1);
                    g.DrawLine(pen, line.Key, line.Value);
                }
            }
        }
        /// <summary>
        /// Get the point relared line's locations
        /// </summary>
        /// <param name="bm"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        protected static List<KeyValuePair<Point, Point>> getLineLocation(Bitmap bm, Point p)
        {
            Point hl = p, hu = p, vl = p, vu = p;
            for (int i = hl.X; i >= 0 && i >= hl.X - CellSize; i--)
            {
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
                if (p.Y > 10)
                    t += bm.GetPixel(p.X - 10, i).A;
                if (p.Y < bm.Height - 10)
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
                if (p.Y > 10)
                    t += bm.GetPixel(p.X - 10, i).A;
                if (p.Y < bm.Height - 10)
                    t += bm.GetPixel(p.X + 10, i).A;
                if (t > 0)
                {
                    vu = new Point(p.X, i);
                    break;
                }
            }
            List<KeyValuePair<Point, Point>> l = new List<KeyValuePair<Point,Point>>();
            if(hl != hu)
                l.Add(new KeyValuePair<Point,Point>(hl, hu));
            if(vl != vu)
                l.Add(new KeyValuePair<Point,Point>(vl, vu));
            return l;
        }
        /// <summary>
        /// Get size of grid in bitmap scale
        /// </summary>
        /// <param name="Size"></param>
        /// <returns></returns>
        public static Size GetSizeOfGrid(Size Size) { return new Size(Size.Height * CellSize, Size.Width * CellSize); }
        /// <summary>
        /// Draw an empty box
        /// </summary>
        /// <param name="p"></param>
        protected void __drawBox(Point p)
        {
            Pen pen = new Pen(Color.Black);
            Graphic.DrawLine(pen, p.X * CellSize, p.Y * CellSize, (p.X + 1) * CellSize, (p.Y) * CellSize);
            Graphic.DrawLine(pen, p.X * CellSize, p.Y * CellSize, p.X * CellSize, (p.Y + 1) * CellSize);
            Graphic.DrawLine(pen, (p.X + 1) * CellSize, p.Y * CellSize, (p.X + 1) * CellSize, (p.Y + 1) * CellSize);
            Graphic.DrawLine(pen, (p.X) * CellSize, (p.Y + 1) * CellSize, (p.X + 1) * CellSize, (p.Y + 1) * CellSize);
        }
        /// <summary>
        /// Dispose resources
        /// </summary>
        public void Dispose()
        {
            this.Graphic.Dispose();
        }
    }
}
