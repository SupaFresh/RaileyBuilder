namespace RaileyBuilder
{
    partial class RaileyBuilder
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.installServerButton = new System.Windows.Forms.Button();
            this.targetFolderTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.browseServerFolderButton = new System.Windows.Forms.Button();
            this.updateServerButton = new System.Windows.Forms.Button();
            this.progressLabel = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.installClientButton = new System.Windows.Forms.Button();
            this.updateClientButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // installServerButton
            // 
            this.installServerButton.Location = new System.Drawing.Point(12, 63);
            this.installServerButton.Name = "installServerButton";
            this.installServerButton.Size = new System.Drawing.Size(92, 45);
            this.installServerButton.TabIndex = 0;
            this.installServerButton.Text = "Install Server";
            this.installServerButton.UseVisualStyleBackColor = true;
            this.installServerButton.Click += new System.EventHandler(this.installServerButton_Click);
            // 
            // targetFolderTextBox
            // 
            this.targetFolderTextBox.Location = new System.Drawing.Point(12, 25);
            this.targetFolderTextBox.Name = "targetFolderTextBox";
            this.targetFolderTextBox.Size = new System.Drawing.Size(425, 20);
            this.targetFolderTextBox.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Installation Folder:";
            // 
            // browseServerFolderButton
            // 
            this.browseServerFolderButton.Location = new System.Drawing.Point(443, 22);
            this.browseServerFolderButton.Name = "browseServerFolderButton";
            this.browseServerFolderButton.Size = new System.Drawing.Size(25, 23);
            this.browseServerFolderButton.TabIndex = 3;
            this.browseServerFolderButton.Text = "...";
            this.browseServerFolderButton.UseVisualStyleBackColor = true;
            this.browseServerFolderButton.Click += new System.EventHandler(this.browseServerFolderButton_Click);
            // 
            // updateServerButton
            // 
            this.updateServerButton.Location = new System.Drawing.Point(110, 63);
            this.updateServerButton.Name = "updateServerButton";
            this.updateServerButton.Size = new System.Drawing.Size(92, 45);
            this.updateServerButton.TabIndex = 4;
            this.updateServerButton.Text = "Update Server";
            this.updateServerButton.UseVisualStyleBackColor = true;
            this.updateServerButton.Click += new System.EventHandler(this.updateServerButton_Click);
            // 
            // progressLabel
            // 
            this.progressLabel.AutoSize = true;
            this.progressLabel.Location = new System.Drawing.Point(12, 114);
            this.progressLabel.Name = "progressLabel";
            this.progressLabel.Size = new System.Drawing.Size(0, 13);
            this.progressLabel.TabIndex = 6;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 130);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(456, 23);
            this.progressBar.TabIndex = 7;
            // 
            // installClientButton
            // 
            this.installClientButton.Location = new System.Drawing.Point(278, 63);
            this.installClientButton.Name = "installClientButton";
            this.installClientButton.Size = new System.Drawing.Size(92, 45);
            this.installClientButton.TabIndex = 8;
            this.installClientButton.Text = "Install Client";
            this.installClientButton.UseVisualStyleBackColor = true;
            this.installClientButton.Click += new System.EventHandler(this.installClientButton_Click);
            // 
            // updateClientButton
            // 
            this.updateClientButton.Location = new System.Drawing.Point(376, 63);
            this.updateClientButton.Name = "updateClientButton";
            this.updateClientButton.Size = new System.Drawing.Size(92, 45);
            this.updateClientButton.TabIndex = 9;
            this.updateClientButton.Text = "Update Client";
            this.updateClientButton.UseVisualStyleBackColor = true;
            this.updateClientButton.Click += new System.EventHandler(this.updateClientButton_Click);
            // 
            // RaileyBuilder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(480, 160);
            this.Controls.Add(this.updateClientButton);
            this.Controls.Add(this.installClientButton);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.progressLabel);
            this.Controls.Add(this.updateServerButton);
            this.Controls.Add(this.browseServerFolderButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.targetFolderTextBox);
            this.Controls.Add(this.installServerButton);
            this.Name = "RaileyBuilder";
            this.Text = "Railey Builder";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button installServerButton;
        private System.Windows.Forms.TextBox targetFolderTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button browseServerFolderButton;
        private System.Windows.Forms.Button updateServerButton;
        private System.Windows.Forms.Label progressLabel;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button installClientButton;
        private System.Windows.Forms.Button updateClientButton;
    }
}

