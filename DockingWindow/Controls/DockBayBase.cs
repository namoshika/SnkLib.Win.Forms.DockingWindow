using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Windows.Forms.Layout;
using System.Drawing;

namespace SunokoLibrary.Controls
{
    public partial class DockBayBase : DockBase
    {
        public DockBayBase()
        {
            BackColor = Color.Firebrick;
            _splitterWidth = 6;
            _panes = new List<DockPaneBase>();
            _neigh = DockNeigh.InitializeNeigh(this);
            _layoutEngine = new DockBayLayoutEngine(this);
        }

        DockBayLayoutEngine _layoutEngine;
        DockNeigh _neigh;
        List<DockPaneBase> _panes;
        int _splitterWidth;

        public override LayoutEngine LayoutEngine
        {
            get { return _layoutEngine; }
        }
        internal override DockBaseNeigh Neigh
        {
            get { return _neigh; }
        }
        public List<DockPaneBase> Panes
        {
            get { return _panes; }
        }
        public int SplitterWidth
        {
            get { return _splitterWidth; }
            set { _splitterWidth = value; }
        }

        public override void AddPane(DockBayBase bay, DockDirection dire)
        {
            var config = bay.GetConfig();
            var pane = bay.Neigh.Children[0];
            AddPane(pane, dire);
            for (int i = config.ChildNode.Count - 1; i > 0; i--)
            {
                var child = (DockPaneConfig)config.ChildNode[i];
                pane.AddPane((DockPaneBase)child.Node, child.Direction, child.SplitRate);
                AddChildPane(child);
            }
            base.AddChildPane(config.ChildNode[0]);
        }
        public override void AddPane(DockPaneBase pane, DockDirection dire)
        {
            double splitRate = 0.25;
            AddPane(pane, dire, splitRate);
        }
        public override void AddPane(DockPaneBase pane, DockDirection dire, double splitRate)
        {
            if (pane.Neigh.Owner != null)
                pane.Remove();

            Neigh.Add(pane, dire);
            _layoutEngine.AddPane(pane, dire, splitRate);
        }
        public virtual DockConfig GetConfig()
        {
            var config = CreateConfig();
            GetChildPaneConfig(config);
            return config;
        }
        public virtual void SetConfig(DockConfig config)
        {
            var panes = new List<DockPaneBase>(Panes);
            while (Panes.Count > 0)
                Panes[0].Remove();

            SetChildPaneConfig(config, panes);
        }
        protected override DockConfig CreateConfig()
        {
            return new DockBayConfig();
        }
        protected override void GetChildPaneConfig(DockConfig config)
        {
            if (config is DockBayConfig)
                ((DockBayConfig)config).ChildCount = Panes.Count;

            config.Index = -1;
            config.Width = ((double)Width) / ((double)Neigh.Owner.Width);
            config.Height = ((double)Height) / ((double)Neigh.Owner.Height);
            base.GetChildPaneConfig(config);
        }
        protected override void Dispose(bool disposing)
        {
            for (int i = 0; i < Panes.Count; i++)
                Panes[0].Remove();

            base.Dispose(disposing);
        }

        void DockPane_PaneAdded(object sender, DockEventArgs e)
        {
            _panes.Add(e.DockPane);
            var node = ((DockBaseNeigh)sender).Node;
            var neigh = (DockPaneBase.DockNeigh)e.DockPane.Neigh;
            neigh.PaneAdded += DockPane_PaneAdded;
            neigh.Removed += DockPane_Removed;
            e.DockPane.SplitterMoved += DockPane_SplitterMoved;
            Controls.Add(e.DockPane);

            OnPaneAdded(new PaneAddedEventArgs(node, e.DockPane, e.Align));
        }
        void DockPane_Removed(object sender, EventArgs e)
        {
            var neigh = (DockPaneBase.DockNeigh)sender;
            var node = (DockPaneBase)neigh.Node;
            _panes.Remove(node);
            Controls.Remove(neigh.Node);
            OnPaneRemoved(new PaneRemovedEventArgs(node));
        }
        void DockPane_SplitterMoved(object sender, DockEventArgs e)
        {
            var direInfoOf = ((DockPaneBase)sender).Neigh.GetDireInfoOf(e.Align);
            foreach (var pane in direInfoOf.Outers)
                _layoutEngine.Layout(this, new LayoutEventArgs(pane, null));
            foreach (var pane in direInfoOf.Inners)
                _layoutEngine.Layout(this, new LayoutEventArgs(pane, null));
        }

