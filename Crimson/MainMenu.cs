using System;
using System.Windows.Forms;

namespace Crimson
{
    public partial class MainMenu : Form
    {
        public MainMenu()
        {
            InitializeComponent();
        }

        private void LaunchButton_Click(object sender, EventArgs e)
        {
            launchButton.Enabled = false;
            genLevel.Show();
            var frm = new MainForm
            {
                Location = Location,
                StartPosition = FormStartPosition.Manual
            };
            frm.FormClosing += delegate { Show(); };
            frm.Show();
            Hide();
        }
    }
}
