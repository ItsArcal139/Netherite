using System;
using System.Collections.Generic;
using System.Text;

namespace Netherite.Utils.Extensions
{
    public static class Extension
    {
        public static IEnumerable<O> Map<I, O>(this IEnumerable<I> input, Func<I, O> mapper)
        {
            List<O> result = new List<O>();
            foreach (I i in input)
            {
                result.Add(mapper(i));
            }
            return result;
        }
    }
}
