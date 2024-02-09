using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Interfaces;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities
{
    internal class RecurrencePatternQuerySupplier : IQuerySupplier
    {
        public string Create()
        {
            return @"INSERT INTO [dbo].[RecurrencePattern] (DTSTART,FREQ,UNTILL,COUNT,INTERVAL,BYDAY,BYWEEK,BYMONTH,BYYEAR,BYMONTHDAY)	
	                        values(@DTSTART,@FREQ,@UNTILL,@COUNT,@INTERVAL,@BYDAY,@BYWEEK,@BYMONTH,@BYYEAR,@BYMONTHDAY)	
                            SET @Id = SCOPE_IDENTITY()";
        }

        public string Delete()
        {
            return @"Delete from [dbo].[RecurrencePattern]	
	                        where [dbo].[RecurrencePattern].Id = @Id";
        }

        public string Read()
        {
            return @"SELECT [dbo].[RecurrencePattern].Id	
                           ,[dbo].[RecurrencePattern].DTSTART	
                           ,[dbo].[RecurrencePattern].FREQ  	
                           ,[dbo].[RecurrencePattern].UNTILL	
                           ,[dbo].[RecurrencePattern].COUNT	
                           ,[dbo].[RecurrencePattern].INTERVAL	
                           ,[dbo].[RecurrencePattern].BYDAY	
                           ,[dbo].[RecurrencePattern].BYWEEK	
                           ,[dbo].[RecurrencePattern].BYMONTH	
                           ,[dbo].[RecurrencePattern].BYYEAR	
                           ,[dbo].[RecurrencePattern].BYMONTHDAY	
                            from [dbo].[RecurrencePattern]";
        }

        public string ReadById()
        {
            return @"SELECT [dbo].[RecurrencePattern].Id	
                           ,[dbo].[RecurrencePattern].DTSTART	
                           ,[dbo].[RecurrencePattern].FREQ  	
                           ,[dbo].[RecurrencePattern].UNTILL	
                           ,[dbo].[RecurrencePattern].COUNT	
                           ,[dbo].[RecurrencePattern].INTERVAL	
                           ,[dbo].[RecurrencePattern].BYDAY	
                           ,[dbo].[RecurrencePattern].BYWEEK	
                           ,[dbo].[RecurrencePattern].BYMONTH	
                           ,[dbo].[RecurrencePattern].BYYEAR	
                           ,[dbo].[RecurrencePattern].BYMONTHDAY	
                            from[dbo].[RecurrencePattern]	
                            WHERE [dbo].[RecurrencePattern].Id = @Id";
        }

        public string Update()
        {
            return @"Update [dbo].[RecurrencePattern] 	
	                        set [dbo].[RecurrencePattern].DTSTART = @DTSTART	
		                        ,[dbo].[RecurrencePattern].UNTILL = @UNTILL	
		                        ,[dbo].[RecurrencePattern].FREQ = @FREQ	
		                        ,[dbo].[RecurrencePattern].COUNT = @COUNT	
		                        ,[dbo].[RecurrencePattern].INTERVAL = @INTERVAL	
		                        ,[dbo].[RecurrencePattern].BYDAY = @BYDAY	
		                        ,[dbo].[RecurrencePattern].BYWEEK = @BYWEEK	
		                        ,[dbo].[RecurrencePattern].BYMONTH = @BYMONTH	
		                        ,[dbo].[RecurrencePattern].BYYEAR = @BYYEAR	
		                        ,[dbo].[RecurrencePattern].BYMONTHDAY = @BYMONTHDAY	
		                        ,[dbo].[RecurrencePattern].ModificationDate = GETDATE()	
	                        where [dbo].[RecurrencePattern].Id = @UpdateId";
        }
    }
}