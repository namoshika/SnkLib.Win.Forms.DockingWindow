using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Windows.Forms.Layout;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;

namespace SunokoLibrary.Controls
{
    public class DockPaneBase : DockBase
    {
        public DockPaneBase()
        {
            BackColor = SystemColors.Control;
            ContentPanel = new Panel();
            ContentPanel.BackColor = SystemColors.Control;
            _layoutEngine = new DockPaneLayoutEngine(this);
            _neigh = DockNeigh.InitializeNeigh(this, null);
            Controls.Add(ContentPanel);
            InnerAreaResize += DockPane_InnerAreaResize;
        }

        const double _default_splitRate = 0.35;
        Cursor _cursor;
        Size _sizeOf_beforeReSize;
        SplitterDire _dragSplitter;
        DockPaneLayoutEngine _layoutEngine;
        protected DockNeigh _neigh;
        bool _splitterVisible_bottom;
        bool _splitterVisible_right;

        public Size IncludingSizeOfChildNodes
        {
            get
            {
                int width = Width;
                int height = Height;
                foreach (DockPaneBase child in Neigh.Children)
                {
                    Size includingSizeOfChildNodes = child.IncludingSizeOfChildNodes;
                    DockDirection align = ((DockNeigh)child.Neigh).Align;
                    switch (align)
                    {
                        case DockDirection.Top:
                        case DockDirection.Bottom:
                            {
                                height += includingSizeOfChildNodes.Height;
                                continue;
                            }
                        case DockDirection.Left:
                        case DockDirection.Right:
                            {
                                width += includingSizeOfChildNodes.Height;
                                continue;
                            }
                        default:
                            throw new Exception("なぜか引数valueにTop,Bottom,Left,Right以外の値が入っています。なんで？");
                    }
                }
                return new Size(width, height);
            }
        }
        public virtual Panel ContentPanel { get; protected set; }
        public override LayoutEngine LayoutEngine { get { return _layoutEngine; } }
        internal override DockBaseNeigh Neigh { get { return _neigh; } }
        protected bool SplitterVisible_bottom
        {
            get { return _splitterVisible_bottom; }
            private set
            {
                _splitterVisible_bottom = value;
                OnInnerAreaResize(new EventArgs());
            }
        }
        protected bool SplitterVisible_right
        {
            get { return _splitterVisible_right; }
            private set
            {
                _splitterVisible_right = value;
                OnInnerAreaResize(new EventArgs());
            }
        }

