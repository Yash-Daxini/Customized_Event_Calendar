using System.Text;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Enums;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Models;

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
            List<EventModel> upcommingEvents = _notificationService.GetUpcomingEvents();

            StringBuilder upcommingEventsNotificationTable = new();

            upcommingEventsNotificationTable.AppendLine($"Your today's events :- \n{PrintService.GetHorizontalLine()}\n");

            upcommingEventsNotificationTable.AppendLine(PrintService.GenerateTableForNotification(GetUpCommingEventInformation(upcommingEvents)));

            if (upcommingEvents.Count == 0) return "";

            return upcommingEventsNotificationTable.ToString();
        }

        private static List<List<string>> GetUpCommingEventInformation(List<EventModel> upcommingEvents)
        {
            return upcommingEvents.InsertInto2DList(["Event", "Description", "Date", "Start Time", "End Time"],
                                                    [
                                                          eventModel => eventModel.Title,
                                                          eventModel => eventModel.Description,
                                                          eventModel => eventModel.EventDate,
                                                          eventModel => DateTimeManager.ConvertTo12HourFormat(eventModel.Duration.StartHour),
                                                          eventModel => DateTimeManager.ConvertTo12HourFormat(eventModel.Duration.EndHour)
                                                    ]);
        }

        private string NotificationsForProposedEvents()
        {

            List<EventModel> proposedEventCollaborators = _notificationService.GetProposedEvents();

            if (proposedEventCollaborators.Count == 0) return "";

            StringBuilder proposedEventsNotificationTable = new();

            proposedEventsNotificationTable.AppendLine($"Proposed Events : \n {PrintService.GetHorizontalLine()} \n");

            proposedEventsNotificationTable.AppendLine(PrintService.GenerateTableForNotification(GetProposedEventInformation(proposedEventCollaborators)));

            return proposedEventsNotificationTable.ToString();
        }

        private static List<List<string>> GetProposedEventInformation(List<EventModel> proposedEventCollaborators)
        {
            return proposedEventCollaborators.InsertInto2DList(["Event Proposed by", "Date", "Start Time", "End Time", "Event", "Description"],
                                                               [
                                                                 eventModel => GetEventProposer(eventModel.Participants),
                                                                 eventModel => eventModel.EventDate,
                                                                 eventModel => DateTimeManager.ConvertTo12HourFormat(eventModel.Duration.StartHour),
                                                                 eventModel => DateTimeManager.ConvertTo12HourFormat(eventModel.Duration.EndHour),
                                                                 eventModel => eventModel.Title,
                                                                 eventModel => eventModel.Description,
                                                                ]);
        }

        private static string GetEventProposer(List<ParticipantModel> participantModels)
        {
            return participantModels.First(participant => participant.ParticipantRole == ParticipantRole.Organizer).User.Name;
        }
    }
}
