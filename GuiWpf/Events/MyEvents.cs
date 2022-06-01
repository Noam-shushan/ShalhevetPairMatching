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

    public class NewNoteEvent : PubSubEvent<Note> { }

    public class DeleteNoteEvent : PubSubEvent<Note> { }


}
