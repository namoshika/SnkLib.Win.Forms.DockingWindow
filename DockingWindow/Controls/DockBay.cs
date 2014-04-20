using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace SunokoLibrary.Controls
{
    public class DockBay : DockBayBase
    {
        public DockBay()
        {
            _paneTitleBarHeight = SystemInformation.ToolWindowCaptionHeight;
            _helper = new DockingHelper(this, TopLevelControl as Form);
            _helper.FormDragDrop += _helper_FormDragDrop;
            _helper.TopMost = true;
            TitleBarHeight = 16;

            FloatBays = new List<DockBayFloating>();
            FloatFormOrder = new LinkedList<DockFormFloating>();

            MainPane = new RootDockPane(this);
            MainPane.Name = "mainPane";
            Neigh.Add(MainPane, DockDirection.Top);
        }

        int _paneTitleBarHeight;
        bool _isFloatFormOver;
        DockingHelper _helper;
        DockBayFloating _dragBay;
        DockPaneBase _pane;

        public List<DockBayFloating> FloatBays { get; protected set; }
        public LinkedList<DockFormFloating> FloatFormOrder { get; protected set; }
        public virtual DockPaneBase MainPane { get; protected set; }
        public int TitleBarHeight { get; set; }

        public virtual void AddPane(DockPane pane, Point location)
        {
            var floating = new DockFormFloating(this);
            floating.DockBay.PaneAdded += DockBay_PaneAdded;
            floating.DockBay.AddPane(pane, DockDirection.Top);
            floating.Location = location;
            floating.Show();
            DockPane_Floated(pane, new EventArgs());
        }
        public virtual void ShowDockHelper(DockBase node)
        {
            _helper.Bounds = Parent.RectangleToScreen(Bounds);
            if (node != null)
            {
                _helper.Owner = TopLevelControl as Form;
                _helper.TopMost = true;
                _helper.Show();
                _helper.SetPaneHelper(node.Bounds);
            }
        }
        public virtual void HideDockHelper()
        {
            _helper.Size = new Size(0, 0);
        }

        void _helper_FormDragDrop(object sender, ChangeStateEventArgs e)
        {
            switch (e.Effect)
            {
                case DragDropEffects.Top:
                    _pane.AddPane(_dragBay, DockDirection.Top);
                    return;
                case DragDropEffects.Bottom:
                    _pane.AddPane(_dragBay, DockDirection.Bottom);
                    return;
                case DragDropEffects.Left:
                    _pane.AddPane(_dragBay, DockDirection.Left);
                    return;
                case DragDropEffects.Right:
                    _pane.AddPane(_dragBay, DockDirection.Right);
                    return;
                case DragDropEffects.Fill:
                    break;
                case DragDropEffects.OuterTop:
                    AddPane(_dragBay, DockDirection.Top);
                    return;
                case DragDropEffects.OuterBottom:
                    AddPane(_dragBay, DockDirection.Bottom);
                    return;
                case DragDropEffects.OuterLeft:
                    AddPane(_dragBay, DockDirection.Left);
                    return;
                case DragDropEffects.OuterRight:
                    AddPane(_dragBay, DockDirection.Right);
                    break;
                default:
                    return;
            }
        }
        void DockPane_Disposed(object sender, EventArgs e)
        {
            DockPane pane = (DockPane)sender;
            pane.Floated -= new EventHandler(DockPane_Floated);
        }
        void DockPane_Floated(object sender, EventArgs e)
        {
            var pane = (DockPaneBase)sender;
            if (!FloatBays.Contains((DockBayFloating)pane.Neigh.Owner))
            {
                var topLevelControl = (DockFormFloating)pane.TopLevelControl;
                topLevelControl.Owner = TopLevelControl as Form;
                topLevelControl.TopMost = (TopLevelControl as Form).TopMost;
                topLevelControl.FormMoving += new EventHandler(floatForm_FormMoving);
                topLevelControl.FormEndMoving += new EventHandler(floatForm_FormEndMoving);
                topLevelControl.Activated += new EventHandler(floatForm_Activated);
                topLevelControl.Disposed += new EventHandler(floatForm_Disposed);
                FloatBays.Add((DockBayFloating)pane.Neigh.Owner);
                FloatFormOrder.AddFirst(topLevelControl);
                OnFloatFormCreated(new FloatFormEventArgs(topLevelControl, pane));
            }
        }
        void DockBay_PaneAdded(object sender, PaneAddedEventArgs e)
        {
            //floatBayに直接AddPaneされた場合はPaneAddedによる
            //Floatedイベント登録されないため強制的に発動させる。
            OnPaneAdded(e);
        }
        void floatForm_Activated(object sender, EventArgs e)
        {
            DockFormFloating floating = (DockFormFloating)sender;
            FloatFormOrder.Remove(floating);
            FloatFormOrder.AddFirst(floating);
        }
        void floatForm_Disposed(object sender, EventArgs e)
        {
            DockFormFloating floating = (DockFormFloating)sender;
            floating.FormMoving -= new EventHandler(floatForm_FormMoving);
            floating.FormEndMoving -= new EventHandler(floatForm_FormEndMoving);
            floating.Activated -= new EventHandler(floatForm_Activated);
            floating.Disposed -= new EventHandler(floatForm_Disposed);
            FloatBays.Remove(floating.DockBay);
            FloatFormOrder.Remove(floating);
        }
        void floatForm_FormEndMoving(object sender, EventArgs e)
        {
            if (_isFloatFormOver)
            {
                OnFloatFormEndMove(new FloatFormEventArgs(sender as DockFormFloating, _pane));
            }
            HideDockHelper();
            _isFloatFormOver = false;
            _pane = null;
        }
        void floatForm_FormMoving(object sender, EventArgs e)
        {
            var form = (DockFormFloating)sender;
            var mousePosition = Control.MousePosition;
            var childAtPoint = GetChildAtPoint(PointToClient(mousePosition)) as DockPaneBase;
            var flag = Parent.RectangleToScreen(Bounds).Contains(mousePosition);
            if (flag)
            {
                foreach (var floatBay in FloatFormOrder)
                    if ((floatBay != form) && floatBay.Bounds.Contains(mousePosition))
                    {
                        flag = false;
                        childAtPoint = null;
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
            }
            _dragBay = form.DockBay;
            _isFloatFormOver = flag;
        }

        public event FloatFormEventHandler FloatFormCreated;
        protected virtual void OnFloatFormCreated(FloatFormEventArgs e)
        {
            if (FloatFormCreated != null)
                FloatFormCreated(this, e);
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
            ShowDockHelper(e.DockPane);
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
        protected override void OnPaneAdded(PaneAddedEventArgs e)
        {
            if (e.DockPane is DockPane)
            {
                var dockPane = (DockPane)e.DockPane;
                dockPane.Floated += new EventHandler(DockPane_Floated);
                dockPane.Disposed += new EventHandler(DockPane_Disposed);
            }
            base.OnPaneAdded(e);
        }

        private class RootDockPane : DockPaneBase
        {
            public RootDockPane(DockBayBase bay)
            {
                _neigh = DockPaneBase.DockNeigh.InitializeNeigh(this, bay);
                ContentPanel.BackColor = SystemColors.ControlDark;
            }
        }
    }
    public enum HelperModes
    {
        Hide,
        InnerHelperOnly,
        OuterHelperOnly,
        Both
    }
    public enum DragDropEffects
    {
        None,
        Enter,
        Top,
        Bottom,
        Left,
        Right,
        Fill,
        OuterTop,
        OuterBottom,
        OuterLeft,
        OuterRight
    }
    public class ChangeStateEventArgs
    {
        public ChangeStateEventArgs(DragDropEffects effect)
        {
            Effect = effect;
        }

        public DragDropEffects Effect { get; set; }
    }
}
