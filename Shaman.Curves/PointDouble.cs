
using System;

namespace Shaman.Curves
{
    public struct PointDouble : IEquatable<PointDouble>
    {

        private double x;
        private double y;

        public override bool Equals(object obj)
        {
            if (obj is PointDouble)
            {
                return (PointDouble)obj == this;
            }
            return false;
        }

        public double X
        {
            get
            {
                return this.x;
            }
            set
            {
                this.x = value;
            }
        }

        public double Y
        {
            get
            {
                return this.y;
            }
            set
            {
                this.y = value;
            }
        }


        public PointDouble(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public bool Equals(PointDouble other)
        {
            return this.x == other.x && this.y == other.y;
        }

        public static bool operator ==(PointDouble lhs, PointDouble rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(PointDouble lhs, PointDouble rhs)
        {
            return !lhs.Equals(rhs);
        }

        public override int GetHashCode()
        {
            return this.x.GetHashCode() ^ (this.y.GetHashCode() >> 1);
        }

    }
}
