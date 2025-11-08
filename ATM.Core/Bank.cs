using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATM.Core
{
    public class Bank
    {
        public string Name { get; set; }
        public List<Account> Accounts { get; set; }

        public Bank(string name)
        {
            Name = name;
            Accounts = new List<Account>();
        }

        public void AddAccount(Account account)
        {
            Accounts.Add(account);
        }

        public Account GetAccount(string cardNumber, string pin)
        {
            return Accounts.Find(a => a.CardNumber == cardNumber && a.PinCode == pin);
        }

        public Account FindAccount(string cardNumber)
        {
            return Accounts.Find(a => a.CardNumber == cardNumber);
        }
    }
}
