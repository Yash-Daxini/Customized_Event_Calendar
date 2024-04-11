using System.Data;
using System.Text;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Models;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal static class ProposedEventHandler
    {
        public static void GetInputForProposedEvent(Event? eventObj)
        {
            if (IsMessagePrintedOnUnavailabilityOfInvitee()) return;

            eventObj ??= new();

            EventHandling.GetEventDetailsFromUser(eventObj);

            TimeHandler.GetStartingAndEndingHourOfEvent(eventObj);

            eventObj.EventStartDate = ValidatedInputProvider.GetValidDateOnly("Enter date for the proposed event (Enter date in dd-MM-yyyy) :- ");
            eventObj.EventEndDate = eventObj.EventStartDate;

            string invitees = GetInviteesFromUser();

            eventObj.UserId = GlobalData.GetUser().Id;

            eventObj.IsProposed = true;

            UpsertProposedEvent(eventObj);

            MultipleInviteesEventService.AddInviteesInProposedEvent(eventObj, invitees);
        }

        private static void UpsertProposedEvent(Event eventObj)
        {
            if (eventObj.Id > 0)
                EventHandling.UpdateEvent(eventObj.Id, eventObj);
            else
                EventHandling.AddEvent(eventObj);
        }

        private static bool IsMessagePrintedOnUnavailabilityOfInvitee()
        {
            if (GetInsensitiveUserInformationList().Count == 0)
            {
                PrintHandler.PrintWarningMessage("No invitees are available !");
                return true;
            }
            return false;
        }

        private static string GetInviteesFromUser()
        {
            ShowAllUser();

            string inviteesSerialNumber = ValidatedInputProvider.GetValidCommaSeparatedInputInRange("Enter users you want to Invite. (Enter users Sr No. comma separated Ex:- 1,2,3) : ", 1, GetInsensitiveUserInformationList().Count);

            return GetInviteesUserIdFromSerialNumber(inviteesSerialNumber);
        }

        private static string GetInviteesUserIdFromSerialNumber(string inviteesSerialNumber)
        {
            List<User> users = GetInsensitiveUserInformationList();

            StringBuilder invitees = new();

            foreach (int inviteeSerialNumber in inviteesSerialNumber.Split(",").Select(number => Convert.ToInt32(number.Trim())))
            {
                invitees.Append(users[inviteeSerialNumber - 1].Id + ",");
            }

            return invitees.ToString()[..(invitees.Length - 1)];
        }

        private static void ShowAllUser()
        {
            List<User> users = GetInsensitiveUserInformationList();

            StringBuilder userInformation = new();

            List<List<string>> userTableContent = users.InsertInto2DList(["Sr. No", "Name", "Email"],
                                                  [
                                                      user => users.IndexOf(user)+1,
                                                      user => user.Name,
                                                      user => user.Email,
                                                  ]);

            userInformation.AppendLine(PrintService.GenerateTable(userTableContent));

            Console.WriteLine(userInformation);
        }

        private static List<User> GetInsensitiveUserInformationList()
        {
            UserService userService = new();
            List<User> users = userService.GetInsensitiveInformationOfUser();

            return users;
        }
    }
}