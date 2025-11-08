using ATM.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ATM.WinApp
{
    public partial class OperationsForm : Form
    {
        private AutomatedTellerMachine atm;

        public OperationsForm(AutomatedTellerMachine atm)
        {
            InitializeComponent();
            this.atm = atm;

            atm.OnBalanceChecked += ShowMessage;
            atm.OnWithdraw += ShowMessage;
            atm.OnDeposit += ShowMessage;
            atm.OnTransfer += ShowMessage;

            UpdateUserInfo();
        }


        private void ShowMessage(string message)
        {
            MessageBox.Show(message, "Банкомат", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void UpdateUserInfo()
        {
            if (atm.CurrentAccount != null)
            {
                lblUserName.Text = $"Користувач: {atm.CurrentAccount.OwnerName}";
                lblBalance.Text = $"Баланс: {atm.CurrentAccount.Balance} грн";
            }
        }


        private void btnBalance_Click(object sender, EventArgs e)
        {
            atm.CheckBalance();
            UpdateUserInfo();
        }


        private void btnWithdraw_Click(object sender, EventArgs e)
        {
            string input = Prompt.ShowDialog("Введіть суму для зняття:", "Зняття");
            if (decimal.TryParse(input, out decimal amount))
                atm.Withdraw(amount);

            UpdateUserInfo();
        }

        private void btnDeposit_Click(object sender, EventArgs e)
        {
            string input = Prompt.ShowDialog("Введіть суму для поповнення:", "Поповнення");
            if (decimal.TryParse(input, out decimal amount))
                atm.Deposit(amount);

            UpdateUserInfo();
        }


        private void btnTransfer_Click(object sender, EventArgs e)
        {
            string cardTo = Prompt.ShowDialog("Номер картки отримувача:", "Переказ");
            string sAmount = Prompt.ShowDialog("Сума переказу:", "Переказ");

            if (decimal.TryParse(sAmount, out decimal amount))
                atm.Transfer(cardTo, amount);

            UpdateUserInfo();
        }


        private void btnLogout_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OperationsForm_Load(object sender, EventArgs e)
        {
            if (atm.CurrentAccount != null)
            {
                lblUserName.Text = $"Користувач: {atm.CurrentAccount.OwnerName}";
                lblBalance.Text = $"Баланс: {atm.CurrentAccount.Balance} грн";
            }
        }
    }
}
