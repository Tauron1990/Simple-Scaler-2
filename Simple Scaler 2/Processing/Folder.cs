using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Simple_Scaler_2.Processing
{
    public class Folder : INotifyPropertyChanged
    {
        private string _name;
        private string _path;

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged();}
        }

        public string Path
        {
            get => _path;
            set { _path = value; OnPropertyChanged();}
        }

        public Folder(string name, string path)
        {
            Name = name;
            Path = path;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}