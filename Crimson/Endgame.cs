using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
