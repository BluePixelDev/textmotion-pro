using NUnit.Framework;

namespace BP.TextMotionPro.Editor.Tests
{
    [ComponentDescriptor("Fake Tag Component", description: "A test component", category: "test")]
    public class FakeTagComponent : TagComponent
    {
        public override string Key => "fake_component";
        public override void Apply(MotionContext context) { }
        public override bool ValidateTag(string tag, string attributes) => true;
    }

    [ComponentDescriptor("Fake Transition Component", description: "A test component", category: "test")]
    public class FakeTransitionComponent : TransitionComponent
    {
        public override string Key => "fake_component";
        public override void Apply(MotionContext context) { }
    }


    public class MotionComponentCatalogTests
    {
        [SetUp]
        public void SetUp()
        {
            MotionComponentCatalog.Init();
        }

        // Tag Components

        [Test]
        public void TryGetComponentOfType_Returns_FakeTagComponentWithCorrectProperties()
        {
            bool hasFoundComponent = MotionComponentCatalog.TryGetComponentOfType(typeof(FakeTagComponent), out var result);
            Assert.IsTrue(hasFoundComponent);
            AssertTagComponentDescriptor(result);
        }

        [Test]
        public void TryGetComponentOfTypeGeneric_Returns_FakeTagComponentWithCorrectProperties()
        {
            bool hasFoundComponent = MotionComponentCatalog.TryGetComponentOfType<FakeTagComponent>(out var result);
            Assert.IsTrue(hasFoundComponent);
            AssertTagComponentDescriptor(result);
        }

        private void AssertTagComponentDescriptor(ComponentDescriptor descriptor)
        {
            Assert.AreEqual(typeof(FakeTagComponent), descriptor.Type);
            Assert.AreEqual("Fake Tag Component", descriptor.DisplayName);
            Assert.AreEqual(ComponentRole.Tag, descriptor.Role);
            Assert.AreEqual("A test component", descriptor.Description);
            Assert.AreEqual("test", descriptor.Category);
        }

        // Transition Components

        [Test]
        public void TryGetComponentOfType_Returns_FakeTransitionComponentWithCorrectProperties()
        {
            bool hasFoundComponent = MotionComponentCatalog.TryGetComponentOfType(typeof(FakeTransitionComponent), out var result);
            Assert.IsTrue(hasFoundComponent);
            AssertTransitionComponentDescriptor(result);
        }

        [Test]
        public void TryGetComponentOfTypeGeneric_Returns_FakeTransitionComponentWithCorrectProperties()
        {
            bool hasFoundComponent = MotionComponentCatalog.TryGetComponentOfType<FakeTransitionComponent>(out var result);
            Assert.IsTrue(hasFoundComponent);
            AssertTransitionComponentDescriptor(result);
        }

        private void AssertTransitionComponentDescriptor(ComponentDescriptor descriptor)
        {
            Assert.AreEqual(typeof(FakeTransitionComponent), descriptor.Type);
            Assert.AreEqual("Fake Transition Component", descriptor.DisplayName);
            Assert.AreEqual(ComponentRole.Transition, descriptor.Role);
            Assert.AreEqual("A test component", descriptor.Description);
            Assert.AreEqual("test", descriptor.Category);
        }

        // General tests

        [Test]
        public void GetComponents_Returns_DoesNotContainDuplicateComponents()
        {
            var components = MotionComponentCatalog.GetComponents();
            CollectionAssert.AllItemsAreUnique(components, "All components in registry should be unique");
        }
    }
}