        public override void AddPane(DockBayBase bay, DockDirection dire)
        {
            var bayConfig = bay.GetConfig();
            var pane = bay.Neigh.Children[0];
            AddPane(pane, dire);
            for (var i = bayConfig.ChildNode.Count - 1; i > 0; i--)
            {
                var config = (DockPaneConfig)bayConfig.ChildNode[i];
                pane.AddPane((DockPaneBase)config.Node, config.Direction, config.SplitRate);
                AddChildPane(config);
            }
            AddChildPane(bayConfig.ChildNode[0]);
        }
        public override void AddPane(DockPaneBase pane, DockDirection dire)
        {
            AddPane(pane, dire, _default_splitRate);
        }
        public override void AddPane(DockPaneBase pane, DockDirection dire, double splitRate)
        {
            if (Neigh.Owner == null)
                throw new ArgumentException("Bayに挿入されていない状態で他のPaneを自身に挿入することは出来ません。");
            if (pane.Neigh.Owner != null)
                pane.Remove();

            Neigh.Add(pane, dire);
            _layoutEngine.AddPane(pane, dire, splitRate);
            OnPaneAdded(new DockEventArgs(pane, dire));
        }
        public void Remove()
        {
            if (Parent == null)
                return;
            var e = new CancelEventArgs();
            OnRemoving(e);
            if (e.Cancel)
                return;

            if (Neigh.Owner.Panes.Count > 1)
            {
                var succeedingDireWhenDeletingPane = _neigh.GetSucceedingDireWhenDeletingPane();
                switch (succeedingDireWhenDeletingPane)
                {
                    case DockDirection.Top:
                    case DockDirection.Bottom:
                        MoveSplitter(succeedingDireWhenDeletingPane, -Height);
                        break;

                    case DockDirection.Left:
                    case DockDirection.Right:
                        MoveSplitter(succeedingDireWhenDeletingPane, -Width);
                        break;
                }
            }
            Neigh.Clear();
            _neigh = DockNeigh.InitializeNeigh(this, null);
            OnRemoved(new EventArgs());
        }
        public double GetSplitRate()
        {
            Size includingSizeOfChildNodes = IncludingSizeOfChildNodes;
            List<DockPaneBase> children = Neigh.Parent.Neigh.Children;
            DockNeigh neigh = (DockNeigh)Neigh;
            if (neigh.Parent is DockPaneBase)
            {
                if ((neigh.Align == DockDirection.Top) || (neigh.Align == DockDirection.Bottom))
                {
                    DockNeigh neigh2;
                    for (int i = children.IndexOf(this) + 1; i < children.Count; i++)
                    {

                        neigh2 = children[i].Neigh as DockNeigh;
                        includingSizeOfChildNodes.Height += children[i].IncludingSizeOfChildNodes.Height;
                        if ((neigh2.Align == DockDirection.Left) || (neigh2.Align == DockDirection.Right))
                        {
                            goto Label_0248;
                        }
                    }
                    includingSizeOfChildNodes.Height += neigh.Parent.Height;
                }
                else
                {
                    DockNeigh neigh2;
                    for (int j = children.IndexOf(this) + 1; j < children.Count; j++)
                    {
                        neigh2 = children[j].Neigh as DockNeigh;
                        includingSizeOfChildNodes.Width += children[j].IncludingSizeOfChildNodes.Width;
                        if ((neigh2.Align == DockDirection.Top) || (neigh2.Align == DockDirection.Bottom))
                        {
                            goto Label_0248;
                        }
                    }
                    includingSizeOfChildNodes.Width += neigh.Parent.Width;
                }
            }
            else if (neigh.Parent is DockBayBase)
            {
                if ((neigh.Align == DockDirection.Top) || (neigh.Align == DockDirection.Bottom))
                {
                    DockNeigh neigh2;
                    for (int k = children.IndexOf(this) - 1; k > -1; k--)
                    {
                        neigh2 = children[k].Neigh as DockNeigh;
                        includingSizeOfChildNodes.Height += children[k].IncludingSizeOfChildNodes.Height;
                        if ((neigh2.Align == DockDirection.Left) || (neigh2.Align == DockDirection.Right))
                        {
                            break;
                        }
                    }
                }
                else
                {
                    DockNeigh neigh2;
                    for (int m = children.IndexOf(this) - 1; m > -1; m--)
                    {
                        neigh2 = children[m].Neigh as DockNeigh;
                        includingSizeOfChildNodes.Width += children[m].IncludingSizeOfChildNodes.Width;
                        if ((neigh2.Align == DockDirection.Top) || (neigh2.Align == DockDirection.Bottom))
                        {
                            break;
                        }
                    }
                }
            }
        Label_0248:
            if ((neigh.Align != DockDirection.Top) && (neigh.Align != DockDirection.Bottom))
                return (((double)IncludingSizeOfChildNodes.Width) / ((double)includingSizeOfChildNodes.Width));
            else
                return (((double)IncludingSizeOfChildNodes.Height) / ((double)includingSizeOfChildNodes.Height));
        }
        public void MoveSplitter(DockDirection dire, int distance)
        {
            if (_neigh.GetDireInfoOf(dire).Outers.Contains(_neigh.Owner))
                return;

            switch (dire)
            {
                case DockDirection.Top:
                    if (_neigh.Top.Outers.Count > 0)
                        (_neigh.Top.Outers[0] as DockPaneBase).MoveSplitter(DockDirection.Bottom, -distance);
                    break;
                case DockDirection.Bottom:
                    if (_neigh.Bottom.Outers.Contains(_neigh.Owner))
                        break;

                    //動かせる最大値を超えた場合は最大値分移動させる。
                    var height = int.MaxValue;
                    _neigh.Bottom.Outers.ForEach(
                        pane => { height = height < pane.Height ? pane.Height : height; });
                    distance = Math.Min(distance, height);
                    //境界面の座標を動かす
                    foreach (Control c in _neigh.Bottom.Inners)
                        c.Height += distance;
                    foreach (Control c in _neigh.Bottom.Outers)
                    {
                        c.Top += distance;
                        c.Height -= distance;
                    }
                    break;
                case DockDirection.Left:
                    if (_neigh.Left.Outers.Count > 0)
                        (_neigh.Left.Outers[0] as DockPaneBase).MoveSplitter(DockDirection.Right, -distance);
                    break;
                case DockDirection.Right:
                    if (!_neigh.Right.Outers.Contains(_neigh.Owner))
                    {
                        //動かせる最大値を超えた場合は最大値分移動させる。
                        var width = int.MaxValue;
                        _neigh.Bottom.Outers.ForEach(
                            pane => { width = Math.Min(width, pane.Width); });
                        distance = Math.Min(distance, width);
                        //境界面の座標を動かす
                        foreach (Control c in _neigh.Right.Inners)
                            c.Width += distance;
                        foreach (Control c in _neigh.Right.Outers)
                        {
                            c.Left += distance;
                            c.Width -= distance;
                        }
                    }
                    break;
                default:
                    throw new ArgumentException("引数direの値が異常です。");
            }
            OnSplitterMoved(new DockEventArgs(null, dire));
        }
        protected virtual void ChangedInnerSize()
        {
            var point = new Point(1, 1);
            var size = new Size(
                (SplitterVisible_right ? (Width - _neigh.Owner.SplitterWidth) : Width) - 2,
                (SplitterVisible_bottom ? (Height - _neigh.Owner.SplitterWidth) : Height) - 2);
            ContentPanel.Location = point;
            ContentPanel.Size = size;
        }
        protected override void SetChildPaneConfig(DockConfig config, List<DockPaneBase> panes)
        {
            foreach (DockPaneConfig c in config.ChildNode)
            {
                var pane = panes[c.Index];
                AddPane(pane, c.Direction, c.SplitRate);
            }
            base.SetChildPaneConfig(config, panes);
        }
        private void ShowSplitBar(DockDirection dire)
        {
            Rectangle rect;
            _cursor = CreateSplitBar(dire, SystemColors.ControlDark, (float)_neigh.Owner.SplitterWidth);
            var splitCursorClip = GetSplitCursorClip(dire);
            switch (dire)
            {
                case DockDirection.Bottom:
                    rect = new Rectangle(new Point(splitCursorClip.X + (splitCursorClip.Width / 2), splitCursorClip.Y), new Size(1, splitCursorClip.Height));
                    break;
                case DockDirection.Right:
                    rect = new Rectangle(new Point(splitCursorClip.X, splitCursorClip.Y + (splitCursorClip.Height / 2)), new Size(splitCursorClip.Width, 1));
                    break;
                default:
                    throw new ArgumentException("引数direの値が異常です。");
            }
            Cursor.Clip = rect;
            Cursor.Current = _cursor;
        }
        private Rectangle GetSplitCursorClip(DockDirection dire)
        {
            Rectangle rect;
            int num = int.MaxValue;
            int height, width, left, top;
            switch (dire)
            {
                case DockDirection.Bottom:
                    left = height = int.MaxValue;
                    top = int.MinValue;
                    width = 0;
                    if (!_neigh.Bottom.Outers.Contains(_neigh.Owner))
                    {
                        foreach (DockBase pane in _neigh.Bottom.Outers)
                            height = Math.Min(height, pane.Height);
                    }
                    else
                        height = 0;

                    foreach (DockBase pane in _neigh.Bottom.Inners)
                    {
                        width += pane.Width;
                        num = Math.Min(num, pane.Height);
                        left = Math.Min(left, pane.Left);
                        top = Math.Max(top, pane.Top);
                    }
                    rect = new Rectangle(new Point(left, top), new Size(width, num + height));
                    break;

                case DockDirection.Right:
                    left = int.MinValue;
                    top = width = int.MaxValue;
                    height = 0;
                    if (!_neigh.Right.Outers.Contains(_neigh.Owner))
                    {
                        foreach (DockBase pane in _neigh.Right.Outers)
                            width = Math.Min(width, pane.Width);
                    }
                    else
                        width = 0;

                    foreach (DockBase pane in _neigh.Right.Inners)
                    {
                        height += pane.Height;
                        num = Math.Min(num, pane.Width);
                        left = Math.Max(left, pane.Left);
                        top = Math.Min(top, pane.Top);
                    }
                    rect = new Rectangle(new Point(left, top), new Size(num + width, height));
                    break;

                default:
                    throw new ArgumentException("引数direの値が異常です。このメソッドはdireのRightとBottomしか使えません。");
            }
            return _neigh.Owner.RectangleToScreen(rect);
        }
        private Cursor CreateSplitBar(DockDirection dire, Color mesh, double width)
        {
            int clipWidth;
            int clipHeight;
            Rectangle splitCursorClip = GetSplitCursorClip(dire);
            Point position = Cursor.Position;
            Point hotSpot = Cursor.HotSpot;
            Size size = Cursor.Size;
            switch (dire)
            {
                case DockDirection.Top:
                case DockDirection.Bottom:
                    clipWidth = Math.Max(size.Width, splitCursorClip.Width);
                    clipHeight = size.Height;
                    break;
                case DockDirection.Left:
                case DockDirection.Right:
                    clipHeight = Math.Max(size.Height, splitCursorClip.Height);
                    clipWidth = size.Width;
                    break;
                default:
                    throw new ArgumentException("direの値が異常です。");
            }

            using (Bitmap bitmap = new Bitmap(clipWidth, clipHeight))
                using (Graphics graphics = Graphics.FromImage(bitmap))
                    using (HatchBrush brush = new HatchBrush(HatchStyle.Percent50, mesh))
                        using (Pen pen = new Pen(brush, (float)width))
                        {
                            if ((dire == DockDirection.Top) || (dire == DockDirection.Bottom))
                                graphics.DrawLine(pen,
                                    new Point(0, bitmap.Height / 2),
                                    new Point(bitmap.Width, bitmap.Height / 2));
                            else if ((dire == DockDirection.Left) || (dire == DockDirection.Right))
                                graphics.DrawLine(pen,
                                    new Point(bitmap.Width / 2, 0),
                                    new Point(bitmap.Width / 2, bitmap.Height));

                            using (Icon icon = Icon.FromHandle(bitmap.GetHicon()))
                            {
                                return new Cursor(icon.Handle);
                            }
                        }
        }
        protected override DockConfig CreateConfig()
        {
            return new DockPaneConfig();
        }
        protected override void GetChildPaneConfig(DockConfig config)
        {
            config.Index = Neigh.Owner.Panes.IndexOf(this);
            ((DockPaneConfig)config).Direction = ((DockNeigh)Neigh).Align;
            ((DockPaneConfig)config).SplitRate = GetSplitRate();
            Size includingSizeOfChildNodes = IncludingSizeOfChildNodes;
            config.Width = ((double)includingSizeOfChildNodes.Width) / ((double)Neigh.Owner.Width);
            config.Height = ((double)includingSizeOfChildNodes.Height) / ((double)Neigh.Owner.Height);
            base.GetChildPaneConfig(config);
        }
        protected virtual void InitializeNode()
        {
            //paneの外側がbayの場合はスプリッターを非表示
            SplitterVisible_right = !_neigh.Right.Outers.Contains(_neigh.Owner);
            SplitterVisible_bottom = !_neigh.Bottom.Outers.Contains(_neigh.Owner);
        }

