using System.Windows;

namespace Simple_Scaler_2
{
    /// <summary>
    ///     Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = (MainWindowViewModel) DataContext;
        }

        private void RandUpper(object sender, ButtonControlClickEventArgs e) => _viewModel.RandUpper(e.ClickType);

        private void RandLower(object sender, ButtonControlClickEventArgs e) => _viewModel.RandLower(e.ClickType);

        private void KorLeftUpper(object sender, ButtonControlClickEventArgs e) => _viewModel.KorLeftUpper(e.ClickType);

        private void KorRightUpper(object sender, ButtonControlClickEventArgs e) => _viewModel.KorRightUpper(e.ClickType);

        private void KorLeftLower(object sender, ButtonControlClickEventArgs e) => _viewModel.KorLeftLower(e.ClickType);

        private void KorRightLower(object sender, ButtonControlClickEventArgs e) => _viewModel.KorRightLower(e.ClickType);
    }
}