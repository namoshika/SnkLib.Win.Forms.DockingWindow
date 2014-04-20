using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DockingWindowSample
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            var pane = new SunokoLibrary.Controls.DockPane();
            pane.ContentPanel.Controls.Add(new SampleControler(dockBay) { Dock = DockStyle.Fill });
            dockBay.AddPane(pane, MousePosition);
        }
    }
}
