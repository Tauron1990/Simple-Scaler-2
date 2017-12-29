using System;
using System.Windows;
using System.Windows.Controls;

namespace Simple_Scaler_2
{
    /// <inheritdoc cref="UserControl" />
    /// <summary>
    ///     Interaktionslogik für ButtonControl.xaml
    /// </summary>
    public sealed partial class ButtonControl
    {
        public ButtonControl()
        {
            InitializeComponent();
        }

        public event EventHandler<ButtonControlClickEventArgs> ButtonClickEvent;

        private void OnButtonClickEvent(ButtenControlClickType type)
        {
            ButtonClickEvent?.Invoke(this, new ButtonControlClickEventArgs(type));
        }

        private void TopLeftClick(object sender, RoutedEventArgs e)
        {
            OnButtonClickEvent(ButtenControlClickType.TopLeft);
        }

        private void TopClick(object sender, RoutedEventArgs e)
        {
            OnButtonClickEvent(ButtenControlClickType.Top);
        }

        private void TopRightClick(object sender, RoutedEventArgs e)
        {
            OnButtonClickEvent(ButtenControlClickType.TopRight);
        }

        private void LeftClick(object sender, RoutedEventArgs e)
        {
            OnButtonClickEvent(ButtenControlClickType.Left);
        }

        private void RightClick(object sender, RoutedEventArgs e)
        {
            OnButtonClickEvent(ButtenControlClickType.Right);
        }

        private void BottomClick(object sender, RoutedEventArgs e)
        {
            OnButtonClickEvent(ButtenControlClickType.Bottom);
        }

        private void BottomLeft(object sender, RoutedEventArgs e)
        {
            OnButtonClickEvent(ButtenControlClickType.BottomLeft);
        }

        private void BottumRight(object sender, RoutedEventArgs e)
        {
            OnButtonClickEvent(ButtenControlClickType.BottumRight);
        }
    }
}