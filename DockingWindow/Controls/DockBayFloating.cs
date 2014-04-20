using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SunokoLibrary.Controls
{
    public partial class DockFormFloating : Form
    {
        public DockFormFloating(DockBay root)
        {
            InitializeComponent();

            DockBay = new DockBayFloating(this, root);
            DockBay.PaneRemoved += _bay_PaneRemoved;
            DockBay.Disposed += DockBay_Disposed;
            DockBay.Dock = DockStyle.Fill;
            Controls.Add(DockBay);
        }

        int _lastNCHittestRes;
        public DockBayFloating DockBay { get; set; }

        void _bay_PaneRemoved(object sender, PaneRemovedEventArgs e)
        {
            var bay = (DockBayBase)sender;
            if (bay.Panes.Count < 1)
            {
                DockBay.Dispose();
                Dispose();
            }
        }
        void DockBay_Disposed(object sender, EventArgs e)
        {
            DockBay.Dispose();
        }

        public event EventHandler FormEndMoving;
        protected void OnFormEndMoving(EventArgs e)
        {
            if (FormEndMoving != null)
                FormEndMoving(this, e);
        }
        public event EventHandler FormMoving;
        protected void OnFormMoving(EventArgs e)
        {
            if (FormMoving != null)
                FormMoving(this, e);
        }
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 0x84:
                    _lastNCHittestRes = m.Result.ToInt32();
                    break;
                case 0x216:
                    OnFormMoving(new EventArgs());
                    break;
                case 0x232:
                    OnFormEndMoving(new EventArgs());
                    break;
            }
            base.WndProc(ref m);
        }
    }
    public class DockBayFloating : DockBayBase
    {
        public DockBayFloating(DockFormFloating form, DockBay root)
        {
            _neigh = DockNeigh.InitializeNeigh(this, root);
            _helper = new DockingHelper(this, root.TopLevelControl as Form);
            _helper.IndicatorStyle = HelperModes.InnerHelperOnly;
            _helper.FormDragDrop += _helper_FormDragDrop;
            root.FloatFormCreated += root_FloatFormCreated;
            foreach (var bay in root.FloatBays)
            {
                var topLevelControl = bay.TopLevelControl as DockFormFloating;
                topLevelControl.FormMoving += otherForm_FormMoving;
                topLevelControl.FormEndMoving += otherForm_FormEndMoving;
                topLevelControl.Disposed += otherForm_Disposed;
            }
        }

        DockBayFloating _dragBay;
        DockingHelper _helper;
        bool _isFloatFormOver;
        DockNeigh _neigh;
        DockPaneBase _pane;
        internal override DockBaseNeigh Neigh { get { return _neigh; } }

        public void ShowDockHelper(DockBase node, double splitRate)
        {
            _helper.TopMost = true;
            if (node != null)
            {
                var neigh = ((DockNeigh)Neigh);
                var rootBay = neigh.TopLevelRootBay;
                _helper.Owner = neigh.TopLevelRootBay.TopLevelControl as Form;
                _helper.Show();
                _helper.SetPaneHelper(node.Bounds);
            }
        }
        public void HideDockHelper()
        {
            _helper.Size = new Size(0, 0);
        }
        protected override void Dispose(bool disposing)
        {
            var neigh = (DockNeigh)Neigh;
            var topLevelRootBay = neigh.TopLevelRootBay;
            topLevelRootBay.FloatFormCreated -= root_FloatFormCreated;
            foreach (var floating in topLevelRootBay.FloatBays)
                if (floating != this)
                {
                    var topLevelControl = (DockFormFloating)floating.TopLevelControl;
                    topLevelControl.FormMoving -= otherForm_FormMoving;
                    topLevelControl.FormEndMoving -= otherForm_FormEndMoving;
                    topLevelControl.Disposed -= otherForm_Disposed;
                }

            base.Dispose(disposing);
        }

        protected override void OnPaneAdded(PaneAddedEventArgs e)
        {
            DockPane dockPane = (DockPane)e.DockPane;
            if (Panes.Count > 1)
            {
                dockPane.CanFloating = true;
                foreach (DockPane pane2 in Panes)
                {
                    pane2.CanFloating = true;
                }
            }
            else
            {
                dockPane.CanFloating = false;
            }
            base.OnPaneAdded(e);
        }
        protected override void OnPaneRemoved(PaneRemovedEventArgs e)
        {
            var dockPane = (DockPane)e.DockPane;
            if (Panes.Count <= 1)
            {
                dockPane.CanFloating = true;
                foreach (DockPane pane in Panes)
                {
                    pane.CanFloating = false;
                }
            }
            base.OnPaneRemoved(e);
        }
        void _helper_FormDragDrop(object sender, ChangeStateEventArgs e)
        {
            switch (e.Effect)
            {
                case DragDropEffects.Top:
                    _pane.AddPane(_dragBay, DockDirection.Top);
                    break;
                case DragDropEffects.Bottom:
                    _pane.AddPane(_dragBay, DockDirection.Bottom);
                    break;
                case DragDropEffects.Left:
                    _pane.AddPane(_dragBay, DockDirection.Left);
                    break;
                case DragDropEffects.Right:
                    _pane.AddPane(_dragBay, DockDirection.Right);
                    break;
            }
        }
        void otherForm_Disposed(object sender, EventArgs e)
        {
            DockFormFloating floating = (DockFormFloating)sender;
            floating.FormMoving -= new EventHandler(otherForm_FormMoving);
            floating.FormEndMoving -= new EventHandler(otherForm_FormEndMoving);
            floating.Disposed -= new EventHandler(otherForm_Disposed);
        }
        void otherForm_FormEndMoving(object sender, EventArgs e)
        {
            if (_isFloatFormOver)
            {
                OnFloatFormEndMove(new FloatFormEventArgs(sender as DockFormFloating, _pane));
            }
            HideDockHelper();
            _isFloatFormOver = false;
            _pane = null;
        }
        void otherForm_FormMoving(object sender, EventArgs e)
        {
            var form = (DockFormFloating)sender;
            var mousePosition = Control.MousePosition;
            var childAtPoint = GetChildAtPoint(PointToClient(mousePosition)) as DockPaneBase;
            var flag = Parent.RectangleToScreen(Bounds).Contains(mousePosition);
            if (TopLevelControl != form)
            {
                if (flag)
                {
                    foreach (DockFormFloating floating2 in ((DockNeigh)Neigh).TopLevelRootBay.FloatFormOrder)
                    {
                        if (floating2.Bounds.Contains(mousePosition) && (floating2 != form))
                        {
                            if (floating2.DockBay != this)
                            {
                                flag = false;
                            }
                            break;
                        }
                    }
                }
                if (flag)
                {
                    OnFloatFormMove(new FloatFormEventArgs(form, childAtPoint));
                    if ((childAtPoint != _pane) || (flag != _isFloatFormOver))
                    {
                        OnFloatFormOver(new FloatFormEventArgs(form, childAtPoint));
                        OnFloatFormEnter(new FloatFormEventArgs(form, childAtPoint));
                    }
                    _pane = childAtPoint;
                }
                else if (!flag && (flag != _isFloatFormOver))
                {
                    OnFloatFormOver(new FloatFormEventArgs(form, childAtPoint));
                    OnFloatFormLeave(new FloatFormEventArgs(form, childAtPoint));
                    _pane = null;
                }
                _dragBay = form.DockBay;
            }
            _isFloatFormOver = flag;
        }
        void root_FloatFormCreated(object sender, FloatFormEventArgs e)
        {
            if (e.FloatForm != TopLevelControl)
            {
                var floatForm = e.FloatForm;
                floatForm.FormMoving += new EventHandler(otherForm_FormMoving);
                floatForm.FormEndMoving += new EventHandler(otherForm_FormEndMoving);
                floatForm.Disposed += new EventHandler(otherForm_Disposed);
            }
        }

        public event FloatFormEventHandler FloatFormEndMove;
        protected void OnFloatFormEndMove(FloatFormEventArgs e)
        {
            if (FloatFormEndMove != null)
                FloatFormEndMove(this, e);
        }
        public event FloatFormEventHandler FloatFormEnter;
        protected void OnFloatFormEnter(FloatFormEventArgs e)
        {
            ShowDockHelper(e.DockPane, 0.25);
            if (FloatFormEnter != null)
                FloatFormEnter(this, e);
        }
        public event FloatFormEventHandler FloatFormLeave;
        protected void OnFloatFormLeave(FloatFormEventArgs e)
        {
            HideDockHelper();
            if (FloatFormLeave != null)
                FloatFormLeave(this, e);
        }
        public event FloatFormEventHandler FloatFormMove;
        protected void OnFloatFormMove(FloatFormEventArgs e)
        {
            if (FloatFormMove != null)
                FloatFormMove(this, e);
        }
        public event FloatFormEventHandler FloatFormOver;
        protected void OnFloatFormOver(FloatFormEventArgs e)
        {
            if (FloatFormOver != null)
                FloatFormOver(this, e);
        }

        public new class DockNeigh : DockBayBase.DockNeigh
        {
            public DockNeigh(DockBay root)
            {
                TopLevelRootBay = root;
            }
            public static DockNeigh InitializeNeigh(DockBayFloating node, DockBay root)
            {
                var neigh = new DockBayFloating.DockNeigh(root);
                neigh.Owner = node;
                DockBaseNeigh.InitializeOf(neigh, node);
                DockBayBase.DockNeigh.InitEvent(neigh, node);

                return neigh;
            }

            public DockBay TopLevelRootBay { get; set; }
        }
    }

    public delegate void FloatFormEventHandler(object sender, FloatFormEventArgs e);
    public class FloatFormEventArgs
    {
        public FloatFormEventArgs(DockFormFloating form, DockPaneBase pane)
        {
            FloatForm = form;
            DockPane = pane;
        }

        public DockPaneBase DockPane { get; protected set; }
        public DockFormFloating FloatForm { get; protected set; }
        public bool IsFloatFormOver { get; protected set; }
    }
}
