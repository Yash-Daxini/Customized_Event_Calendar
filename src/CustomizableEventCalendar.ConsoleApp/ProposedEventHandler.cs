using System.Data;
using System.Text;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal static class ProposedEventHandler
    {
        public static void GetInputForProposedEvent(Event? eventObj)
        {
            if (GetInsensitiveUserInformationList().Count == 0)
            {
                PrintHandler.PrintWarningMessage("No invitees are available !");
                return;
            }

            eventObj ??= new();

            EventHandling.GetEventDetailsFromUser(eventObj);

            TimeHandler.GetStartingAndEndingHourOfEvent(eventObj);

            eventObj.EventStartDate = ValidatedInputProvider.GetValidatedDateOnly("Enter date for the proposed event (Enter date in dd-MM-yyyy) :- ");

            eventObj.EventEndDate = eventObj.EventStartDate;

            eventObj.UserId = GlobalData.GetUser().Id;

            string invitees = GetInviteesFromUser();

            eventObj.IsProposed = true;

            if (eventObj.Id > 0)
                EventHandling.UpdateEvent(eventObj.Id, eventObj);
            else
                EventHandling.AddEvent(eventObj);

            MultipleInviteesEventService.AddInviteesInProposedEvent(eventObj, invitees);
        }

        private static string GetInviteesFromUser()
        {
            bool isUsersAvailable = ShowAllUser();

            if (!isUsersAvailable) return "";

            string inviteesSerialNumber = ValidatedInputProvider.GetValidatedCommaSeparatedInputInRange("Enter users you want to Invite. (Enter users Sr No. comma separated Ex:- 1,2,3) : ", 1, GetInsensitiveUserInformationList().Count);

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

        private static bool ShowAllUser()
        {
            List<User> users = GetInsensitiveUserInformationList();

            StringBuilder userInformation = new();

            if (users.Count != 0)
            {
                List<List<string>> userTableContent = users.InsertInto2DList(["Sr. No", "Name", "Email"],
                                                      [
                                                          user => users.IndexOf(user)+1,
                                                          user => user.Name,
                                                          user => user.Email,
                                                      ]);

                userInformation.AppendLine(PrintService.GenerateTable(userTableContent));

                Console.WriteLine(userInformation);
            }
            else
            {
                Console.WriteLine("No Users are available!");
            }
            return users.Count > 0;

        }

        private static List<User> GetInsensitiveUserInformationList()
        {
            UserService userService = new();
            List<User> users = userService.GetInsensitiveInformationOfUser();

            return users;
        }
    }
}