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
        Grid g { get; set; }
        public GridForm()
        {
            InitializeComponent();
            /**
             * Draw the grid
             */
            Size gridSize = new Size(6, 6);
            Size s = Grid.GetSizeOfGrid(gridSize);
            this.grid.Image = new Bitmap(s.Width + 1, s.Height + 1, this.grid.CreateGraphics());
            using (Graphics gfx = Graphics.FromImage(this.grid.Image))
            {
                g = new Grid(gridSize, gfx);
                g.Draw();
                this.Size = new Size(s.Width + 21, s.Height + 40);
            }
            /**
             * 
             */
            this.Load += new EventHandler((sender, e) =>
            {
                Timer t = new Timer();
                t.Interval = 100;
                t.Tick += new EventHandler((_sender, _e) =>
                {
                    t.Stop();
                    g.BlockBorders(this.grid);
                });
                t.Start();
            });
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler((sender, e) =>
            {
                switch (e.KeyCode)
                {
                    case Keys.Escape: Application.Exit(); break;
                    case Keys.S: g.GetStatus(); break;
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
                        if (g.GetNearByLineInfo(this.grid, p).Value.A != 0)
                            Cursor.Current = Cursors.Hand;
                        else Cursor.Current = Cursors.Default;
                    });
                }
                if (g.GetNearByLineInfo(this.grid, e.Location).Value.A != 0)
                    Cursor.Current = Cursors.Hand;
                this.__NearBySearchTimer.Tag = e.Location;
                this.__NearBySearchTimer.Start();
            });
            this.grid.MouseDown +=new MouseEventHandler(grid_MouseDown);
        }
        public void grid_MouseDown(object sender, MouseEventArgs e)
        {
            var inf = g.GetNearByLineInfo(this.grid, e.Location);
            if (inf.Value.A != 0)
            {
                g.ToggleBlock(this.grid, inf.Key);
                this.Invalidate();
            }
        }
    }
}