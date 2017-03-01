using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ManualMapper
{
    public unsafe class Mapper
    {
        private readonly Dictionary<TypeMap, Func<object, object>> _mappings =
            new Dictionary<TypeMap, Func<object, object>>();

        public void CreateMap<TSource, TDestination>(Func<TSource, TDestination> mappingFunc)
            where TSource : class
            where TDestination : class
        {
            var typelessFunc = ((Expression<Func<object, object>>) (value => mappingFunc((TSource) value))).Compile();
            var typeMap = new TypeMap(typeof(TSource), typeof(TDestination));
            if (_mappings.ContainsKey(typeMap))
                throw new ArgumentException($"Mapping <{typeof(TSource)}, {typeof(TDestination)}> already exists.");
            _mappings.Add(typeMap, typelessFunc);
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