using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Layout;
using System.Drawing;
using System.Xml.Serialization;

namespace SunokoLibrary.Controls
{
    public abstract class DockBase : Control
    {
        public Color BorderColor { get; set; }
        internal abstract DockBaseNeigh Neigh { get; }

        public abstract void AddPane(DockBayBase bay, DockDirection align);
        public abstract void AddPane(DockPaneBase pane, DockDirection dire);
        public abstract void AddPane(DockPaneBase pane, DockDirection dire, double splitRate);
        protected abstract DockConfig CreateConfig();
        protected void AddChildPane(DockConfig config)
        {
            for (int i = 0; i < config.ChildNode.Count; i++)
            {
                var c = (DockPaneConfig)config.ChildNode[i];
                config.Node.AddPane((DockPaneBase)c.Node, c.Direction, c.SplitRate);
                AddChildPane(c);
            }
        }
        protected virtual void GetChildPaneConfig(DockConfig config)
        {
            config.Node = this;
            foreach (var pane in Neigh.Children)
            {
                var sconfig = pane.CreateConfig();
                sconfig.ParentNode = config;
                config.ChildNode.Add(sconfig);
                pane.GetChildPaneConfig(sconfig);
            }
        }
        protected virtual void SetChildPaneConfig(DockConfig config, List<DockPaneBase> panes)
        {
            foreach (DockConfig c in config.ChildNode)
            {
                panes[c.Index].SetChildPaneConfig(c, panes);
            }
        }
    }
    public abstract class DockLayoutEngineBase : LayoutEngine
    {
        public abstract void AddPane(DockPaneBase pane, DockDirection dire, double rate);
        public Dictionary<RectRate, Rectangle> GetSplitPaneRect(Rectangle pane, DockDirection dire, double splitRate)
        {
            Dictionary<RectRate, Rectangle> dictionary = new Dictionary<RectRate, Rectangle>();
            Rectangle rectA = new Rectangle(pane.Location, pane.Size);
            Rectangle rectB = new Rectangle(pane.Location, pane.Size);
            switch (dire)
            {
                case DockDirection.Top:
                    rectA.Height = (int)(pane.Height * splitRate);
                    rectB.Height -= rectA.Height;
                    rectB.Location = new Point(rectA.Left, rectA.Bottom);
                    break;

                case DockDirection.Bottom:
                    rectA.Height = (int)(pane.Height * splitRate);
                    rectB.Height -= rectA.Height;
                    rectA.Location = new Point(rectB.Left, rectB.Bottom);
                    break;

                case DockDirection.Left:
                    rectA.Width = (int)(pane.Width * splitRate);
                    rectB.Width -= rectA.Width;
                    rectB.Location = new Point(rectA.Right, rectA.Top);
                    break;

                case DockDirection.Right:
                    rectA.Width = (int)(pane.Width * splitRate);
                    rectB.Width -= rectA.Width;
                    rectA.Location = new Point(rectB.Right, rectB.Top);
                    break;
            }
            dictionary.Add(RectRate.SmallRect, rectA);
            dictionary.Add(RectRate.LargeRect, rectB);
            return dictionary;
        }

        public enum RectRate
        {
            SmallRect,
            LargeRect
        }
    }
    public abstract class DockBaseNeigh
    {
        public DockBaseNeigh()
        {
            Top = new DireInfo();
            Bottom = new DireInfo();
            Left = new DireInfo();
            Right = new DireInfo();
            Children = new List<DockPaneBase>();
        }

        public DockBase Node { get; internal set; }
        public DockBayBase Owner { get; internal set; }
        public List<DockPaneBase> Children { get; internal set; }
        public DockBase Parent { get; internal set; }
        public DireInfo Top { get; internal set; }
        public DireInfo Bottom { get; internal set; }
        public DireInfo Left { get; internal set; }
        public DireInfo Right { get; internal set; }

