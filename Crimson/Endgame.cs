using System;
using System.Windows.Forms;

namespace Crimson
{
    public partial class Endgame : Form
    {
        public Endgame(string message)
        {
            InitializeComponent();
            resultLabel.Text = message;
        }

        private void BackToMenuButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
