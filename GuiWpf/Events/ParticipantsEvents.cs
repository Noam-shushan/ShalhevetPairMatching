using PairMatching.Models;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuiWpf.Events
{
    public class GetEmailAddressToParticipaintsEvent : PubSubEvent<IEnumerable<EmailAddress>> { }

    public class DeleteNoteFromParticipiantEvent : PubSubEvent<(string, Note)> { }

    public class NewNoteForParticipaintEvent : PubSubEvent<(string, Note)> { }

    public class AddParticipantEvent : PubSubEvent<Participant> { }

    public class ManageNotesForParticipiantEvent : PubSubEvent<Participant> { }

    public class OnParticipaintSelected : PubSubEvent<Participant> { }

    public class ShowFullParticipaintEvent : PubSubEvent<bool> { }
}
