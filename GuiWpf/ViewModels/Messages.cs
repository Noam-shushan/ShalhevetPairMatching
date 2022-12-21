using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GuiWpf.ViewModels
{
    public static class Messages
    {
        public static void MessageBoxSimple(string message)
        {
            MessageBox.Show(message, "הודעה", MessageBoxButton.OK,
                MessageBoxImage.Information, MessageBoxResult.None,
                MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
        }

        public static void MessageBoxError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK,
                MessageBoxImage.Error, MessageBoxResult.None,
                MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
        }

        public static void MessageBoxWarning(string message)
        {
            MessageBox.Show(message, "Warning", MessageBoxButton.OK,
                MessageBoxImage.Warning, MessageBoxResult.None,
                MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
        }

        public static bool MessageBoxConfirmation(string message)
        {
            return MessageBox.Show(message, "Confirmation", MessageBoxButton.YesNo,
                        MessageBoxImage.Question, MessageBoxResult.OK,
                        MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading)
                            == MessageBoxResult.Yes;
        }
    }
}
