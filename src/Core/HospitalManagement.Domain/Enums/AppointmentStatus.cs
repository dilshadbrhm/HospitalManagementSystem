using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Domain.Enums
{
    public enum AppointmentStatus
    {
        Pending = 1,
        Confirmed = 2,        
        Completed = 3,
        Cancelled = 4,
        Rescheduled =5,
        NoShow = 6
    }
}
