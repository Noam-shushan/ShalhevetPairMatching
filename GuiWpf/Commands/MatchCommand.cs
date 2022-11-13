using GuiWpf.Events;
using PairMatching.DomainModel.MatchingCalculations;
using PairMatching.DomainModel.Services;
using PairMatching.Models;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GuiWpf.Commands
{
    public class MatchCommand : ICommand
    {
        IEventAggregator _ea;

        IPairsService _pairsService;

        IMatchingService _matchingService;

        public MatchCommand(IEventAggregator ea, IPairsService pairsService, IMatchingService matchingService)
        {
            _ea = ea;
            _pairsService = pairsService;
            _matchingService = matchingService;
        }

        async Task Match(PairSuggestion pairSuggestion)
        {
            _ea.GetEvent<CloseDialogEvent>().Publish(false);
            var newPair = await _pairsService.AddNewPair(pairSuggestion);
            await _matchingService.Refresh();
            _ea.GetEvent<RefreshMatchingEvent>().Publish();


            _ea.GetEvent<NewMatchEvent>()
                    .Publish(new() 
                    {
                        Pair = newPair,
                        PairSuggestion = pairSuggestion
                    });
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public async void Execute(object? parameter)
        {
            if (parameter is PairSuggestion pair)
            {
                await Match(pair);
            }
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
