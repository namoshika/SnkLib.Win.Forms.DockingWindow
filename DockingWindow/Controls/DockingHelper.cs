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
    public partial class DockingHelper : Form
    {
        public DockingHelper(DockBay owner, Form dockOwner)
        {
            InitializeComponent();
            TransparencyKey = BackColor;
            Disposed += new EventHandler(DockingHelper_Disposed);
            owner.FloatFormMove += new FloatFormEventHandler(owner_FloatFormMove);
            owner.FloatFormEndMove += new FloatFormEventHandler(owner_FloatFormEndMove);
            _owner = owner;
        }
        public DockingHelper(DockBayFloating owner, Form dockOwner)
        {
            InitializeComponent();
            ShowInTaskbar = false;
            TransparencyKey = BackColor;
            owner.FloatFormMove += new FloatFormEventHandler(owner_FloatFormMove);
            owner.FloatFormEndMove += new FloatFormEventHandler(owner_FloatFormEndMove);
            _owner = owner;
        }

        DragDropEffects _effect;
        bool _innerIndicatorVisible;
        bool _outerIndicatorVisible;
        DockBayBase _owner;
        HelperModes _state;

        public DragDropEffects ActiveIndicator
        {
            get { return _effect; }
            set
            {
                InCenterIndicator.IsActive = value == DragDropEffects.Fill;
                InTopIndicator.IsActive = value == DragDropEffects.Top;
                InBottomIndicator.IsActive = value == DragDropEffects.Bottom;
                InLeftIndicator.IsActive = value == DragDropEffects.Left;
                InRightIndicator.IsActive = value == DragDropEffects.Right;
                OutTopIndicator.IsActive = value == DragDropEffects.OuterTop;
                OutBottomIndicator.IsActive = value == DragDropEffects.OuterBottom;
                OutLeftIndicator.IsActive = value == DragDropEffects.OuterLeft;
                OutRightIndicator.IsActive = value == DragDropEffects.OuterRight;
                _effect = value;
            }
        }
        public bool BayIndicatorVisible
        {
            get { return _outerIndicatorVisible; }
            set
            {
                OutTopIndicator.Visible = value;
                OutBottomIndicator.Visible = value;
                OutLeftIndicator.Visible = value;
                OutRightIndicator.Visible = value;
                _outerIndicatorVisible = value;
            }
        }
        public HelperModes IndicatorStyle
        {
            get { return _state; }
            set
            {
                switch (value)
                {
                    case HelperModes.Hide:
                        PaneIndicatorVisible = false;
                        BayIndicatorVisible = false;
                        break;

                    case HelperModes.InnerHelperOnly:
                        PaneIndicatorVisible = true;
                        BayIndicatorVisible = false;
                        break;

                    case HelperModes.OuterHelperOnly:
                        PaneIndicatorVisible = false;
                        BayIndicatorVisible = true;
                        break;

                    case HelperModes.Both:
                        PaneIndicatorVisible = true;
                        BayIndicatorVisible = true;
                        break;

                    default:
                        throw new ArgumentException("引数が異常です。");
                }
                _state = value;
            }
        }
        public bool PaneIndicatorVisible
        {
            get { return _innerIndicatorVisible; }
            set
            {
                PaneHelper.Visible = value;
                _innerIndicatorVisible = value;
            }
        }

        public DragDropEffects GetDragDropEffect(Point mousePosition)
        {
            FitOwnerSize();
            Point pt = PointToClient(mousePosition);
            Point point2 = PaneHelper.PointToClient(mousePosition);

            DragDropEffects res;
            if (InTopIndicator.Visible && InTopIndicator.Bounds.Contains(point2))
                res = DragDropEffects.Top;
            else if (InBottomIndicator.Visible && InBottomIndicator.Bounds.Contains(point2))
                res = DragDropEffects.Bottom;
            else if (InLeftIndicator.Visible && InLeftIndicator.Bounds.Contains(point2))
                res = DragDropEffects.Left;
            else if (InRightIndicator.Visible && InRightIndicator.Bounds.Contains(point2))
                res = DragDropEffects.Right;
            else if (OutTopIndicator.Visible && OutTopIndicator.Bounds.Contains(pt))
                res = DragDropEffects.OuterTop;
            else if (OutBottomIndicator.Visible && OutBottomIndicator.Bounds.Contains(pt))
                res = DragDropEffects.OuterBottom;
            else if (OutLeftIndicator.Visible && OutLeftIndicator.Bounds.Contains(pt))
                res = DragDropEffects.OuterLeft;
            else if (OutRightIndicator.Visible && OutRightIndicator.Bounds.Contains(pt))
                res = DragDropEffects.OuterRight;
            else if (InCenterIndicator.Visible && InCenterIndicator.Bounds.Contains(point2))
                res = DragDropEffects.Fill;
            else if (Bounds.Contains(mousePosition))
                res = DragDropEffects.Enter;
            else
                res = DragDropEffects.None;

            return res;
        }
        public void SetPaneHelper(Rectangle bounds)
        {
            FitOwnerSize();
            PaneHelper.Location = new Point(
                ((bounds.Width - PaneHelper.Width) / 2) + bounds.Left,
                ((bounds.Height - PaneHelper.Height) / 2) + bounds.Top);
        }
        protected void FitOwnerSize()
        {
            if (_owner.Parent == null)
                throw new Exception("_ownerのスクリーン座標を取得できません。");

            Rectangle rectangle = _owner.Parent.RectangleToScreen(_owner.Bounds);
            Bounds = rectangle;
        }

        void DockingHelper_Disposed(object sender, EventArgs e)
        {
            Owner = null;
        }
        void owner_FloatFormEndMove(object sender, FloatFormEventArgs e)
        {
            var dragDropEffect = GetDragDropEffect(Control.MousePosition);
            OnFormDragDrop(new ChangeStateEventArgs(dragDropEffect));
        }
        void owner_FloatFormMove(object sender, FloatFormEventArgs e)
        {
            DragDropEffects dragDropEffect = GetDragDropEffect(Control.MousePosition);
            ActiveIndicator = dragDropEffect;
            if (dragDropEffect != _effect)
            {
                OnChangeStateEventHandler(new ChangeStateEventArgs(dragDropEffect));
            }
            _effect = dragDropEffect;
        }
        public event ChangeStateEventHandler ChangeState;
        protected void OnChangeStateEventHandler(ChangeStateEventArgs e)
        {
            if (ChangeState != null)
                ChangeState(this, e);
        }
        public event ChangeStateEventHandler FormDragDrop;
        protected void OnFormDragDrop(ChangeStateEventArgs e)
        {
            if (FormDragDrop != null)
                FormDragDrop(this, e);
        }
        protected override void OnResize(EventArgs e)
        {
            HelperModes modes1 = _state;
            base.OnResize(e);
        }
    }
    public delegate void ChangeStateEventHandler(object sender, ChangeStateEventArgs e);
}
