using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeArcher.Helpers
{
    public static class IntegerExtensions
    {
        public static bool GreaterThan(this int current, int compareTo)
        {
            return current > compareTo;
        }
        public static bool GreaterThanOrEqual(this int current, int compareTo)
        {
            return current >= compareTo;
        }
        public static bool LessThan(this int current, int compareTo)
        {
            return current < compareTo;
        }
        public static bool LessThanOrEqual(this int current, int compareTo)
        {
            return current <= compareTo;
        }
    }
}
