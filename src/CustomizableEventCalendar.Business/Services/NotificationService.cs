using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class NotificationService
    {
        private readonly EventCollaboratorService _eventCollaboratorsService = new();


        public string GenerateNotification()
        {
            StringBuilder notification = new();

            EventService eventService = new();

            List<Event> events = eventService.GetAllEvents();

            List<EventCollaborator> eventCollaborators = GetConsiderableEventCollaborators();

            notification.AppendLine(new string('═', Console.WindowWidth) + "\n");
            notification.AppendLine(new string(' ', (Console.WindowWidth - 20) / 2) + "Notifications\n");

            notification.AppendLine(GetAllNotifications(eventCollaborators, events));

            notification.AppendLine(new string('═', Console.WindowWidth));

            return notification.ToString();
        }

        private string GetAllNotifications(List<EventCollaborator> eventCollaborators, List<Event> events)
        {
            StringBuilder notification = new();

            string completedEvents = GetCompletedEvents(eventCollaborators, events);
            string upcommingEvents = GetUpcomingEvents(eventCollaborators, events);
            string proposedEvents = GetProposedEvents(events);

            if (completedEvents.Length > 0) notification.AppendLine(completedEvents);
            if (upcommingEvents.Length > 0) notification.AppendLine(upcommingEvents);
            if (proposedEvents.Length > 0) notification.AppendLine(proposedEvents);

            return notification.ToString();
        }

        private static bool IsEventOrganizer(EventCollaborator eventCollaborator)
        {
            return eventCollaborator.ParticipantRole.Equals("organizer");
        }
        private List<EventCollaborator> GetConsiderableEventCollaborators()
        {
            return [.. _eventCollaboratorsService.GetAllEventCollaborators()
                                                 .Where(eventCollaborator => eventCollaborator.UserId == GlobalData.GetUser().Id &&
                                                 (IsEventOrganizer(eventCollaborator)
                                                 || (!IsEventOrganizer(eventCollaborator)
                                                      && eventCollaborator.ConfirmationStatus != null
                                                      && !(eventCollaborator.ConfirmationStatus.Equals("reject")
                                                            && eventCollaborator.ProposedStartHour == null
                                                            && eventCollaborator.ProposedEndHour == null
                                                          )
                                                    )
                                                 ))];
        }

        public static string GetCompletedEvents(List<EventCollaborator> eventCollaborators, List<Event> events)
        {
            StringBuilder completedEventsNotificationTable = new();

            List<EventCollaborator> completedEvents = [..eventCollaborators.Where(scheduleEvent =>
                                                                             scheduleEvent.EventDate.Date < DateTime.Now.Date)];

            if (completedEvents.Count == 0) return "";

            completedEventsNotificationTable.AppendLine($"Completed events :- \n{PrintHandler.PrintHorizontalLine()}\n");

            completedEventsNotificationTable.AppendLine(PrintHandler.GiveTableForNotification(InsertCompletedEventsInto2DList
                                                        (completedEvents, events)));

            return completedEventsNotificationTable.ToString();
        }

        private static List<List<string>> InsertCompletedEventsInto2DList(List<EventCollaborator> completedEvents, List<Event> events)
        {
            List<List<string>> completedEventNotificationsTableContent = [["Event", "Description", "Date"]];

            foreach (EventCollaborator eventCollaborator in completedEvents)
            {

                Event? eventObj = events.FirstOrDefault(eventObj => eventObj.Id == eventCollaborator.EventId);

                completedEventNotificationsTableContent.Add([eventObj.Title, eventObj.Description,
                                                             eventCollaborator.EventDate.ToString()]);
            }

            return completedEventNotificationsTableContent;
        }

        public static string GetUpcomingEvents(List<EventCollaborator> scheduleEvents, List<Event> events)
        {
            StringBuilder upcommingEventsNotificationTable = new();

            List<EventCollaborator> upcommingEvents = [..scheduleEvents.Where(eventCollaborators =>
                                                                            eventCollaborators.EventDate.Date ==
                                                                            DateTime.Now.Date)];

            if (upcommingEvents.Count == 0) return "";

            upcommingEventsNotificationTable.AppendLine($"Your today's events :- \n{PrintHandler.PrintHorizontalLine()}\n");

            upcommingEventsNotificationTable.AppendLine(PrintHandler.GiveTableForNotification(InsertUpcommingEventsInto2DList
                                                        (upcommingEvents, events)));

            return upcommingEventsNotificationTable.ToString();
        }

        private static List<List<string>> InsertUpcommingEventsInto2DList(List<EventCollaborator> upcommingEvents, List<Event> events)
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

        public string GetProposedEvents(List<Event> events)
        {
            StringBuilder proposedEventsNotificationTable = new();

            HashSet<int> proposedEventIds = events.Where(eventObj => eventObj.IsProposed
                                                         && eventObj.EventStartDate >= DateOnly.FromDateTime(DateTime.Now))
                                                  .Select(eventObj => eventObj.Id)
                                                  .ToHashSet();

            List<EventCollaborator> proposedEventCollabprators = _eventCollaboratorsService.GetAllEventCollaborators();

            proposedEventCollabprators = [..proposedEventCollabprators.Where(eventCollaborator => proposedEventIds
                                                          .Contains(eventCollaborator.EventId) &&
                                                           eventCollaborator.UserId == GlobalData.GetUser().Id)];


            if (events.Count == 0 || proposedEventCollabprators.Count == 0) return "";

            proposedEventsNotificationTable.AppendLine($"Proposed Events : \n {PrintHandler.PrintHorizontalLine()} \n");

            proposedEventsNotificationTable.AppendLine(PrintHandler.GiveTableForNotification(InsertProposedEventsInto2DList
                                                      (proposedEventCollabprators)));

            return proposedEventsNotificationTable.ToString();
        }

        private static List<List<string>> InsertProposedEventsInto2DList(List<EventCollaborator> proposedEventCollabprators)
        {
            List<List<string>> propsedEventNotificationsTableContent = [["Event Proposed by", "Date", "Start Time", "End Time",
                                                                         "Event","Description"]];

            UserService userService = new();
            EventService eventService = new();

            foreach (var eventCollaborator in proposedEventCollabprators)
            {
                Event eventObj = eventService.GetEventsById(eventCollaborator.EventId);

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