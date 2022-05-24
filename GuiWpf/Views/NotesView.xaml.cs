using PairMatching.Models;
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

namespace GuiWpf.Views
{
    /// <summary>
    /// Interaction logic for NotesView.xaml
    /// </summary>
    public partial class NotesView : UserControl
    {
        public List<Note> Notes
        {
            get { return (List<Note>)GetValue(NotesProperty); }
            set { SetValue(NotesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NotesProperty =
            DependencyProperty.Register("Notes", typeof(List<Note>), typeof(NotesView));



        public ICommand AddNoteCommand
        {
            get { return (ICommand)GetValue(AddNoteCommandProperty); }
            set { SetValue(AddNoteCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AddNoteCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddNoteCommandProperty =
            DependencyProperty.Register("AddNoteCommand", typeof(ICommand), typeof(NotesView), new PropertyMetadata(null));


        public NotesView()
        {
            InitializeComponent();
        }
    }
}
