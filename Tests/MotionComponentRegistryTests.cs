using BP.TextMotion;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace BP.TextMotionTests
{
    public class FakeMotionComponent : MotionComponent
    {
        public override string Key => "A";
    }
    public class FakeMotionComponentKeyed : MotionComponent
    {
        private string key;
        public override string Key => key;

        public void SetKey(string key) => this.key = key;
    }

    public class MotionComponentRegistryTests
    {
        private MotionComponentRegistry<MotionComponent> _collection;
        private FieldInfo _componentsField;
        private FieldInfo _cacheField;

        [SetUp]
        public void SetUp()
        {
            _collection = new MotionComponentRegistry<MotionComponent>();
            _componentsField = typeof(MotionComponentRegistry<MotionComponent>)
                .GetField("components", BindingFlags.NonPublic | BindingFlags.Instance);
            _cacheField = typeof(MotionComponentRegistry<MotionComponent>)
                .GetField("cacheByKey", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        private List<MotionComponent> ComponentsList =>
            (List<MotionComponent>)_componentsField.GetValue(_collection);

        private Dictionary<string, MotionComponent> CacheDict =>
            (Dictionary<string, MotionComponent>)_cacheField.GetValue(_collection);

        [Test]
        public void Add_CreatesInstanceAndFiresChanged()
        {
            int calls = 0;
            _collection.Changed += () => calls++;

            var inst = _collection.Add<FakeMotionComponent>();
            Assert.IsNotNull(inst);
            Assert.AreEqual(1, _collection.GetAll().Length);
            Assert.AreEqual(1, calls, "Changed must fire once on Add");
        }

        [Test]
        public void Add_SameTypeTwice_Throws()
        {
            _collection.Add<FakeMotionComponent>();
            var ex = Assert.Throws<InvalidOperationException>(
                () => _collection.Add<FakeMotionComponent>()
            );
            StringAssert.Contains("already exists", ex.Message);
        }

        [Test]
        public void Has_ReturnsTrueOnlyIfTypePresent()
        {
            Assert.IsFalse(_collection.Has(typeof(FakeMotionComponent)));
            _collection.Add<FakeMotionComponent>();
            Assert.IsTrue(_collection.Has(typeof(FakeMotionComponent)));
        }

        [Test]
        public void Remove_Type_RemovesAllAndFiresChanged()
        {
            int calls = 0;
            _collection.Changed += () => calls++;

            _collection.Add<FakeMotionComponent>();
            Assert.AreEqual(1, _collection.GetAll().Length);

            calls = 0;
            _collection.Remove(typeof(FakeMotionComponent));
            Assert.AreEqual(0, _collection.GetAll().Length);
            Assert.AreEqual(1, calls, "Changed must fire once on Remove");
        }

        [Test]
        public void Remove_Nonexistent_DoesNotFireChanged()
        {
            int calls = 0;
            _collection.Changed += () => calls++;
            _collection.Remove(typeof(FakeMotionComponent));
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
            var fetched = _collection.TryGetByKey("B", out var component);
            Assert.IsTrue(fetched);
            Assert.AreSame(b, component);
            Assert.IsTrue(CacheDict.ContainsKey("B"), "Should have cached B");

            // TryGetByKey should hit cache
            bool ok = _collection.TryGetByKey("B", out var fromTry);
            Assert.IsTrue(ok);
            Assert.AreSame(b, fromTry);
        }

        private FakeMotionComponentKeyed CreateTestComponent(string key)
        {
            var component = ScriptableObject.CreateInstance<FakeMotionComponentKeyed>();
            component.SetKey(key);
            return component;
        }
    }
}
