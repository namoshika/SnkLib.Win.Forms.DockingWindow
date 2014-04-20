using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DockingWindowSample
{
    public partial class SampleControler : UserControl
    {
        public SampleControler(SunokoLibrary.Controls.DockBay bay)
        {
            InitializeComponent();
            _bay = bay;
        }
        SunokoLibrary.Controls.DockBay _bay;

        void innerTop_Click(object sender, EventArgs e)
        {
            _bay.MainPane.AddPane(new SunokoLibrary.Controls.DockPane(),
                SunokoLibrary.Controls.DockDirection.Top);
        }
        void innerLeft_Click(object sender, EventArgs e)
        {
            _bay.MainPane.AddPane(new SunokoLibrary.Controls.DockPane(),
                SunokoLibrary.Controls.DockDirection.Left);
        }
        void innerRight_Click(object sender, EventArgs e)
        {
            _bay.MainPane.AddPane(new SunokoLibrary.Controls.DockPane(),
                SunokoLibrary.Controls.DockDirection.Right);
        }
        void innerBottom_Click(object sender, EventArgs e)
        {
            _bay.MainPane.AddPane(new SunokoLibrary.Controls.DockPane(),
                SunokoLibrary.Controls.DockDirection.Bottom);
        }

        void outterLeft_Click(object sender, EventArgs e)
        {
            _bay.AddPane(new SunokoLibrary.Controls.DockPane(),
                SunokoLibrary.Controls.DockDirection.Left);
        }
        void outterTop_Click(object sender, EventArgs e)
        {
            _bay.AddPane(new SunokoLibrary.Controls.DockPane(),
                SunokoLibrary.Controls.DockDirection.Top);
        }
        void outterRight_Click(object sender, EventArgs e)
        {
            _bay.AddPane(new SunokoLibrary.Controls.DockPane(),
                SunokoLibrary.Controls.DockDirection.Right);
        }
        void outterBottom_Click(object sender, EventArgs e)
        {
            _bay.AddPane(new SunokoLibrary.Controls.DockPane(),
                SunokoLibrary.Controls.DockDirection.Bottom);
        }
    }
}
