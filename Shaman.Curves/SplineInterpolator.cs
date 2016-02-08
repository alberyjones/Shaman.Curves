using System;
using System.Collections.Generic;
namespace Shaman.Curves
{
    internal sealed class SplineInterpolator : Interpolator
    {
        private double[] y2;



        protected override void Reset()
        {
            y2 = null;
        }
        public override double Interpolate(double x)
        {
            if (this.y2 == null)
            {
                this.PreCompute();
            }
            IList<double> xa = this.points.Keys;
            IList<double> ya = this.points.Values;
            int i = ya.Count;
            int klo = 0;
            int khi = i - 1;
            while (khi - klo > 1)
            {
                int j = khi + klo >> 1;
                if (xa[j] > x)
                {
                    khi = j;
                }
                else
                {
                    klo = j;
                }
            }
            double h = xa[khi] - xa[klo];
            double a = (xa[khi] - x) / h;
            double b = (x - xa[klo]) / h;
            return a * ya[klo] + b * ya[khi] + ((a * a * a - a) * this.y2[klo] + (b * b * b - b) * this.y2[khi]) * (h * h) / 6.0;
        }
        private void PreCompute()
        {
            int i = this.points.Count;
            double[] u = new double[i];
            IList<double> xa = this.points.Keys;
            IList<double> ya = this.points.Values;
            this.y2 = new double[i];
            u[0] = 0.0;
            this.y2[0] = 0.0;
            for (int j = 1; j < i - 1; j++)
            {
                double wx = xa[j + 1] - xa[j - 1];
                double sig = (xa[j] - xa[j - 1]) / wx;
                double p = sig * this.y2[j - 1] + 2.0;
                this.y2[j] = (sig - 1.0) / p;
                double ddydx = (ya[j + 1] - ya[j]) / (xa[j + 1] - xa[j]) - (ya[j] - ya[j - 1]) / (xa[j] - xa[j - 1]);
                u[j] = (6.0 * ddydx / wx - sig * u[j - 1]) / p;
            }
            this.y2[i - 1] = 0.0;
            for (int k = i - 2; k >= 0; k--)
            {
                this.y2[k] = this.y2[k] * this.y2[k + 1] + u[k];
            }
        }
    }
}
