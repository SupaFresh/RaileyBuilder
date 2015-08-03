using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RaileyBuilder
{
    public partial class RaileyBuilder : Form
    {
        public RaileyBuilder()
        {
            InitializeComponent();
        }

        private void browseServerFolderButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowNewFolderButton = true;
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                serverFolderPathTextBox.Text = fbd.SelectedPath;
            }
        }
    }
}
