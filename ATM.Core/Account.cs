using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATM.Core
{
    public class Account
    {
        public string CardNumber { get; set; }
        public string OwnerName { get; set; }
        public string PinCode { get; set; }
        public decimal Balance { get; private set; }

        public Account(string cardNumber, string ownerName, string pinCode, decimal balance)
        {
            CardNumber = cardNumber;
            OwnerName = ownerName;
            PinCode = pinCode;
            Balance = balance;
        }

        public void Deposit(decimal amount)
        {
            if (amount <= 0) throw new ArgumentException("Сума має бути більшою за 0.");
            Balance += amount;
        }

        public bool Withdraw(decimal amount)
        {
            if (amount <= 0) throw new ArgumentException("Сума має бути більшою за 0.");
            if (amount > Balance) return false;
            Balance -= amount;
            return true;
        }
    }
}
