using System;
using System.Collections.Generic;
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

            HashSet<int> eventIds = events.Select(eventObj => eventObj.Id)
                                          .ToHashSet();

            List<EventCollaborator> eventCollaborators = _eventCollaboratorsService.GetAllEventCollaborators()
                                                          .Where(eventCollaborator => eventCollaborator.UserId == GlobalData.user.Id &&
                                                           eventCollaborator.ProposedStartHour == null &&
                                                           eventCollaborator.ProposedEndHour == null)
                                                          .ToList();

            notification.AppendLine(new string('═', Console.WindowWidth));

            notification.AppendLine();

            notification.AppendLine(new string(' ', (Console.WindowWidth - 20) / 2) + "Notifications\n");

            string completedEvents = GetCompletedEvents(eventCollaborators, events);
            string upcommingEvents = GetUpcomingEvents(eventCollaborators, events);
            string proposedEvents = GetProposedEvents(events);

            if (completedEvents.Length > 0) notification.AppendLine(completedEvents);
            if (upcommingEvents.Length > 0) notification.AppendLine(upcommingEvents);
            if (proposedEvents.Length > 0) notification.AppendLine(proposedEvents);

            notification.AppendLine(new string('═', Console.WindowWidth));

            return notification.ToString();
        }

        public static string GetCompletedEvents(List<EventCollaborator> eventCollaborators, List<Event> events)
        {
            StringBuilder completedEvents = new();

            DateTime todayDate = DateTime.Now;

            List<EventCollaborator> missedEvents = eventCollaborators.Where(scheduleEvent =>
                                                                             scheduleEvent.EventDate.Date < todayDate.Date)
                                                                      .ToList();

            if (missedEvents.Count == 0) return "";

            completedEvents.AppendLine($"Completed events :- ");
            completedEvents.AppendLine($"{PrintHandler.PrintHorizontalLine()}");

            List<List<string>> completedEventNotificationsTableContent = [["Event", "Description", "Date"]];

            foreach (EventCollaborator eventCollaborator in missedEvents)
            {

                Event? eventObj = events.FirstOrDefault(eventObj => eventObj.Id == eventCollaborator.EventId);

                completedEventNotificationsTableContent.Add([eventObj.Title,eventObj.Description,
                                                             eventCollaborator.EventDate.ToString() ]);
            }

            completedEvents.AppendLine(PrintHandler.GiveTableForNotification(completedEventNotificationsTableContent));

            return completedEvents.ToString();
        }

        public static string GetUpcomingEvents(List<EventCollaborator> scheduleEvents, List<Event> events)
        {
            StringBuilder upcommingEvents = new();

            DateTime todayDate = DateTime.Now;

            List<EventCollaborator> upcommingEventsList = scheduleEvents.Where(eventCollaborators =>
                                                                            eventCollaborators.EventDate.Date == todayDate.Date)
                                                                         .ToList();

            if (upcommingEventsList.Count == 0) return "";

            upcommingEvents.AppendLine("Your today's events :- ");
            upcommingEvents.AppendLine($"{PrintHandler.PrintHorizontalLine()}");

            List<List<string>> upcommingEventNotificationsTableContent = [["Event", "Description", "Date", "Start Time", "End Time"]];

            EventCollaboratorService eventCollaboratorsService = new();

            foreach (EventCollaborator eventCollaborators in upcommingEventsList)
            {
                Event? eventObj = events.FirstOrDefault(eventObj => eventObj.Id == eventCollaborators.EventId);

                upcommingEventNotificationsTableContent.Add([eventObj.Title,eventObj.Description,eventCollaborators.EventDate.ToString(),
                                                             DateTimeManager.ConvertTo12HourFormat(eventObj.EventStartHour),
                                                             DateTimeManager.ConvertTo12HourFormat(eventObj.EventEndHour)]);
            }

            upcommingEvents.AppendLine(PrintHandler.GiveTableForNotification(upcommingEventNotificationsTableContent));

            return upcommingEvents.ToString();
        }

        public string GetProposedEvents(List<Event> events)
        {
            StringBuilder proposedEvents = new();

            HashSet<int> proposedEventIds = events.Where(eventObj => eventObj.IsProposed)
                                                  .Select(eventObj => eventObj.Id)
                                                  .ToHashSet();

            List<EventCollaborator> eventCollaborators = _eventCollaboratorsService.GetAllEventCollaborators();

            eventCollaborators = eventCollaborators.Where(eventCollaborator => proposedEventIds
                                                          .Contains(eventCollaborator.EventId) &&
                                                           eventCollaborator.UserId == GlobalData.user.Id)
                                                   .ToList();


            if (events.Count == 0 || eventCollaborators.Count == 0) return "";

            proposedEvents.AppendLine("Proposed Events");
            proposedEvents.AppendLine($"{PrintHandler.PrintHorizontalLine()}");

            List<List<string>> propsedEventNotificationsTableContent = [[ "Event Proposed by","Date","Start Time","End Time","Event",
                                                                           "Description"]];

            UserService userService = new();
            EventService eventService = new();


            foreach (var eventCollaborator in eventCollaborators)
            {
                Event eventObj = eventService.GetEventsById(eventCollaborator.EventId);

                User? eventProposer = userService.Read(eventObj.UserId);

                propsedEventNotificationsTableContent.Add([ eventProposer.Name,
                                                                             eventObj.EventStartDate.ToString(),
                                                                             DateTimeManager.ConvertTo12HourFormat(eventObj.EventStartHour),
                                                                             DateTimeManager.ConvertTo12HourFormat(eventObj.EventEndHour),
                                                                             eventObj.Title,eventObj.Description
                                                                           ]
                                                         );
            }

            proposedEvents.AppendLine(PrintHandler.GiveTableForNotification(propsedEventNotificationsTableContent));

            return proposedEvents.ToString();
        }
    }
}