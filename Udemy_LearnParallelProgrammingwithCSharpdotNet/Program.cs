using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Udemy_LearnParallelProgrammingwithCSharpdotNet
{

    
    class Program
    {
      

        static void Main(string[] args)
        {
            // TaskProgramming.TaskProgrammingSection();

            // LockDemo();
            //SpinLockDemo();
            //LockRecursion(5);

            //MutexExample();

            const string appName = "MyApp";
            Mutex mutex;
            try
            {
                mutex = Mutex.OpenExisting(appName);

                Console.WriteLine($"Sorry, {appName} is already running");

            }
            catch (WaitHandleCannotBeOpenedException ex)
            {

                Console.WriteLine("We can run the program");
                mutex = new Mutex(false, appName);
            }
            Console.ReadKey();
            mutex.ReleaseMutex();

        }

        private static void MutexExample()
        {
            var tasks = new List<Task>();
            var ba = new BankAccount();
            var ba2 = new BankAccount();

            Mutex mutex = new Mutex();
            Mutex mutex2 = new Mutex();

            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        bool haveLock = mutex.WaitOne();

                        try
                        {
                            ba.Deposit(1);
                        }
                        finally
                        {
                            if (haveLock)
                            {
                                mutex.ReleaseMutex();
                            }

                        }

                    }

                }));

                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        bool haveLock = mutex2.WaitOne();

                        try
                        {
                            ba2.Deposit(1);
                        }
                        finally
                        {
                            if (haveLock)
                            {
                                mutex2.ReleaseMutex();
                            }

                        }

                    }

                }));

                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        bool haveLock = Mutex.WaitAll(new[] { mutex, mutex2 });
                        try
                        {
                            ba.Transter(ba2, 1);
                        }
                        finally
                        {

                            if (haveLock)
                            {
                                mutex.ReleaseMutex();
                                mutex2.ReleaseMutex();
                            }
                        }
                    }
                }));

            }


            Task.WaitAll(tasks.ToArray());
            Console.WriteLine($"Final balance  of ba is {ba.Balance}");
            Console.WriteLine($"Final balance of ba2 is {ba2.Balance}");
        }

        static SpinLock sl = new SpinLock(true);

        public static void LockRecursion(int x)
        {
            bool lockTaken = false;
            try
            {
                sl.Enter(ref lockTaken);
            }
            catch(LockRecursionException e)
            {

                Console.WriteLine($"Exception: {e}");
            }
            finally
            {
                if (lockTaken)
                {
                    Console.WriteLine($"Took a lock, x = {x}");
                    LockRecursion(x - 1);
                    sl.Exit();
                }
                else
                {
                    Console.WriteLine($"Faled to take a lock, x = {x}");
                }
            }


        }

        private static void SpinLockDemo()
        {
            var tasks = new List<Task>();
            var ba = new BankAccount();

            SpinLock sl = new SpinLock();



            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        var lockTaken = false;
                        try
                        {
                            sl.Enter(ref lockTaken);
                            ba.Deposit(100);
                        }
                        finally 
                        {

                            if (lockTaken)
                            {
                                sl.Exit();
                            }
                        }
                      
                    }
                }));

                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        var lockTaken = false;
                        try
                        {
                            sl.Enter(ref lockTaken);
                            ba.Withdraw(100);
                        }
                        finally
                        {

                            if (lockTaken)
                            {
                                sl.Exit();
                            }
                        }
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());
            Console.WriteLine($"Final balance is {ba.Balance}");
        }

        private static void LockDemo()
        {
            var tasks = new List<Task>();


            var ba = new BankAccount();



            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        ba.Deposit(100);
                    }
                }));

                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        ba.Withdraw(100);
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());
            Console.WriteLine($"Final balance is {ba.Balance}");
        }

    }
}
