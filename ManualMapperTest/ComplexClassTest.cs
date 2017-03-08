using System.Collections.Generic;
using System.Linq;
using ManualMapper;
using NUnit.Framework;

namespace ManualMapperTest
{
    [TestFixture]
    internal class ComplexClassTest
    {
        [SetUp]
        public void Setup()
        {
            _mapper = new Mapper();
            _mapper.RegisterMap<ComplexClass, ComplexClass>(MappingFunc);

            _complexClass = new ComplexClass
            {
                Prop1 = "Hello ",
                Prop2 = 4,
                Prop3 = new List<object>
                {
                    "Test1",
                    2,
                    "3Test"
                },
                Prop4 = 45
            };
        }

        private class ComplexClass
        {
            public string Prop1 { get; set; }

            public int Prop2 { get; set; }

            public List<object> Prop3 { get; set; }

            public long Prop4 { get; set; }
        }

        private Mapper _mapper;
        private ComplexClass _complexClass;

        private ComplexClass MappingFunc(ComplexClass complexClassObj)
        {
            var complexClass = complexClassObj;

            return new ComplexClass
            {
                Prop1 = complexClass.Prop1,
                Prop2 = complexClass.Prop2,
                Prop3 = complexClass.Prop3.ToList(),
                Prop4 = complexClass.Prop4
            };
        }

        [Test]
        public void Mapping()
        {
            var result = _mapper.Map<ComplexClass>(_complexClass);

            Assert.AreEqual(_complexClass.Prop1, result.Prop1);
            Assert.AreEqual(_complexClass.Prop2, result.Prop2);
            Assert.AreEqual(_complexClass.Prop3, result.Prop3);
            Assert.AreEqual(_complexClass.Prop4, result.Prop4);

            Assert.AreNotSame(_complexClass, result);
            Assert.AreNotSame(_complexClass.Prop3, result.Prop3);
        }
    }
}