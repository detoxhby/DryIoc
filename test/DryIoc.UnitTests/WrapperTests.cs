using System;
using System.Linq.Expressions;
using DryIoc.UnitTests.CUT;
using NUnit.Framework;

namespace DryIoc.UnitTests
{
    [TestFixture]
    public class WrapperTests
    {
        [Test]
        public void IsRegistered_wont_work_for_generic_wrappers()
        {
            var container = new Container();
            container.Register(typeof(IService), typeof(Service));

            var registered = container.IsRegistered<Func<IService>>();

            Assert.That(registered, Is.False);
        }

        [Test]
        public void When_registered_both_named_and_default_service_Then_resolving_Lazy_with_the_name_should_return_named_service()
        {
            var container = new Container();
            container.Register(typeof(IService), typeof(Service));
            container.Register(typeof(IService), typeof(AnotherService), serviceKey: "named");

            var service = container.Resolve<Lazy<IService>>("named");

            Assert.That(service.Value, Is.InstanceOf<AnotherService>());
        }

        [Test]
        public void Given_the_same_type_service_and_wrapper_registered_When_resolved_Then_service_will_preferred_over_wrapper()
        {
            var container = new Container();
            container.Register<Lazy<IService>>(made: Made.Of(
                t => t.GetConstructorOrNull(args: typeof(Func<>).MakeGenericType(t.GetGenericParamsAndArgs()))));
            container.Register<IService, Service>();

            var service = container.Resolve<Lazy<IService>>();

            Assert.That(service.Value, Is.InstanceOf<Service>());
        }

        [Test]
        public void Given_the_same_type_service_and_wrapper_registered_Wrapper_will_be_used_to_for_names_other_than_one_with_registered_service()
        {
            var container = new Container();
            container.Register<Lazy<IService>>(made: Made.Of(
                t => t.GetConstructorOrNull(args: typeof(Func<>).MakeGenericType(t.GetGenericParamsAndArgs()))));
            container.Register<IService, Service>();
            container.Register<IService, AnotherService>(serviceKey: "named");

            var service = container.Resolve<Lazy<IService>>("named");

            Assert.That(service.Value, Is.InstanceOf<AnotherService>());
        }

        [Test]
        public void Wrapper_is_only_working_if_used_in_enumerable_or_other_wrapper_It_means_that_resolving_array_of_multiple_wrapper_should_throw()
        {
            var container = new Container();
            
            var ex = Assert.Throws<ContainerException>(() => 
                container.Register(typeof(WrapperWithTwoArgs<,>), setup: Setup.Wrapper));

            Assert.AreEqual(Error.GenericWrapperWithMultipleTypeArgsShouldSpecifyArgIndex, ex.Error);
        }

        [Test]
        public void Wrapper_may_not_be_generic_as_WeakReference()
        {
            var container = new Container();

            container.Register(typeof(WeakReference), 
                made: Made.Of(t => t.GetConstructorOrNull(args: typeof(object))),
                setup: Setup.Wrapper);

            container.Register<Service>();

            var serviceWeakRef = container.Resolve<WeakReference>(typeof(Service));
            Assert.IsInstanceOf<Service>(serviceWeakRef.Target);

            var servicesWeakRef = container.Resolve<Func<WeakReference>[]>(typeof(Service));
            Assert.IsInstanceOf<Service>(servicesWeakRef[0]().Target);

            var factoryWeakRef = container.Resolve<WeakReference>(typeof(Func<Service>));
            Assert.IsInstanceOf<Func<Service>>(factoryWeakRef.Target);

            var func = factoryWeakRef.Target as Func<Service>;
            Assert.IsInstanceOf<Service>(func.ThrowIfNull().Invoke());

            var factoryWeakRefs = container.Resolve<WeakReference[]>(typeof(Func<Service>));
            Assert.AreEqual(1, factoryWeakRefs.Length);
        }
        public class WrapperWithTwoArgs<T0, T1>
        {
            public T0 Arg0 { get; set; }
            public T1 Arg1 { get; set; }

            public WrapperWithTwoArgs(T0 arg0, T1 arg1)
            {
                Arg0 = arg0;
                Arg1 = arg1;
            }
        }

        [Test]
        public void Resolving_array_of_required_service_type_wrapper_should_work()
        {
            var container = new Container();

            container.Resolve<LambdaExpression[]>(typeof(A));
        }

        [Test]
        public void Injected_container_wrapper_should_NOT_be_tracked_as_transient_disposable()
        {
            var container = new Container(rules => rules.WithTrackingDisposableTransients());

            container.Register<Blah>();

            var blah = container.Resolve<Blah>();

            Assert.IsNotNull(blah);

            container.Dispose();
        }

        [Test]
        public void Resolve_factory_identifier()
        {
            var container = new Container();

            container.Register<Blah>();

            var blahFactoryId = container.Resolve<FactoryIdentifier<Blah>>();

            Assert.Greater(blahFactoryId, 0);

            container.Dispose();
        }

        public class Blah
        {
            public IContainer Container { get; private set; }

            public Blah(IContainer container)
            {
                Container = container;
            }
        }
    }
}
