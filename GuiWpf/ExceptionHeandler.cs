using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PairMatching.DomainModel.Services;
using System.Windows;
using PairMatching.Loggin;

namespace GuiWpf
{
    public class ExceptionHeandler
    {
        readonly Logger _logger;

        public ExceptionHeandler(Logger logger)
        {
            _logger = logger;
        }

        public void HeandleException(Exception exception)
        {
            MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK,
            MessageBoxImage.Error, MessageBoxResult.None,
            MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);

            if (exception is not UserException)
            {
                _logger.LogError("Unknown error", exception);
            }
        }


    }
}
