using System.Data.SqlClient;
using System.Text;
using System.Transactions;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class EventService
    {
        EventRepository eventRepository = new EventRepository();
        RecurrencePatternRepository recurrencePatternRepository = new RecurrencePatternRepository();

        public int Create(Event eventObj, RecurrencePattern recurrencePattern)
        {
            int Id = 0;

            using (TransactionScope transactionScope = new TransactionScope())
            {
                try
                {
                    int recurrenceId = recurrencePatternRepository.Create(recurrencePattern);

                    eventObj.RecurrenceId = recurrenceId;

                    Id = eventRepository.Create<Event>(eventObj);

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
                    transactionScope.Dispose();
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
                listOfEvents = eventRepository.Read<Event>(data => new Event(data)).Where(eventObj => eventObj.UserId == GlobalData.user.Id).ToList();
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
                listOfEvents = eventRepository.Read<Event>(data => new Event(data), Id);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Some error occurred ! " + ex.Message);
            }

            return listOfEvents;
        }
        public void Delete(int eventId)
        {
            using (TransactionScope transactionScope = new TransactionScope())
            {
                try
                {

                    EventCollaboratorsService eventCollaboratorsService = new EventCollaboratorsService();

                    ScheduleEventService scheduleEventService = new ScheduleEventService();

                    Event? eventObj = eventRepository.Read<Event>(data => new Event(data), eventId);

                    int recurrenceId = eventObj == null ? 0 : Convert.ToInt32(eventObj.RecurrenceId);

                    scheduleEventService.DeleteByEventId(eventId);

                    eventCollaboratorsService.DeleteByEventId(eventId);

                    eventRepository.Delete<Event>(eventId);

                    recurrencePatternRepository.Delete<RecurrencePattern>(recurrenceId);

                    transactionScope.Complete();
                }
                catch (Exception ex)
                {
                    transactionScope.Dispose();
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }
        }
        public void Update(Event eventObj, RecurrencePattern recurrencePattern, int eventId, int recurrenceId)
        {
            using (TransactionScope transactionScope = new TransactionScope())
            {
                try
                {
                    recurrencePatternRepository.Update(recurrencePattern, recurrenceId);

                    eventRepository.Update<Event>(eventObj, eventId);

                    ScheduleEventService scheduleEventService = new ScheduleEventService();

                    scheduleEventService.DeleteByEventId(eventId);

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
                    transactionScope.Dispose();
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }
        }
        public string GenerateEventList()
        {
            StringBuilder eventList = new StringBuilder();

            List<Event> events = Read().Where(eventObj => eventObj.UserId == GlobalData.user.Id).ToList();

            eventList.AppendLine("\tEvent NO.\tTitle,\tDescription,\tLocation,\tTimeBlock");

            foreach (var eventObj in events)
            {
                eventList.AppendLine($"\t{eventObj.Id}\t{eventObj.Title}\t{eventObj.Description}\t{eventObj.Location}\t{eventObj.TimeBlock}");
            }

            return eventList.ToString();
        }
    }
}
