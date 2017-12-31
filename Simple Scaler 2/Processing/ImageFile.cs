using System;
using System.IO;
using JetBrains.Annotations;

namespace Simple_Scaler_2.Processing
{
    [PublicAPI]
    public class ImageFile
    {
        private readonly FileManager _manager;
        private readonly Transformer _transformer;

        public string Name { get; set; }

        public string Path { get; set; }

        public ImageFileInfo FileInfo { get; set; }

        public PreparedImageFileInfo PreparedFileInfo { get; private set; }

        public Exception Error { get; set; }

        public Folder Folder { get; }

        public ImageFile(FileManager manager, Transformer transformer, Folder folder)
        {
            Folder = folder;
            _manager     = manager;
            _transformer = transformer;
        }

        public void Prepare()
        {
            Error = null;
            var writeTime = File.GetLastWriteTime(Path);

            if(FileInfo == null || (PreparedFileInfo != null && PreparedFileInfo.Lastedit == writeTime)) return;

            var info = _manager.GetCache(Path);

            if (info != null && info.Lastedit == writeTime)
            {
                PreparedFileInfo = info;
                return;
            }

            var result = _transformer.PrepareFile(FileInfo);

            switch (result)
            {
                case ExceptionResult exceptionResult:
                    Error = exceptionResult.Exception;
                    break;
                case GenericResult<PreparedImageFileInfo> infoResult:
                    PreparedFileInfo = infoResult.Result;
                    _manager.SetCache(Path, info);
                    break;
            }
        }
    }
}