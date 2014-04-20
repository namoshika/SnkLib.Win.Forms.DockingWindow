namespace DockingWindowSample
{
    partial class SampleControler
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

        #region コンポーネント デザイナで生成されたコード

        /// <summary> 
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.innerRight = new System.Windows.Forms.Button();
            this.innerLeft = new System.Windows.Forms.Button();
            this.innerTop = new System.Windows.Forms.Button();
            this.innerBottom = new System.Windows.Forms.Button();
            this.outterLeft = new System.Windows.Forms.Button();
            this.outterTop = new System.Windows.Forms.Button();
            this.outterRight = new System.Windows.Forms.Button();
            this.outterBottom = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // innerRight
            // 
            this.innerRight.Location = new System.Drawing.Point(115, 97);
            this.innerRight.Name = "innerRight";
            this.innerRight.Size = new System.Drawing.Size(80, 32);
            this.innerRight.TabIndex = 0;
            this.innerRight.Text = "右辺追加";
            this.innerRight.UseVisualStyleBackColor = true;
            this.innerRight.Click += new System.EventHandler(this.innerRight_Click);
            // 
            // innerLeft
            // 
            this.innerLeft.Location = new System.Drawing.Point(29, 97);
            this.innerLeft.Name = "innerLeft";
            this.innerLeft.Size = new System.Drawing.Size(80, 32);
            this.innerLeft.TabIndex = 0;
            this.innerLeft.Text = "左辺追加";
            this.innerLeft.UseVisualStyleBackColor = true;
            this.innerLeft.Click += new System.EventHandler(this.innerLeft_Click);
            // 
            // innerTop
            // 
            this.innerTop.Location = new System.Drawing.Point(71, 59);
            this.innerTop.Name = "innerTop";
            this.innerTop.Size = new System.Drawing.Size(80, 32);
            this.innerTop.TabIndex = 0;
            this.innerTop.Text = "上辺追加";
            this.innerTop.UseVisualStyleBackColor = true;
            this.innerTop.Click += new System.EventHandler(this.innerTop_Click);
            // 
            // innerBottom
            // 
            this.innerBottom.Location = new System.Drawing.Point(71, 135);
            this.innerBottom.Name = "innerBottom";
            this.innerBottom.Size = new System.Drawing.Size(80, 32);
            this.innerBottom.TabIndex = 0;
            this.innerBottom.Text = "下辺追加";
            this.innerBottom.UseVisualStyleBackColor = true;
            this.innerBottom.Click += new System.EventHandler(this.innerBottom_Click);
            // 
            // outterLeft
            // 
            this.outterLeft.Location = new System.Drawing.Point(3, 3);
            this.outterLeft.Name = "outterLeft";
            this.outterLeft.Size = new System.Drawing.Size(50, 50);
            this.outterLeft.TabIndex = 0;
            this.outterLeft.Text = "外周左辺追加";
            this.outterLeft.UseVisualStyleBackColor = true;
            this.outterLeft.Click += new System.EventHandler(this.outterLeft_Click);
            // 
            // outterTop
            // 
            this.outterTop.Location = new System.Drawing.Point(59, 3);
            this.outterTop.Name = "outterTop";
            this.outterTop.Size = new System.Drawing.Size(50, 50);
            this.outterTop.TabIndex = 0;
            this.outterTop.Text = "外周上辺追加";
            this.outterTop.UseVisualStyleBackColor = true;
            this.outterTop.Click += new System.EventHandler(this.outterTop_Click);
            // 
            // outterRight
            // 
            this.outterRight.Location = new System.Drawing.Point(115, 3);
            this.outterRight.Name = "outterRight";
            this.outterRight.Size = new System.Drawing.Size(50, 50);
            this.outterRight.TabIndex = 0;
            this.outterRight.Text = "外周左辺追加";
            this.outterRight.UseVisualStyleBackColor = true;
            this.outterRight.Click += new System.EventHandler(this.outterRight_Click);
            // 
            // outterBottom
            // 
            this.outterBottom.Location = new System.Drawing.Point(171, 3);
            this.outterBottom.Name = "outterBottom";
            this.outterBottom.Size = new System.Drawing.Size(50, 50);
            this.outterBottom.TabIndex = 0;
            this.outterBottom.Text = "外周右辺追加";
            this.outterBottom.UseVisualStyleBackColor = true;
            this.outterBottom.Click += new System.EventHandler(this.outterBottom_Click);
            // 
            // SampleControler
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.innerBottom);
            this.Controls.Add(this.innerRight);
            this.Controls.Add(this.outterBottom);
            this.Controls.Add(this.innerLeft);
            this.Controls.Add(this.outterRight);
            this.Controls.Add(this.innerTop);
            this.Controls.Add(this.outterTop);
            this.Controls.Add(this.outterLeft);
            this.Name = "SampleControler";
            this.Size = new System.Drawing.Size(231, 179);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button innerRight;
        private System.Windows.Forms.Button innerLeft;
        private System.Windows.Forms.Button innerTop;
        private System.Windows.Forms.Button innerBottom;
        private System.Windows.Forms.Button outterLeft;
        private System.Windows.Forms.Button outterTop;
        private System.Windows.Forms.Button outterRight;
        private System.Windows.Forms.Button outterBottom;
    }
}
