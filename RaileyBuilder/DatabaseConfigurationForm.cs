using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RaileyBuilder
{
    public partial class DatabaseConfigurationForm : Form
    {
        public string DatabaseUsername { get; private set; }
        public string DatabasePassword { get; private set; }
        public int DatabasePort { get; private set; }

        public DatabaseConfigurationForm()
        {
            InitializeComponent();
        }

        private void confirmButton_Click(object sender, EventArgs e)
        {
            this.DatabaseUsername = databaseUsernameTextBox.Text;
            this.DatabasePassword = databasePasswordTextBox.Text;
            this.DatabasePort = (int)numericUpDown1.Value;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }
    }
}
