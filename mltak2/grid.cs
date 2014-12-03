using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace mltak2
{
    class Grid : IDisposable
    {
        public Size Size { get; private set; }
        public Graphics Graphic { get; private set; }
        const int CellSize = 100;
        public Grid(Size size, Graphics graphic)
        {
            this.Size = size;
            this.Graphic = graphic;
        }
        protected static int[] bitmap;
        protected static void init_bitmap(Size Size)
        {
            var s = Size;
            bitmap = new int[(s.Height - 1) * s.Width];
        }
        public Size Draw()
        {
            init_bitmap(this.Size);
            for (int i = 0; i < this.Size.Width; i++)
            {
                for (int j = 0; j < this.Size.Height; j++)
                {
                    this.__drawBox(new Point(i, j));
                }
            }
            return Grid.GetSizeOfGrid(this.Size);
        }
        public static void ToggleBlock(System.Windows.Forms.PictureBox pb, Point p)
        {
            var bm = (Bitmap)pb.Image;
            Color c = bm.GetPixel(p.X, p.Y);
            if (c.A != 255) return;

            Graphics g = pb.CreateGraphics();
            foreach (var line in Grid.getLineLocation(bm, p))
            {
                int v = 0;
                System.Windows.Forms.MessageBox.Show(
                    String.Format("{0} {1}", line.Key.X / CellSize + line.Value.X / CellSize, (line.Value.Y / CellSize) + (line.Key.Y / CellSize)));
                Pen pen = new Pen(Color.Black, 10);
                Color flag = Color.Black;
                if (v != 0)
                {
                    pen.Color = Color.FromKnownColor(KnownColor.Control);
                    g.DrawLine(pen, line.Key, line.Value);
                    flag = Color.FromArgb(0, 240, 240, 240);
                    pen = new Pen(Color.Black, 1);
                }
                g.DrawLine(pen, line.Key, line.Value);
                for (int i = line.Key.X; i <= line.Value.X; i++)
                    for (int j = line.Key.Y; j <= line.Value.Y; j++)
                    {
                        ;
                    }
            }
            g.Dispose();
            //pb.Image = new Bitmap(bm.Width, bm.Height, g);
        }
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
        public static Size GetSizeOfGrid(Size Size) { return new Size(Size.Height * CellSize, Size.Width * CellSize); }
        protected void __drawBox(Point p)
        {
            Pen pen = new Pen(Color.Black);
            Graphic.DrawLine(pen, p.X * CellSize, p.Y * CellSize, (p.X + 1) * CellSize, (p.Y) * CellSize);
            Graphic.DrawLine(pen, p.X * CellSize, p.Y * CellSize, p.X * CellSize, (p.Y + 1) * CellSize);
            Graphic.DrawLine(pen, (p.X + 1) * CellSize, p.Y * CellSize, (p.X + 1) * CellSize, (p.Y + 1) * CellSize);
            Graphic.DrawLine(pen, (p.X) * CellSize, (p.Y + 1) * CellSize, (p.X + 1) * CellSize, (p.Y + 1) * CellSize);
        }

        public void Dispose()
        {
            this.Graphic.Dispose();
        }
    }
}
