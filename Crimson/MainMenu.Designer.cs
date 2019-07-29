namespace Crimson
{
    partial class MainMenu
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
            this.sortofLabel = new System.Windows.Forms.Label();
            this.launchButton = new System.Windows.Forms.Button();
            this.headerPicture = new System.Windows.Forms.PictureBox();
            this.genLevel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.headerPicture)).BeginInit();
            this.SuspendLayout();
            // 
            // sortofLabel
            // 
            this.sortofLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sortofLabel.AutoSize = true;
            this.sortofLabel.Font = new System.Drawing.Font("Impact", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.sortofLabel.ForeColor = System.Drawing.Color.White;
            this.sortofLabel.Location = new System.Drawing.Point(321, 242);
            this.sortofLabel.Name = "sortofLabel";
            this.sortofLabel.Size = new System.Drawing.Size(0, 117);
            this.sortofLabel.TabIndex = 1;
            // 
            // launchButton
            // 
            this.launchButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.launchButton.BackColor = System.Drawing.Color.IndianRed;
            this.launchButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.launchButton.ForeColor = System.Drawing.Color.White;
            this.launchButton.Location = new System.Drawing.Point(467, 576);
            this.launchButton.Name = "launchButton";
            this.launchButton.Size = new System.Drawing.Size(312, 102);
            this.launchButton.TabIndex = 2;
            this.launchButton.Text = "Press to start";
            this.launchButton.UseVisualStyleBackColor = false;
            this.launchButton.Click += new System.EventHandler(this.LaunchButton_Click);
            // 
            // headerPicture
            // 
            this.headerPicture.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.headerPicture.Image = global::Crimson.Properties.Resources.Crimsonland;
            this.headerPicture.InitialImage = global::Crimson.Properties.Resources.Crimsonland;
            this.headerPicture.Location = new System.Drawing.Point(12, 12);
            this.headerPicture.Name = "headerPicture";
            this.headerPicture.Size = new System.Drawing.Size(1280, 720);
            this.headerPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.headerPicture.TabIndex = 0;
            this.headerPicture.TabStop = false;
            // 
            // genLevel
            // 
            this.genLevel.AutoSize = true;
            this.genLevel.BackColor = System.Drawing.Color.IndianRed;
            this.genLevel.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.genLevel.ForeColor = System.Drawing.Color.White;
            this.genLevel.Location = new System.Drawing.Point(412, 350);
            this.genLevel.Name = "genLevel";
            this.genLevel.Size = new System.Drawing.Size(451, 37);
            this.genLevel.TabIndex = 3;
            this.genLevel.Text = "Generating level, please wait...";
            this.genLevel.Visible = false;
            // 
            // MainMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Firebrick;
            this.ClientSize = new System.Drawing.Size(1304, 747);
            this.Controls.Add(this.genLevel);
            this.Controls.Add(this.launchButton);
            this.Controls.Add(this.sortofLabel);
            this.Controls.Add(this.headerPicture);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "MainMenu";
            this.Text = "Main menu";
            ((System.ComponentModel.ISupportInitialize)(this.headerPicture)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox headerPicture;
        private System.Windows.Forms.Label sortofLabel;
        private System.Windows.Forms.Button launchButton;
        private System.Windows.Forms.Label genLevel;
    }
}