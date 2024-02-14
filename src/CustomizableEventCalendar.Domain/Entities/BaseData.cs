using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities
{
    internal class BaseData
    {
        [NotMapped]
        public DateTime CreationDate { get; }
        [NotMapped]
        public DateTime ModificationDate { get; }
    }
}