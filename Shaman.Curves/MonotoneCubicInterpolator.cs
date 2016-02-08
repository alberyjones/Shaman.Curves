using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shaman.Curves
{
    public class MonotoneCubicInterpolator : Interpolator
    {
        private Func<double, double> func;

        public override double Interpolate(double x)
        {
            return (func ?? (func = Train()))(x);
        }

        private Func<double, double> Train()
        {
            var xs = this.points.Select(x => x.Key).ToList();
            var ys = this.points.Select(x => x.Value).ToList();
            var length = xs.Count;

            if (length == 0) { return x => 0; }
            if (length == 1)
            {
                // Impl: Precomputing the result prevents problems if ys is mutated later and allows garbage collection of ys
                // Impl: Unary plus properly converts values to numbers
                var result = +ys[0];
                return x => result;
            }

            // Rearrange xs and ys so that xs is sorted
            var indexes = new List<int>();
            for (var i = 0; i < length; i++) { indexes.Add(i); }
            indexes.Sort((a, b) => xs[a] < xs[b] ? -1 : 1);
            var oldXs = xs;
            var oldYs = ys;
            // Impl: Creating new arrays also prevents problems if the input arrays are mutated later
            xs = new List<double>();
            ys = new List<double>();
            // Impl: Unary plus properly converts values to numbers
            for (var j = 0; j < length; j++) { xs.Add(+oldXs[indexes[j]]); ys.Add(+oldYs[indexes[j]]); }

            // Get consecutive differences and slopes
            var dys = new List<double>();
            var dxs = new List<double>();
            var ms = new List<double>();
            for (var j = 0; j < length - 1; j++)
            {
                var dx = xs[j + 1] - xs[j];
                var dy = ys[j + 1] - ys[j];
                dxs.Add(dx); dys.Add(dy); ms.Add(dy / dx);
            }

            // Get degree-1 coefficients
            var c1s = new List<double>() { ms[0] };
            for (var j = 0; j < dxs.Count - 1; j++)
            {
                var m = ms[j];
                var mNext = ms[j + 1];
                if (m * mNext <= 0)
                {
                    c1s.Add(0);
                }
                else
                {
                    var dx = dxs[j];
                    var dxNext = dxs[j + 1];
                    var common = dx + dxNext;
                    c1s.Add(3 * common / ((common + dxNext) / m + (common + dx) / mNext));
                }
            }
            c1s.Add(ms[ms.Count - 1]);

            // Get degree-2 and degree-3 coefficients
            var c2s = new List<double>();
            var c3s = new List<double>();
            for (var j = 0; j < c1s.Count - 1; j++)
            {
                var c1 = c1s[j];
                var m = ms[j];
                var invDx = 1 / dxs[j];
                var common = c1 + c1s[j + 1] - m - m;
                c2s.Add((m - c1 - common) * invDx);
                c3s.Add(common * invDx * invDx);
            }

            var lastidx = ys.Count - 1;
            var finalSlope = xs.Count <= 1 ? 0 : (ys[lastidx] - ys[lastidx - 1]) / (xs[lastidx] - xs[lastidx - 1]);
            var initialSlope = xs.Count <= 1 ? 0 : (ys[0] - ys[1]) / (xs[0] - xs[1]);

            return x =>
            {

                var i = xs.Count - 1;
                if (x >= xs[i])
                {
                    return ys[i] + (x - xs[i]) * finalSlope;

                }
                if (x <= xs[0])
                {
                    return ys[0] + (x - xs[0]) * initialSlope;
                }

                // Search for the interval x is in, returning the corresponding y if x is one of the original xs
                var low = 0;
                int mid;
                var high = c3s.Count - 1;
                while (low <= high)
                {
                    mid = (int)Math.Floor(0.5 * (low + high));
                    var xHere = xs[mid];
                    if (xHere < x) { low = mid + 1; }
                    else if (xHere > x) { high = mid - 1; }
                    else { return ys[mid]; }
                }
                i = Math.Max(0, high);

                // Interpolate
                var diff = x - xs[i];
                var diffSq = diff * diff;
                return ys[i] + c1s[i] * diff + c2s[i] * diffSq + c3s[i] * diff * diffSq;
            };
        }

        protected override void Reset()
        {
            func = null;
        }
    }
}
