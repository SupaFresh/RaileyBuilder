using System;
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
            DatabaseUsername = databaseUsernameTextBox.Text;
            DatabasePassword = databasePasswordTextBox.Text;
            DatabasePort = (int)numericUpDown1.Value;
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }
    }
}