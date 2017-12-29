using System;

namespace Simple_Scaler_2
{
    public sealed class ButtonControlClickEventArgs : EventArgs
    {
        public ButtonControlClickEventArgs(ButtenControlClickType clickType) => ClickType = clickType;

        public ButtenControlClickType ClickType { get; }
    }
}