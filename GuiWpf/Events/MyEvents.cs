using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PairMatching.Models;

namespace GuiWpf.Events
{
    public class CloseDialogEvent : PubSubEvent<bool> { }

    public class AddParticipantEvent : PubSubEvent<Participant> { }

    public class GetNotesListEvent : PubSubEvent<IEnumerable<Note>> { }

    public class NewNoteForParticipaintEvent : PubSubEvent<Note> { }
    
    public class NewNoteForPairEvent : PubSubEvent<Note> { }

    public class DeleteNoteFromParticipiantEvent : PubSubEvent<Note> { }
    
    public class DeleteNoteFromPairEvent : PubSubEvent<Note> { }

    public class ModelEnterEvent : PubSubEvent<ModelType> { }

    public class ParticipantEnterEvent : PubSubEvent<Participant> { }

    public class GetEmailAddressToParticipaintsEvent : PubSubEvent<IEnumerable<EmailAddress>> { }

    public class IsSendEmailEvent : PubSubEvent<bool> { }

    public enum ModelType
    {
        Participant,
        Pair
    }
}
