namespace SunokoLibrary.Controls
{
    partial class DockingHelper
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
            this.PaneHelper = new System.Windows.Forms.Panel();
            this.InBottomIndicator = new SunokoLibrary.Controls.Indicator();
            this.InTopIndicator = new SunokoLibrary.Controls.Indicator();
            this.InRightIndicator = new SunokoLibrary.Controls.Indicator();
            this.InLeftIndicator = new SunokoLibrary.Controls.Indicator();
            this.InCenterIndicator = new SunokoLibrary.Controls.Indicator();
            this.OutRightIndicator = new SunokoLibrary.Controls.Indicator();
            this.OutLeftIndicator = new SunokoLibrary.Controls.Indicator();
            this.OutBottomIndicator = new SunokoLibrary.Controls.Indicator();
            this.OutTopIndicator = new SunokoLibrary.Controls.Indicator();
            this.PaneHelper.SuspendLayout();
            this.SuspendLayout();
            // 
            // PaneHelper
            // 
            this.PaneHelper.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.PaneHelper.Controls.Add(this.InBottomIndicator);
            this.PaneHelper.Controls.Add(this.InTopIndicator);
            this.PaneHelper.Controls.Add(this.InRightIndicator);
            this.PaneHelper.Controls.Add(this.InLeftIndicator);
            this.PaneHelper.Controls.Add(this.InCenterIndicator);
            this.PaneHelper.Location = new System.Drawing.Point(106, 106);
            this.PaneHelper.Name = "PaneHelper";
            this.PaneHelper.Size = new System.Drawing.Size(88, 88);
            this.PaneHelper.TabIndex = 4;
            // 
            // InBottomIndicator
            // 
            this.InBottomIndicator.ActiveImage = global::SunokoLibrary.Properties.Resources.InnerBottomtIndicator_Active;
            this.InBottomIndicator.IsActive = false;
            this.InBottomIndicator.Location = new System.Drawing.Point(29, 58);
            this.InBottomIndicator.Name = "InBottomIndicator";
            this.InBottomIndicator.NormalImage = global::SunokoLibrary.Properties.Resources.InnerBottomtIndicator;
            this.InBottomIndicator.Size = new System.Drawing.Size(29, 30);
            this.InBottomIndicator.TabIndex = 5;
            // 
            // InTopIndicator
            // 
            this.InTopIndicator.ActiveImage = global::SunokoLibrary.Properties.Resources.InnerTopIndicator_Active;
            this.InTopIndicator.IsActive = false;
            this.InTopIndicator.Location = new System.Drawing.Point(29, 0);
            this.InTopIndicator.Name = "InTopIndicator";
            this.InTopIndicator.NormalImage = global::SunokoLibrary.Properties.Resources.InnerTopIndicator;
            this.InTopIndicator.Size = new System.Drawing.Size(29, 29);
            this.InTopIndicator.TabIndex = 5;
            // 
            // InRightIndicator
            // 
            this.InRightIndicator.ActiveImage = global::SunokoLibrary.Properties.Resources.InnerRightIndicator_Active;
            this.InRightIndicator.IsActive = false;
            this.InRightIndicator.Location = new System.Drawing.Point(58, 29);
            this.InRightIndicator.Name = "InRightIndicator";
            this.InRightIndicator.NormalImage = global::SunokoLibrary.Properties.Resources.InnerRightIndicator;
            this.InRightIndicator.Size = new System.Drawing.Size(30, 29);
            this.InRightIndicator.TabIndex = 5;
            // 
            // InLeftIndicator
            // 
            this.InLeftIndicator.ActiveImage = global::SunokoLibrary.Properties.Resources.InnerLeftIndicator_Active;
            this.InLeftIndicator.IsActive = false;
            this.InLeftIndicator.Location = new System.Drawing.Point(0, 29);
            this.InLeftIndicator.Name = "InLeftIndicator";
            this.InLeftIndicator.NormalImage = global::SunokoLibrary.Properties.Resources.InnerLeftIndicator;
            this.InLeftIndicator.Size = new System.Drawing.Size(29, 29);
            this.InLeftIndicator.TabIndex = 5;
            // 
            // InCenterIndicator
            // 
            this.InCenterIndicator.ActiveImage = global::SunokoLibrary.Properties.Resources.InnerCenterIndicator_Active;
            this.InCenterIndicator.IsActive = false;
            this.InCenterIndicator.Location = new System.Drawing.Point(23, 23);
            this.InCenterIndicator.Name = "InCenterIndicator";
            this.InCenterIndicator.NormalImage = global::SunokoLibrary.Properties.Resources.InnerCenterIndicator;
            this.InCenterIndicator.Size = new System.Drawing.Size(41, 41);
            this.InCenterIndicator.TabIndex = 3;
            // 
            // OutRightIndicator
            // 
            this.OutRightIndicator.ActiveImage = global::SunokoLibrary.Properties.Resources.DockIndicator_PanelRight_Active;
            this.OutRightIndicator.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.OutRightIndicator.IsActive = false;
            this.OutRightIndicator.Location = new System.Drawing.Point(257, 135);
            this.OutRightIndicator.Name = "OutRightIndicator";
            this.OutRightIndicator.NormalImage = global::SunokoLibrary.Properties.Resources.DockIndicator_PanelRight;
            this.OutRightIndicator.Size = new System.Drawing.Size(31, 29);
            this.OutRightIndicator.TabIndex = 2;
            // 
            // OutLeftIndicator
            // 
            this.OutLeftIndicator.ActiveImage = global::SunokoLibrary.Properties.Resources.DockIndicator_PanelLeft_Active;
            this.OutLeftIndicator.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.OutLeftIndicator.IsActive = false;
            this.OutLeftIndicator.Location = new System.Drawing.Point(12, 135);
            this.OutLeftIndicator.Name = "OutLeftIndicator";
            this.OutLeftIndicator.NormalImage = global::SunokoLibrary.Properties.Resources.DockIndicator_PanelLeft;
            this.OutLeftIndicator.Size = new System.Drawing.Size(31, 29);
            this.OutLeftIndicator.TabIndex = 2;
            // 
            // OutBottomIndicator
            // 
            this.OutBottomIndicator.ActiveImage = global::SunokoLibrary.Properties.Resources.DockIndicator_PanelBottom_Active;
            this.OutBottomIndicator.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.OutBottomIndicator.IsActive = false;
            this.OutBottomIndicator.Location = new System.Drawing.Point(135, 257);
            this.OutBottomIndicator.Name = "OutBottomIndicator";
            this.OutBottomIndicator.NormalImage = global::SunokoLibrary.Properties.Resources.DockIndicator_PanelBottom;
            this.OutBottomIndicator.Size = new System.Drawing.Size(29, 31);
            this.OutBottomIndicator.TabIndex = 2;
            // 
            // OutTopIndicator
            // 
            this.OutTopIndicator.ActiveImage = global::SunokoLibrary.Properties.Resources.DockIndicator_PanelTop_Active;
            this.OutTopIndicator.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.OutTopIndicator.IsActive = false;
            this.OutTopIndicator.Location = new System.Drawing.Point(135, 12);
            this.OutTopIndicator.Name = "OutTopIndicator";
            this.OutTopIndicator.NormalImage = global::SunokoLibrary.Properties.Resources.DockIndicator_PanelTop;
            this.OutTopIndicator.Size = new System.Drawing.Size(29, 31);
            this.OutTopIndicator.TabIndex = 2;
            // 
            // DockingHelper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Fuchsia;
            this.ClientSize = new System.Drawing.Size(300, 300);
            this.Controls.Add(this.PaneHelper);
            this.Controls.Add(this.OutRightIndicator);
            this.Controls.Add(this.OutLeftIndicator);
            this.Controls.Add(this.OutBottomIndicator);
            this.Controls.Add(this.OutTopIndicator);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "DockingHelper";
            this.Opacity = 0.8;
            this.ShowInTaskbar = false;
            this.Text = "DockHelper";
            this.PaneHelper.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Indicator InBottomIndicator;
        private Indicator InCenterIndicator;
        private Indicator InLeftIndicator;
        private Indicator InRightIndicator;
        private Indicator InTopIndicator;
        private Indicator OutBottomIndicator;
        private Indicator OutLeftIndicator;
        private Indicator OutRightIndicator;
        private Indicator OutTopIndicator;
        private System.Windows.Forms.Panel PaneHelper;

    }
}