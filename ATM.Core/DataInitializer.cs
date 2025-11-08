using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATM.Core
{
    public static class DataInitializer
    {
        public static Bank CreateSampleBank()
        {
            var bank = new Bank("DemoBank");
            bank.AddAccount(new Account("1111", "Іван Іванов", "1234", 5000m));
            bank.AddAccount(new Account("2222", "Петро Петренко", "5678", 3000m));
            bank.AddAccount(new Account("3333", "Оля Олексіївна", "0000", 10000m));
            return bank;
        }
    }
}