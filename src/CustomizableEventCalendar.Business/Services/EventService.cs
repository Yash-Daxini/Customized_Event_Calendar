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
        RecurrenceService recurrenceService = new RecurrenceService();
        ScheduleRepository scheduleRepository = new ScheduleRepository();

        public int Create(Event eventObj)
        {
            int Id = 0;

            try
            {
                Id = eventRepository.Create<Event>(eventObj);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Some error occurred!");
            }

            return Id;
        }
        public List<Event> Read()
        {
            List<Event> listOfEvents = new List<Event>();

            try
            {
                listOfEvents = eventRepository.Read<Event>(data => new Event(data));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Some error occurred!");
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

            }

            return listOfEvents;
        }
        public void Delete(int Id)
        {
            using (TransactionScope transactionScope = new TransactionScope())
            {
                try
                {
                    Event? eventObj = eventRepository.Read<Event>(data => new Event(data), Id);

                    int recurrenceId = eventObj == null ? 0 : Convert.ToInt32(eventObj.RecurrenceId);

                    scheduleRepository.DeleteByEventId(Id);

                    eventRepository.Delete<Event>(Id);

                    recurrenceService.Delete(recurrenceId);

                    transactionScope.Complete();
                }
                catch (Exception ex)
                {
                    transactionScope.Dispose();
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }
        }
        public void Update(Event eventObj, int Id)
        {
            try
            {
                eventRepository.Update(eventObj, Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Some error occurred!");
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
