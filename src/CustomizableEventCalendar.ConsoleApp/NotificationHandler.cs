using System.Text;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal class NotificationHandler
    {

        private readonly NotificationService _notificationService = new();

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
            List<EventByDate> upcommingEvents = _notificationService.GetUpcomingEvents();

            StringBuilder upcommingEventsNotificationTable = new();

            upcommingEventsNotificationTable.AppendLine($"Your today's events :- \n{PrintService.GetHorizontalLine()}\n");

            upcommingEventsNotificationTable.AppendLine(PrintService.GenerateTableForNotification(GetUpCommingEventInformation(upcommingEvents)));

            if (upcommingEvents.Count == 0) return "";

            return upcommingEventsNotificationTable.ToString();
        }

        private static List<List<string>> GetUpCommingEventInformation(List<EventByDate> upcommingEvents)
        {
            return upcommingEvents.InsertInto2DList(["Event", "Description", "Date", "Start Time", "End Time"],
                                                    [
                                                          eventByDate => eventByDate.Event.Title,
                                                          eventByDate => eventByDate.Event.Description,
                                                          eventByDate => eventByDate.Date,
                                                          eventByDate => DateTimeManager.ConvertTo12HourFormat(eventByDate.Event.EventStartHour),
                                                          eventByDate => DateTimeManager.ConvertTo12HourFormat(eventByDate.Event.EventEndHour)
                                                    ]);
        }

        private string NotificationsForProposedEvents()
        {

            List<EventByDate> proposedEventCollaborators = _notificationService.GetProposedEvents();

            if (proposedEventCollaborators.Count == 0) return "";

            StringBuilder proposedEventsNotificationTable = new();

            proposedEventsNotificationTable.AppendLine($"Proposed Events : \n {PrintService.GetHorizontalLine()} \n");

            proposedEventsNotificationTable.AppendLine(PrintService.GenerateTableForNotification(GetProposedEventInformation(proposedEventCollaborators)));

            return proposedEventsNotificationTable.ToString();
        }

        private static List<List<string>> GetProposedEventInformation(List<EventByDate> proposedEventCollaborators)
        {
            return proposedEventCollaborators.InsertInto2DList(["Event Proposed by", "Date", "Start Time", "End Time", "Event", "Description"],
                                                               [
                                                                 eventByDate => GetUserName(eventByDate.Event.UserId),
                                                                 eventByDate => eventByDate.Date,
                                                                 eventByDate => DateTimeManager.ConvertTo12HourFormat(eventByDate.Event.EventStartHour),
                                                                 eventByDate => DateTimeManager.ConvertTo12HourFormat(eventByDate.Event.EventEndHour),
                                                                 eventByDate => eventByDate.Event.Title,
                                                                 eventByDate => eventByDate.Event.Description,
                                                                ]);
        }

        private static string GetUserName(int userId)
        {
            UserService userService = new();

            return userService.Read(userId)?.Name ?? "-";
        }
    }
}
