using System;
using System.Windows.Media;
using JetBrains.Annotations;

namespace Simple_Scaler_2.Processing
{
    [PublicAPI]
    public sealed class PreviewGeneratedEventArgs : EventArgs
    {
        public PreviewGeneratedEventArgs(ImageSource previewLeft, ImageSource previewRight)
        {
            PreviewLeft  = previewLeft;
            PreviewRight = previewRight;
        }

        public ImageSource PreviewLeft { get; }

        public ImageSource PreviewRight { get; }
    }
}