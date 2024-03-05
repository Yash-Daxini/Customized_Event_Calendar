using System.Data.SqlClient;
using System.Text;
using System.Transactions;
using CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class EventService
    {
        private readonly EventRepository _eventRepository = new();

        private readonly RecurrencePatternRepository _recurrencePatternRepository = new();

        public int InsertEventWithRecurrencePattern(Event eventObj, RecurrencePatternCustom recurrencePattern)
        {
            int eventId = 0;

            OverlappingEventService overlappingEventService = new();

            if (overlappingEventService.IsOverlappingEvent(recurrencePattern))
            {
                Console.WriteLine("This event overlaps with other event. Please enter valid event");
                return 0;
            }

            using (TransactionScope transactionScope = new())
            {
                try
                {
                    int recurrenceId = _recurrencePatternRepository.Insert(recurrencePattern);

                    eventObj.RecurrenceId = recurrenceId;

                    eventId = _eventRepository.Insert(eventObj);

                    eventObj.Id = eventId;

                    recurrencePattern.Id = recurrenceId;

                    EventCollaboratorsService eventCollaboratorsService = new();

                    int eventCollaboratorId = eventCollaboratorsService.InsertEventCollaborators(new EventCollaborators
                                                                                    (GlobalData.user.Id, eventObj.Id));

                    if (!eventObj.IsProposed)
                    {
                        RecurrenceEngine engine = new();

                        engine.AddEventToScheduler(eventObj, eventCollaboratorId);
                    }
                    transactionScope.Complete();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }

            return eventId;
        }

        public List<Event> GetAllEvents()
        {
            List<Event> listOfEvents = [];

            try
            {
                listOfEvents = [.. _eventRepository.GetAll<Event>(data => new Event(data))];
            }
            catch (Exception ex)
            {
                Console.WriteLine("Some error occurred! " + ex.Message);
            }

            return listOfEvents;
        }

        public Event GetEventsById(int eventId)
        {
            Event listOfEvents = new();

            try
            {
                listOfEvents = _eventRepository.GetById<Event>(data => new Event(data), eventId);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Some error occurred ! " + ex.Message);
            }

            return listOfEvents;
        }

        public void DeleteEventWithRecurrencePattern(int eventId)
        {
            using (TransactionScope transactionScope = new())
            {
                try
                {

                    EventCollaboratorsService eventCollaboratorsService = new();

                    ScheduleEventService scheduleEventService = new();

                    Event? eventObj = _eventRepository.GetById(data => new Event(data), eventId);

                    int recurrenceId = eventObj == null ? 0 : Convert.ToInt32(eventObj.RecurrenceId);

                    scheduleEventService.DeleteByEventId(eventId);

                    eventCollaboratorsService.DeletEventCollaboratorsByEventId(eventId);

                    _eventRepository.Delete<Event>(eventId);

                    _recurrencePatternRepository.Delete<RecurrencePatternCustom>(recurrenceId);

                    transactionScope.Complete();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }
        }

        public void UpdateEventWithRecurrencePattern(Event eventObj, RecurrencePatternCustom recurrencePattern, int eventId, int recurrenceId)
        {
            using (TransactionScope transactionScope = new())
            {
                try
                {
                    _recurrencePatternRepository.Update(recurrencePattern, recurrenceId);

                    _eventRepository.Update(eventObj, eventId);

                    ScheduleEventService scheduleEventService = new();

                    scheduleEventService.DeleteByEventId(eventId);

                    EventCollaboratorsService eventCollaboratorsService = new();

                    List<EventCollaborators> eventCollaborators = eventCollaboratorsService.GetAllEventCollaborators();

                    eventCollaborators = eventCollaborators.Where(eventCollaborator => eventCollaborator.EventId ==
                                                                  eventId)
                                                           .ToList();

                    RecurrenceEngine engine = new();

                    foreach (var eventCollaborator in eventCollaborators)
                    {
                        engine.AddEventToScheduler(eventObj, eventCollaborator.Id);
                    }

                    transactionScope.Complete();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }
        }

        public string GenerateEventTable()
        {
            List<Event> events = GetAllEvents().Where(eventObj => eventObj.UserId == GlobalData.user.Id)
                                               .ToList();

            List<List<string>> outputRows = [["Event NO.", "Title", "Description", "Location", "TimeBlock"]];

            foreach (var eventObj in events)
            {
                outputRows.Add([eventObj.Id.ToString(), eventObj.Title, eventObj.Description, eventObj.Location,
                                eventObj.TimeBlock]);
            }

            string eventTable = PrintHandler.GiveTable(outputRows);

            if (events.Count > 0)
            {
                return eventTable;
            }

            return "";
        }

        public void ConvertProposedEventToScheduleEvent(int eventId)
        {
            try
            {
                _eventRepository.ConvertProposedEventToScheduleEvent(eventId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }
    }
}