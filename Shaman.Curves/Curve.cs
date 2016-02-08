using Shaman.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace Shaman.Curves
{
    public class Curve
    {
        private class ConstantInterpolator : Interpolator
        {
            public override double Interpolate(double x)
            {
                return 0;
            }

            protected override void Reset()
            {
            }
        }
        public static Curve CreateNonConfigured()
        {
            return new Curve() { Interpolator = new ConstantInterpolator() };
        }
        public static Curve Parse(string definition)
        {
            if (string.IsNullOrEmpty(definition)) return CreateNonConfigured();
            var data = definition.SplitFast(',');
            var interpolator = new MonotoneCubicInterpolator();
            for (int i = 0; i < data.Length; i += 2)
            {
                var a = Utils.ParseDoubleInvariant(data[i]);
                var b = Utils.ParseDoubleInvariant(data[i + 1]);
                interpolator.Add(a, b);
            }
            var c = new Curve();
            c.Interpolator = interpolator;
            return c;
        }

        public string Serialize()
        {
            var sb = ReseekableStringBuilder.AcquirePooledStringBuilder();
            var inter = (MonotoneCubicInterpolator)Interpolator;
            foreach (var item in inter.points)
            {
                sb.Append(item.Key.ToString(CultureInfo.InvariantCulture));
                sb.Append(',');
                sb.Append(item.Value.ToString(CultureInfo.InvariantCulture));
                sb.Append(',');
            }
            if (inter.points.Count != 0) sb.Length--;
            return ReseekableStringBuilder.GetValueAndRelease(sb);
        }


        private Curve()
        {
        }


        public Interpolator Interpolator { get; set; }

        public double GetValue(double input)
        {
            return Interpolator.Interpolate(input);
        }

    }
}
