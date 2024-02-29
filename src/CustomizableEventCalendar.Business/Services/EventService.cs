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
        private readonly EventRepository _eventRepository = new EventRepository();
        private readonly RecurrencePatternRepository _recurrencePatternRepository = new RecurrencePatternRepository();

        public int Create(Event eventObj, RecurrencePatternCustom recurrencePattern)
        {
            int Id = 0;

            OverlappingEventService overlappingEventService = new OverlappingEventService();

            if (overlappingEventService.IsOverlappingEvent(recurrencePattern))
            {
                Console.WriteLine("This event overlaps with other event. Please enter valid event");
                return 0;
            }

            using (TransactionScope transactionScope = new())
            {
                try
                {
                    int recurrenceId = _recurrencePatternRepository.Create(recurrencePattern);

                    eventObj.RecurrenceId = recurrenceId;

                    Id = _eventRepository.Create<Event>(eventObj);

                    eventObj.Id = Id;

                    recurrencePattern.Id = recurrenceId;

                    EventCollaboratorsService eventCollaboratorsService = new EventCollaboratorsService();

                    int eventCollaboratorId = eventCollaboratorsService.Create(new EventCollaborators(GlobalData.user.Id, eventObj.Id));

                    if (!eventObj.IsProposed)
                    {
                        RecurrenceEngine engine = new RecurrenceEngine();

                        engine.AddEventToScheduler(eventObj, eventCollaboratorId);
                    }
                    transactionScope.Complete();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }

            return Id;
        }
        public List<Event> Read()
        {
            List<Event> listOfEvents = new List<Event>();

            try
            {
                listOfEvents = _eventRepository.Read<Event>(data => new Event(data)).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Some error occurred! " + ex.Message);
            }

            return listOfEvents;
        }
        public Event Read(int Id)
        {
            Event listOfEvents = new Event();

            try
            {
                listOfEvents = _eventRepository.Read<Event>(data => new Event(data), Id);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Some error occurred ! " + ex.Message);
            }

            return listOfEvents;
        }
        public void Delete(int eventId)
        {
            using (TransactionScope transactionScope = new())
            {
                try
                {

                    EventCollaboratorsService eventCollaboratorsService = new EventCollaboratorsService();

                    ScheduleEventService scheduleEventService = new ScheduleEventService();

                    Event? eventObj = _eventRepository.Read<Event>(data => new Event(data), eventId);

                    int recurrenceId = eventObj == null ? 0 : Convert.ToInt32(eventObj.RecurrenceId);

                    scheduleEventService.DeleteByEventId(eventId, GlobalData.user.Id);

                    eventCollaboratorsService.DeleteByEventId(eventId);

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
        public void Update(Event eventObj, RecurrencePatternCustom recurrencePattern, int eventId, int recurrenceId)
        {
            using (TransactionScope transactionScope = new())
            {
                try
                {
                    _recurrencePatternRepository.Update(recurrencePattern, recurrenceId);

                    _eventRepository.Update<Event>(eventObj, eventId);

                    ScheduleEventService scheduleEventService = new ScheduleEventService();

                    scheduleEventService.DeleteByEventId(eventId, GlobalData.user.Id);

                    EventCollaboratorsService eventCollaboratorsService = new EventCollaboratorsService();

                    List<EventCollaborators> eventCollaborators = eventCollaboratorsService.Read();

                    eventCollaborators = eventCollaborators.Where(eventCollaborator => eventCollaborator.EventId ==
                                                                  eventId)
                                                           .ToList();

                    RecurrenceEngine engine = new RecurrenceEngine();

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
            List<Event> events = Read().Where(eventObj => eventObj.UserId == GlobalData.user.Id).ToList();

            List<List<string>> outputRows = new List<List<string>>
            {
                new List<string> { "Event NO." , "Title" , "Description" , "Location" , "TimeBlock" }
            };

            foreach (var eventObj in events)
            {
                outputRows.Add(new List<string> { eventObj.Id.ToString(), eventObj.Title, eventObj.Description, eventObj.Location, eventObj.TimeBlock });
            }

            string eventTable = PrintHandler.GiveTable(outputRows);

            return eventTable;
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