﻿using PairMatching.Models;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuiWpf.Events
{
    public class ResiveParticipantsEvent : PubSubEvent<IEnumerable<Participant>> { }

    public class GetEmailAddressToParticipaintsEvent : PubSubEvent<IEnumerable<EmailAddress>> { }

    public class DeleteNoteFromParticipiantEvent : PubSubEvent<(string, Note)> { }

    public class NewNoteForParticipaintEvent : PubSubEvent<(string, Note)> { }

    public class AddParticipantEvent : PubSubEvent<Participant> { }

    public class ManageNotesForParticipiantEvent : PubSubEvent<Participant> { }

    public class NewParticipaintEvent : PubSubEvent<bool> { }

    public class EditParticipaintEvent : PubSubEvent<Participant> { }
    
    public class ParticipaintWesUpdate : PubSubEvent<Participant> { }

    public class ExloadeFromArciveEvent : PubSubEvent<Participant> { }
    public class SendToArciveEvent : PubSubEvent<Participant> { }


}
