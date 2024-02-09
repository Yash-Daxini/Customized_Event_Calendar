using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Interfaces;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities
{
    internal class InviteesQuerySupplier : IQuerySupplier
    {
        public string Create()
        {
            return @"INSERT INTO [dbo].[Invitees] (EventId,UserId)	
	                        values(@EventId,@UserId)	
                            SET @Id = SCOPE_IDENTITY()";
        }

        public string Delete()
        {
            return @"Delete from [dbo].[Invitees]	
	                        where [dbo].[Invitees].Id = @Id";
        }

        public string Read()
        {
            return @"SELECT [dbo].[Invitees].Id	
                           ,[dbo].[Invitees].EventId	
                           ,[dbo].[Invitees].UserId  	
                            from [dbo].[Invitees]";
        }

        public string ReadById()
        {
            return @"SELECT [dbo].[Invitees].Id	
                           ,[dbo].[Invitees].EventId	
                           ,[dbo].[Invitees].UserId 	
                            from[dbo].[Invitees]	
                            WHERE [dbo].[Invitees].Id = @Id";
        }

        public string Update()
        {
            return @"Update [dbo].[Invitees] 	
	                        set [dbo].[Invitees].EventId = @EventId	
		                        ,[dbo].[Invitees].UserId = @UserId	
		                        ,[dbo].[Invitees].ModificationDate = GETDATE()	
	                        where [dbo].[Invitees].Id = @UpdateId";
        }
    }
}
