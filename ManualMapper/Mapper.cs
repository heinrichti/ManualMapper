using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ManualMapper
{
    public class Mapper
    {
        private readonly ConcurrentDictionary<TypeMap, Func<object, object>> _mappings =
            new ConcurrentDictionary<TypeMap, Func<object, object>>();

        /// <summary>
        /// Registers a new mapping function.
        /// </summary>
        /// <typeparam name="TSource">Source type to be mapped from.</typeparam>
        /// <typeparam name="TDestination">Target type to map to.</typeparam>
        /// <param name="mappingFunc">Mapping function which will perform the mapping on call of <see cref="Map{TDestination}"/>.</param>
        /// <exception cref="ArgumentException">If a mapping between <typeparamref name="TSource"/> and <typeparamref name="TDestination"/> already exists.</exception>
        public void RegisterMap<TSource, TDestination>(Func<TSource, TDestination> mappingFunc)
            where TSource : class
            where TDestination : class
        {
            Func<object, object> typelessFunc = (value => mappingFunc((TSource) value));
            var typeMap = new TypeMap(typeof(TSource), typeof(TDestination));

            if (!_mappings.TryAdd(typeMap, typelessFunc))
                throw new ArgumentException($"Mapping <{typeof(TSource)}, {typeof(TDestination)}> already exists.");
        }

        private Func<object, object> GetMappingFunc<TDestination>(Type sourceType)
        {
            Func<object, object> mappingFunc;
            if (!_mappings.TryGetValue(new TypeMap(sourceType, typeof(TDestination)), out mappingFunc))
                throw new ArgumentException($"No mapping found for <{sourceType}, {typeof(TDestination)}>.");
            return mappingFunc;
        }

        /// <summary>
        /// Maps the <paramref name="source"/> object to <typeparamref name="TDestination"/> type.
        /// </summary>
        /// <typeparam name="TDestination">Target type to map to.</typeparam>
        /// <param name="source">Source object to map from. The source type is looked up at runtime. If this is a <see cref="IEnumerable"/> consider using <see cref="MapList{TDestination}"/> instead.</param>
        /// <returns>The mapped object.</returns>
        public TDestination Map<TDestination>(object source) where TDestination : class
        {
            return (TDestination) GetMappingFunc<TDestination>(source.GetType())(source);
        }

        /// <summary>
        /// Maps the <paramref name="source"/> <see cref="IEnumerable"/> to an <see cref="IEnumerable"/> of <typeparamref name="TDestination"/>.
        /// Caches the mapping func and type of elements and therefore should perform better than <see cref="Map{TDestination}"/> for IEnumerables.
        /// </summary>
        /// <typeparam name="TDestination">Target type to map to.</typeparam>
        /// <param name="source">Source enumerable to map from. The type of all elements of <paramref name="source"/> has to be the same type 
        /// as the mapping function is only evaluated for the first element in the enumerable.</param>
        /// <returns>Enumerable of target type objects. The mappings are lazy evaluated, therefore if the result is not consumed no mapping will take place.</returns>
        public IEnumerable<TDestination> MapList<TDestination>(IEnumerable source)
        {
            var enumerator = source.GetEnumerator();
            if (!enumerator.MoveNext())
                yield break;

            var mappingFunc = GetMappingFunc<TDestination>(enumerator.Current.GetType());

            yield return (TDestination) mappingFunc(enumerator.Current);
            while (enumerator.MoveNext())
                yield return (TDestination) mappingFunc(enumerator.Current);
        }
    }
}