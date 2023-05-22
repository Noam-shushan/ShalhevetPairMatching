using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PairMatching.Models;
using PairMatching.DomainModel.MatchingCalculations;
using GuiWpf.UIModels;
using PairMatching.DomainModel.BLModels;
using GuiWpf.Views;

namespace GuiWpf.Events
{
    public class CloseDialogEvent : PubSubEvent<bool> { }

    public class OpenCloseDialogEvent : PubSubEvent<(bool, Type)> 
    {
        public void CloseAll() 
        {
            Publish((false, typeof(FullPairMatchingComparisonView)));
            Publish((false, typeof(FullParticipaintView)));

        }
    }

    public class GetNotesListEvent : PubSubEvent<IEnumerable<Note>> { }
    
    public class NewNoteForPairEvent : PubSubEvent<(string, Note)> { }

    public class DeleteNoteFromPairEvent : PubSubEvent<(string, Note)> { }

    public class ModelEnterEvent : PubSubEvent<(BaseModel, ModelType)> { }

    public class IsSendEmailEvent : PubSubEvent<bool> { }

    public class NewEmailSendEvent : PubSubEvent<EmailModel> { }

    public class RefreshAll : PubSubEvent { }


    public enum ModelType
    {
        Participant,
        Pair
    }
}
