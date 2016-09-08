using System;
using System.Collections.Generic;
using System.Linq;

namespace Shaman.Curves
{
#if !CORECLR
    [Serializable]
#endif
    public abstract class Interpolator
    {
        internal SortedList<double, double> points = new SortedList<double, double>();

        public int Count
        {
            get
            {
                return this.points.Count;
            }
        }
        public void Add(double x, double y)
        {
            if (double.IsInfinity(x) || double.IsNaN(x)) throw new ArgumentException();
            if (double.IsInfinity(y) || double.IsNaN(y)) throw new ArgumentException();
            this.points[x] = y;
            Reset();
        }

        protected abstract void Reset();

        public void Clear()
        {
            this.points.Clear();
        }
        public abstract double Interpolate(double x);

        public IEnumerable<PointDouble> Points
        {
            get
            {
                return points.Select(x => new PointDouble(x.Key, x.Value));
            }
        }

    }
}
