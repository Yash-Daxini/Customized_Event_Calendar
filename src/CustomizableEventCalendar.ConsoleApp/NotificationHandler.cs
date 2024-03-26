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

            upcommingEventsNotificationTable.AppendLine(PrintService.GenerateTableForNotification(InsertUpcommingEventsInto2DList
                                                        (upcommingEvents)));

            if (upcommingEvents.Count == 0) return "";

            return upcommingEventsNotificationTable.ToString(); 
        }

        private List<List<string>> InsertUpcommingEventsInto2DList(List<EventCollaborator> upcommingEvents)
        {
            List<List<string>> upcommingEventNotificationsTableContent = [["Event", "Description", "Date", "Start Time", "End Time"]];

            foreach (EventCollaborator eventCollaborators in upcommingEvents)
            {
                Event? eventObj = events.FirstOrDefault(eventObj => eventObj.Id == eventCollaborators.EventId);

                upcommingEventNotificationsTableContent.Add([eventObj.Title, eventObj.Description,
                                                             eventCollaborators.EventDate.ToString(),
                                                             DateTimeManager.ConvertTo12HourFormat(eventObj.EventStartHour),
                                                             DateTimeManager.ConvertTo12HourFormat(eventObj.EventEndHour)]);
            }

            return upcommingEventNotificationsTableContent;
        }

        private string NotificationsForProposedEvents()
        {

            List<EventCollaborator> proposedEventCollaborators = _notificationService.GetProposedEvents();

            if (proposedEventCollaborators.Count == 0) return "";

            StringBuilder proposedEventsNotificationTable = new();

            proposedEventsNotificationTable.AppendLine($"Proposed Events : \n {PrintService.GetHorizontalLine()} \n");

            proposedEventsNotificationTable.AppendLine(PrintService.GenerateTableForNotification(InsertProposedEventsInto2DList
                                                      (proposedEventCollaborators)));

            return proposedEventsNotificationTable.ToString();
        }

        private List<List<string>> InsertProposedEventsInto2DList(List<EventCollaborator> proposedEventCollabprators)
        {
            List<List<string>> propsedEventNotificationsTableContent = [["Event Proposed by", "Date", "Start Time", "End Time",
                                                                         "Event","Description"]];

            UserService userService = new();

            foreach (var eventCollaborator in proposedEventCollabprators)
            {
                Event eventObj = _eventService.GetEventById(eventCollaborator.EventId);

                User? eventProposer = userService.Read(eventObj.UserId);

                propsedEventNotificationsTableContent.Add([eventProposer.Name,
                                                           eventObj.EventStartDate.ToString(),
                                                           DateTimeManager.ConvertTo12HourFormat(eventObj.EventStartHour),
                                                           DateTimeManager.ConvertTo12HourFormat(eventObj.EventEndHour),
                                                           eventObj.Title, eventObj.Description
                                                           ]
                                                         );
            }

            return propsedEventNotificationsTableContent;
        }
    }
}
