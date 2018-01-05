using System;
using JetBrains.Annotations;

namespace Simple_Scaler_2.Processing
{
    [PublicAPI]
    public class ImageFile
    {
        private readonly object _lock = new object();
        private readonly FileManager _manager;
        private readonly Transformer _transformer;
        private Exception _error;
        private PreparedImageFileInfo _preparedFileInfo;

        public string Name { get; set; }

        public string Path { get; set; }

        public ImageFileInfo FileInfo { get; set; }

        public PreparedImageFileInfo PreparedFileInfo
        {
            get { lock(_lock)return _preparedFileInfo; }
            private set { lock(_lock)_preparedFileInfo = value; }
        }

        public Exception Error
        {
            get { lock(_lock)return _error; }
            set { lock(_lock)_error = value; }
        }

        public Folder Folder { get; }

        public ImageFile(FileManager manager, Transformer transformer, Folder folder)
        {
            Folder = folder;
            _manager     = manager;
            _transformer = transformer;
        }

        public void Prepare()
        {
            lock(_lock)
            {
                _error = null;
                //var writeTime = File.GetLastWriteTime(Path);

                //if(FileInfo == null || _preparedFileInfo != null && _preparedFileInfo.Lastedit == writeTime) return;

                //var info = _manager.GetCache(Path);

                //if (info != null && info.Lastedit == writeTime)
                //{
                //    _preparedFileInfo = info;
                //    return;
                //}

                var result = _transformer.PrepareFile(Folder.Path, FileInfo);

                switch (result)
                {
                    case ExceptionResult exceptionResult:
                        _error = exceptionResult.Exception;
                        break;
                    case GenericResult<PreparedImageFileInfo> infoResult:
                        _preparedFileInfo = infoResult.Result;
                        FileInfo = _preparedFileInfo.FileInfo;
                        //_manager.SetCache(Path, info);
                        break;
                }
            }
        }
    }
}