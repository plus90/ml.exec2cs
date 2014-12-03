using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace mltak2
{
    class GridHelper
    {
        [Flags]
        public enum Directions { NORTH = 0x0, EAST = 0x1, SOUTH = 0x2, WEST = 0x4, HOLD = 0x8 }
        public class Status
        {
            public bool Succeed { get; private set; }
            public Point NewPoint { get; private set; }
            public Point OldPoint { get; private set; }
            public Status(Point newPoint, Point oldPoint, bool succeed)
                : this(newPoint, oldPoint)
            {
                this.Succeed = succeed;
            }
            public Status(Point newPoint, Point oldPoint)
            {
                this.Succeed = (newPoint != oldPoint);
                this.NewPoint = newPoint;
                this.OldPoint = oldPoint;
            }
        }
        public Grid BoundedGrid { get; private set; }
        public void Bind(Grid grid) { this.BoundedGrid = grid; }
        public bool CanMove(Directions direction)
        {
            if (this.BoundedGrid.IsBlocked(this.BoundedGrid.AgentPoint.X, this.BoundedGrid.AgentPoint.Y, Grid.BlockStatus.BLOCKED))
                return false;
            if (this.BoundedGrid.IsBlocked(this.BoundedGrid.AgentPoint.X, this.BoundedGrid.AgentPoint.Y, Grid.BlockStatus.UNBLOCKED))
                return true;
            switch (direction)
            {
                case Directions.NORTH: return !this.BoundedGrid.IsBlocked(this.BoundedGrid.AgentPoint.X, this.BoundedGrid.AgentPoint.Y, Grid.BlockStatus.NORTH);
                case Directions.EAST: return !this.BoundedGrid.IsBlocked(this.BoundedGrid.AgentPoint.X, this.BoundedGrid.AgentPoint.Y, Grid.BlockStatus.EAST);
                case Directions.SOUTH: return !this.BoundedGrid.IsBlocked(this.BoundedGrid.AgentPoint.X, this.BoundedGrid.AgentPoint.Y, Grid.BlockStatus.SOUTH);
                case Directions.WEST: return !this.BoundedGrid.IsBlocked(this.BoundedGrid.AgentPoint.X, this.BoundedGrid.AgentPoint.Y, Grid.BlockStatus.WEST);
            }
            return false;
        }
        public List<Directions> AvailMoves()
        {
            var am = new List<Directions>() { Directions.HOLD };
            foreach (var d in Enum.GetValues(typeof(Directions)))
            {
                if (this.CanMove((Directions)d))
                    am.Add((Directions)d);
            }
            return am;
        }
        protected Point __getPoint(Point from, Directions direction)
        {
            throw new NotImplementedException();
        }
        public Status Move(Directions direction)
        {
            if (!this.CanMove(direction))
                return new Status(this.BoundedGrid.AgentPoint, this.BoundedGrid.AgentPoint);
            var s = new Status(__getPoint(this.BoundedGrid.AgentPoint, direction), this.BoundedGrid.AgentPoint);
            this.BoundedGrid.AgentPoint = s.NewPoint;
            return s;
        }
    }
}