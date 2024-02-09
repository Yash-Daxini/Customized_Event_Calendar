using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Interfaces;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities
{
    internal class UserQuerySupplier : IQuerySupplier
    {
        public string Read()
        {
            return @"SELECT [dbo].[User].Id	
                           ,[dbo].[User].Name	
                           ,[dbo].[User].Email  	
                           ,[dbo].[User].Password	
                            from [dbo].[User]";
        }

        public string ReadById()
        {
            return @"SELECT [dbo].[User].Id	
                           ,[dbo].[User].Name	
                           ,[dbo].[User].Email	
                           ,[dbo].[User].Password 	
                            from[dbo].[User]	
                            WHERE [dbo].[User].Id = @Id";
        }
        public string Create()
        {
            return @"INSERT INTO [dbo].[User] (Name,Email,Password)	
	                        values(@Name,@Email,@Password)	
                            SET @Id = SCOPE_IDENTITY()";
        }

        public string Update()
        {
            return @"Update [dbo].[User] 	
	                        set [dbo].[User].Name = @Name	
		                        ,[dbo].[User].Email = @Email	
		                        ,[dbo].[User].Password = @Password	
		                        ,[dbo].[User].ModificationDate = GETDATE()	
	                        where [dbo].[User].Id = @UpdateId";
        }

        public string Delete()
        {
            return @"Delete from [dbo].[User]	
	                        where [dbo].[User].Id = @Id";
        }
    }
}