        protected override void Dispose(bool disposing)
        {
            Remove();
            base.Dispose(disposing);
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            var point = PointToClient(Cursor.Position);
            var splitterWidth = _neigh.Owner.SplitterWidth;
            if (SplitterVisible_right && (point.X > ((Width - splitterWidth) - 1)))
            {
                ShowSplitBar(DockDirection.Right);
                _dragSplitter = SplitterDire.RightSplitter;
            }
            else if (SplitterVisible_bottom && (point.Y > ((Height - splitterWidth) - 1)))
            {
                ShowSplitBar(DockDirection.Bottom);
                _dragSplitter = SplitterDire.BottomSplitter;
            }
            base.OnMouseDown(e);
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            var splitterWidth = _neigh.Owner.SplitterWidth;
            int distance;
            switch (_dragSplitter)
            {
                case SplitterDire.RightSplitter:
                    distance = (_neigh.Owner.PointToClient(Cursor.Position).X - Right) + (splitterWidth / 2);
                    MoveSplitter(DockDirection.Right, distance);
                    break;
                case SplitterDire.BottomSplitter:
                    distance = (_neigh.Owner.PointToClient(Cursor.Position).Y - Bottom) + (splitterWidth / 2);
                    MoveSplitter(DockDirection.Bottom, distance);
                    break;
            }

            _dragSplitter = SplitterDire.None;
            Cursor.Clip = Rectangle.Empty;
            base.OnMouseUp(e);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            int num = _neigh.Owner.SplitterWidth + 1;
            var graphics = e.Graphics;
            using (var pen = new Pen(SystemColors.WindowFrame))
            {
                var width = SplitterVisible_right ? (Width - num) : (Width - 1);
                var height = SplitterVisible_bottom ? (Height - num) : (Height - 1);
                var rect = new Rectangle(new Point(0, 0), new Size(width, height));
                graphics.DrawRectangle(pen, rect);
            }
            base.OnPaint(e);
        }
        protected override void OnResize(EventArgs e)
        {
            Rectangle rectangle;
            int width = _neigh.Owner.SplitterWidth + 1;
            if (Size.Width > _sizeOf_beforeReSize.Width)
            {
                rectangle = new Rectangle(
                    new Point(_sizeOf_beforeReSize.Width - width, 0),
                    new Size(Size.Width - (_sizeOf_beforeReSize.Width - width), _sizeOf_beforeReSize.Height));
            }
            else
            {
                rectangle = new Rectangle(
                    new Point(Size.Width - width, 0),
                    new Size(width, Size.Height));
            }
            Rectangle rectangle2;
            if (Size.Height > _sizeOf_beforeReSize.Height)
            {
                rectangle2 = new Rectangle(
                    new Point(0, _sizeOf_beforeReSize.Height - width),
                    new Size(Size.Width, Size.Height - (_sizeOf_beforeReSize.Height - width)));
            }
            else
            {
                rectangle2 = new Rectangle(
                    new Point(0, Size.Height - width),
                    new Size(Size.Width, width));
            }
            Invalidate(rectangle);
            Invalidate(rectangle2);
            _sizeOf_beforeReSize = Size;
            OnInnerAreaResize(e);
            base.OnResize(e);
        }
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
        }
        private void DockPane_InnerAreaResize(object sender, EventArgs e)
        {
            ChangedInnerSize();
        }
        private void DockPane_PaneAdded(object sender, DockEventArgs e)
        {
            SplitterVisible_right = !Neigh.Right.Outers.Contains(Neigh.Owner);
            SplitterVisible_bottom = !Neigh.Bottom.Outers.Contains(Neigh.Owner);
        }
        private void Neigh_NeighPaneAdded(object sender, EventArgs e)
        {
            SplitterVisible_right = !Neigh.Right.Outers.Contains(Neigh.Owner);
            SplitterVisible_bottom = !Neigh.Bottom.Outers.Contains(Neigh.Owner);
        }
        private void Neigh_NeighPaneRemoved(object sender, EventArgs e)
        {
            SplitterVisible_right = !Neigh.Right.Outers.Contains(Neigh.Owner);
            SplitterVisible_bottom = !Neigh.Bottom.Outers.Contains(Neigh.Owner);
        }
        protected void NeighInitialized(DockNeigh neigh)
        {
            neigh.NeighPaneAdded += new EventHandler(Neigh_NeighPaneAdded);
            neigh.NeighPaneRemoved += new EventHandler(Neigh_NeighPaneRemoved);
            neigh.PaneAdded += new DockEventHandler(DockPane_PaneAdded);
        }

