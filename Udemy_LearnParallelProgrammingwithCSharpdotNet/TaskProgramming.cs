using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Udemy_LearnParallelProgrammingwithCSharpdotNet
{
    class TaskProgramming
    {
        public static void Write(char c)
        {
            int i = 1000;

            while (i-- > 0)
            {
                Console.Write(c);
            }
        }

        public static void Write(object o)
        {
            int i = 1000;

            while (i-- > 0)
            {
                Console.Write(o);
            }
        }

        public static int TextLength(object o)
        {
            Console.WriteLine($"\nTask with id {Task.CurrentId} processing object {o}");
            return o.ToString().Length;
        }
        public static void TaskProgrammingSection()
        {
            //Task.Factory.StartNew( () => Write('.'));

            //var t = new Task(() => Write('?'));
            //t.Start();

            //Write('-');

            //Task t = new Task(Write, "hello");
            //t.Start();

            //Task.Factory.StartNew(Write, 123);

            //string text1 = "testing";
            //string text2 = "this";

            //var task1 = new Task<int>(TextLength, text1);
            //task1.Start();

            //Task<int> task2 = Task.Factory.StartNew<int>(TextLength, text2);

            //Console.WriteLine($"Length of '{text1}' is {task1.Result}");
            //Console.WriteLine($"Length of '{text2}' is {task2.Result}");


            //var cts = new CancellationTokenSource();
            //var token = cts.Token;

            //token.Register(() => Console.WriteLine("Cancellation has been requested"));

            //var t = new Task( () =>
            //{
            //    int i = 0;
            //    //while (true)
            //    //{
            //    if (token.IsCancellationRequested)
            //    {
            //        throw new OperationCanceledException();
            //    }
            //    else
            //        Console.WriteLine($"{i++}\t");
            //}


            //    while (true)
            //    {
            //        token.ThrowIfCancellationRequested();
            //        Console.WriteLine($"{i++}\t");
            //    }

            //}, token);
            //t.Start();

            //Task.Factory.StartNew( () => {
            //    token.WaitHandle.WaitOne();
            //    Console.WriteLine("Wait handle has been released, cancellation was requested");
            //});
            //Console.ReadKey();
            //cts.Cancel();

            //    var planned = new CancellationTokenSource();
            //    var preventative = new CancellationTokenSource();
            //    var emergency = new CancellationTokenSource();

            //    var paranoid = CancellationTokenSource.CreateLinkedTokenSource(
            //                                                                                    planned.Token,
            //                                                                                    preventative.Token,
            //                                                                                    emergency.Token);

            //Task.Factory.StartNew( () => {
            //    int i = 0;

            //    while (true)
            //    {
            //        paranoid.Token.ThrowIfCancellationRequested();
            //        Console.WriteLine($"{i++}\t");
            //        Thread.Sleep(1000);
            //    }

            //}, paranoid.Token);


            ////emergency.Cancel();
            //Console.ReadKey();


            //var t = new Task( () => Thread.Sleep(500));
            //var t = new Task(() => Thread.SpinWait(500));
            // var t = new Task(() => SpinWait.SpinUntil());

            //var cts = new CancellationTokenSource();
            //var token = cts.Token;

            //var t = new Task( () => {
            //    Console.WriteLine("Press any key to disarm, you have 5 seconds");
            //    bool cancelled = token.WaitHandle.WaitOne(5000);
            //    Console.WriteLine(cancelled? "Bomb Disarmed":"BOOM!");

            //}, token);

            //t.Start();

            //Console.ReadKey();
            //cts.Cancel();
            //var cts = new CancellationTokenSource();
            //var token = cts.Token;


            //var t = new Task(() =>
            //{
            //    Console.WriteLine("Pause 5 seconds");

            //    for (int i = 0; i < 5; i++)
            //    {
            //        token.ThrowIfCancellationRequested();
            //        Thread.Sleep(1000);
            //    }

            //    Console.WriteLine("Done");
            //}, token);

            //t.Start();

            //Task t2 = Task.Factory.StartNew(() => Thread.Sleep(3000), token);
            ////CancellTask(cts);

            //// Task.WaitAll(t, t2);
            //Task.WaitAll(new[] { t, t2 }, 4000);

            //Console.WriteLine($"Task t status is {t.Status}");
            //Console.WriteLine($"Task t2 status is {t2.Status}");


            try
            {
                Test();
            }
            catch (AggregateException ae)
            {

                foreach (var e in ae.InnerExceptions)
                {
                    Console.WriteLine($"Handled elsewhere: {e.GetType()}");
                }
            }
        }

        private static void Test()
        {
            var t1 = Task.Factory.StartNew(() =>
            {
                throw new InvalidOperationException("Cat't do this!") { Source = "t1" };
            });

            var t2 = Task.Factory.StartNew(() =>
            {
                throw new AccessViolationException("Can't access this") { Source = "t2" };
            });


            try
            {
                Task.WaitAll(t1, t2);

            }
            catch (AggregateException ae)
            {
                //foreach (var ex in ae.InnerExceptions)
                //{
                //    Console.WriteLine($"Exception {ex.GetType()} from {ex.Source}");

                //}
                ae.Handle(e =>
                {
                    if (e is InvalidOperationException)
                    {
                        Console.WriteLine("Invalid Operation!");
                        return true;
                    }
                    return false;
                });
            }
        }

        private static void CancellTask(CancellationTokenSource cts)
        {
            Console.ReadKey();
            cts.Cancel();
        }
    }
}
