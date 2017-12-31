using System;
using JetBrains.Annotations;

namespace Simple_Scaler_2.Processing
{
    [PublicAPI]
    public sealed class PreparedImageFileInfo
    {
        public PreparedImageFileInfo(string realFile, ImageFileInfo fileInfo, TransformSettings transformSettings, DateTime lastedit)
        {
            RealFile          = realFile;
            FileInfo          = fileInfo;
            TransformSettings = transformSettings;
            Lastedit          = lastedit;
        }

        public string RealFile { get; }

        public ImageFileInfo FileInfo { get; }

        public TransformSettings TransformSettings { get; }

        public DateTime Lastedit { get; }
    }
}