using System.Windows.Forms;
using System.Drawing;

namespace Crimson
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
            this.components = new System.ComponentModel.Container();
            this.gameTimer = new System.Windows.Forms.Timer(this.components);
            this.fpsTimer = new System.Windows.Forms.Timer(this.components);
            this.healthLabel = new System.Windows.Forms.Label();
            this.healthBar = new PercentageBar(Color.DarkRed);
            this.ammoBar = new PercentageBar(Color.DarkGray);
            this.label1 = new System.Windows.Forms.Label();
            this.mapPanel = new Crimson.DoubleBufferedPanel();
            this.reloadingLabel = new System.Windows.Forms.Label();
            this.pauseLabel = new System.Windows.Forms.Label();
            this.mapPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // gameTimer
            // 
            this.gameTimer.Interval = 20;
            this.gameTimer.Tick += new System.EventHandler(this.GameTimer_Tick);
            // 
            // fpsTimer
            // 
            this.fpsTimer.Enabled = true;
            this.fpsTimer.Interval = 1000;
            this.fpsTimer.Tick += new System.EventHandler(this.FpsTimer_Tick);
            // 
            // healthLabel
            // 
            this.healthLabel.AutoSize = true;
            this.healthLabel.Location = new System.Drawing.Point(327, 861);
            this.healthLabel.Name = "healthLabel";
            this.healthLabel.Size = new System.Drawing.Size(56, 20);
            this.healthLabel.TabIndex = 4;
            this.healthLabel.Text = "Health";
            // 
            // healthBar
            // 
            this.healthBar.Color = System.Drawing.Color.DarkRed;
            this.healthBar.Location = new System.Drawing.Point(22, 832);
            this.healthBar.MarqueeAnimationSpeed = 0;
            this.healthBar.Name = "healthBar";
            this.healthBar.Size = new System.Drawing.Size(290, 73);
            this.healthBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.healthBar.TabIndex = 2;
            this.healthBar.Val = 0F;
            // 
            // ammoBar
            // 
            this.ammoBar.Color = System.Drawing.Color.DarkGreen;
            this.ammoBar.Location = new System.Drawing.Point(1041, 832);
            this.ammoBar.MarqueeAnimationSpeed = 0;
            this.ammoBar.Name = "ammoBar";
            this.ammoBar.RightToLeftLayout = true;
            this.ammoBar.Size = new System.Drawing.Size(290, 73);
            this.ammoBar.Step = 1;
            this.ammoBar.TabIndex = 3;
            this.ammoBar.Val = 0F;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(960, 861);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 20);
            this.label1.TabIndex = 5;
            this.label1.Text = "Ammo";
            // 
            // mapPanel
            // 
            this.mapPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mapPanel.Controls.Add(this.pauseLabel);
            this.mapPanel.Location = new System.Drawing.Point(12, 12);
            this.mapPanel.Name = "mapPanel";
            this.mapPanel.Size = new System.Drawing.Size(1319, 806);
            this.mapPanel.TabIndex = 1;
            this.mapPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MapPanel_MouseDown);
            this.mapPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MapPanel_MouseMove);
            this.mapPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MapPanel_MouseUp);
            // 
            // reloadingLabel
            // 
            this.reloadingLabel.AutoSize = true;
            this.reloadingLabel.Font = new System.Drawing.Font("Impact", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.reloadingLabel.Location = new System.Drawing.Point(1119, 852);
            this.reloadingLabel.Name = "reloadingLabel";
            this.reloadingLabel.Size = new System.Drawing.Size(135, 35);
            this.reloadingLabel.TabIndex = 0;
            this.reloadingLabel.Text = "RELOADING";
            // 
            // pauseLabel
            // 
            this.pauseLabel.AutoSize = true;
            this.pauseLabel.Font = new System.Drawing.Font("Impact", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.pauseLabel.Location = new System.Drawing.Point(19, 20);
            this.pauseLabel.Name = "pauseLabel";
            this.pauseLabel.Size = new System.Drawing.Size(98, 35);
            this.pauseLabel.TabIndex = 6;
            this.pauseLabel.Text = "PAUSED";
            this.pauseLabel.Visible = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1343, 917);
            this.Controls.Add(this.reloadingLabel);
            this.Controls.Add(this.healthBar);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ammoBar);
            this.Controls.Add(this.healthLabel);
            this.Controls.Add(this.mapPanel);
            this.DoubleBuffered = true;
            this.Name = "MainForm";
            this.Text = "Crimson";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyUp);
            this.mapPanel.ResumeLayout(false);
            this.mapPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Timer gameTimer;
        private DoubleBufferedPanel mapPanel;
        private Timer fpsTimer;
        private PercentageBar healthBar;
        private PercentageBar ammoBar;
        private Label healthLabel;
        private Label label1;
        private Label reloadingLabel;
        private Label pauseLabel;
    }

    class DoubleBufferedPanel : Panel
    {
        public DoubleBufferedPanel() : base()
        {
            DoubleBuffered = true;
        }
    }
}

