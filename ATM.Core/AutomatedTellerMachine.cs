using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATM.Core
{
    public delegate void ATMEventHandler(string message);

    public class AutomatedTellerMachine
    {
        public string Id { get; set; }
        public string Address { get; set; }
        public decimal CashAmount { get; set; }

        public Account CurrentAccount => currentAccount;


        public event ATMEventHandler OnAuthenticate;
        public event ATMEventHandler OnBalanceChecked;
        public event ATMEventHandler OnWithdraw;
        public event ATMEventHandler OnDeposit;
        public event ATMEventHandler OnTransfer;

        private Account currentAccount;
        private Bank bank;

        public AutomatedTellerMachine(Bank bank, string id, string address, decimal cash)
        {
            this.bank = bank;
            Id = id;
            Address = address;
            CashAmount = cash;
        }

        public bool Authenticate(string cardNumber, string pin)
        {
            currentAccount = bank.GetAccount(cardNumber, pin);
            if (currentAccount != null)
            {
                OnAuthenticate?.Invoke($"Успішний вхід. Власник: {currentAccount.OwnerName}");
                return true;
            }
            OnAuthenticate?.Invoke("Помилка аутентифікації: неправильний номер картки або PIN.");
            return false;
        }

        public void Logout()
        {
            currentAccount = null;
            OnAuthenticate?.Invoke("Вихід (сесія завершена).");
        }

        public decimal CheckBalance()
        {
            if (currentAccount == null) throw new InvalidOperationException("Необхідна аутентифікація.");
            OnBalanceChecked?.Invoke($"Баланс: {currentAccount.Balance} грн.");
            return currentAccount.Balance;
        }

        public bool Withdraw(decimal amount)
        {
            if (currentAccount == null) throw new InvalidOperationException("Необхідна аутентифікація.");
            if (amount <= 0)
            {
                OnWithdraw?.Invoke("Недопустима сума для зняття.");
                return false;
            }

            if (CashAmount < amount)
            {
                OnWithdraw?.Invoke("У банкоматі недостатньо готівки.");
                return false;
            }

            if (currentAccount.Withdraw(amount))
            {
                CashAmount -= amount;
                OnWithdraw?.Invoke($"Знято {amount} грн. Новий баланс: {currentAccount.Balance} грн.");
                return true;
            }

            OnWithdraw?.Invoke("Недостатньо коштів на картці.");
            return false;
        }

        public void Deposit(decimal amount)
        {
            if (currentAccount == null) throw new InvalidOperationException("Необхідна аутентифікація.");
            if (amount <= 0)
            {
                OnDeposit?.Invoke("Недопустима сума для поповнення.");
                return;
            }
            currentAccount.Deposit(amount);
            CashAmount += amount;
            OnDeposit?.Invoke($"Зараховано {amount} грн. Новий баланс: {currentAccount.Balance} грн.");
        }

        public bool Transfer(string toCardNumber, decimal amount)
        {
            if (currentAccount == null) throw new InvalidOperationException("Необхідна аутентифікація.");
            if (amount <= 0)
            {
                OnTransfer?.Invoke("Недопустима сума для переказу.");
                return false;
            }

            var target = bank.FindAccount(toCardNumber);
            if (target == null)
            {
                OnTransfer?.Invoke("Отримувача не знайдено.");
                return false;
            }

            if (!currentAccount.Withdraw(amount))
            {
                OnTransfer?.Invoke("Недостатньо коштів для переказу.");
                return false;
            }

            target.Deposit(amount);
            OnTransfer?.Invoke($"Переказ {amount} грн на {toCardNumber} виконано. Ваш новий баланс: {currentAccount.Balance} грн.");
            return true;
        }
    }
}
