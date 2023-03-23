using GuiWpf.Events;
using PairMatching.DomainModel.MatchingCalculations;
using PairMatching.DomainModel.Services;
using Prism.Events;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GuiWpf.Commands
{
    public class MatchCommand : ICommand
    {
        readonly IEventAggregator _ea;

        readonly IPairsService _pairsService;

        readonly IMatchingService _matchingService;

        readonly ExceptionHeandler _exceptionHeandler;

        public MatchCommand(IEventAggregator ea, IPairsService pairsService, IMatchingService matchingService, ExceptionHeandler exceptionHeandler)
        {
            _ea = ea;
            _pairsService = pairsService;
            _exceptionHeandler = exceptionHeandler;
            _matchingService = matchingService;
        }

        async Task Match(PairSuggestion pairSuggestion)
        {
            try
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
            catch (Exception ex)
            {
                _exceptionHeandler.HeandleException(ex);
            }
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
