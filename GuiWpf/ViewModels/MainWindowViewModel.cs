﻿using Prism.Mvvm;
using Prism.Events;
using Prism.Commands;
using GuiWpf.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuiWpf.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        readonly IEventAggregator _ea;

        public MainWindowViewModel(IEventAggregator ea)
        {
            _ea = ea;
            _ea.GetEvent<IsSendEmailEvent>().Subscribe((val) => IsSendEmail = val);
        }

        private bool _isSendEmail;
        public bool IsSendEmail
        {
            get => _isSendEmail;
            set => SetProperty(ref _isSendEmail, value);
        }


    }
}