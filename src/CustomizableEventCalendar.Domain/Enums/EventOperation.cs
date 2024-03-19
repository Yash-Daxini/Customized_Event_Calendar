using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Enums
{
    internal enum EventOperation
    {
        Add = 1,
        Display = 2,
        Delete = 3,
        Update = 4,
        View = 5,
        ShareCalendar = 6,
        ViewSharedCalendar = 7,
        SharedEventCollaboration = 8,
        EventWithMultipleInvitees = 9,
        GiveResponseToProposedEvent = 10,
        Back = 0,
    }
}
