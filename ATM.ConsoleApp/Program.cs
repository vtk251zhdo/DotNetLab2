using System;
using ATM.Core;

namespace ATM.ConsoleApp
{
    internal class Program
    {
        static void Main()
        {
            var bank = DataInitializer.CreateSampleBank();
            var atm = new AutomatedTellerMachine(bank, "ATM-01", "Головна вулиця, 1", 20000m);

            atm.OnAuthenticate += msg => Console.WriteLine("[AUTH] " + msg);
            atm.OnBalanceChecked += msg => Console.WriteLine("[BALANCE] " + msg);
            atm.OnWithdraw += msg => Console.WriteLine("[WITHDRAW] " + msg);
            atm.OnDeposit += msg => Console.WriteLine("[DEPOSIT] " + msg);
            atm.OnTransfer += msg => Console.WriteLine("[TRANSFER] " + msg);

            Console.WriteLine("=== Тестовий режим банкомату (консоль) ===");

            Console.Write("Введіть номер картки: ");
            string card = Console.ReadLine() ?? "";
            Console.Write("Введіть PIN: ");
            string pin = Console.ReadLine() ?? "";

            if (!atm.Authenticate(card, pin))
            {
                Console.WriteLine("Аутентифікація не пройшла. Закінчення.");
                return;
            }

            while (true)
            {
                Console.WriteLine("\n1) Баланс  2) Зняти  3) Поповнити  4) Переказ  5) Вихід");
                Console.Write("Вибір: ");
                var choice = Console.ReadLine() ?? "";

                try
                {
                    if (choice == "1") atm.CheckBalance();
                    else if (choice == "2")
                    {
                        Console.Write("Сума: ");
                        if (decimal.TryParse(Console.ReadLine(), out decimal sum)) atm.Withdraw(sum);
                        else Console.WriteLine("Некоректна сума.");
                    }
                    else if (choice == "3")
                    {
                        Console.Write("Сума: ");
                        if (decimal.TryParse(Console.ReadLine(), out decimal sum)) atm.Deposit(sum);
                        else Console.WriteLine("Некоректна сума.");
                    }
                    else if (choice == "4")
                    {
                        Console.Write("Картка отримувача: ");
                        string to = Console.ReadLine() ?? "";
                        Console.Write("Сума: ");
                        if (decimal.TryParse(Console.ReadLine(), out decimal sum)) atm.Transfer(to, sum);
                        else Console.WriteLine("Некоректна сума.");
                    }
                    else if (choice == "5")
                    {
                        atm.Logout();
                        break;
                    }
                    else Console.WriteLine("Невідомий вибір.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Помилка: " + ex.Message);
                }
            }

            Console.WriteLine("Кінець роботи. Натисніть Enter...");
            Console.ReadLine();
        }
    }
}
