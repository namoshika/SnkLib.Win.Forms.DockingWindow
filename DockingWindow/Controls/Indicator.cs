using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SunokoLibrary.Controls
{
    public partial class Indicator : UserControl
    {
        public Indicator()
        {
            InitializeComponent();
            IsActive = false;
        }
        private bool _isActive;

        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if ((NormalImage != null) && (ActiveImage != null))
                {
                    pictureBox1.Image = value ? ActiveImage : NormalImage;
                    _isActive = value;
                }
            }
        }
        public Image ActiveImage { get; set; }
        public Image NormalImage { get; set; }
    }
}