        public event EventHandler InnerAreaResize;
        protected virtual void OnInnerAreaResize(EventArgs e)
        {
            if (InnerAreaResize != null)
                InnerAreaResize(this, e);
        }
        public event DockEventHandler PaneAdded;
        protected virtual void OnPaneAdded(DockEventArgs e)
        {
            if (PaneAdded != null)
                PaneAdded(this, e);
        }
        public event EventHandler Removed;
        protected virtual void OnRemoved(EventArgs e)
        {
            if (Removed != null)
                Removed(this, e);
        }
        public event CancelEventHandler Removing;
        protected virtual void OnRemoving(CancelEventArgs e)
        {
            if (Removing != null)
                Removing(this, e);
        }
        public event DockEventHandler SplitterMoved;
        protected virtual void OnSplitterMoved(DockEventArgs e)
        {
            if (SplitterMoved != null)
                SplitterMoved(this, e);
        }

        public class DockNeigh : DockBaseNeigh
        {
            public DockDirection Align { get; protected internal set; }

            public sealed override void Add(DockPaneBase pane, DockDirection dire)
            {
                pane._neigh.Owner = Owner;
                pane._neigh.Parent = Node;
                Children.Add(pane);
                switch (dire)
                {
                    case DockDirection.Top:
                        pane.Neigh.Top = Top;
                        pane.Neigh.Left = Left;
                        pane.Neigh.Right = Right;
                        pane.Neigh.Top.Inners.Remove(Node);
                        pane.Neigh.Top.Inners.Add(pane);
                        pane.Neigh.Left.Inners.Add(pane);
                        pane.Neigh.Right.Inners.Add(pane);
                        pane.Neigh.Bottom = DockBaseNeigh.DireInfo.Initialize(pane);
                        Top = DockBaseNeigh.DireInfo.Initialize(Node);
                        pane.Neigh.Bottom.Outers = Top.Inners;
                        Top.Outers = pane.Neigh.Bottom.Inners;
                        break;
                    case DockDirection.Bottom:
                        pane.Neigh.Bottom = Bottom;
                        pane.Neigh.Right = Right;
                        pane.Neigh.Left = Left;
                        pane.Neigh.Bottom.Inners.Remove(Node);
                        Bottom.Inners.Add(pane);
                        Left.Inners.Add(pane);
                        Right.Inners.Add(pane);
                        pane.Neigh.Top = DockBaseNeigh.DireInfo.Initialize(pane);
                        Bottom = DockBaseNeigh.DireInfo.Initialize(Node);
                        Bottom.Outers = pane.Neigh.Top.Inners;
                        pane.Neigh.Top.Outers = Bottom.Inners;
                        break;
                    case DockDirection.Left:
                        pane.Neigh.Bottom = Bottom;
                        pane.Neigh.Left = Left;
                        pane.Neigh.Top = Top;
                        pane.Neigh.Left.Inners.Remove(Node);
                        pane.Neigh.Left.Inners.Add(pane);
                        pane.Neigh.Top.Inners.Add(pane);
                        pane.Neigh.Bottom.Inners.Add(pane);
                        pane.Neigh.Right = DockBaseNeigh.DireInfo.Initialize(pane);
                        Left = DockBaseNeigh.DireInfo.Initialize(Node);
                        Left.Outers = pane.Neigh.Right.Inners;
                        pane.Neigh.Right.Outers = Left.Inners;
                        break;

                    case DockDirection.Right:
                        pane.Neigh.Bottom = Bottom;
                        pane.Neigh.Right = Right;
                        pane.Neigh.Top = Top;
                        pane.Neigh.Right.Inners.Remove(Node);
                        pane.Neigh.Right.Inners.Add(pane);
                        pane.Neigh.Top.Inners.Add(pane);
                        pane.Neigh.Bottom.Inners.Add(pane);
                        pane.Neigh.Left = DockBaseNeigh.DireInfo.Initialize(pane);
                        Right = DockBaseNeigh.DireInfo.Initialize(Node);
                        Right.Outers = pane.Neigh.Left.Inners;
                        pane.Neigh.Left.Outers = Right.Inners;
                        break;
                    default:
                        throw new ApplicationException("direの値が異常です。");
                }
                pane._neigh.Align = dire;
                OnPaneAdded(new DockEventArgs(pane, dire));
            }
            public sealed override void Clear()
            {
                OnRemoving(new EventArgs());
                RemoveEventNeighPanes();
                var succeedingDireWhenDeletingPane = GetSucceedingDireWhenDeletingPane();
                Top.Inners.Remove(Node);
                Bottom.Inners.Remove(Node);
                Left.Inners.Remove(Node);
                Right.Inners.Remove(Node);
                RemoveFromPaneTree();
                if (Owner.Panes.Count > 1)
                {
                    switch (succeedingDireWhenDeletingPane)
                    {
                        case DockDirection.Top:
                            foreach (DockBase pane in Top.Outers)
                            {
                                pane.Neigh.Bottom = Bottom;
                                pane.Neigh.Bottom.Inners.Add(pane);
                            }
                            break;

                        case DockDirection.Bottom:
                            foreach (DockBase pane in Bottom.Outers)
                            {
                                pane.Neigh.Top = Top;
                                Top.Inners.Add(pane);
                            }
                            break;

                        case DockDirection.Left:
                            foreach (DockBase pane in Left.Outers)
                            {
                                pane.Neigh.Right = Right;
                                Right.Inners.Add(pane);
                            }
                            break;

                        case DockDirection.Right:
                            foreach (DockBase pane in Right.Outers)
                            {
                                pane.Neigh.Left = Left;
                                Left.Inners.Add(pane);
                            }
                            break;
                    }
                }
                OnRemoved(new EventArgs());
            }
            public DockDirection GetSucceedingDireWhenDeletingPane()
            {
                if (!Parent.Neigh.Children.Contains((DockPaneBase)Node))
                    throw new Exception();
                if (Children.Count > 0)
                    return ((DockPaneBase.DockNeigh)Children[Children.Count - 1].Neigh).Align;
                if (((Parent is DockBayBase) && (this == Parent.Neigh.Children[0].Neigh)) && (Parent.Neigh.Children.Count > 1))
                    return ((DockPaneBase.DockNeigh)Parent.Neigh.Children[1].Neigh).Align;

                switch (Align)
                {
                    case DockDirection.Top:
                        return DockDirection.Bottom;
                    case DockDirection.Bottom:
                        return DockDirection.Top;
                    case DockDirection.Left:
                        return DockDirection.Right;
                    case DockDirection.Right:
                        return DockDirection.Left;
                }
                throw new Exception("direの値が異常です。値の引き継ぎを見直してください");
            }
            public static DockPaneBase.DockNeigh InitializeNeigh(DockBase pane, DockBayBase bay)
            {
                DockPaneBase.DockNeigh neigh = new DockPaneBase.DockNeigh();
                InitializeOf(neigh, (DockPaneBase)pane);
                neigh.Owner = bay;
                return neigh;
            }
            protected static void InitializeOf(DockBaseNeigh neigh, DockPaneBase node)
            {
                node.NeighInitialized((DockPaneBase.DockNeigh)neigh);
                DockBaseNeigh.InitializeOf(neigh, node);
            }
            protected override void InitNode(DockBase pane, DockBayBase bay)
            {
                ((DockPaneBase)Node).InitializeNode();
                base.InitNode(pane, bay);
            }
            protected internal override void NeighPane_PaneAdded(object sender, DockEventArgs e)
            {
                var node = ((DockBaseNeigh)sender).Node;
                var neigh = node.Neigh;
                var dire = DirectionOfNeighPane(node);
                var direChild = DirectionOfNeighPane(e.DockPane);

                if ((dire == DockDirection.None) && (node != Node))
                    neigh.PaneAdded -= NeighPane_PaneAdded;
                if (node is DockPaneBase)
                {
                    var n = (DockPaneBase.DockNeigh)neigh;
                    n.PaneAdded -= NeighPane_PaneAdded;
                    n.Removed -= NeighPane_Removed;
                    n.Removing -= NeighPane_Removing;
                    PaneAdded -= n.NeighPane_PaneAdded;
                    Removed -= n.NeighPane_Removed;
                    Removing -= n.NeighPane_Removing;
                }

                if (direChild != DockDirection.None)
                {
                    var n = (DockPaneBase.DockNeigh)e.DockPane.Neigh;
                    n.PaneAdded += NeighPane_PaneAdded;
                    n.Removed +=NeighPane_Removed;
                    n.Removing += NeighPane_Removing;
                }
                if (sender != this)
                    OnNeighPaneAdded(new EventArgs());
            }
            protected internal override void NeighPane_Removed(object sender, EventArgs e)
            {
                var node = ((DockBaseNeigh)sender).Node;
                var neigh = (DockPaneBase.DockNeigh)node.Neigh;
                neigh.PaneAdded -= NeighPane_PaneAdded;
                neigh.Removed -= NeighPane_Removed;
                neigh.Removing -= NeighPane_Removing;

                OnNeighPaneRemoved(new EventArgs());
            }
            protected internal override void NeighPane_Removing(object sender, EventArgs e)
            {
                var node = ((DockBaseNeigh)sender).Node;
                var dire = DirectionOfNeighPane(node);
                var succeedingDireWhenDeletingPane = ((DockPaneBase.DockNeigh)node.Neigh).GetSucceedingDireWhenDeletingPane();
                switch (dire)
                {
                    case DockDirection.Top:
                    case DockDirection.Bottom:
                        switch (succeedingDireWhenDeletingPane)
                        {
                            case DockDirection.Top:
                            case DockDirection.Bottom:
                                foreach (DockBase pane in node.Neigh.GetDireInfoOf(dire).Outers)
                                {
                                    pane.Neigh.PaneAdded -= NeighPane_PaneAdded;
                                    pane.Neigh.PaneAdded += NeighPane_PaneAdded;
                                    if (pane.Neigh is DockPaneBase.DockNeigh)
                                    {
                                        var neigh = (DockPaneBase.DockNeigh)pane.Neigh;
                                        neigh.Removed -= NeighPane_Removed;
                                        neigh.Removed += NeighPane_Removed;
                                        neigh.Removing -= NeighPane_Removing;
                                        neigh.Removing += NeighPane_Removing;
                                    }
                                }
                                break;
                        }
                        break;
                    case DockDirection.Left:
                    case DockDirection.Right:
                        switch (succeedingDireWhenDeletingPane)
                        {
                            case DockDirection.Left:
                            case DockDirection.Right:
                                foreach (DockBase pane in node.Neigh.GetDireInfoOf(dire).Outers)
                                {
                                    pane.Neigh.PaneAdded -= new DockEventHandler(NeighPane_PaneAdded);
                                    pane.Neigh.PaneAdded += new DockEventHandler(NeighPane_PaneAdded);
                                    if (pane.Neigh is DockPaneBase.DockNeigh)
                                    {
                                        var neigh = (DockPaneBase.DockNeigh)pane.Neigh;
                                        neigh.Removed -= NeighPane_Removed;
                                        neigh.Removed += NeighPane_Removed;
                                        neigh.Removing -= NeighPane_Removing;
                                        neigh.Removing += NeighPane_Removing;
                                    }
                                }
                                break;
                        }
                        break;
                    default:
                        throw new ApplicationException("想定外の動作が確認されました。");
                }
                OnNeighPaneRemoving(new EventArgs());
            }
            protected override void OnPaneAdded(DockEventArgs e)
            {
                base.OnPaneAdded(e);
                NeighPane_PaneAdded(this, new DockEventArgs(e.DockPane, e.Align));
            }
            private void RemoveFromPaneTree()
            {
                if (Children.Count > 0)
                {
                    DockPaneBase item = Children[Children.Count - 1];
                    item.Neigh.Children.InsertRange(0, Children);
                    item.Neigh.Children.Remove(item);
                    DockPaneBase.DockNeigh neigh = (DockPaneBase.DockNeigh)item.Neigh;
                    neigh.Align = Align;
                    neigh.Parent = Parent;
                    neigh.Parent.Neigh.Children[Parent.Neigh.Children.IndexOf((DockPaneBase)Node)] = item;
                    foreach (DockPaneBase pane in item.Neigh.Children)
                    {
                        ((DockPaneBase.DockNeigh)pane.Neigh).Parent = item;
                    }
                }
                else
                {
                    Parent.Neigh.Children.Remove((DockPaneBase)base.Node);
                }
            }

