using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Interfaces;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities
{
    internal class EventQuerySupplier : IQuerySupplier
    {
        public string Read()
        {
            return @"SELECT [dbo].[Event].Title	
                           ,[dbo].[Event].Location	
                           ,[dbo].[Event].Description	
                           ,[dbo].[Event].UserId	
                           ,[dbo].[Event].RecurrenceId	
                            from [dbo].[Event]";
        }

        public string ReadById()
        {
            return @"SELECT [dbo].[Event].Title	
                           ,[dbo].[Event].Location	
                           ,[dbo].[Event].Description	
                           ,[dbo].[Event].UserId	
                           ,[dbo].[Event].RecurrenceId	
                            from [dbo].[Event]	
                            WHERE [dbo].[Event].Id = @Id";
        }

        public string Create()
        {
            return @"INSERT INTO [dbo].[Event] (Title,Location,Description,UserId,RecurrenceId)	
	                        values(@Title,@Location,@Description,@UserId,@RecurrenceId)	
                            SET @Id = SCOPE_IDENTITY()";
        }

        public string Delete()
        {
            return @"Update [dbo].[Event] 	
	                        set [dbo].[Event].Title = @Title	
		                        ,[dbo].[Event].Location = @Location	
		                        ,[dbo].[Event].Description = @Description	
		                        ,[dbo].[Event].UserId = @UserId	
		                        ,[dbo].[Event].RecurrenceId = @RecurrenceId	
		                        ,[dbo].[Event].ModificationDate = GETDATE()	
	                        where [dbo].[Event].Id = @UpdateId";
        }

        public string Update()
        {
            return @"Delete from [dbo].[Event]	
	                        where [dbo].[Event].Id = @Id";
        }
    }
}