        public event PaneAddedEventHandler PaneAdded;
        protected virtual void OnPaneAdded(PaneAddedEventArgs e)
        {
            if (PaneAdded != null)
                PaneAdded(this, e);
        }
        public event PaneRemovedEventHandler PaneRemoved;
        protected virtual void OnPaneRemoved(PaneRemovedEventArgs e)
        {
            if (PaneRemoved != null)
                PaneRemoved(this, e);
        }
        protected override void SetChildPaneConfig(DockConfig config, List<DockPaneBase> panes)
        {
            if ((config is DockBayConfig) && (panes.Count != ((DockBayConfig)config).ChildCount))
            {
                throw new ArgumentException("引数configが持つノードの総数が自身の所有していたノードの総数と一致しません。");
            }
            foreach (DockPaneConfig config2 in config.ChildNode)
            {
                DockPaneBase pane = panes[config2.Index];
                AddPane(pane, config2.Direction);
            }
            for (int i = config.ChildNode.Count - 1; i >= 1; i--)
            {
                int num;
                DockPaneBase base3 = panes[config.ChildNode[i].Index];
                switch (((DockPaneBase.DockNeigh)base3.Neigh).Align)
                {
                    case DockDirection.Top:
                        num = ((int)((Height * config.ChildNode[i].Height) + 0.5)) - base3.Height;
                        base3.MoveSplitter(DockDirection.Bottom, num);
                        break;

                    case DockDirection.Bottom:
                        num = ((int)((Height * config.ChildNode[i].Height) + 0.5)) - base3.Height;
                        base3.MoveSplitter(DockDirection.Top, num);
                        break;

                    case DockDirection.Left:
                        num = ((int)((Width * config.ChildNode[i].Width) + 0.5)) - base3.Width;
                        base3.MoveSplitter(DockDirection.Right, num);
                        break;

                    case DockDirection.Right:
                        num = ((int)((Width * config.ChildNode[i].Width) + 0.5)) - base3.Width;
                        base3.MoveSplitter(DockDirection.Left, num);
                        break;

                    default:
                        throw new Exception("なにやらサイズ復元中のPaneのDirectionがTop,Bottom,Left,Right以外になってまっせ。");
                }
            }
            base.SetChildPaneConfig(config, panes);
        }

        public class DockBayLayoutEngine : DockLayoutEngineBase
        {
            public DockBayLayoutEngine(DockBayBase bay)
            {
                _bay = bay;
                _bay.PaneRemoved += _bay_PaneRemoved;
                _rateOf_paneSize = new Dictionary<Control, ResizeRate>();
            }

            DockBayBase _bay;
            Dictionary<Control, ResizeRate> _rateOf_paneSize;

