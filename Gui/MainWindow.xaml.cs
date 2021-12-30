using PairMatching.DomainModel.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PairMatching.Gui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DomainsContainer _domains;
        public MainWindow(DomainsContainer domains) 
        {
            InitializeComponent();
            _domains = domains;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            //var s = await _domains.StudentsDomain.GetAllStudents();
            await _domains.Initialization();
        }
    }
}
