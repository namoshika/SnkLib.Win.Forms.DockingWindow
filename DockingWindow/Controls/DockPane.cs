using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using SunokoLibrary.Win32;

namespace SunokoLibrary.Controls
{
    public class DockPane : DockPaneBase
    {
        public DockPane()
        {
            _canFloating = true;
            _canDocking = true;
            _moveDistance = 10;
            _titleBarMinHeight = 18;
            _titleBar = new DockTitleBar();
            _titleBar.VisibleChanged += _titleBar_VisibleChanged;
            _titleBar.Resize += _titleBar_Resize;
            _titleBar.MouseDown += _titleBar_MouseDown;
            _titleBar.MouseMove += _titleBar_MouseMove;
            _titleBar.MouseLeave += _titleBar_MouseLeave;
            Controls.Add(_titleBar);
        }

        bool _canDocking;
        bool _canFloating;
        bool _isDragTitleBar;
        int _moveDistance;
        int _titleBarMinHeight;
        Point _point_mouseDown;
        DockTitleBar _titleBar;

        public bool CanDocking
        {
            get { return _canDocking; }
            set { _canDocking = value; }
        }
        public bool CanFloating
        {
            get { return _canFloating; }
            set
            {
                _canFloating = value;
                _titleBar.Visible = value;
            }
        }
        public int TitleBarHeight
        {
            get
            {
                int titleBarHeight = 0;
                if (Neigh.Owner is DockBayFloating)
                {
                    titleBarHeight = ((DockBayFloating.DockNeigh)Neigh.Owner.Neigh).TopLevelRootBay.TitleBarHeight;
                    titleBarHeight = Math.Max(titleBarHeight, _titleBarMinHeight);
                }
                else if (Neigh.Owner is DockBay)
                {
                    titleBarHeight = ((DockBay)Neigh.Owner).TitleBarHeight;
                    titleBarHeight = Math.Max(titleBarHeight, _titleBarMinHeight);
                }
                return titleBarHeight;
            }
        }

        public DockFormFloating Floating()
        {
            DockFormFloating floating;
            var neigh = (DockPaneBase.DockNeigh)Neigh;
            if (neigh.Owner is DockBayFloating)
            {
                var owner = (DockBayFloating.DockNeigh)neigh.Owner.Neigh;
                floating = new DockFormFloating(owner.TopLevelRootBay);
                ((DockBayFloating.DockNeigh)floating.DockBay.Neigh).TopLevelRootBay = owner.TopLevelRootBay;
            }
            else
            {
                floating = new DockFormFloating((DockBay)neigh.Owner);
                ((DockBayFloating.DockNeigh)floating.DockBay.Neigh).TopLevelRootBay = (DockBay)neigh.Owner;
            }
            Remove();
            floating.DockBay.AddPane(this, neigh.Align);
            floating.Show();
            OnFloating(new EventArgs());

            return floating;
        }
        protected override void ChangedInnerSize()
        {
            var point = new Point(1, 1);
            var titleHeight = CanFloating ? TitleBarHeight : 0;
            var contentHeight = Height - _neigh.Owner.SplitterWidth;
            if (SplitterVisible_bottom)
                titleHeight = Math.Min(titleHeight, contentHeight - 2);

            var w = (SplitterVisible_right ? (Width - _neigh.Owner.SplitterWidth) : Width) - 2;
            var h = (SplitterVisible_bottom ? (Height - _neigh.Owner.SplitterWidth) : Height) - 2;
            var size = new Size(w, titleHeight);
            _titleBar.Location = point;
            _titleBar.Size = size;

            ContentPanel.Location = new Point(1, _titleBar.Bottom);
            ContentPanel.Size = new Size(w, h - _titleBar.Bottom);
        }
        protected override void InitializeNode()
        {
            CanFloating = true;
            base.InitializeNode();
        }

        void _titleBar_MouseDown(object sender, MouseEventArgs e)
        {
            _isDragTitleBar = true;
            _point_mouseDown = Control.MousePosition;
        }
        void _titleBar_MouseLeave(object sender, EventArgs e)
        {
            if (_isDragTitleBar)
            {
                var rectangle = Parent.RectangleToScreen(Bounds);
                var floating = Floating();
                var mouse = Control.MousePosition;

                var x = mouse.X - (rectangle.Width / 2);
                var y = mouse.Y - (SystemInformation.ToolWindowCaptionHeight / 2);
                floating.Bounds = new Rectangle(new Point(x, y), rectangle.Size);
                _isDragTitleBar = false;

                var input = new API_SendInput.INPUT();
                input.type = API_SendInput.InputType.INPUT_MOUSE;
                input.mi.dwFlags = API_SendInput.dwFlags.KEYEVENTF_KEYUP;
                var inputs = new API_SendInput.INPUT[] { input };
                API_SendInput.SendInput(inputs);
            }
        }
        void _titleBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragTitleBar && e.Button == MouseButtons.Left)
            {
                var mouse = Control.MousePosition;
                var w = Math.Abs(mouse.X - _point_mouseDown.X);
                var h = Math.Abs(mouse.Y - _point_mouseDown.Y);
                if (w >= _moveDistance || h >= _moveDistance)
                {
                    var rect = Parent.RectangleToScreen(Bounds);
                    var form = Floating();
                    _isDragTitleBar = false;

                    //マウス位置がタイトルバーの真ん中に来るようにする。
                    var x = mouse.X - (rect.Width / 2);
                    var y = mouse.Y - (SystemInformation.ToolWindowCaptionHeight / 2);
                    form.Bounds = new Rectangle(new Point(x, y), rect.Size);

                    //タイトルバー移動モードを作り出す
                    var input = new API_SendInput.INPUT();
                    input.type = API_SendInput.InputType.INPUT_MOUSE;
                    input.mi.dwFlags = API_SendInput.dwFlags.KEYEVENTF_KEYUP;
                    API_SendInput.SendInput(new API_SendInput.INPUT[] { input });
                }
            }
        }
        void _titleBar_Resize(object sender, EventArgs e)
        {
            OnInnerAreaResize(new EventArgs());
        }
        void _titleBar_VisibleChanged(object sender, EventArgs e)
        {
            _titleBar.Height = _titleBar.Visible ? TitleBarHeight : 0;
            OnInnerAreaResize(new EventArgs());
        }

        public event EventHandler Floated;
        protected virtual void OnFloating(EventArgs e)
        {
            if (Floated != null)
                Floated(this, e);
        }
    }
}
