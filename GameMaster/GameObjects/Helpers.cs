using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eacc
{
    public static class ColectionExtension
    {
        private static Random rng = new Random();

        public static T RandomElement<T>(this IList<T> list)
        {
            return list[rng.Next(list.Count)];
        }

        public static KeyValuePair<TKey, TValue> RandomElement<TKey, TValue>(this IDictionary<TKey, TValue> dico)
        {
            return dico.ElementAt(rng.Next(dico.Count));
        }
    }
}