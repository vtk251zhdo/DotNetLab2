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
    public partial class WelcomeForm : Form
    {
        private Bank bank;
        private AutomatedTellerMachine atm;

        public WelcomeForm()
        {
            InitializeComponent();
            InitializeATM();
        }

        private void InitializeATM()
        {
            bank = DataInitializer.CreateSampleBank();
            atm = new AutomatedTellerMachine(bank, "ATM-01", "Головна, 1", 20000m);
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            using (var login = new LoginForm(atm))
            {
                if (login.ShowDialog() == DialogResult.OK)
                {
                    var operations = new OperationsForm(atm);
                    this.Hide();
                    operations.ShowDialog();
                    this.Show();
                }
            }
        }
    }
}
