using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ATM.WinApp
{
    public static class Prompt
    {
        public static string ShowDialog(string text, string caption)
        {
            Form prompt = new Form()
            {
                Width = 380,
                Height = 180,
                Text = caption,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.FromArgb(245, 247, 250)
            };

            Label label = new Label()
            {
                Left = 20,
                Top = 20,
                Width = 320,
                Text = text,
                Font = new Font("Segoe UI", 10, FontStyle.Regular)
            };

            TextBox input = new TextBox()
            {
                Left = 20,
                Top = 55,
                Width = 320,
                Font = new Font("Segoe UI", 10)
            };

            Button ok = new Button()
            {
                Text = "OK",
                Left = 180,
                Width = 70,
                Top = 95,
                DialogResult = DialogResult.OK,
                BackColor = Color.LightGreen
            };

            Button cancel = new Button()
            {
                Text = "Скасувати",
                Left = 260,
                Width = 80,
                Top = 95,
                DialogResult = DialogResult.Cancel,
                BackColor = Color.LightCoral
            };

            prompt.Controls.Add(label);
            prompt.Controls.Add(input);
            prompt.Controls.Add(ok);
            prompt.Controls.Add(cancel);

            prompt.AcceptButton = ok;
            prompt.CancelButton = cancel;

            return prompt.ShowDialog() == DialogResult.OK ? input.Text : string.Empty;
        }
    }
}
