using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SunokoLibrary.Controls.Debug
{
    public partial class DebugForm : Form
    {
        public DebugForm(DockBase pane)
        {
            InitializeComponent();
            property.SelectedObject = pane;
            var infoArray = new DockBaseNeigh.DireInfo[] { pane.Neigh.Top, pane.Neigh.Bottom, pane.Neigh.Left, pane.Neigh.Right };
            var strArray = new string[] { "Top", "Bottom", "Left", "Right" };
            var view = new TreeView { Dock = DockStyle.Fill };
            for (int i = 0; i < infoArray.Length; i++)
            {
                var node = new TreeNode(strArray[i]);
                var outers = new TreeNode("outers");
                var inners = new TreeNode("inner");
                foreach (var control in infoArray[i].Inners)
                    inners.Nodes.Add(control.Location + ":" + control.Size);
                foreach (var control2 in infoArray[i].Outers)
                    outers.Nodes.Add(control2.Location + ":" + control2.Size);
                node.Nodes.Add(inners);
                node.Nodes.Add(outers);

                view.Nodes.Add(node);
            }
            splitContainer1.Panel2.Controls.Add(view);
            Text = String.Format("{0}[{1}]", pane.GetType().Name, pane.Bounds.ToString());
            Show();
        }
    }
}
