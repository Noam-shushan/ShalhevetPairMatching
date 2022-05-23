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

namespace GuiWpf.Controlers
{
    

    /// <summary>
    /// Interaction logic for SearchTextBox.xaml
    /// </summary>
    public partial class SearchTextBox : UserControl
    {
        public ICommand SearchCommand
        {
            get { return (ICommand)GetValue(SearchCommandProperty); }
            set { SetValue(SearchCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SearchCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SearchCommandProperty =
            DependencyProperty.Register("SearchCommand", typeof(ICommand), typeof(SearchTextBox));



        public string SearchWord
        {
            get { return (string)GetValue(SearchWordProperty); }
            set { SetValue(SearchWordProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SearchWord.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SearchWordProperty =
            DependencyProperty.Register("SearchWord", typeof(string), typeof(SearchTextBox),
                new FrameworkPropertyMetadata("",
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    new PropertyChangedCallback(ModifySearchText),
                    new CoerceValueCallback(ValidSearchText))
                );

        private static object ValidSearchText(DependencyObject d, object baseValue)
        {
            return baseValue;
        }

        private static void ModifySearchText(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var current = d as SearchTextBox;
            if(current is not null)
            {
                current.TextSearchWord.Text = e.NewValue?.ToString();
            }
        }

        public SearchTextBox()
        {
            InitializeComponent();
        }
    }
}
