using System.Text;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Enums;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Models;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal static class ProposedEventHandler
    {
        public static void GetInputForProposedEvent(EventModel? eventModel)
        {
            if (IsMessagePrintedOnUnavailabilityOfInvitee()) return;

            eventModel ??= new();

            EventHandling.GetEventDetailsFromUser(eventModel);

            TimeHandler.GetStartingAndEndingHourOfEvent(eventModel);

            eventModel.RecurrencePattern.StartDate = ValidatedInputProvider.GetValidDateOnly("Enter date for the proposed event (Enter date in dd-MM-yyyy) :- ");
            eventModel.RecurrencePattern.EndDate = eventModel.RecurrencePattern.StartDate;

            List<int> invitees = GetInviteesFromUser();

            eventModel.Participants = GetParticipantsOfEvent(invitees);

            AddOrganizerInParticipantList(eventModel);

            UpsertProposedEvent(eventModel);

            MultipleInviteesEventService.AddInviteesInProposedEvent(eventModel);
        }

        private static void AddOrganizerInParticipantList(EventModel eventModel)
        {
            eventModel.Participants.Add(new ParticipantModel(ParticipantRole.Organizer, ConfirmationStatus.Accept, null, null, new DateOnly(), GlobalData.GetUser()));
        }

        private static List<ParticipantModel> GetParticipantsOfEvent(List<int> invitees)
        {
            UserService userService = new ();

            List<ParticipantModel> participantModels = [];

            foreach (var invitee in invitees)
            {
                UserModel userModel = userService.GetUserById(invitee);

                participantModels.Add(new(ParticipantRole.Participant,ConfirmationStatus.Accept,null,null,new DateOnly(),userModel));
            }

            return participantModels;
        }

        private static void UpsertProposedEvent(EventModel eventModel)
        {
            if (eventModel.Id > 0)
                EventHandling.UpdateEvent(eventModel.Id, eventModel,true);
            else
                EventHandling.AddEvent(eventModel,true);
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

        private static List<int> GetInviteesFromUser()
        {
            ShowAllUser();

            List<int> invitees = [];

            while (true)
            {
                int inviteeIdSerialNumber = ValidatedInputProvider.GetValidIntegerBetweenRange("Enter users you want to Invite. Press 0 to exit : ", 0, GetInsensitiveUserInformationList().Count);

                if (inviteeIdSerialNumber == 0) break;

                invitees.Add(GetInviteesUserIdFromSerialNumber(inviteeIdSerialNumber));
            }

            return invitees;
        }

        private static int GetInviteesUserIdFromSerialNumber(int inviteeSerialNumber)
        {
            List<UserModel> users = GetInsensitiveUserInformationList();

            return users[inviteeSerialNumber - 1].Id;
        }

        private static void ShowAllUser()
        {
            List<UserModel> users = GetInsensitiveUserInformationList();

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

        private static List<UserModel> GetInsensitiveUserInformationList()
        {
            return new UserService().GetInsensitiveInformationOfUser();
        }
    }
}