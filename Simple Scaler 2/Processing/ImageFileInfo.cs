using JetBrains.Annotations;

namespace Simple_Scaler_2.Processing
{
    [PublicAPI]
    public sealed class ImageFileInfo
    {
        public ImageFileInfo(bool isSingleLayer, bool isCorrectResolution, bool isCorrectType, string filePath, bool isAccesible, bool isResolutionUnKnowen, bool isGreyScale)
        {
            IsSingleLayer        = isSingleLayer;
            IsCorrectResolution  = isCorrectResolution;
            IsCorrectType        = isCorrectType;
            FilePath             = filePath;
            IsAccesible          = isAccesible;
            IsResolutionUnKnowen = isResolutionUnKnowen;
            IsGreyScale          = isGreyScale;
        }

        public bool IsSingleLayer { get; }

        public bool IsCorrectResolution { get; }

        public bool IsResolutionUnKnowen { get; }

        public bool IsGreyScale { get; }

        public bool IsCorrectType { get; }

        public string FilePath { get; }

        public bool IsAccesible { get; }
    }
}