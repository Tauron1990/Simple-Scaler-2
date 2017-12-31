using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using JetBrains.Annotations;
using Simple_Scaler_2.Processing;

namespace Simple_Scaler_2
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Common

        private readonly Dispatcher _dispatcher = Application.Current.Dispatcher;

        public bool IsBusy
        {
            get => GetProperty(() => IsBusy);
            set => SetProperty(() => IsBusy, value);
        }

        public string BusyLabel
        {
            get => GetProperty(() => BusyLabel);
            set => SetProperty(() => BusyLabel, value);
        }

        private void RunOperation(string title, Action action)
        {
            IsBusy    = true;
            BusyLabel = title;

            Task.Run(() =>
                     {
                         action();
                         IsBusy = false;
                     });
        }

        protected override void OnInitializeInRuntime()
        {
            _fileManager = new FileManager(_transformer);
            _transformer.PreviewGeneratedEvent += TransformerOnPreviewGeneratedEvent;

            Rand1X = 0;
            Rand2X = 0;

            Rand1Y = 0;
            Rand2Y = 0;

            Kor1X = 0;
            Kor1Y = 0;

            Kor2X = 0;
            Kor2Y = 0;

            Kor3X = 0;
            Kor3Y = 0;

            Kor4X = 0;
            Kor4Y = 0;

            if (!_fileManager.IsPathValid())
            {
                RunOperation("Starting", () =>
                                         {
                                             do
                                             {
                                                 MessageBox.Show("Verzeichnisspfade sind Ungültig!", "Fehler");
                                                 _dispatcher.BeginInvoke(new Action(() =>new SettingsWindow.SettingsWindow {WindowStartupLocation = WindowStartupLocation.CenterScreen}.ShowDialog()))
                                                            .Wait();

                                             } while (!_fileManager.IsPathValid());

                                             _fileManager.Initialize();
                                         });
            }
            else
                _fileManager.Initialize();

            base.OnInitializeInRuntime();
        }

        public bool Checker
        {
            get => GetProperty(() => Checker);
            set => SetProperty(() => Checker, value);
        }

        [Command, UsedImplicitly]
        public void ShowSettings() => new SettingsWindow.SettingsWindow {Owner = Application.Current.MainWindow, WindowStartupLocation = WindowStartupLocation.CenterOwner}.ShowDialog();

        #endregion

        #region Coordinates

        private void UpdateLabels()
        {
            Rand1XLabel = Rand1X.ToString();
            Rand2XLabel = Rand2X.ToString();

            Rand1YLabel = Rand1Y.ToString();
            Rand2YLabel = Rand2Y.ToString();

            Kor1XLabel = Kor1X.ToString();
            Kor1YLabel = Kor1Y.ToString();

            Kor2XLabel = Kor2X.ToString();
            Kor2YLabel = Kor2Y.ToString();

            Kor3XLabel = Kor3X.ToString();
            Kor3YLabel = Kor3Y.ToString();

            Kor4XLabel = Kor4X.ToString();
            Kor4YLabel = Kor4Y.ToString();
        }

        private void SetTransformSettings(TransformSettings settings)
        {
            Checker = settings.Checker;
            Rand1X = settings.Rand1X;
            Rand1Y = settings.Rand1Y;
            Rand2X = settings.Rand2X;
            Rand2Y = settings.Rand2Y;
            Kor1X = settings.Kor1X;
            Kor1Y = settings.Kor1Y;
            Kor2X = settings.Kor2X;
            Kor2Y = settings.Kor2Y;
            Kor3X = settings.Kor3X;
            Kor3Y = settings.Kor3Y;
            Kor4X = settings.Kor4X;
            Kor4Y = settings.Kor4Y;
        }

        private void FillTransformSettings(TransformSettings settings)
        {
            settings.Checker                    = Checker;
            if (Rand1X != null) settings.Rand1X = (int) Rand1X;
            if (Rand1Y != null) settings.Rand1Y = (int) Rand1Y;
            if (Rand2X != null) settings.Rand2X = (int) Rand2X;
            if (Rand2Y != null) settings.Rand2Y = (int) Rand2Y;
            if (Kor1X != null) settings.Kor1X   = (int) Kor1X;
            if (Kor1Y != null) settings.Kor1Y   = (int) Kor1Y;
            if (Kor2X != null) settings.Kor2X   = (int) Kor2X;
            if (Kor2Y != null) settings.Kor2Y   = (int) Kor2Y;
            if (Kor3X != null) settings.Kor3X   = (int) Kor3X;
            if (Kor3Y != null) settings.Kor3Y   = (int) Kor3Y;
            if (Kor4X != null) settings.Kor4X   = (int) Kor4X;
            if (Kor4Y != null) settings.Kor4Y   = (int) Kor4Y;
        }

        public long? Rand1X
        {
            get => GetProperty(() => Rand1X);
            set => SetProperty(() => Rand1X, value);
        }

        public long? Rand1Y
        {
            get => GetProperty(() => Rand1Y);
            set => SetProperty(() => Rand1Y, value);
        }

        public long? Rand2X
        {
            get => GetProperty(() => Rand2X);
            set => SetProperty(() => Rand2X, value);
        }

        public long? Rand2Y
        {
            get => GetProperty(() => Rand2Y);
            set => SetProperty(() => Rand2Y, value);
        }

        public long? Kor1X
        {
            get => GetProperty(() => Kor1X);
            set => SetProperty(() => Kor1X, value);
        }

        public long? Kor1Y
        {
            get => GetProperty(() => Kor1Y);
            set => SetProperty(() => Kor1Y, value);
        }

        public long? Kor2X
        {
            get => GetProperty(() => Kor2X);
            set => SetProperty(() => Kor2X, value);
        }

        public long? Kor2Y
        {
            get => GetProperty(() => Kor2Y);
            set => SetProperty(() => Kor2Y, value);
        }

        public long? Kor3X
        {
            get => GetProperty(() => Kor3X);
            set => SetProperty(() => Kor3X, value);
        }

        public long? Kor3Y
        {
            get => GetProperty(() => Kor3Y);
            set => SetProperty(() => Kor3Y, value);
        }

        public long? Kor4X
        {
            get => GetProperty(() => Kor4X);
            set => SetProperty(() => Kor4X, value);
        }

        public long? Kor4Y
        {
            get => GetProperty(() => Kor4Y);
            set => SetProperty(() => Kor4Y, value);
        }

        public string Rand1XLabel
        {
            get => GetProperty(() => Rand1XLabel);
            set => SetProperty(() => Rand1XLabel, value);
        }

        public string Rand1YLabel
        {
            get => GetProperty(() => Rand1YLabel);
            set => SetProperty(() => Rand1YLabel, value);
        }

        public string Rand2XLabel
        {
            get => GetProperty(() => Rand2XLabel);
            set => SetProperty(() => Rand2XLabel, value);
        }

        public string Rand2YLabel
        {
            get => GetProperty(() => Rand2YLabel);
            set => SetProperty(() => Rand2YLabel, value);
        }

        public string Kor1XLabel
        {
            get => GetProperty(() => Kor1XLabel);
            set => SetProperty(() => Kor1XLabel, value);
        }

        public string Kor1YLabel
        {
            get => GetProperty(() => Kor1YLabel);
            set => SetProperty(() => Kor1YLabel, value);
        }

        public string Kor2XLabel
        {
            get => GetProperty(() => Kor2XLabel);
            set => SetProperty(() => Kor2XLabel, value);
        }

        public string Kor2YLabel
        {
            get => GetProperty(() => Kor2YLabel);
            set => SetProperty(() => Kor2YLabel, value);
        }

        public string Kor3XLabel
        {
            get => GetProperty(() => Kor3XLabel);
            set => SetProperty(() => Kor3XLabel, value);
        }

        public string Kor3YLabel
        {
            get => GetProperty(() => Kor3YLabel);
            set => SetProperty(() => Kor3YLabel, value);
        }

        public string Kor4XLabel
        {
            get => GetProperty(() => Kor4XLabel);
            set => SetProperty(() => Kor4XLabel, value);
        }

        public string Kor4YLabel
        {
            get => GetProperty(() => Kor4YLabel);
            set => SetProperty(() => Kor4YLabel, value);
        }

        #endregion

        #region Update Cordinates

        private void UpdatePos(ButtenControlClickType clickType, Expression<Func<long?>> x, Expression<Func<long?>> y)
        {
            switch (clickType)
            {
                case ButtenControlClickType.Top:
                    SetProperty(y, GetProperty(y) - 1);
                    break;
                case ButtenControlClickType.TopLeft:
                    SetProperty(y, GetProperty(y) - 1);
                    SetProperty(x, GetProperty(x) - 1);
                    break;
                case ButtenControlClickType.TopRight:
                    SetProperty(y, GetProperty(y) - 1);
                    SetProperty(x, GetProperty(x) + 1);
                    break;
                case ButtenControlClickType.Left:
                    SetProperty(x, GetProperty(x) - 1);
                    break;
                case ButtenControlClickType.Right:
                    SetProperty(x, GetProperty(x) + 1);
                    break;
                case ButtenControlClickType.Bottom:
                    SetProperty(y, GetProperty(y) + 1);
                    break;
                case ButtenControlClickType.BottomLeft:
                    SetProperty(y, GetProperty(y) + 1);
                    SetProperty(x, GetProperty(x) - 1);
                    break;
                case ButtenControlClickType.BottumRight:
                    SetProperty(y, GetProperty(y) + 1);
                    SetProperty(x, GetProperty(x) + 1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(clickType), clickType, null);
            }
        }

        public void RandUpper(ButtenControlClickType clickType) => UpdatePos(clickType, () => Rand1X, () => Rand1Y);

        public void RandLower(ButtenControlClickType clickType) => UpdatePos(clickType, () => Rand2Y, () => Rand2X);

        public void KorLeftUpper(ButtenControlClickType clickType) => UpdatePos(clickType, () => Kor1X, () => Kor1Y);

        public void KorRightUpper(ButtenControlClickType clickType) => UpdatePos(clickType, () => Kor2X, () => Kor2Y);

        public void KorLeftLower(ButtenControlClickType clickType) => UpdatePos(clickType, () => Kor3X, () => Kor3Y);

        public void KorRightLower(ButtenControlClickType clickType) => UpdatePos(clickType, () => Kor4X, () => Kor4Y);

        #endregion

        private readonly Transformer _transformer = new Transformer();
        private FileManager _fileManager;
        private Folder _selectedFolder;
        private ImageFile _imageFile;
        private bool _selectionValid = true;

        public ImageSource PreviewLeft
        {
            get => GetProperty(() => PreviewLeft);
            set => SetProperty(() => PreviewLeft, value);
        }

        public ImageSource PreviewRight
        {
            get => GetProperty(() => PreviewRight);
            set => SetProperty(() => PreviewRight, value);
        }

        public ObservableCollection<Folder> Folders => _fileManager.Folders;

        public ObservableCollection<ImageFile> Files => _fileManager.Files;

        public Folder SelectedFolder
        {
            get => _selectedFolder;
            set { _selectedFolder = value;
                OnFolderChanged();
            }
        }

        public ImageFile SelectedImageFile
        {
            get => _imageFile;
            set { _imageFile = value;
                OnFileChanged();
            }
        }

        public string Log
        {
            get => GetProperty(() => Log);
            set => SetProperty(() => Log, value);
        }

        private void OnFileChanged()
        {
            RunOperation("Vorbereitung", () =>
                                                    {
                                                        _selectionValid = true;
                                                        Log = string.Empty;
                                                        ImageFile file = SelectedImageFile;
                                                        if (file == null)
                                                        {
                                                            PreviewLeft = null;
                                                            PreviewRight = null;
                                                            return;
                                                        }
                                                        FormatFileInfo(file.FileInfo);
                                                        if(!file.FileInfo.IsAccesible) return;

                                                        file.Prepare();

                                                        if (file.Error != null)
                                                        {
                                                            Log += "Fehler: " + file.Error.GetType() + Environment.NewLine;
                                                            Log += "\t" + file.Error.Message + Environment.NewLine;
                                                        }
                                                        else
                                                        {
                                                            var settings = file.PreparedFileInfo.TransformSettings;
                                                            SetTransformSettings(settings);
                                                            UpdateLabels();

                                                            BusyLabel = "Vorschau wird Generiert";
                                                            var result = _transformer.GeneratePreview(file.PreparedFileInfo.RealFile, settings);

                                                            switch (result)
                                                            {
                                                                case ExceptionResult exceptionResult:
                                                                    Log += "Fehler: " + exceptionResult.Exception.GetType() + Environment.NewLine;
                                                                    Log += "\t" + exceptionResult.Exception.Message + Environment.NewLine;
                                                                    break;
                                                                case GenericResult<bool> boolResult:
                                                                    if (boolResult.Result)
                                                                    {
                                                                        Log               += "Fertig" + Environment.NewLine;
                                                                        _selectionValid =  true;
                                                                    }
                                                                    else
                                                                        Log += "Fehler" + Environment.NewLine;
                                                                    break;
                                                            }
                                                        }

                                                        _dispatcher.Invoke(CommandManager.InvalidateRequerySuggested);
                                                    });
        }

        private void OnFolderChanged() => RunOperation("Daten Werden Gelesen", () => _fileManager.ReadFiles(SelectedFolder));

        private void TransformerOnPreviewGeneratedEvent(object sender, PreviewGeneratedEventArgs previewGeneratedEventArgs)
        {
            PreviewLeft = previewGeneratedEventArgs.PreviewLeft;
            PreviewRight = previewGeneratedEventArgs.PreviewRight;
        }

        private void FormatFileInfo(ImageFileInfo info)
        {
            if (!info.IsAccesible)
            {
                Log += "Kein Zugriff";
                return;
            }
            
            if (!info.IsCorrectResolution)
            {
                if (info.IsResolutionUnKnowen)
                    Log += "Unbekannte Auflösung" + Environment.NewLine;
                else
                    Log += "Auflösung ist nicht Richtig" + Environment.NewLine;
            }
            if (!info.IsGreyScale)
                Log += "Die Datei ist nicht in Graustufen." + Environment.NewLine;
            if (!info.IsCorrectType)
                Log += "Die Datei ist keine Tiff-Datei." + Environment.NewLine;
            if (!info.IsSingleLayer)
                Log += "Die Datei hat mehrere Ebenen." + Environment.NewLine;
        }

        [Command, UsedImplicitly]
        public void GeneratePreview()
        {
            RunOperation("Vorschau wird Generiert", () =>
                                                    {
                                                        Log          = string.Empty;
                                                        var settings = SelectedImageFile.PreparedFileInfo.TransformSettings;
                                                        FillTransformSettings(settings);
                                                        UpdateLabels();

                                                        var result = _transformer.GeneratePreview(SelectedImageFile.PreparedFileInfo.RealFile, settings);

                                                        switch (result)
                                                        {
                                                            case ExceptionResult exceptionResult:
                                                                Log += "Fehler: " + exceptionResult.Exception.GetType() + Environment.NewLine;
                                                                Log += "\t" + exceptionResult.Exception.Message + Environment.NewLine;
                                                                break;
                                                            case GenericResult<bool> boolResult:
                                                                if (boolResult.Result)
                                                                {
                                                                    Log               += "Fertig" + Environment.NewLine;
                                                                    _selectionValid =  true;
                                                                }
                                                                else
                                                                    Log += "Fehler" + Environment.NewLine;

                                                                break;
                                                        }
                                                    });
        }

        [UsedImplicitly]
        public bool CanGeneratePreview() => _selectionValid;

        [Command, UsedImplicitly]
        public void TransformFile()
        {
            RunOperation("Datei wird erstellt", () =>
                                                    {
                                                        _selectionValid = false;
                                                        Log          = string.Empty;
                                                        var settings = SelectedImageFile.PreparedFileInfo.TransformSettings;
                                                        FillTransformSettings(settings);
                                                        UpdateLabels();

                                                        var result = _transformer.Transform(SelectedImageFile.PreparedFileInfo.RealFile, GetTransformPath(SelectedImageFile), settings);

                                                        switch (result)
                                                        {
                                                            case ExceptionResult exceptionResult:
                                                                Log += "Fehler: " + exceptionResult.Exception.GetType() + Environment.NewLine;
                                                                Log += "\t" + exceptionResult.Exception.Message + Environment.NewLine;
                                                                break;
                                                            case GenericResult<bool> boolResult:
                                                                if (boolResult.Result)
                                                                {
                                                                    Log               += "Fertig" + Environment.NewLine;
                                                                    _selectionValid =  true;
                                                                }
                                                                else
                                                                    Log += "Fehler" + Environment.NewLine;

                                                                break;
                                                        }
                                                    });
        }

        [UsedImplicitly]
        public bool CanTransformFile() => _selectionValid;

        private string GetTransformPath(ImageFile file)
        {
            string targetPath = Properties.Settings.Default.TargetPath;

            if (!Directory.Exists(targetPath))
                Directory.CreateDirectory(targetPath);

            targetPath = Path.Combine(targetPath, file.Folder.Name);

            if (!Directory.Exists(targetPath))
                Directory.CreateDirectory(targetPath);

            return Path.Combine(targetPath, Path.GetFileNameWithoutExtension(file.Name) + "_out.tiff");
        }
    }
}