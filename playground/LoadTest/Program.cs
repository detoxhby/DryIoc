using System;

namespace LoadTest
{
    public class Program
    {

/*
## v4.1.5 - Singleton decorators 

Validation finished
00:01:24.33

ResolveAllControllersOnce of 156 controllers is done in 0.1533677 seconds

----------------------------------
 Starting compiled + cached tests
----------------------------------

New container created

container with ambient ScopeContext DryIoc.AsyncExecutionFlowScopeContext without scope
 with Rules  with Made={FactoryMethod=ConstructorWithResolvableArguments}

ResolveAllControllersOnce of 156 controllers is done in 0.0073486 seconds
ResolveAllControllersOnce of 156 controllers is done in 0.1591292 seconds
-- Starting Load test --
32 Threads.

-- Load Test Finished --
00:00:00.16

New container created

container with ambient ScopeContext DryIoc.AsyncExecutionFlowScopeContext without scope
 with Rules  with Made={FactoryMethod=ConstructorWithResolvableArguments}

ResolveAllControllersOnce of 156 controllers is done in 0.0080617 seconds
ResolveAllControllersOnce of 156 controllers is done in 0.1478801 seconds
-- Starting Randomized Load test --
155 Threads.

-- Randomized Load Finished --
00:00:00.36
*/
        static void Main(string[] args)
        {
            Console.WriteLine("Starting up!");

            InvalidProgramExceptionTest.Start();
            SplitDependencyGraphTest.Start();
            LoadTestBenchmark.Start();

            Console.WriteLine("Success!");
            Console.ReadKey();
        }
    }
}