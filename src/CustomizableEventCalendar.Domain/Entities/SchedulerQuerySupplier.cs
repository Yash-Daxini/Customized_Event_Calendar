using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Interfaces;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities
{
    internal class SchedulerQuerySupplier : IQuerySupplier
    {
        public string Create()
        {
            return @"INSERT INTO [dbo].[Scheduler] (EventId,TimeBlock,Date)	
	                        values(@EventId,@TimeBlock,@Date)	
                            SET @Id = SCOPE_IDENTITY()";
        }

        public string Delete()
        {
            return @"Delete from [dbo].[Scheduler]	
	                        where [dbo].[Scheduler].Id = @Id";
        }

        public string Read()
        {
            return @"SELECT [dbo].[Scheduler].EventId	
                           ,[dbo].[Scheduler].TimeBlock	
                           ,[dbo].[Scheduler].Date  	
                            from [dbo].[Scheduler]";
        }

        public string ReadById()
        {
            return @"SELECT [dbo].[Scheduler].EventId	
                           ,[dbo].[Scheduler].TimeBlock	
                           ,[dbo].[Scheduler].Date 	
                            from[dbo].[Scheduler]	
                            WHERE [dbo].[Scheduler].Id = @Id";
        }

        public string Update()
        {
            return @"Update [dbo].[Scheduler] 	
	                        set [dbo].[Scheduler].EventId = @EventId	
		                        ,[dbo].[Scheduler].TimeBlock = @TimeBlock	
		                        ,[dbo].[Scheduler].Date = @Date	
		                        ,[dbo].[Scheduler].ModificationDate = GETDATE()	
	                        where [dbo].[Scheduler].Id = @UpdateId";
        }
    }
}
