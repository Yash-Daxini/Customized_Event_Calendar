//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp;
//using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

//namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
//{
//    internal class MultipleInviteesEventService
//    {
//        public void StartSchedulingProcessOfProposedEvent()
//        {

//            EventService eventService = new();

//            List<Event> events = eventService.GetAllEvents()
//                                             .Where(eventObj => eventObj.IsProposed
//                                              && eventObj.UserId == GlobalData.user.Id)
//                                             .ToList();

//            RecurrenceService recurrenceService = new();

//            foreach (var eventObj in events)
//            {
//                RecurrencePatternCustom recurrencePattern = recurrenceService.GetRecurrencePatternById(eventObj.RecurrenceId);

//                int remainingDays = (DateTime.Now - recurrencePattern.DTSTART).Days;

//                if (remainingDays <= 1)
//                {
//                    CalculateMutualTime(eventObj);
//                    ScheduleProposedEvent(eventObj, recurrencePattern);
//                }
//            }
//        }

//        public void AddInviteesInProposedEvent(int eventId, string invitees)
//        {
//            HashSet<int> invitedUsers = invitees.Split(",")
//                                                .Select(invitedUser => Convert.ToInt32(invitedUser))
//                                                .ToHashSet();

//            EventCollaboratorsService eventCollaboratorsService = new();

//            foreach (int userId in invitedUsers)
//            {
//                eventCollaboratorsService.InsertEventCollaborators(new EventCollaborators(userId, eventId));
//            }

//        }

//        public DateOnly GetDateOnlyFromDateTime(DateTime date)
//        {
//            return DateOnly.FromDateTime(date);
//        }

//        public void CalculateMutualTime(Event eventObj)
//        {
//            EventCollaboratorsService eventCollaboratorsService = new();

//            HashSet<int> eventCollaborationIds = GetInviteesOfEvent(eventObj.Id);

//            HashSet<int> eventInviteesUserIds = eventCollaborationIds.Select(eventCollaboratorsService
//                                                                        .GetUserIdFromEventCollaborationId)
//                                                                     .ToHashSet();

//            RecurrenceService recurrenceService = new();

//            RecurrencePatternCustom recurrencePattern = recurrenceService.GetRecurrencePatternById(eventObj.RecurrenceId);

//            CalendarSharingService calendarSharingService = new();

//            List<SharedCalendar> sharedCalendars = calendarSharingService.GetSharedEvents().ToList();

//            HashSet<int> inviteesThatSharedCalendar = sharedCalendars.Where(sharedCalendar => eventInviteesUserIds.Contains
//                                                                                            (sharedCalendar.SharedByUserId)
//                                                                        && sharedCalendar.FromDate <=
//                                                                        GetDateOnlyFromDateTime(recurrencePattern.DTSTART)
//                                                                        && sharedCalendar.ToDate >= GetDateOnlyFromDateTime
//                                                                                                (recurrencePattern.DTSTART))
//                                                                     .Select(sharedCalendar =>                  
//                                                                             sharedCalendar.SharedByUserId)
//                                                                     .ToHashSet();

//            int[] nonFreeHours = new int[23];

//            foreach (var inviteeId in inviteesThatSharedCalendar)
//            {
//                CountNonFreeHours(recurrencePattern.DTSTART, inviteeId, ref nonFreeHours);
//            }

//            string timeBlock = FindMaximumMutualTimeBlock(nonFreeHours);

//            if (!timeBlock.Equals("")) eventObj.TimeBlock = timeBlock;

//            eventObj.IsProposed = false;

//            EventService eventService = new();

//            eventService.ConvertProposedEventToScheduleEvent(eventObj.Id);

//        }

//        public void CountNonFreeHours(DateTime proposedEventDate, int inviteeId, ref int[] nonFreeHours)
//        {
//            ScheduleEventService scheduleEventService = new();

//            List<ScheduleEvent> scheduleEvents = scheduleEventService.GetAllScheduleEvents()
//                                                                     .Where(scheduleEvent => scheduleEventService
//                                                                     .GetUserIdFromEventCollaborators
//                                                                        (scheduleEvent.EventCollaboratorsId)
//                                                                        == inviteeId
//                                                                        && scheduleEvent.ScheduledDate.Date ==
//                                                                            proposedEventDate.Date)
//                                                                     .ToList();


//            foreach (var scheduleDate in scheduleEvents.Select(scheduleEvent => scheduleEvent.ScheduledDate))
//            {
//                nonFreeHours[scheduleDate.Hour]++;
//            }

//        }

//        public string FindMaximumMutualTimeBlock(int[] nonFreeHours)
//        {
//            int mininumBusyUsers = nonFreeHours.Min();

//            int maxHourBlock = -1, curHourBlock = 0;

//            string timeBlock = "";

//            for (int i = 0; i < nonFreeHours.Length; i++)
//            {
//                if (nonFreeHours[i] == mininumBusyUsers)
//                {
//                    curHourBlock++;
//                }
//                else
//                {
//                    if (maxHourBlock < curHourBlock)
//                    {
//                        maxHourBlock = curHourBlock;

//                        int startHour = (i - curHourBlock);
//                        int endHour = i - 1;

//                        timeBlock = startHour + (startHour > 12 ? "PM" : "AM") + "-" +
//                                                endHour + (endHour > 12 ? "PM" : "AM");

//                        timeBlock = timeBlock.Replace("0AM", "12AM");
//                    }
//                    curHourBlock = 0;
//                }
//            }

//            return timeBlock;
//        }

//        public void ScheduleProposedEvent(Event eventObj, RecurrencePatternCustom recurrencePatternCustom)
//        {
//            RecurrenceEngine recurrenceEngine = new();

//            HashSet<int> eventCollaboratorsIds = GetInviteesOfEvent(eventObj.Id);

//            foreach (var eventCollaboratorsId in eventCollaboratorsIds)
//            {

//                recurrenceEngine.ScheduleNonRecurrenceEvents(eventObj, recurrencePatternCustom.DTSTART,
//                                                            recurrencePatternCustom.UNTILL, eventCollaboratorsId);
//            }

//        }

//        public HashSet<int> GetInviteesOfEvent(int eventId)
//        {
//            EventCollaboratorsService eventCollaboratorsService = new();

//            HashSet<int> eventCollaboratorsId = eventCollaboratorsService.GetAllEventCollaborators()
//                                                                      .Where(eventCollaborator =>
//                                                                             eventCollaborator.EventId == eventId
//                                                                             && eventCollaborator.UserId !=
//                                                                             GlobalData.user.Id)
//                                                                      .Select(eventCollaborator =>
//                                                                             eventCollaborator.Id)
//                                                                      .ToHashSet();
//            return eventCollaboratorsId;
//        }
//    }
//}