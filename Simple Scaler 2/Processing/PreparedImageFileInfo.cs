using JetBrains.Annotations;

namespace Simple_Scaler_2.Processing
{
    [PublicAPI]
    public sealed class PreparedImageFileInfo
    {
        public PreparedImageFileInfo(string realFile, ImageFileInfo fileInfo, TransformSettings transformSettings)
        {
            RealFile          = realFile;
            FileInfo          = fileInfo;
            TransformSettings = transformSettings;
        }

        public string RealFile { get; }

        public ImageFileInfo FileInfo { get; }

        public TransformSettings TransformSettings { get; }
    }
}