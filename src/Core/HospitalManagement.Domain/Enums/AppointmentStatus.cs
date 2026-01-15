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
        InProgress =3,
        Completed = 4,
        Cancelled = 5,
        Rescheduled =6
    }
}
