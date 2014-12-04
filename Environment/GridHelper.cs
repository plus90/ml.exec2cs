using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Environment
{
    public class GridHelper
    {
        const float MovementAccuracy = 0.9F;
        Random propExaminer { get; set; }
        System.Timers.Timer Timer { get; set; }
        /// <summary>
        /// Get or set the bounded grid to the helper
        /// </summary>
        public Grid BoundedGrid { get; private set; }
        [Flags]
        public enum Directions { NORTH = 0x0, EAST = 0x1, SOUTH = 0x2, WEST = 0x4, HOLD = 0x8 }
        /// <summary>
        /// A movement status configuration
        /// </summary>
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
        /// <summary>
        /// Init the helper
        /// </summary>
        /// <param name="grid">The grid to bind with the helper</param>
        public GridHelper(Grid grid)
        {
            this.Bind(grid);
            this.propExaminer = new Random(System.Environment.TickCount);
            this.Timer = new System.Timers.Timer(400);
            this.Timer.Elapsed += new System.Timers.ElapsedEventHandler((sender, e) =>
            {
                lock (this.propExaminer)
                    this.propExaminer = new Random(System.Environment.TickCount + e.SignalTime.Millisecond);
            });
            this.Timer.Start();
        }
        /// <summary>
        /// de-init the helper
        /// </summary>
        ~GridHelper()
        {
            this.Timer.Stop();
            this.Timer.Dispose();
        }
        /// <summary>
        /// Bind a grid to the helper
        /// </summary>
        /// <param name="grid">The target grid</param>
        public void Bind(Grid grid) { this.BoundedGrid = grid; }
        /// <summary>
        /// Checks if a point can go to a direction based on the bounded grid
        /// </summary>
        /// <param name="point">The target point to check</param>
        /// <param name="direction">The direction of desire</param>
        /// <returns>Returns true if the point can be moved to a direction without encountering any obstacles; Otherwise returns false.</returns>
        public bool CanMove(Point point, Directions direction)
        {
            if (this.BoundedGrid.IsBlocked(point.X, point.Y, Grid.BlockStatus.BLOCKED))
                return false;
            switch (direction)
            {
                case Directions.NORTH: return !this.BoundedGrid.IsBlocked(point.X, point.Y, Grid.BlockStatus.NORTH);
                case Directions.EAST: return !this.BoundedGrid.IsBlocked(point.X, point.Y, Grid.BlockStatus.EAST);
                case Directions.SOUTH: return !this.BoundedGrid.IsBlocked(point.X, point.Y, Grid.BlockStatus.SOUTH);
                case Directions.WEST: return !this.BoundedGrid.IsBlocked(point.X, point.Y, Grid.BlockStatus.WEST);
            }
            return true;
        }
        /// <summary>
        /// Returns a list of available move at a point
        /// </summary>
        /// <param name="point">The point of desire</param>
        /// <returns>A list of availble directions which the point can move on without having any obstacles ahead.</returns>
        public List<Directions> AvailMoves(Point point)
        {
            var am = new List<Directions>() { Directions.HOLD };
            foreach (var d in Enum.GetValues(typeof(Directions)))
            {
                if (this.CanMove(point, (Directions)d))
                    am.Add((Directions)d);
            }
            return am;
        }
        /// <summary>
        /// Get the exact point with respect to a point with a direction
        /// </summary>
        /// <param name="from">The current point</param>
        /// <param name="direction">The desired direction</param>
        /// <returns>The destination point</returns>
        protected Point __getDirectionPoint(Point from, Directions direction)
        {
            switch (direction)
            {
                case Directions.NORTH:
                    if (from.Y <= 0)
                        return from;
                    return new Point(from.X, from.Y - 1);
                case Directions.EAST:
                    if (from.X >= this.BoundedGrid.Size.Width - 1)
                        return from;
                    return new Point(from.X + 1, from.Y);
                case Directions.SOUTH:
                    if (from.Y >= this.BoundedGrid.Size.Height - 1)
                        return from;
                    return new Point(from.X, from.Y + 1);
                case Directions.WEST:
                    if (from.X <= 0)
                        return from;
                    return new Point(from.X - 1, from.Y);
                case Directions.HOLD:
                    return from;
                default:
                    throw new ArgumentException("Unsupported direction");
            }
        }
        /// <summary>
        /// Get cross directions on the given direction
        /// </summary>
        /// <param name="direction">The origin direction</param>
        /// <returns>The cross directions based on the origin direction; For HOLD direction if will return an empty direction array</returns>
        protected Directions[] __getCrossDirection(Directions direction)
        {
            switch (direction)
            {
                case Directions.SOUTH:
                case Directions.NORTH:
                    return new Directions[] { Directions.EAST, Directions.WEST };
                case Directions.EAST:
                case Directions.WEST:
                    return new Directions[] { Directions.SOUTH, Directions.NORTH };
                case Directions.HOLD:
                    return new Directions[] { };
                default:
                    throw new ArgumentException("Unsupported direction");
            }
        }
        /// <summary>
        /// Get destination point of a direction, specifically coded based upon the exercise instruction; See remarks for more details.
        /// </summary>
        /// <remarks>
        /// At any call to this function with 90% probability it will retrun the true destination point, or HOLD and retruns the current point if the movement is not permitted
        /// due to the bounded grid's restrictions.
        /// With 10% probability it will return one of the point which is in cross of the desired direction, or HOLD and retruns the current point if the movement is not
        /// permitted due to the bounded grid's restrictions.
        /// If HOLD direction passed it will 100% retrun the current point.
        /// </remarks>
        /// <param name="from">The current point</param>
        /// <param name="direction">The desired direction</param>
        /// <returns>The desination point</returns>
        protected Point __getPoint(Point from, Directions direction)
        {
            switch (direction)
            {
                case Directions.NORTH:
                case Directions.EAST:
                case Directions.SOUTH:
                case Directions.WEST:
                    lock (this.propExaminer)
                    {
                        // validate the probability of origin move occurrence
                        if (this.propExaminer.NextDouble() <= MovementAccuracy)
                        {
                            // if it is NOT a valid move?
                            if (!this.CanMove(from, direction))
                                // return the origin point
                                return from;
                            // return the destination point directly
                            return this.__getDirectionPoint(from, direction);
                        }
                        // if we are here it means there's been a exceptional move event
                        // randomly choose a cross direction
                        var choosen_cross_direction = this.__getCrossDirection(direction)[this.propExaminer.Next(0, 1)];
                        // if it is possible to move with choosen cross direction?
                        if (this.CanMove(from, choosen_cross_direction))
                            // return the destination point following up the cross direction
                            return this.__getDirectionPoint(from, choosen_cross_direction);
                        // otherwise return the origin point
                        return from;
                    }
                case Directions.HOLD:
                    // hold-up to the origin point
                    return from;
                default:
                    throw new ArgumentException("Not supported direction!!!");
            }
        }
        /// <summary>
        /// Moves a point to a direction
        /// </summary>
        /// <param name="point">The target point to move</param>
        /// <param name="direction">The desired direction</param>
        /// <returns>The movement status instance</returns>
        public Status Move(Point point, Directions direction) { return new Status(__getPoint(point, direction), point); }
    }
}