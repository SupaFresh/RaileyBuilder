﻿namespace RaileyBuilder
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
            this.serverFolderPathTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.browseServerFolderButton = new System.Windows.Forms.Button();
            this.updateServerButton = new System.Windows.Forms.Button();
            this.logBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // installServerButton
            // 
            this.installServerButton.Location = new System.Drawing.Point(12, 63);
            this.installServerButton.Name = "installServerButton";
            this.installServerButton.Size = new System.Drawing.Size(110, 45);
            this.installServerButton.TabIndex = 0;
            this.installServerButton.Text = "Install Server";
            this.installServerButton.UseVisualStyleBackColor = true;
            this.installServerButton.Click += new System.EventHandler(this.installServerButton_Click);
            // 
            // serverFolderPathTextBox
            // 
            this.serverFolderPathTextBox.Location = new System.Drawing.Point(12, 25);
            this.serverFolderPathTextBox.Name = "serverFolderPathTextBox";
            this.serverFolderPathTextBox.Size = new System.Drawing.Size(425, 20);
            this.serverFolderPathTextBox.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Server Folder:";
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
            this.updateServerButton.Location = new System.Drawing.Point(128, 63);
            this.updateServerButton.Name = "updateServerButton";
            this.updateServerButton.Size = new System.Drawing.Size(110, 45);
            this.updateServerButton.TabIndex = 4;
            this.updateServerButton.Text = "Update Server";
            this.updateServerButton.UseVisualStyleBackColor = true;
            this.updateServerButton.Click += new System.EventHandler(this.updateServerButton_Click);
            // 
            // logBox
            // 
            this.logBox.Location = new System.Drawing.Point(12, 135);
            this.logBox.Multiline = true;
            this.logBox.Name = "logBox";
            this.logBox.Size = new System.Drawing.Size(456, 218);
            this.logBox.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 119);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(25, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Log";
            // 
            // RaileyBuilder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(480, 365);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.logBox);
            this.Controls.Add(this.updateServerButton);
            this.Controls.Add(this.browseServerFolderButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.serverFolderPathTextBox);
            this.Controls.Add(this.installServerButton);
            this.Name = "RaileyBuilder";
            this.Text = "Railey Builder";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button installServerButton;
        private System.Windows.Forms.TextBox serverFolderPathTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button browseServerFolderButton;
        private System.Windows.Forms.Button updateServerButton;
        private System.Windows.Forms.TextBox logBox;
        private System.Windows.Forms.Label label2;
    }
}