            public override void AddPane(DockPaneBase pane, DockDirection dire, double rate)
            {
                var dictionary = GetSplitPaneRect(_bay.Bounds, dire, rate);
                Rectangle rectangle = dictionary[DockLayoutEngineBase.RectRate.LargeRect];
                Rectangle rectangle2 = dictionary[DockLayoutEngineBase.RectRate.SmallRect];
                if (_bay.Panes.Count < 2)
                {
                    rectangle = new Rectangle();
                    rectangle2 = new Rectangle(new Point(0, 0), _bay.Size);
                }
                else
                {
                    foreach (var child in _bay._panes)
                    {
                        if (child != pane)
                        {
                            ResizeRate rate2 = _rateOf_paneSize[child];
                            Rectangle rectangle3 = new Rectangle(((int)(((rectangle.Width - 1) * rate2.X) + 0.5)) + ((dire == DockDirection.Left) ? rectangle2.Width : 0), ((int)(((rectangle.Height - 1) * rate2.Y) + 0.5)) + ((dire == DockDirection.Top) ? rectangle2.Height : 0), (int)((rectangle.Width * rate2.Width) + 0.5), (int)((rectangle.Height * rate2.Height) + 0.5));
                            child.Bounds = rectangle3;
                        }
                    }
                }
                pane.Bounds = rectangle2;
            }
            public override bool Layout(object container, LayoutEventArgs layoutEventArgs)
            {
                var affectedControl = layoutEventArgs.AffectedControl;
                if (affectedControl is DockPaneBase)
                {
                    _rateOf_paneSize[affectedControl] = GetResizeRate(_bay.Bounds, affectedControl);
                }
                else if (affectedControl is DockBayBase)
                {
                    _bay.SuspendLayout();
                    foreach (var pane in _bay._panes)
                    {
                        var point = new Point((int)(((_bay.Width - 1) * _rateOf_paneSize[pane].X) + 0.5), (int)(((_bay.Height - 1) * _rateOf_paneSize[pane].Y) + 0.5));
                        var size = new Size((int)(_bay.Width * _rateOf_paneSize[pane].Width), (int)(_bay.Height * _rateOf_paneSize[pane].Height));
                        pane.Location = point;
                        pane.Size = size;
                    }
                    CoordinateLocationError();
                    _bay.ResumeLayout(true);
                }
                return base.Layout(container, layoutEventArgs);
            }
            ResizeRate GetResizeRate(Rectangle bounds, Control pane)
            {
                var x = ((pane.Right == 0) && (bounds.Right == 0)) ? 0.0 : (((double)pane.Left) / ((double)(bounds.Width - 1)));
                var y = ((pane.Top == 0) && (bounds.Bottom == 0)) ? 0.0 : (((double)pane.Top) / ((double)(bounds.Height - 1)));
                var w = ((pane.Width == 0) && (bounds.Width == 0)) ? 1.0 : (((double)pane.Width) / ((double)bounds.Width));
                return new ResizeRate(x, y, w, ((pane.Height == 0) && (bounds.Height == 0)) ? 1.0 : (((double)pane.Height) / ((double)bounds.Height)));
            }
            void CoordinateLocationError()
            {
                foreach (var pane in _bay._panes)
                {
                    pane.SuspendLayout();

                    int right, bottom;
                    var topIn = pane.Neigh.Top.Inners[0];
                    var leftIn = pane.Neigh.Left.Inners[0];
                    var bottomOut = pane.Neigh.Bottom.Outers[0];
                    var rightOut = pane.Neigh.Right.Outers[0];
                    var num = pane.Left - leftIn.Left;
                    var num2 = pane.Top - topIn.Top;
                    pane.Left -= num;
                    pane.Top -= num2;

                    if (bottomOut is DockPaneBase)
                        bottom = bottomOut.Top - pane.Bottom;
                    else
                        bottom = bottomOut.Height - pane.Bottom;

                    if (rightOut is DockPaneBase)
                        right = rightOut.Left - pane.Right;
                    else
                        right = rightOut.Width - pane.Right;

                    pane.Height += bottom;
                    pane.Width += right;
                    pane.ResumeLayout(true);
                }
            }
            void _bay_PaneRemoved(object sender, PaneRemovedEventArgs e)
            {
                _rateOf_paneSize.Remove(e.DockPane);
            }
            struct ResizeRate
            {
                public ResizeRate(double x, double y, double w, double h)
                {
                    X = x;
                    Y = y;
                    Width = w;
                    Height = h;
                }

