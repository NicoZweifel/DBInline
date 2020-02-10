using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace DBInline
{
    public static partial class Extensions
    {
        public static IEnumerable<T> Do<T>(this IEnumerable @this, Func<object, T> action)
        {
            foreach (var obj in @this)
            {
                var res = (T) obj;
                action(res);
                yield return res;
            }
        }
    }
}