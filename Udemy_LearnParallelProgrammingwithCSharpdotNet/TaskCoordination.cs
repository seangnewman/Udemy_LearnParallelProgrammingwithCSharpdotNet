using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Udemy_LearnParallelProgrammingwithCSharpdotNet
{
    class TaskCoordination
    {

        static Barrier barrier = new Barrier(2, b => {
            Console.WriteLine($"Phase {b.CurrentPhaseNumber} has finished");

        });


        public static void Demo()
        {
            //ContinuationDemo();

            //ChildTaskDemo();

            //BarrierDemo();

            //CountdownEventDemo();

            //ManualResetEventSlimDemo();

            //AutoResetEvent();

            SemaphoreSlimDemo();

        }

        public static void Water()
        {
            Console.WriteLine("Putting the kettle on (takes a bit longer)");
            Thread.Sleep(2000);
            barrier.SignalAndWait();
            Console.WriteLine("Pouring water into cup");
            barrier.SignalAndWait();
            Console.WriteLine("Putting kettle away");

        }


        public static void Cup()
        {
            Console.WriteLine("Finding the nicest cup of tea (fast)");
            barrier.SignalAndWait();
            Console.WriteLine("Adding tea to cup");
            barrier.SignalAndWait();
            Console.WriteLine("Adding sugar");
        }

        private static void SemaphoreSlimDemo()
        {
            var semaphore = new SemaphoreSlim(2, 10);

            for (int i = 0; i < 20; i++)
            {
                Task.Factory.StartNew(() =>
                {
                    Console.WriteLine($"Entering task {Task.CurrentId}");
                    semaphore.Wait(); /// Release cout --
                    Console.WriteLine($"Processing task {Task.CurrentId}");
                });
            }
            while (semaphore.CurrentCount <= 2)
            {
                Console.WriteLine($"Semaphore count: {semaphore.CurrentCount}");
                Console.ReadKey();
                semaphore.Release(2);
            }
        }

        private static void AutoResetEvent()
        {
            var evt = new AutoResetEvent(false); // false

            Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Boiling Water");
                evt.Set();   // true
            });

            var makeTea = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Waiting for water");
                evt.WaitOne();  // set to false again
                Console.WriteLine("Here is your tea");
                var ok = evt.WaitOne(1000);

                if (ok)
                {
                    Console.WriteLine("Enjoy your team");
                }
                else
                {
                    Console.WriteLine("No tea for you!");
                }
            });

            makeTea.Wait();
        }

        private static void ManualResetEventSlimDemo()
        {
            var evt = new ManualResetEventSlim();

            Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Boiling Water");
                evt.Set();
            });

            var makeTea = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Waiting for water");
                evt.Wait();
                Console.WriteLine("Here is your tea");
            });

            makeTea.Wait();
        }

        private static void CountdownEventDemo()
        {
            for (int i = 0; i < taskCount; i++)
            {
                Task.Factory.StartNew(() =>
                {
                    Console.WriteLine($"Entering task {Task.CurrentId}");
                    Thread.Sleep(3000);
                    cte.Signal();
                    Console.WriteLine($"Exiting task {Task.CurrentId}");

                });
            }

            var finalTask = Task.Factory.StartNew(() =>
            {
                Console.WriteLine($"Waiting for other tasks to tcomplete in {Task.CurrentId}");
                cte.Wait();
                Console.WriteLine("All tasks are completed");
            });

            finalTask.Wait();
        }

        private static int taskCount = 5;
        static CountdownEvent cte = new CountdownEvent(taskCount);
        static Random random = new Random();

        private static void BarrierDemo()
        {
            var water = Task.Factory.StartNew(Water);
            var cup = Task.Factory.StartNew(Cup);

            var tea = Task.Factory.ContinueWhenAll(new[] { water, cup }, tasks => Console.WriteLine("Enjoy your cup of tea"));

            tea.Wait();
        }

        private static void ChildTaskDemo()
        {
            var parent = new Task(() =>
            {
                var child = new Task(() =>
                {
                    Console.WriteLine("Child Task Starting");
                    Thread.Sleep(3000);
                    Console.WriteLine("Child Task Finishing");
                    throw new AggregateException();
                }, TaskCreationOptions.AttachedToParent);

                var completionHandler = child.ContinueWith((t) =>
                {
                    Console.WriteLine($"Hooray, task {t.Id}'s state is {t.Status} ");
                }, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.OnlyOnRanToCompletion);

                var failHandler = child.ContinueWith(t =>
                {
                    Console.WriteLine($"Oops , task {t.Id}'s state is {t.Status} ");
                }, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.OnlyOnFaulted);
                child.Start();
            });
            parent.Start();

            try
            {
                parent.Wait();
            }
            catch (AggregateException ae)
            {

                ae.Handle(e => true);
            }
        }

        private static void ContinuationDemo()
        {
            // var task = Task.Factory.StartNew(() => Console.WriteLine("Boiling Water"));

            //var task2 = task.ContinueWith(t => Console.WriteLine($"Completed {t.Id}, pour water into cup.") );

            // task2.Wait();

            var task = Task.Factory.StartNew(() => "Task 1");
            var task2 = Task.Factory.StartNew(() => "Task 2");

            //var task3 = Task.Factory.ContinueWhenAll(new[] { task, task2},  tasks => {
            //    Console.WriteLine($"Tasks completed");
            //    foreach (var t in tasks)
            //    {
            //        Console.WriteLine(" - " + t.Result);

            //    }
            //    Console.WriteLine("All Tasks Done");
            //});

            var task3 = Task.Factory.ContinueWhenAny(new[] { task, task2 }, t =>
            {
                Console.WriteLine($"Tasks completed");
                Console.WriteLine(" - " + t.Result);
                Console.WriteLine("All Tasks Done");
            });

            task3.Wait();
        }
    }
}
