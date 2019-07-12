using System;
using System.Collections.Generic;
using System.Linq;

namespace ManualMapper
{
    public class Mapper
    {
        /// <summary>
        /// Registers a new mapping function.
        /// </summary>
        /// <typeparam name="TSource">Source type to be mapped from.</typeparam>
        /// <typeparam name="TDestination">Target type to map to.</typeparam>
        /// <param name="mappingFunc">Mapping function which will perform the mapping on call of <see cref="Map{TSource, TDestination}"/>.</param>
        public void RegisterMap<TSource, TDestination>(Func<TSource, TDestination> mappingFunc)
            where TSource : class
            where TDestination : class
        {
            InnerMapper<TSource, TDestination>.RegisterMap(mappingFunc);
        }

        /// <summary>
        /// Maps the <paramref name="source"/> object to <typeparamref name="TDestination"/> type.
        /// </summary>
        /// <typeparam name="TSource">Source type to map from.</typeparam>
        /// <typeparam name="TDestination">Target type to map to.</typeparam>
        /// <param name="source">Source object to map from. The source type is looked up at runtime. If this is a <see cref="System.Collections.IEnumerable"/> consider using <see cref="MapList{TSource, TDestination}"/> instead.</param>
        /// <returns>The mapped object.</returns>
        public TDestination Map<TSource, TDestination>(TSource source)
            where TSource : class
            where TDestination : class =>
            InnerMapper<TSource, TDestination>.GetMap()(source);

        /// <summary>
        /// Maps the <paramref name="source"/> <see cref="System.Collections.IEnumerable"/> to an <see cref="System.Collections.IEnumerable"/> of <typeparamref name="TDestination"/>.
        /// Caches the mapping func and type of elements and therefore should perform better than <see cref="Map{TSource, TDestination}"/> for IEnumerables.
        /// </summary>
        /// <typeparam name="TSource">Source type to map from.</typeparam>
        /// <typeparam name="TDestination">Target type to map to.</typeparam>
        /// <param name="source">Source enumerable to map from. The type of all elements of <paramref name="source"/> has to be the same type 
        /// as the mapping function is only evaluated for the first element in the enumerable.</param>
        /// <returns>Enumerable of target type objects. The mappings are lazy evaluated, therefore if the result is not consumed no mapping will take place.</returns>
        public IEnumerable<TDestination> MapList<TSource, TDestination>(IEnumerable<TSource> source)
        {
            var func = InnerMapper<TSource, TDestination>.GetMap();
            return source.Select(func);
        }

        private static class InnerMapper<T, TD>
        {
            private static Func<T, TD> _mappingFunc;

            public static void RegisterMap(Func<T, TD> mappingFunc) => _mappingFunc = mappingFunc;

            public static Func<T, TD> GetMap() => _mappingFunc;
        }
    }
}