                public double X;
                public double Y;
                public double Width;
                public double Height;
            }
        }
        public class DockNeigh : DockBaseNeigh
        {
            public sealed override void Add(DockPaneBase pane, DockDirection dire)
            {
                ((DockPaneBase.DockNeigh)pane.Neigh).Owner = Node as DockBayBase;
                ((DockPaneBase.DockNeigh)pane.Neigh).Parent = Node;

                var node = (DockBayBase)Node;
                if (node.Panes.Count > 0)
                    SplitPane(pane, dire);
                else
                {
                    pane.Neigh.Top.Outers = Top.Inners;
                    pane.Neigh.Bottom.Outers = Bottom.Inners;
                    pane.Neigh.Left.Outers = Left.Inners;
                    pane.Neigh.Right.Outers = Right.Inners;
                    pane.Neigh.Top.Inners = Top.Outers;
                    pane.Neigh.Bottom.Inners = Bottom.Outers;
                    pane.Neigh.Left.Inners = Left.Outers;
                    pane.Neigh.Right.Inners = Right.Outers;

                    pane.Neigh.Top.Inners.Add(pane);
                    pane.Neigh.Bottom.Inners.Add(pane);
                    pane.Neigh.Left.Inners.Add(pane);
                    pane.Neigh.Right.Inners.Add(pane);
                    pane.Neigh.Top.Inners.Remove(Node);
                    pane.Neigh.Bottom.Inners.Remove(Node);
                    pane.Neigh.Left.Inners.Remove(Node);
                    pane.Neigh.Right.Inners.Remove(Node);
                }
                Children.Add(pane);
                ((DockPaneBase.DockNeigh)pane.Neigh).Align = dire;
                OnPaneAdded(new DockEventArgs(pane, dire));
            }
            public sealed override void Clear()
            {
                throw new NotSupportedException("DockBayはClearメソッドに対応していません。");
            }
            public static DockBayBase.DockNeigh InitializeNeigh(DockBayBase node)
            {
                var neigh = new DockBayBase.DockNeigh();
                neigh.Owner = node;
                DockBaseNeigh.InitializeOf(neigh, node);
                InitEvent(neigh, node);

                return neigh;
            }
            void SplitPane(DockPaneBase pane, DockDirection dire)
            {
                if ((dire == DockDirection.Top) || (dire == DockDirection.Bottom))
                {
                    DockBaseNeigh.DireInfo pairInfoA;
                    DockBaseNeigh.DireInfo pairInfoB;
                    pane.Neigh.Left = Left.Outers[0].Neigh.Left;
                    pane.Neigh.Left.Inners.Add(pane);
                    pane.Neigh.Right = Right.Outers[0].Neigh.Right;
                    pane.Neigh.Right.Inners.Add(pane);
                    if (dire == DockDirection.Top)
                    {
                        pane.Neigh.Bottom = Top;
                        pane.Neigh.Bottom.Inners.Remove(Node);
                        pane.Neigh.Bottom.Inners.Add(pane);
                        pairInfoA = Top = DockBaseNeigh.DireInfo.Initialize(Node);
                        pairInfoB = pane.Neigh.Top = DockBaseNeigh.DireInfo.Initialize(pane);
                    }
                    else
                    {
                        pane.Neigh.Top = Bottom;
                        pane.Neigh.Top.Inners.Remove(Node);
                        pane.Neigh.Top.Inners.Add(pane);
                        pairInfoA = Bottom = DockBaseNeigh.DireInfo.Initialize(Node);
                        pairInfoB = pane.Neigh.Bottom = DockBaseNeigh.DireInfo.Initialize(pane);
                    }
                    pairInfoA.Outers = pairInfoB.Inners;
                    pairInfoB.Outers = pairInfoA.Inners;
                }
                else if ((dire == DockDirection.Left) || (dire == DockDirection.Right))
                {
                    DockBaseNeigh.DireInfo pairInfoA;
                    DockBaseNeigh.DireInfo pairInfoB;
                    pane.Neigh.Top = Top.Outers[0].Neigh.Top;
                    pane.Neigh.Top.Inners.Add(pane);
                    pane.Neigh.Bottom = Bottom.Outers[0].Neigh.Bottom;
                    pane.Neigh.Bottom.Inners.Add(pane);
                    if (dire == DockDirection.Left)
                    {
                        pane.Neigh.Right = Left;
                        pane.Neigh.Right.Inners.Remove(Node);
                        pane.Neigh.Right.Inners.Add(pane);
                        pairInfoA = Left = DockBaseNeigh.DireInfo.Initialize(Node);
                        pairInfoB = pane.Neigh.Left = DockBaseNeigh.DireInfo.Initialize(pane);
                    }
                    else
                    {
                        pane.Neigh.Left = Right;
                        pane.Neigh.Left.Inners.Remove(Node);
                        pane.Neigh.Left.Inners.Add(pane);
                        pairInfoA = Right = DockBaseNeigh.DireInfo.Initialize(Node);
                        pairInfoB = pane.Neigh.Right = DockBaseNeigh.DireInfo.Initialize(pane);
                    }
                    pairInfoA.Outers = pairInfoB.Inners;
                    pairInfoB.Outers = pairInfoA.Inners;
                }
            }

            protected static void InitEvent(DockBayBase.DockNeigh neigh, DockBayBase node)
            {
                neigh.PaneAdded += new DockEventHandler(node.DockPane_PaneAdded);
            }
            protected internal override void NeighPane_PaneAdded(object sender, DockEventArgs e)
            {

            }
            protected internal override void NeighPane_Removed(object sender, EventArgs e)
            {

            }
            protected internal override void NeighPane_Removing(object sender, EventArgs e)
            {

            }
        }
    }
    public class PaneAddedEventArgs : DockEventArgs
    {
        public PaneAddedEventArgs(DockBase addedpane, DockPaneBase pane, DockDirection dire)
            : base(pane, dire)
        {
            AddedPane = addedpane;
        }

        public DockBase AddedPane { get; private set; }
    }
    public delegate void PaneAddedEventHandler(object sender, PaneAddedEventArgs e);
    public class PaneRemovedEventArgs
    {
        public PaneRemovedEventArgs(DockPaneBase pane)
        {
            DockPane = pane;
        }
        public DockPaneBase DockPane { get; set; }
    }
    public delegate void PaneRemovedEventHandler(object sender, PaneRemovedEventArgs e);

    [Serializable]
    public class DockBayConfig : DockConfig
    {
        [XmlAttribute("Count")]
        public int ChildCount { get; set; }
        [XmlArrayItem(Type = typeof(DockPaneConfig))]
        public override List<DockConfig> ChildNode
        {
            get { return base.ChildNode; }
            set { base.ChildNode = value; }
        }
    }
}
