namespace DockingWindowSample
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナ変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナで生成されたコード

        /// <summary>
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.dockBay = new SunokoLibrary.Controls.DockBay();
            this.SuspendLayout();
            // 
            // dockBay
            // 
            this.dockBay.BackColor = System.Drawing.Color.Firebrick;
            this.dockBay.BorderColor = System.Drawing.Color.Empty;
            this.dockBay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dockBay.Location = new System.Drawing.Point(0, 0);
            this.dockBay.Name = "dockBay";
            this.dockBay.Size = new System.Drawing.Size(284, 263);
            this.dockBay.SplitterWidth = 6;
            this.dockBay.TabIndex = 0;
            this.dockBay.Text = "dockBay1";
            this.dockBay.TitleBarHeight = 16;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 263);
            this.Controls.Add(this.dockBay);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private SunokoLibrary.Controls.DockBay dockBay;

    }
}

