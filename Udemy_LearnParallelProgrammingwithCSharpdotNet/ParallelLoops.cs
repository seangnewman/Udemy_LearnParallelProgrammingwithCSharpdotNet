using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Udemy_LearnParallelProgrammingwithCSharpdotNet
{
    class ParallelLoops
    {



        [Benchmark]
        public void SquareEachValue()
        {
            const int count = 100000;

            var values = Enumerable.Range(0, count);
            var results = new int[count];

            Parallel.ForEach(values, x => results[x] = (int)Math.Pow(x, 2));

        }

        [Benchmark]
        public void SquareEachValueChunked()
        {
            const int count = 100000;

            var values = Enumerable.Range(0, count);
            var results = new int[count];

            var part = Partitioner.Create(0, count, 10000);
            Parallel.ForEach(part, range => {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    results[i] = (int)Math.Pow(i, 2);
                }
            });


        }

        private static void ThreadLocalStorageDemo()
        {
            int sum = 0;

            Parallel.For(1, 1001, () => 0, (counter, state, tls) =>
            {
                tls += counter;
                Console.WriteLine($"Task of {Task.CurrentId }has sum of {tls}");
                return tls;
            },
            partialSum =>
            {
                Console.WriteLine($"Partial value of task {Task.CurrentId} is {partialSum}");
                Interlocked.Add(ref sum, partialSum);
            });

            Console.WriteLine($"Sum of 1 .. 100 = {sum}");
        }

        private static void BreakingAndCancellingLoops()
        {
            try
            {
                Demo();
            }
            catch (AggregateException ae)
            {

                ae.Handle(e =>
                {
                    Console.WriteLine(e.Message);
                    return true;
                });
            }
            catch (OperationCanceledException)
            {

            }

            Console.WriteLine();
            Console.WriteLine($"Was the loop completed? {result.IsCompleted}");

            if (result.LowestBreakIteration.HasValue)
            {
                Console.WriteLine($"Lowest break iteration is {result.LowestBreakIteration}");
            }
        }

        private static ParallelLoopResult result;

        public static void Demo()
        {
            //Parallel.For(0, 20, x =>
            //{
            //    Console.WriteLine($"{x} [{Task.CurrentId}]\t");
            //});

            var cts = new CancellationTokenSource();
            ParallelOptions po = new ParallelOptions();


            result = Parallel.For(0, 20, po, (int x, ParallelLoopState state) => {
                Console.WriteLine($"{x} [{Task.CurrentId}]\t");
                if (x == 10)
                {
                    //state.Stop();
                    // state.Break();
                    //throw new Exception();
                    cts.Cancel();
                }
            });
        }
        public static IEnumerable<int> Range(int start, int end, int step)
        {
            for (int i = start; i < end; i += step)
            {
                yield return i;
            }
        }

        private static void ParallelDemo()
        {
            var a = new Action(() => Console.WriteLine($"First {Task.CurrentId}"));
            var b = new Action(() => Console.WriteLine($"Second  {Task.CurrentId}"));
            var c = new Action(() => Console.WriteLine($"Third  {Task.CurrentId}"));

            Parallel.Invoke(a, b, c);

            Parallel.For(1, 11, i =>
            {
                Console.WriteLine($"{i * i} \t");
            });


            string[] words = { "oh", "what", "a", "night" };
            Parallel.ForEach(words, w =>
            {
                Console.WriteLine($"The length of {w} is {w.Length} (task {Task.CurrentId})");
            });
        }

    }
}
