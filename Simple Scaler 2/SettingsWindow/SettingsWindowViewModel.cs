using System;
using System.Windows;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using JetBrains.Annotations;
using Ookii.Dialogs.Wpf;
using Simple_Scaler_2.Properties;

namespace Simple_Scaler_2.SettingsWindow
{
    public sealed class SettingsWindowViewModel : ViewModelBase
    {
        private readonly Settings _settings = Settings.Default;

        public Window Window { get; set; }

        public string SourcePath
        {
            get => _settings.SourcePath;
            set
            {
                _settings.SourcePath = value;
                RaisePropertyChanged(nameof(SourcePath));
            }
        }

        public string TargetPath
        {
            get => _settings.TargetPath;
            set
            {
                _settings.TargetPath = value;
                RaisePropertyChanged(nameof(TargetPath));
            }
        }

        public double Resolution
        {
            get => _settings.Resolution;
            set => _settings.Resolution = value;
        }

        public double PrevResolution
        {
            get => _settings.PrevResolution;
            set => _settings.PrevResolution = value;
        }
        
        public void Save() => _settings.Save();

        public void Reload() => _settings.Reload();

        private string ShowFolderBrowserDialog(string select)
        {
            var dialog = new VistaFolderBrowserDialog {Description = "Bitte Pfad auswählen", RootFolder = Environment.SpecialFolder.MyDocuments, SelectedPath = select, ShowNewFolderButton = true};
            if (dialog.ShowDialog(Window) == true) return dialog.SelectedPath;

            return select;
        }

        [Command, UsedImplicitly]
        public void GetTargetPath() => TargetPath = ShowFolderBrowserDialog(TargetPath);

        [Command, UsedImplicitly]
        public void GetSourcePath() => SourcePath = ShowFolderBrowserDialog(SourcePath);
    }
}