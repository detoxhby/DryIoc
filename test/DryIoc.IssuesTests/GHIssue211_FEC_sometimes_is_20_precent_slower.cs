using System;
using NUnit.Framework;

namespace DryIoc.IssuesTests
{
    [TestFixture]
    public class GHIssue211_FEC_sometimes_is_20_precent_slower
    {
        [Test]
        public void Should_get_a_singleton_constant_expression_from_the_cache()
        {
            var c = new Container();

            c.Register<A>();
            c.Register<B>(Reuse.Singleton);

            var a = c.Resolve<A>();

            Assert.IsNotNull(a);
        }

        [Test]
        public void Should_be_able_to_use_Func_with_argument_and_argument_in_scoped_dependency()
        {
            var c = new Container();

            c.Register<T>();
            c.Register<S>(Reuse.Scoped);

            using (var scope = c.OpenScope())
            {
                scope.Resolve<Func<X, T>>();            // interpreting
                var t = scope.Resolve<Func<X, T>>();    // compiling
                Assert.IsNotNull(t(new X()));
            }
        }

        public class T
        {
            public S S;
            public T(S s) => S = s;
        }

        public class S
        {
            public X X;
            public S(X x) => X = x;
        }

        public class X {}

        public class A
        {
            public A(B b1, B b2)
            {

            }
        }

        public class B
        {
        }
    }
}
