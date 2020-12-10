using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace AmplifierCalculator {
    /// <summary>
    /// A <see cref="TextBox"/> used for variable value input.
    /// </summary>
    public class VarBox : TextBox {
        /// <summary>
        /// Set the dimensions from here, this makes the XAML markup cleaner.
        /// </summary>
        public VarBox() {
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
            VerticalContentAlignment = VerticalAlignment.Center;
            Width = 50;
            Height = 23;
            TextWrapping = TextWrapping.NoWrap;
        }

        /// <summary>
        /// Select all text on focus.
        /// </summary>
        protected override void OnGotFocus(RoutedEventArgs e) {
            Dispatcher.BeginInvoke(new Action(delegate {
                SelectAll();
            }), DispatcherPriority.Input);
            base.OnGotFocus(e);
        }
    }
}