using Jv.Web.OAuth.WinForms.Properties;

namespace Jv.Web.OAuth.WinForms
{
    class BackButton : PictureButton
    {
        public BackButton()
        {
            BackgroundImage = Resources.Image_BackButton_Normal;
            OverImage = Resources.Image_BackButton_Over;
            PressedImage = Resources.Image_BackButton_Pressed;

            Width = Height = 30;
        }
    }
}
