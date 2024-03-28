using System.Text;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal class NotificationHandler
    {
        private readonly EventService _eventService = new();

        private readonly NotificationService _notificationService = new();

        private readonly List<Event> events;

        public NotificationHandler()
        {
            events = _eventService.GetAllEvents();
        }

        public void PrintNotifications()
        {
            StringBuilder notification = new();

            notification.AppendLine(new string('═', Console.WindowWidth) + "\n");
            notification.AppendLine(new string(' ', (Console.WindowWidth - 20) / 2) + "Notifications\n");

            string upcommingEvents = NotificationForUpcommingEvents();
            string proposedEvents = NotificationsForProposedEvents();

            notification.AppendLine(upcommingEvents);
            notification.AppendLine(proposedEvents);

            notification.AppendLine(new string('═', Console.WindowWidth));

            Console.WriteLine(notification.ToString());
        }

        private string NotificationForUpcommingEvents()
        {
            List<EventCollaborator> upcommingEvents = _notificationService.GetUpcomingEvents();

            StringBuilder upcommingEventsNotificationTable = new();

            upcommingEventsNotificationTable.AppendLine($"Your today's events :- \n{PrintService.GetHorizontalLine()}\n");

            upcommingEventsNotificationTable.AppendLine(PrintService.GenerateTableForNotification(upcommingEvents.InsertInto2DList(["Event", "Description", "Date", "Start Time", "End Time"],
                                                     [
                                                          eventCollaborator => GetEventTitle(eventCollaborator) ,
                                                          eventCollaborator => GetEventDescription(eventCollaborator),
                                                          eventCollaborator => eventCollaborator.EventDate,
                                                          eventCollaborator => GetEventStartHour(eventCollaborator),
                                                          eventCollaborator => GetEventEndHour(eventCollaborator),
                                                     ])));

            if (upcommingEvents.Count == 0) return "";

            return upcommingEventsNotificationTable.ToString();
        }

        private string GetEventTitle(EventCollaborator eventCollaborator) => GetEventFromId(eventCollaborator.EventId)?.Title ?? "-";
        private string GetEventDescription(EventCollaborator eventCollaborator) => GetEventFromId(eventCollaborator.EventId)?.Description ?? "-";
        private string GetEventStartHour(EventCollaborator eventCollaborator) => GetEventFromId(eventCollaborator.EventId)?.EventStartHour.ToString() ?? "-";
        private string GetEventEndHour(EventCollaborator eventCollaborator) => GetEventFromId(eventCollaborator.EventId)?.EventEndHour.ToString() ?? "-";

        private Event? GetEventFromId(int eventId)
        {
            return events.Find(eventObj => eventObj.Id == eventId);
        }

        private string NotificationsForProposedEvents()
        {

            List<EventCollaborator> proposedEventCollaborators = _notificationService.GetProposedEvents();

            if (proposedEventCollaborators.Count == 0) return "";

            StringBuilder proposedEventsNotificationTable = new();

            proposedEventsNotificationTable.AppendLine($"Proposed Events : \n {PrintService.GetHorizontalLine()} \n");

            proposedEventsNotificationTable.AppendLine(PrintService.GenerateTableForNotification(proposedEventCollaborators.InsertInto2DList(["Event Proposed by", "Date", "Start Time", "End Time", "Event", "Description"],
                                                     [
                                                          eventCollaborator => GetUserName(eventCollaborator.UserId),
                                                          eventCollaborator => eventCollaborator.EventDate,
                                                          eventCollaborator => GetEventStartHour(eventCollaborator),
                                                          eventCollaborator => GetEventEndHour(eventCollaborator),
                                                          eventCollaborator => GetEventTitle(eventCollaborator) ,
                                                          eventCollaborator => GetEventDescription(eventCollaborator),
                                                     ])));

            return proposedEventsNotificationTable.ToString();
        }

        private string GetUserName(int userId)
        {
            UserService userService = new();

            return userService.Read(userId)?.Name ?? "-";
        }
    }
}
