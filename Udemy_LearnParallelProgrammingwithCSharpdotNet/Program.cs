using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Udemy_LearnParallelProgrammingwithCSharpdotNet
{
    public class Program
    {

        private static ConcurrentDictionary<string, string> capitals = new ConcurrentDictionary<string, string>();

        public static void AddParis()
        {
            bool success = capitals.TryAdd("France", "Paris");
            string who = Task.CurrentId.HasValue ? $"Task {Task.CurrentId}" : "Main Thread";
            Console.WriteLine($"{who }  {(success?"added":"did not add")}  the element");
        }

        static void Main(string[] args)
        {
            //ConcurrentDictionaryDemo();

            //ConcurrentQueueDemo();

            //ConcurrentStackDemo();

            var bag = new ConcurrentBag<int>();

            var tasks = new List<Task>();

           
            for (int i = 0; i < 10; i++)
            {
                var i1 = i;
                tasks.Add(Task.Factory.StartNew( () => {
                   bag.Add(i1);
                    Console.WriteLine($"{Task.CurrentId} has added {i1}");
                    int result;

                    if(bag.TryPeek(out result))
                    {
                        Console.WriteLine($"{Task.CurrentId} has peeked the value {result}");
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());

            int last;
            if(bag.TryTake(out last))
            {
                Console.WriteLine($"I got {last}");
            }

        }

        private static void ConcurrentStackDemo()
        {
            var stack = new ConcurrentStack<int>();
            stack.Push(1);
            stack.Push(2);
            stack.Push(3);
            stack.Push(4);

            int result;
            if (stack.TryPeek(out result))
            {
                Console.WriteLine($"{result} is on top.");
            }

            if (stack.TryPop(out result))
            {
                Console.WriteLine($"Popped {result}");
            }

            var items = new int[5];

            if (stack.TryPopRange(items, 0, 5) > 0)
            {
                var text = string.Join(", ", items.Select(i => i.ToString()));
                Console.WriteLine($"Popped these items: { text}");
            }
        }

        private static void ConcurrentQueueDemo()
        {
            var q = new ConcurrentQueue<int>();
            q.Enqueue(1);
            q.Enqueue(2);

            int result;

            if (q.TryDequeue(out result)) ;
            {
                Console.WriteLine($"Removed element {result}");
            }

            if (q.TryPeek(out result))
            {
                Console.WriteLine($"Front element is {result}");
            }
        }

        private static void ConcurrentDictionaryDemo()
        {
            Task.Factory.StartNew(AddParis).Wait();
            AddParis();
            //capitals["Russia"] = "Leningrad";
            // capitals["Russia"] = "Moscow";

            //capitals.AddOrUpdate("Russia", "Moscow", (k,old) => old +  "-->  Moscow");
            //Console.WriteLine($"The capital of Russia is {capitals["Russia"]}");

            //capitals["Sweden"] = "Uppsala";
            var capOfSweden = capitals.GetOrAdd("Sweden", "Stockholm");
            Console.WriteLine($"The capital of Sweden is {capitals["Sweden"]}");

            const string toRemove = "Russia";
            string removed;

            var didRemove = capitals.TryRemove(toRemove, out removed);

            if (didRemove)
            {
                Console.WriteLine($"We just removed {removed}");
            }
            else
            {
                Console.WriteLine($"Failed to remove the capital of {toRemove}");
            }

            foreach (var capital in capitals)
            {
                Console.WriteLine($" - {capital.Value} is the capital of  {capital.Key}");
            }
        }

    }
}