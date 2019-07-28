namespace Crimson
{
    partial class Endgame
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
            this.headerPicture = new System.Windows.Forms.PictureBox();
            this.backToMenuButton = new System.Windows.Forms.Button();
            this.resultLabel = new System.Windows.Forms.Label();
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
            this.sortofLabel.Location = new System.Drawing.Point(92, 135);
            this.sortofLabel.Name = "sortofLabel";
            this.sortofLabel.Size = new System.Drawing.Size(0, 117);
            this.sortofLabel.TabIndex = 4;
            // 
            // headerPicture
            // 
            this.headerPicture.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.headerPicture.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.headerPicture.Image = global::Crimson.Properties.Resources.Crimsonland;
            this.headerPicture.InitialImage = null;
            this.headerPicture.Location = new System.Drawing.Point(12, 12);
            this.headerPicture.Name = "headerPicture";
            this.headerPicture.Size = new System.Drawing.Size(1277, 719);
            this.headerPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.headerPicture.TabIndex = 3;
            this.headerPicture.TabStop = false;
            // 
            // backToMenuButton
            // 
            this.backToMenuButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.backToMenuButton.BackColor = System.Drawing.Color.IndianRed;
            this.backToMenuButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.backToMenuButton.ForeColor = System.Drawing.Color.White;
            this.backToMenuButton.Location = new System.Drawing.Point(1031, 611);
            this.backToMenuButton.Name = "backToMenuButton";
            this.backToMenuButton.Size = new System.Drawing.Size(248, 108);
            this.backToMenuButton.TabIndex = 5;
            this.backToMenuButton.Text = "Back to menu";
            this.backToMenuButton.UseVisualStyleBackColor = false;
            this.backToMenuButton.Click += new System.EventHandler(this.BackToMenuButton_Click);
            // 
            // resultLabel
            // 
            this.resultLabel.BackColor = System.Drawing.Color.IndianRed;
            this.resultLabel.Font = new System.Drawing.Font("Impact", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.resultLabel.ForeColor = System.Drawing.Color.White;
            this.resultLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.resultLabel.Location = new System.Drawing.Point(25, 611);
            this.resultLabel.Name = "resultLabel";
            this.resultLabel.Size = new System.Drawing.Size(992, 108);
            this.resultLabel.TabIndex = 6;
            this.resultLabel.Text = "PLACEHOLDER";
            this.resultLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Endgame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Firebrick;
            this.ClientSize = new System.Drawing.Size(1301, 743);
            this.Controls.Add(this.resultLabel);
            this.Controls.Add(this.backToMenuButton);
            this.Controls.Add(this.sortofLabel);
            this.Controls.Add(this.headerPicture);
            this.Name = "Endgame";
            this.Text = "Endgame";
            ((System.ComponentModel.ISupportInitialize)(this.headerPicture)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label sortofLabel;
        private System.Windows.Forms.PictureBox headerPicture;
        private System.Windows.Forms.Button backToMenuButton;
        private System.Windows.Forms.Label resultLabel;
    }
}