using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SunokoLibrary.Controls
{
    public class DockTitleBar : ToolStrip
    {
        public DockTitleBar()
        {
            _closeButton = new ToolStripButton("\x00d7");
            _closeButton.Alignment = ToolStripItemAlignment.Right;
            _closeButton.Click += _closeButton_Click;
            Items.Add(_closeButton);
            GripStyle = ToolStripGripStyle.Hidden;
        }
        private ToolStripButton _closeButton;

        void _closeButton_Click(object sender, EventArgs e)
        {
            ((DockPaneBase)Parent).Remove();
        }
        void _debugButton_Click(object sender, EventArgs e)
        {
            //new DebugForm((DockBase)Parent).Show();
        }
        void _floatButton_Click(object sender, EventArgs e)
        {
            ((DockPane)Parent).Floating();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            var graphics = e.Graphics;
            var brush = new LinearGradientBrush(new Point(0, 0), new Point(0, Height), SystemColors.ActiveCaption, SystemColors.GradientActiveCaption);
            graphics.FillRectangle(brush, new Rectangle(new Point(0, 0), Size));
            base.OnPaint(e);
        }
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x21)
            {
                Focus();
            }
            base.WndProc(ref m);
        }

    }


}
