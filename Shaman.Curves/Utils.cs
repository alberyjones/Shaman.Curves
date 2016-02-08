using System;
using System.Globalization;

namespace Shaman
{
    internal class Utils
    {
        public static double ParseDoubleInvariant(string str)
        {
            return double.Parse(str, NumberStyles.Number | NumberStyles.AllowExponent, CultureInfo.InvariantCulture);
        }

    }

}