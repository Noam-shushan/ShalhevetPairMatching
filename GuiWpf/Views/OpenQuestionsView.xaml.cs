﻿using PairMatching.Models;
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
    /// Interaction logic for OpenQuestionsView.xaml
    /// </summary>
    public partial class OpenQuestionsView : UserControl
    {
        public object OpenQuestions
        {
            get { return GetValue(OpenQuestionsProperty); }
            set { SetValue(OpenQuestionsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OpenQuestions.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OpenQuestionsProperty =
            DependencyProperty.Register(nameof(OpenQuestions), typeof(object), typeof(OpenQuestionsView));

        public OpenQuestionsView()
        {
            InitializeComponent();
        }
    }
}
