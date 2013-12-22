namespace StonehearthLauncher
{
    partial class MainForm
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
            this.LoginButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.EmailText = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.PasswordText = new System.Windows.Forms.TextBox();
            this.SnifferButton = new System.Windows.Forms.Button();
            this.ConfigureButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // LoginButton
            // 
            this.LoginButton.Location = new System.Drawing.Point(12, 64);
            this.LoginButton.Name = "LoginButton";
            this.LoginButton.Size = new System.Drawing.Size(280, 23);
            this.LoginButton.TabIndex = 4;
            this.LoginButton.Text = "&Login";
            this.LoginButton.UseVisualStyleBackColor = true;
            this.LoginButton.Click += new System.EventHandler(this.LoginButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "&Email:";
            // 
            // EmailText
            // 
            this.EmailText.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EmailText.Location = new System.Drawing.Point(74, 12);
            this.EmailText.MaxLength = 255;
            this.EmailText.Name = "EmailText";
            this.EmailText.Size = new System.Drawing.Size(218, 20);
            this.EmailText.TabIndex = 1;
            this.EmailText.TextChanged += new System.EventHandler(this.EmailText_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(12, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "&Password:";
            // 
            // PasswordText
            // 
            this.PasswordText.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PasswordText.Location = new System.Drawing.Point(74, 38);
            this.PasswordText.MaxLength = 20;
            this.PasswordText.Name = "PasswordText";
            this.PasswordText.PasswordChar = '*';
            this.PasswordText.Size = new System.Drawing.Size(218, 20);
            this.PasswordText.TabIndex = 3;
            this.PasswordText.Text = "test";
            this.PasswordText.TextChanged += new System.EventHandler(this.PasswordText_TextChanged);
            // 
            // SnifferButton
            // 
            this.SnifferButton.Location = new System.Drawing.Point(12, 122);
            this.SnifferButton.Name = "SnifferButton";
            this.SnifferButton.Size = new System.Drawing.Size(280, 23);
            this.SnifferButton.TabIndex = 6;
            this.SnifferButton.Text = "&Sniffer";
            this.SnifferButton.UseVisualStyleBackColor = true;
            this.SnifferButton.Click += new System.EventHandler(this.SnifferButton_Click);
            // 
            // ConfigureButton
            // 
            this.ConfigureButton.Location = new System.Drawing.Point(12, 93);
            this.ConfigureButton.Name = "ConfigureButton";
            this.ConfigureButton.Size = new System.Drawing.Size(280, 23);
            this.ConfigureButton.TabIndex = 5;
            this.ConfigureButton.Text = "&Configure";
            this.ConfigureButton.UseVisualStyleBackColor = true;
            this.ConfigureButton.Click += new System.EventHandler(this.ConfigureButton_Click);
            // 
            // MainForm
            // 
            this.AcceptButton = this.LoginButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::StonehearthLauncher.Properties.Resources.Background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(304, 154);
            this.Controls.Add(this.ConfigureButton);
            this.Controls.Add(this.SnifferButton);
            this.Controls.Add(this.PasswordText);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.EmailText);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.LoginButton);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Stonehearth Launcher";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button LoginButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox EmailText;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox PasswordText;
        private System.Windows.Forms.Button SnifferButton;
        private System.Windows.Forms.Button ConfigureButton;
    }
}

