using BP.TextMotion;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BP.TextMotionTests
{
    public class TestMotionComponent : MotionComponent
    {
        private readonly string key;
        public override string Key => key;
        public TestMotionComponent(string key)
        {
            this.key = key;
        }
    }

    public class MotionCollectionTests
    {
        private MotionCollection<MotionComponent> _collection;
        private FieldInfo _componentsField;
        private FieldInfo _cacheField;

        [SetUp]
        public void SetUp()
        {
            _collection = new MotionCollection<MotionComponent>();
            _componentsField = typeof(MotionCollection<MotionComponent>)
                .GetField("components", BindingFlags.NonPublic | BindingFlags.Instance);
            _cacheField = typeof(MotionCollection<MotionComponent>)
                .GetField("cache", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        private List<MotionComponent> ComponentsList =>
            (List<MotionComponent>)_componentsField.GetValue(_collection);

        private Dictionary<string, MotionComponent> CacheDict =>
            (Dictionary<string, MotionComponent>)_cacheField.GetValue(_collection);

        [Test]
        public void Initialize_RemovesNullsAndClearsCache()
        {
            // Prepare a real component and a null, and prime the cache
            var real = CreateTestComponent("real");
            ComponentsList.Add(real);
            ComponentsList.Add(null);
            CacheDict["real"] = real;

            _collection.Initialize();

            Assert.AreEqual(1, ComponentsList.Count, "Should remove null entries");
            Assert.IsFalse(CacheDict.Any(), "Should clear the cache");
        }

        [Test]
        public void Add_CreatesInstanceAndFiresChanged()
        {
            int calls = 0;
            _collection.Changed += () => calls++;

            var inst = _collection.Add(typeof(TestMotionComponent));
            Assert.IsNotNull(inst);
            Assert.AreEqual(1, _collection.GetAll().Length);
            Assert.AreEqual(1, calls, "Changed must fire once on Add");
        }

        [Test]
        public void Add_SameTypeTwice_Throws()
        {
            _collection.Add(typeof(TestMotionComponent));
            var ex = Assert.Throws<InvalidOperationException>(
                () => _collection.Add(typeof(TestMotionComponent))
            );
            StringAssert.Contains("already exists", ex.Message);
        }

        [Test]
        public void Has_ReturnsTrueOnlyIfTypePresent()
        {
            Assert.IsFalse(_collection.Has(typeof(TestMotionComponent)));
            _collection.Add(typeof(TestMotionComponent));
            Assert.IsTrue(_collection.Has(typeof(TestMotionComponent)));
        }

        [Test]
        public void Remove_Type_RemovesAllAndFiresChanged()
        {
            int calls = 0;
            _collection.Changed += () => calls++;

            _collection.Add(typeof(TestMotionComponent));
            Assert.AreEqual(1, _collection.GetAll().Length);

            calls = 0;
            _collection.Remove(typeof(TestMotionComponent));
            Assert.AreEqual(0, _collection.GetAll().Length);
            Assert.AreEqual(1, calls, "Changed must fire once on Remove");
        }

        [Test]
        public void Remove_Nonexistent_DoesNotFireChanged()
        {
            int calls = 0;
            _collection.Changed += () => calls++;
            _collection.Remove(typeof(TestMotionComponent));
            Assert.AreEqual(0, calls);
        }

        [Test]
        public void HasKey_GetByKey_TryGetByKey_WorkAndCache()
        {
            // Create two TestMotionComponent instances via their ctor
            var a = CreateTestComponent("A");
            var b = CreateTestComponent("B");
            ComponentsList.Add(a);
            ComponentsList.Add(b);

            // No cache yet
            Assert.IsTrue(_collection.HasKey("A"));
            Assert.IsTrue(_collection.HasKey("B"));
            Assert.IsFalse(_collection.HasKey("C"));

            // GetByKey should find and cache
            var fetched = _collection.GetByKey("B");
            Assert.AreSame(b, fetched);
            Assert.IsTrue(CacheDict.ContainsKey("B"), "Should have cached B");

            // TryGetByKey should hit cache
            bool ok = _collection.TryGetByKey("B", out var fromTry);
            Assert.IsTrue(ok);
            Assert.AreSame(b, fromTry);
        }

        private TestMotionComponent CreateTestComponent(string key)
        {
            return (TestMotionComponent)Activator.CreateInstance(
                typeof(TestMotionComponent),
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null,
                new object[] { key },
                null
            );
        }
    }
}
