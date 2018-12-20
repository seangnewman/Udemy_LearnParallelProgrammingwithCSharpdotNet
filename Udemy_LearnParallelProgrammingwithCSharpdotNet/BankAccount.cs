using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Udemy_LearnParallelProgrammingwithCSharpdotNet
{
    public class BankAccount
    {
        private int _balance;

        public object padlock = new object();

        public int Balance { get => _balance; private set => _balance = value; }

        public void Deposit(int amount)
        {
            // DepositLockExample(amount);

            //DepositInterlockedExample(amount);
            Balance += amount;
        }

        private void DepositInterlockedExample(int amount)
        {
            Interlocked.Add(ref _balance, amount);
        }

        private void DepositLockExample(int amount)
        {
            lock (padlock)
            {
                Balance += amount;
            }
        }

        public void Withdraw(int amount)
        {
            //WithdrawalLockExample(amount);

            //WithdrawalInterlockedExample(amount);
            Balance -= amount;
        }

        private void WithdrawalInterlockedExample(int amount)
        {
            Interlocked.Add(ref _balance, -amount);
        }

        private void WithdrawalLockExample(int amount)
        {
            lock (padlock)
            {
                Balance -= amount;
            }
        }

        public void Transter( BankAccount where, int amount)
        {
            Balance -= amount;
            where.Balance += amount;

        }
    }
}
