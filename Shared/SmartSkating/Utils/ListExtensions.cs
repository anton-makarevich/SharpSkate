using System;
using System.Collections.Generic;
using System.Linq;

namespace Sanet.SmartSkating.Utils
{
    public static class ListExtensions
    {
        // Use this until we can go with .NET Standard 2.1
        public static IEnumerable<T> TakeLast<T>(this IList<T> source, int n)
        {
            return source.Skip(Math.Max(0, source.Count() - n));
        }
    }
}