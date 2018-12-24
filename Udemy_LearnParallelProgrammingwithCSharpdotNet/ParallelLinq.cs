using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Udemy_LearnParallelProgrammingwithCSharpdotNet
{
    class ParallelLinq
    {

        public static void Demo()
        {
            // ********************
            // AsParallelDemo();
            //CancellationAndExceptions();

            //MergecOptions();


            CustomAggregationDemo();
        }

        private static void CustomAggregationDemo()
        {
            //var sum = Enumerable.Range(1, 1000).Sum();
            //var sum = Enumerable.Range(1, 1000).Aggregate(0, (i, acc ) => {
            //    return i + acc;
            //});

            var sum = ParallelEnumerable.Range(1, 1000)
                                                                       .Aggregate(0,
                                                                                            (partialSum, i) => partialSum + i,
                                                                                            (total, subTotal) => total += subTotal,
                                                                                            value => value);



            Console.WriteLine($"sum = {sum}");
        }

        private static void MergecOptions()
        {
            var numbers = Enumerable.Range(1, 20).ToArray();

            var results = numbers
                .AsParallel()
                .WithMergeOptions(ParallelMergeOptions.FullyBuffered)
                .Select(x =>
                {
                    var result = Math.Log10(x);
                    Console.Write($"P {result}\t");
                    return result;
                });

            foreach (var result in results)
            {
                Console.Write($"C  {result}\t");
            }
        }

        private static void CancellationAndExceptions()
        {
            var cts = new CancellationTokenSource();
            var items = ParallelEnumerable.Range(1, 20);

            var results = items
                                    .WithCancellation(cts.Token)
                                    .Select(i =>
                                    {
                                        double result = Math.Log10(i);

                                        //if(result > 1)
                                        //{
                                        //    throw new InvalidOperationException();
                                        //}
                                        Console.WriteLine($"i = {i}, tid = {Task.CurrentId}");
                                        return result;
                                    });

            try
            {
                foreach (var result in results)
                {
                    if (result > 1)
                    {
                        cts.Cancel();
                    }
                    Console.WriteLine($"result = {result}");
                }
            }
            catch (AggregateException ae)
            {

                ae.Handle(e =>
                {
                    Console.WriteLine($"{e.GetType().Name} : {e.Message}");
                    return true;
                });
            }
            catch (OperationCanceledException oce)
            {
                Console.WriteLine("Cancelled");
            }
        }

        private static void AsParallelDemo()
        {
            const int count = 50;

            var items = Enumerable.Range(1, count).ToArray();
            var results = new int[count];

            items.AsParallel().ForAll(x =>
            {
                int newValue = x * x * x;
                Console.Write($"{newValue} ({Task.CurrentId})\t");
                results[x - 1] = newValue;
            });

            Console.WriteLine();
            Console.WriteLine();

            //foreach (var result in results)
            //{
            //    Console.WriteLine($"{result}\t");
            //}
            //Console.WriteLine();

            var cubes = items.AsParallel()
                                    .AsOrdered()
                                    //  .AsUnordered()
                                    .Select(x => x * x * x);

            foreach (var cube in cubes)
            {
                Console.Write($"{cube}\t");
            }
            Console.WriteLine();
        }
    }
}
