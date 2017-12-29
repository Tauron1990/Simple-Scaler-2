using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DevExpress.Mvvm;

namespace Simple_Scaler_2
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Common

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

        private void RunOperation(Action action, string title)
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

            base.OnInitializeInRuntime();
        }

        public bool Checker
        {
            get => GetProperty(() => Checker);
            set => SetProperty(() => Checker, value);
        }

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
    }
}