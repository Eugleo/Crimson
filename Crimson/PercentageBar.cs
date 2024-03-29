﻿using System;
using System.Drawing;
using System.Windows.Forms;

namespace Crimson
{
    public partial class PercentageBar : ProgressBar
    {
        public Color Color
        {
            get { return _color; }
            set
            {
                _color = value;
                _brush = new SolidBrush(_color);
                Invalidate();
            }
        }
        Color _color;
        Brush _brush;

        public PercentageBar() : base()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            Color = Color.Black;
        }

        public float Val
        {
            get { return _value; }
            set
            {
                _value = value;
                Invalidate();
            }
        }
        float _value;

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(Pens.Black, new Rectangle(0, 0, Width - 1, Height - 1));
            e.Graphics.FillRectangle(_brush, new Rectangle(2, 2, (int)Math.Round((Width - 4) * (_value / Maximum)), Height - 4));
        }
    }
}
