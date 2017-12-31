using System;
using System.Windows;

namespace Simple_Scaler_2.SettingsWindow
{
    /// <summary>
    /// Interaktionslogik für SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private bool _isSaved;
        private SettingsWindowViewModel _viewModel;

        public SettingsWindow()
        {
            InitializeComponent();
            _viewModel = (SettingsWindowViewModel) DataContext;
            _viewModel.Window = this;
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            _isSaved = true;
            _viewModel.Save();
            DialogResult = true;
        }

        private void SettingsWindow_OnClosed(object sender, EventArgs e)
        {
            if(_isSaved) return;

            _viewModel.Reload();
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            _isSaved = false;
            DialogResult = false;
        }
    }
}
