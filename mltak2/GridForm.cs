using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace mltak2
{
    public partial class GridForm : Form
    {
        protected Timer __NearBySearchTimer { get; set; }
        public GridForm()
        {
            InitializeComponent();
            /**
             * Draw the grid
             */
            Size s = Grid.GetSizeOfGrid(new Size(6, 6));
            Grid g = null;
            this.grid.Image = new Bitmap(s.Width, s.Height, this.grid.CreateGraphics());
            using (Graphics gfx = Graphics.FromImage(this.grid.Image))
            {
                g = new Grid(new Size(6, 6), gfx);
                g.Draw();
                this.Size = new Size(s.Width + 21, s.Height + 45);
            }
            /**
             * 
             */
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler((sender, e) =>
            {
                switch (e.KeyCode)
                {
                    case Keys.Escape: Application.Exit(); break;
                    case Keys.S: g.getStatus(this.grid); break;
                }
            }); 
            this.grid.MouseMove += new MouseEventHandler((sender, e) =>
            {
                if (this.__NearBySearchTimer != null) this.__NearBySearchTimer.Stop();
                else
                {
                    this.__NearBySearchTimer = new Timer();
                    this.__NearBySearchTimer.Interval = 15;
                    this.__NearBySearchTimer.Tick += new EventHandler((__sender, __e) =>
                    {
                        var p = (Point)this.__NearBySearchTimer.Tag;
                        if (this.getNearByLineInfo(this.grid, p).Value.A != 0)
                            Cursor.Current = Cursors.Hand;
                        else Cursor.Current = Cursors.Default;
                    });
                }
                if (this.getNearByLineInfo(this.grid, e.Location).Value.A != 0)
                    Cursor.Current = Cursors.Hand;
                this.__NearBySearchTimer.Tag = e.Location;
                this.__NearBySearchTimer.Start();
            });
            this.grid.MouseDown += new MouseEventHandler((sender, e) =>
            {
                var inf = this.getNearByLineInfo(this.grid, e.Location);
                if (inf.Value.A != 0)
                {
                    g.ToggleBlock(this.grid, inf.Key);
                    this.Invalidate();
                }
            });
        }
        protected KeyValuePair<Point, Color> getNearByLineInfo(PictureBox grip, Point e, uint margin = 7)
        {
            Bitmap b;
            int x = e.X, y = e.Y;
            KeyValuePair<Point, Color> color = new KeyValuePair<Point,Color>(new Point(0, 0), new Color());
            try
            {
                b = (Bitmap)grid.Image;
                color = new KeyValuePair<Point, Color>(new Point(x, y), b.GetPixel(x, y));
                bool exit = false;
                if (x == 0) x = (int)margin;
                if (y == 0) y = (int)margin;
                for (int i = x - (int)margin; i < x + margin; i++)
                {
                    for (int j = y - (int)margin; j < y + margin; j++)
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
    }
}