using System;
using System.Drawing;
using System.Windows.Forms;

namespace Jv.Web.OAuth.WinForms
{
    class PictureButton : Control, IButtonControl
    {
        bool pressed = false;
        bool over = false;

        public PictureButton()
        {
        }

        public new EventHandler Click;

        protected override void OnMouseEnter(EventArgs e)
        {
            over = true;
            this.Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            over = false;
            this.Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            PerformClick();
            base.OnMouseClick(e);
        }

        public Image PressedImage { get; set; }
        public Image OverImage { get; set; }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            this.pressed = true;
            this.Invalidate();
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            this.pressed = false;
            this.Invalidate();
            base.OnMouseUp(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.pressed && this.PressedImage != null)
                e.Graphics.DrawImage(this.PressedImage, 0, 0);
            else if (this.over && this.OverImage != null)
                e.Graphics.DrawImage(this.OverImage, 0, 0);
            else if (BackgroundImage != null)
                e.Graphics.DrawImage(this.BackgroundImage, 0, 0);

            base.OnPaint(e);
        }

        public DialogResult DialogResult { get; set; }

        public void NotifyDefault(bool value)
        {
        }

        public void PerformClick()
        {
            var handler = Click;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}
