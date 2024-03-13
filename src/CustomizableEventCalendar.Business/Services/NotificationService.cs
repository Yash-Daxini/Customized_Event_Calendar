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
        private readonly EventCollaboratorsService _eventCollaboratorsService = new();

        public string GenerateNotification()
        {
            StringBuilder notification = new();

            EventService eventService = new();

            List<Event> events = eventService.GetAllEvents();

            HashSet<int> eventIds = events.Select(eventObj => eventObj.Id)
                                          .ToHashSet();

            List<EventCollaborators> eventCollaborators = _eventCollaboratorsService.GetAllEventCollaborators()
                                                          .Where(eventCollaborator => eventCollaborator.UserId == GlobalData.user.Id)
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

        public string GetCompletedEvents(List<EventCollaborators> eventCollaborators, List<Event> events)
        {
            StringBuilder completedEvents = new();

            DateTime todayDate = DateTime.Now;

            List<EventCollaborators> missedEvents = eventCollaborators.Where(scheduleEvent =>
                                                                             scheduleEvent.EventDate.Date <= todayDate.Date)
                                                                      .ToList();

            if (missedEvents.Count == 0) return "";

            completedEvents.AppendLine($"Completed events :- ");
            completedEvents.AppendLine($"{PrintHandler.PrintHorizontalLine()}");

            List<List<string>> completedEventNotificationsTableContent = [["Event", "Description", "Date"]];

            foreach (EventCollaborators eventCollaborator in missedEvents)
            {

                Event? eventObj = events.FirstOrDefault(eventObj => eventObj.Id == eventCollaborator.EventId);

                completedEventNotificationsTableContent.Add([eventObj.Title,eventObj.Description,
                                                             eventCollaborator.EventDate.ToString() ]);
            }

            completedEvents.AppendLine(PrintHandler.GiveTableForNotification(completedEventNotificationsTableContent));

            return completedEvents.ToString();
        }

        public string GetUpcomingEvents(List<EventCollaborators> scheduleEvents, List<Event> events)
        {
            StringBuilder upcommingEvents = new();

            DateTime todayDate = DateTime.Now;

            List<EventCollaborators> upcommingEventsList = scheduleEvents.Where(eventCollaborators =>
                                                                            eventCollaborators.EventDate.Date == todayDate.Date)
                                                                         .ToList();

            if (upcommingEventsList.Count == 0) return "";

            upcommingEvents.AppendLine("Your today's events :- ");
            upcommingEvents.AppendLine($"{PrintHandler.PrintHorizontalLine()}");

            List<List<string>> upcommingEventNotificationsTableContent = [["Event", "Description", "Date", "Start Time", "End Time"]];

            EventCollaboratorsService eventCollaboratorsService = new();

            foreach (EventCollaborators eventCollaborators in upcommingEventsList)
            {
                Event? eventObj = events.FirstOrDefault(eventObj => eventObj.Id == eventCollaborators.EventId);

                upcommingEventNotificationsTableContent.Add([eventObj.Title,eventObj.Description,eventCollaborators.EventDate.ToString(),
                                                             eventObj.EventStartHour.ToString(),eventObj.EventEndHour.ToString()]);
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

            List<EventCollaborators> eventCollaborators = _eventCollaboratorsService.GetAllEventCollaborators();

            eventCollaborators = eventCollaborators.Where(eventCollaborator => proposedEventIds
                                                          .Contains(eventCollaborator.EventId) &&
                                                           eventCollaborator.UserId == GlobalData.user.Id)
                                                   .ToList();

            bool isUserHasProposedEvent = eventCollaborators.Exists(eventCollaborator =>
                                                             eventCollaborator.UserId == GlobalData.user.Id);


            if (events.Count == 0 || !isUserHasProposedEvent) return "";

            proposedEvents.AppendLine("Proposed Events");
            proposedEvents.AppendLine($"{PrintHandler.PrintHorizontalLine()}");

            List<List<string>> propsedEventNotificationsTableContent = [[ "Event Proposed by","Date","Start Time","End Time","Event",
                                                                           "Description"]];

            UserService userService = new();
            EventService eventService = new();


            foreach (var eventCollaborator in eventCollaborators)
            {
                Event eventObj = eventService.GetEventsById(eventCollaborator.EventId);

                if (eventObj.UserId == GlobalData.user.Id) continue;

                User? eventProposer = userService.Read(eventObj.UserId);

                propsedEventNotificationsTableContent.Add([ eventProposer.Name,
                                                                             eventObj.EventStartDate.ToString(),
                                                                             eventObj.EventStartHour.ToString(),
                                                                             eventObj.EventEndHour.ToString(),
                                                                             eventObj.Title,eventObj.Description
                                                                           ]
                                                         );
            }

            proposedEvents.AppendLine(PrintHandler.GiveTableForNotification(propsedEventNotificationsTableContent));

            return proposedEvents.ToString();
        }
    }
}