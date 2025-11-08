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
    public partial class LoginForm : Form
    {
        private AutomatedTellerMachine atm;
        public string CardNumber => txtCard.Text.Trim();
        public string PinCode => txtPin.Text.Trim();

        public LoginForm(AutomatedTellerMachine atm)
        {
            InitializeComponent();
            this.atm = atm;
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CardNumber) || string.IsNullOrWhiteSpace(PinCode))
            {
                MessageBox.Show("Введіть номер картки та PIN!");
                return;
            }

            bool success = atm.Authenticate(CardNumber, PinCode);
            if (success)
            {
                DialogResult = DialogResult.OK;
                this.Close();
            }

        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

    }
}