            public event EventHandler NeighPaneAdded;
            protected virtual void OnNeighPaneAdded(EventArgs e)
            {
                if (NeighPaneAdded != null)
                    NeighPaneAdded(this, e);
            }
            public event EventHandler NeighPaneRemoved;
            protected virtual void OnNeighPaneRemoved(EventArgs e)
            {
                if (NeighPaneRemoved != null)
                    NeighPaneRemoved(this, e);
            }
            public event EventHandler NeighPaneRemoving;
            protected virtual void OnNeighPaneRemoving(EventArgs e)
            {
                if (NeighPaneRemoving != null)
                    NeighPaneRemoving(this, e);
            }
            public event EventHandler Removed;
            protected virtual void OnRemoved(EventArgs e)
            {
                if (Removed != null)
                    Removed(this, e);
            }
            public event EventHandler Removing;
            protected virtual void OnRemoving(EventArgs e)
            {
                if (Removing != null)
                    Removing(this, e);
            }
        }
        public class DockPaneLayoutEngine : DockLayoutEngineBase
        {
            private DockPaneBase _pane;

            public DockPaneLayoutEngine(DockPaneBase pane)
            {
                _pane = pane;
            }
            public override void AddPane(DockPaneBase pane, DockDirection dire, double rate)
            {
                var dictionary = GetSplitPaneRect(_pane.Bounds, dire, rate);
                Rectangle rectangle2 = dictionary[DockLayoutEngineBase.RectRate.SmallRect];
                Rectangle rectangle = dictionary[DockLayoutEngineBase.RectRate.LargeRect];
                _pane.Size = rectangle.Size;
                _pane.Location = rectangle.Location;
                pane.Size = rectangle2.Size;
                pane.Location = rectangle2.Location;
            }

        }
    }
    [Serializable, XmlRoot("DockPane")]
    public class DockPaneConfig : DockConfig
    {
        [XmlArrayItem(Type = typeof(DockPaneConfig))]
        public override List<DockConfig> ChildNode
        {
            get { return base.ChildNode; }
            set { base.ChildNode = value; }
        }

        public DockDirection Direction { get; set; }
        public double SplitRate { get; set; }
    }
    public enum SplitterDire
    {
        None,
        RightSplitter,
        BottomSplitter
    }

    public delegate void DockEventHandler(object sender, DockEventArgs e);
    public class DockEventArgs : EventArgs
    {
        public DockEventArgs(DockPaneBase pane, DockDirection align)
        {
            DockPane = pane;
            Align = align;
        }

        public DockDirection Align { get; protected set; }
        public DockPaneBase DockPane { get; protected set; }
    }
}
