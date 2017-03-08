using System.Collections.Generic;
using System.Linq;
using ManualMapper;
using NUnit.Framework;

namespace ManualMapperTest
{
    [TestFixture]
    public class StringMappingTest
    {
        private Mapper _mapper;
        private const string NameTest = "Tim";

        [SetUp]
        public void Setup()
        {
            _mapper = new Mapper();
            _mapper.RegisterMap<string, string>(MappingFunc);
        }

        [Test]
        public void SimpleMapping()
        {
            var map = _mapper.Map<string>(NameTest);

            Assert.AreEqual("Hello Tim", map);
        }

        [Test]
        public void ListMapping()
        {
            const string test1 = "Test1";
            const string test2 = "Test2";
            const string test3 = "Test3";

            var list = new List<string>
            {
                test1,
                test2,
                test3
            };

            var result = _mapper.MapList<string>(list).ToList();

            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(MappingFunc(test1), result[0]);
            Assert.AreEqual(MappingFunc(test2), result[1]);
            Assert.AreEqual(MappingFunc(test3), result[2]);
        }

        private string MappingFunc(string s)
        {
            return "Hello " + s;
        }
    }
}
