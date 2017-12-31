using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using ImageMagick;
using JetBrains.Annotations;
using Simple_Scaler_2.Properties;

namespace Simple_Scaler_2.Processing
{
    [PublicAPI]
    public class FileManager
    {
        private class InternalSync : ISynchronizeInvoke
        {
            private readonly Dispatcher _dispatcher = Application.Current.Dispatcher;

            public IAsyncResult BeginInvoke(Delegate method, object[] args)
            {
                return _dispatcher.BeginInvoke(method, args).Task;
            }

            public object EndInvoke(IAsyncResult result)
            {
                throw new NotSupportedException();
            }

            public object Invoke(Delegate method, object[] args)
            {
                return _dispatcher.Invoke(method, args);
            }

            public bool InvokeRequired { get; } = true;
        }

        private readonly Dispatcher _dispatcher = Application.Current.Dispatcher;
        private readonly Dictionary<string, PreparedImageFileInfo> _cache = new Dictionary<string, PreparedImageFileInfo>();
        private readonly Settings _settings = Settings.Default;
        private readonly string[] _filesFormats = MagickNET.SupportedFormats.Select(i => "." + i.Format.ToString()).ToArray();
        private FileSystemWatcher _fileWatcher = new FileSystemWatcher {IncludeSubdirectories = false, SynchronizingObject = new InternalSync(), NotifyFilter = NotifyFilters.LastAccess 
                                                                                                                                                              | NotifyFilters.LastWrite 
                                                                                                                                                              | NotifyFilters.FileName};
        private FileSystemWatcher _folderWatcher;
        private readonly Transformer _transformer;

        public FileManager(Transformer transformer)
        {
            _transformer = transformer;
            _fileWatcher.Created += FileWatcherOnCreated;
            _fileWatcher.Deleted += FileWatcherOnDeleted;
            _fileWatcher.Renamed += FileWatcherOnRenamed;
        }

        public bool IsPathValid()
        {
            string source = _settings.SourcePath;
            string target = _settings.TargetPath;

            return !string.IsNullOrWhiteSpace(source) && !string.IsNullOrWhiteSpace(target) &&
                   Directory.Exists(source) && Directory.Exists(target);
        }

        public ObservableCollection<Folder> Folders { get; } = new ObservableCollection<Folder>();

        public ObservableCollection<ImageFile> Files { get; set; } = new ObservableCollection<ImageFile>();

        public PreparedImageFileInfo GetCache(string path) => _cache.TryGetValue(path, out var value) ? value : null;

        public void SetCache(string path, PreparedImageFileInfo info) => _cache[path] = info;

        public void Initialize()
        {
            ReadDirectorys();
            _folderWatcher = new FileSystemWatcher(_settings.SourcePath) {
                                                                             EnableRaisingEvents = true, 
                                                                             IncludeSubdirectories = false, 
                                                                             NotifyFilter = NotifyFilters.LastAccess 
                                                                                          | NotifyFilters.LastWrite 
                                                                                          | NotifyFilters.DirectoryName,
                                                                             SynchronizingObject = new InternalSync()

                                                                         };
            _folderWatcher.Created += FolderWatcherOnCreated;
            _folderWatcher.Deleted += FolderWatcherOnDeleted;
            _folderWatcher.Renamed += FolderWatcherOnRenamed;
            _settings.SettingsSaving += SettingsOnSettingsSaving;
        }

        private void SettingsOnSettingsSaving(object sender, CancelEventArgs cancelEventArgs)
        {
            if(_folderWatcher.Path == _settings.SourcePath) return;

            ReadDirectorys();
            _folderWatcher.Path = _settings.SourcePath;
        }

        private void FolderWatcherOnRenamed(object sender, RenamedEventArgs renamedEventArgs)
        {
            var folder = Folders.First(f => f.Name == renamedEventArgs.OldName);

            folder.Name = renamedEventArgs.Name;
            folder.Path = renamedEventArgs.FullPath;
        }

        private void FolderWatcherOnDeleted(object sender, FileSystemEventArgs fileSystemEventArgs) => Folders.Remove(Folders.Single(f => f.Path == fileSystemEventArgs.FullPath));

        private void FolderWatcherOnCreated(object sender, FileSystemEventArgs fileSystemEventArgs) => Folders.Add(new Folder(fileSystemEventArgs.Name, fileSystemEventArgs.FullPath));

        private void ReadDirectorys()
        {
            Folders.Clear();
            foreach (var directory in Directory.GetDirectories(_settings.SourcePath))
                Folders.Add(new Folder(new DirectoryInfo(directory).Name, directory));
        }

        public void ReadFiles(Folder folder)
        {
            _fileWatcher.EnableRaisingEvents = false;
            _dispatcher.Invoke(Files.Clear);

            if(folder == null) return;

            foreach (var enumerateFile in Directory.EnumerateFiles(folder.Path).Where(IsFormatSupportet))
            {
                ImageFile file = new ImageFile(this, _transformer, folder);

                Fill(file, enumerateFile);

                if(file.Error != null) continue;

                _dispatcher.Invoke(() => Files.Add(file));
            }

            _fileWatcher.Path = folder.Path;
            _fileWatcher.EnableRaisingEvents = true;
        }

        private bool IsFormatSupportet(string file) => _filesFormats.Contains(Path.GetExtension(file), StringComparer.OrdinalIgnoreCase);

        private void Fill(ImageFile file, string path)
        {
            file.Error = null;
            file.Name = Path.GetFileName(path);
            file.Path = path;
            var result = _transformer.GetInfo(path);

            switch (result)
            {
                case ExceptionResult exceptionResult:
                    file.Error = exceptionResult.Exception;
                    break;
                case GenericResult<ImageFileInfo> info:
                    file.FileInfo = info.Result;
                    break;
            }
        }

        private void FileWatcherOnRenamed(object sender, RenamedEventArgs renamedEventArgs)
        {
            var file = Files.FirstOrDefault(f => f.Name == renamedEventArgs.OldName);
            if(file == null) return;
            if (!IsFormatSupportet(renamedEventArgs.FullPath))
            {
                Files.Remove(file);
                return;
            }

            Fill(file, renamedEventArgs.FullPath);

            if (file.Error != null) Files.Remove(file);
            
        }

        private void FileWatcherOnDeleted(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            var file = Files.FirstOrDefault(f => f.Name == fileSystemEventArgs.Name);
            if(file == null) return;

            Files.Remove(file);
        }

        private void FileWatcherOnCreated(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            if(!IsFormatSupportet(fileSystemEventArgs.FullPath)) return;

            var file = new ImageFile(this, _transformer, Folders.First(f => fileSystemEventArgs.FullPath.Contains(f.Path)));
            Fill(file, fileSystemEventArgs.FullPath);

            if(file.Error != null) return;

            Files.Add(file);
        }
    }
}