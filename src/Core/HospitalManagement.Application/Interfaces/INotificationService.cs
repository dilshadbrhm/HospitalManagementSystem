using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Interfaces
{
    public interface INotificationService
    {
        Task SendAppointmentConfirmedAsync(string userEmail, string userId, string patientName, string doctorName, DateTime appointmentDate, TimeSpan time);
        Task SendAppointmentCancelledAsync(string userEmail, string userId, string patientName, string doctorName, DateTime appointmentDate, string reason);
        Task SendLabResultReadyAsync(string userEmail, string userId, string patientName, string testName);
        Task SendPrescriptionReadyAsync(string userEmail, string userId, string patientName, string doctorName);
    }
}