        public abstract void Add(DockPaneBase pane, DockDirection dire);
        public abstract void Clear();
        public DockDirection DirectionOfNeighPane(DockBase pane)
        {
            DockDirection res;
            if (Top.Outers.Contains(pane))
                res = DockDirection.Top;
            else if (Bottom.Outers.Contains(pane))
                res = DockDirection.Bottom;
            else if (Left.Outers.Contains(pane))
                res = DockDirection.Left;
            else if (Right.Outers.Contains(pane))
                res = DockDirection.Right;
            else
                res = DockDirection.None;

            return res;
        }
        public DireInfo GetDireInfoOf(DockDirection dire)
        {
            DireInfo res;
            switch (dire)
            {
                case DockDirection.Top:
                    res = Top;
                    break;
                case DockDirection.Bottom:
                    res = Bottom;
                    break;
                case DockDirection.Left:
                    res = Left;
                    break;
                case DockDirection.Right:
                    res = Right;
                    break;
                default:
                    throw new ArgumentException("引数direが異常です。");
            }

            return res;
        }
        protected static void InitializeOf(DockBaseNeigh neigh, DockBase node)
        {
            neigh.Node = node;
            neigh.Top = DireInfo.Initialize(node);
            neigh.Bottom = DireInfo.Initialize(node);
            neigh.Left = DireInfo.Initialize(node);
            neigh.Right = DireInfo.Initialize(node);
        }
        protected virtual void InitNode(DockBase pane, DockBayBase bay)
        {
            AddEventNeighPanes();
        }
        protected void AddEventNeighPanes()
        {
            DireInfo[] infos = new DireInfo[] { Top, Bottom, Left, Right };
            foreach (var info in infos)
                foreach (var pane in info.Outers)
                {
                    DockBaseNeigh neigh = pane.Neigh;
                    neigh.PaneAdded -= NeighPane_PaneAdded;
                    neigh.PaneAdded += NeighPane_PaneAdded;
                    if (neigh is DockPaneBase.DockNeigh)
                    {
                        ((DockPaneBase.DockNeigh)neigh).Removed -= NeighPane_Removed;
                        ((DockPaneBase.DockNeigh)neigh).Removed += NeighPane_Removed;
                        ((DockPaneBase.DockNeigh)neigh).Removing -= NeighPane_Removing;
                        ((DockPaneBase.DockNeigh)neigh).Removing += NeighPane_Removing;
                    }
                }
        }
        protected void RemoveEventNeighPanes()
        {
            var infoArray = new DireInfo[] { Top, Bottom, Left, Right };
            foreach (var info in infoArray)
                foreach (var node in info.Outers)
                {
                    var neigh = node.Neigh;
                    neigh.PaneAdded -= NeighPane_PaneAdded;
                    if (node.Neigh is DockPaneBase.DockNeigh)
                    {
                        ((DockPaneBase.DockNeigh)neigh).Removed -= NeighPane_Removed;
                        ((DockPaneBase.DockNeigh)neigh).Removing -= NeighPane_Removing;
                    }
                }
        }

        public event DockEventHandler PaneAdded;
        protected virtual void OnPaneAdded(DockEventArgs e)
        {
            e.DockPane.Neigh.InitNode(Node, Owner);
            if (PaneAdded != null)
                PaneAdded(this, e);
        }
        public event DockEventHandler PaneAdding;
        protected virtual void OnPaneAdding(DockEventArgs e)
        {
            if (PaneAdding != null)
                PaneAdding(this, e);
        }
        protected internal abstract void NeighPane_PaneAdded(object sender, DockEventArgs e);
        protected internal abstract void NeighPane_Removed(object sender, EventArgs e);
        protected internal abstract void NeighPane_Removing(object sender, EventArgs e);

        public class DireInfo
        {
            public DireInfo()
            {
                Inners = new List<DockBase>();
                Outers = new List<DockBase>();
            }
            public static DockBaseNeigh.DireInfo Initialize(DockBase node)
            {
                var info = new DockBaseNeigh.DireInfo();
                info.Inners.Add(node);
                return info;
            }

            public List<DockBase> Inners { get; set; }
            public List<DockBase> Outers { get; set; }
        }
    }
    public class DockConfig
    {
        public DockConfig()
        {
            ChildNode = new List<DockConfig>();
        }

        public double Width { get; set; }
        public double Height { get; set; }
        [XmlElement]
        public virtual List<DockConfig> ChildNode { get; set; }
        [XmlAttribute("index")]
        public int Index { get; set; }

        [XmlIgnore]
        public DockBase Node { get; set; }
        [XmlIgnore]
        public virtual DockConfig ParentNode { get; set; }
    }
    public enum DockDirection
    {
        None, Top, Bottom, Left, Right
    }
}