using System;

namespace ManualMapper
{
    internal struct TypeMap  : IEquatable<TypeMap>
    {
        private readonly Type _source;
        private readonly Type _destination;

        public TypeMap(Type source, Type destination)
        {
            _source = source;
            _destination = destination;
        }

        public bool Equals(TypeMap other)
        {
            return _source == other._source && _destination == other._destination;
        }

        public override bool Equals(object obj)
        {
            return obj is TypeMap && Equals((TypeMap) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_source.GetHashCode() * 397) ^ _destination.GetHashCode();
            }
        }
    }
}