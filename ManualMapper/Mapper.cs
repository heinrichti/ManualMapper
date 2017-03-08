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

        public void CreateMap<TSource, TDestination>(Func<TSource, TDestination> mappingFunc)
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

        public TDestination Map<TDestination>(object source) where TDestination : class
        {
            return (TDestination) GetMappingFunc<TDestination>(source.GetType())(source);
        }

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