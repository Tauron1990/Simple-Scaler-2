using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using JetBrains.Annotations;

namespace Simple_Scaler_2
{
    /// <summary>
    ///     Interaktionslogik für DisplayingIntergerTextBox.xaml
    /// </summary>
    public sealed partial class DisplayingIntergerTextBox : INotifyPropertyChanged
    {
        public static readonly DependencyProperty LabelTextProperty = DependencyProperty.Register(
                                                                                                  "LabelText", typeof(string), typeof(DisplayingIntergerTextBox),
                                                                                                  new FrameworkPropertyMetadata(default(long?))
                                                                                                  {
                                                                                                      DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                                                                                                      BindsTwoWayByDefault       = true
                                                                                                  });

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
                                                                                              "Value", typeof(int), typeof(DisplayingIntergerTextBox),
                                                                                              new FrameworkPropertyMetadata(default(int)) {DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, BindsTwoWayByDefault = true});

        private Orientation _orientation;

        public DisplayingIntergerTextBox()
        {
            InitializeComponent();

            DependencyPropertyDescriptor.FromProperty(VerticalAlignmentProperty, typeof(UserControl)).AddValueChanged(this, (sender, args) => ArrageElements());
            DependencyPropertyDescriptor.FromProperty(HorizontalAlignmentProperty, typeof(UserControl)).AddValueChanged(this, (sender, args) => ArrageElements());
        }

        public string LabelText
        {
            get => (string) GetValue(LabelTextProperty);
            set => SetValue(LabelTextProperty, value);
        }

        public long? Value
        {
            get => (int) GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public Orientation Orientation
        {
            get => _orientation;
            set
            {
                _orientation = value;
                OnPropertyChanged();
                ArrageElements();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ArrageElements()
        {
            switch (Orientation)
            {
                case Orientation.Horizontal:
                    switch (HorizontalAlignment)
                    {
                        case HorizontalAlignment.Left:
                            SetDock(Dock.Left);
                            break;
                        case HorizontalAlignment.Right:
                            SetDock(Dock.Right);
                            break;
                    }

                    break;
                case Orientation.Vertical:
                    switch (VerticalAlignment)
                    {
                        case VerticalAlignment.Top:
                            SetDock(Dock.Top);
                            break;
                        case VerticalAlignment.Bottom:
                            SetDock(Dock.Bottom);
                            break;
                    }

                    break;
            }
        }

        private void SetDock(Dock dock)
        {
            TextBox.SetValue(DockPanel.DockProperty, dock);
            TextBlock.SetValue(DockPanel.DockProperty, dock);

            switch (dock)
            {
                case Dock.Left:
                    TextBlock.TextAlignment = TextAlignment.Left;
                    break;
                case Dock.Right:
                    TextBlock.TextAlignment = TextAlignment.Right;
                    break;
                case Dock.Top:
                case Dock.Bottom:
                    switch (HorizontalAlignment)
                    {
                        case HorizontalAlignment.Center:
                            TextBlock.TextAlignment = TextAlignment.Center;
                            break;
                        case HorizontalAlignment.Left:
                            TextBlock.TextAlignment = TextAlignment.Left;
                            break;
                        case HorizontalAlignment.Right:
                            TextBlock.TextAlignment = TextAlignment.Right;
                            break;
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dock), dock, null);
            }
        }
    }
}