﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PairMatching.Models;
using Prism.Mvvm;
using Prism.Commands;
using PairMatching.DomainModel.Services;

namespace GuiWpf.ViewModels
{
    public class ParticipiantsViewModel : BindableBase
    {
        readonly IParticipantService _participantService;

        public ParticipiantsViewModel(IParticipantService participantService)
        {
            _participantService = participantService;
        }

        public ObservableCollection<Student> Participiants { get; set; } = new();

        private Student _selectedParticipant = new();
        public Student SelectedParticipant
        {
            get => _selectedParticipant; 
            set 
            { 
                if(SetProperty(ref _selectedParticipant, value))
                {
                    var isc = true;
                }; 
            }
        }

        DelegateCommand _load;
        public DelegateCommand Load => _load ??= new(
            async () =>
            {
                var list = await _participantService.GetAllStudents();
                Participiants.Clear();
                Participiants.AddRange(list);
            });


    }
}