using System;
using System.Diagnostics;
using System.Web.Http;
using DryIoc;
using DryIoc.WebApi;

namespace LoadTest
{
    static class SplitDependencyGraphTest
    {

        public static IContainer GetContainerForTest(int depth)
        {
            var container = new Container((rules) =>
                rules
                    .WithDependencyDepthToSplitObjectGraph(depth)
                    .WithoutInterpretationForTheFirstResolution()
                    .WithoutUseInterpretation()
                    .With(FactoryMethod.ConstructorWithResolvableArguments)
            ).WithWebApi(new HttpConfiguration());

            Registrations.RegisterTypes(container, true);

            return container;
        }

        private static void ResolveAllControllers(IContainer container, Type[] controllerTypes)
        {
            foreach (var controllerType in controllerTypes)
            {
                using (var scope = container.OpenScope(Reuse.WebRequestScopeName))
                {
                    var controller = scope.Resolve(controllerType);

                    if (controller == null)
                    {
                        throw new Exception("Invalid result!");
                    }
                }
            }
        }

        public static void Start()
        {
            Console.WriteLine("Starting WithDependencyDepthToSplitObjectGraph test");

            var controllerTypes = TestHelper.GetAllControllers();

            Console.WriteLine("First without depth... a very big depth");
            TryDepth(int.MaxValue, controllerTypes);

            for (var depth = 1; depth < 20; depth++)
            {
                TryDepth(depth, controllerTypes);
            }
        }

        private static void TryDepth(int depth, Type[] controllerTypes)
        {
            Console.WriteLine("Depth " + depth);

            var container = GetContainerForTest(depth);

            try
            {
                ResolveAllControllers(container, controllerTypes);
                ResolveAllControllers(container, controllerTypes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed with '{ex.Message}'");

                var sw = Stopwatch.StartNew();
                GC.Collect(2, GCCollectionMode.Forced);
                GC.WaitForFullGCComplete();
                Console.WriteLine($"GC complete in {sw.ElapsedMilliseconds:## ###} ms");
                sw.Stop();
            }
        }
    }
}
