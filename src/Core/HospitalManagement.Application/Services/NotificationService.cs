using HospitalManagement.Application.Interfaces;
using HospitalManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IEmailService _emailService;
        private readonly INotificationRepository _notificationRepository;

        public NotificationService(IEmailService emailService, INotificationRepository notificationRepository)
        {
            _emailService = emailService;
            _notificationRepository = notificationRepository;
        }

        public async Task SendAppointmentConfirmedAsync(string userEmail, string userId, string patientName, string doctorName, DateTime appointmentDate, TimeSpan time)
        {
            string subject = "Appointment Confirmed";
            string body = $@"
                <h2>Appointment Confirmed</h2>
                <p>Dear {patientName},</p>
                <p>Your appointment has been confirmed.</p>
                <p><strong>Doctor:</strong> {doctorName}</p>
                <p><strong>Date:</strong> {appointmentDate:dd.MM.yyyy}</p>
                <p><strong>Time:</strong> {time:hh\:mm}</p>
                <p>Thank you for choosing our hospital.</p>
            ";

            try
            {
                await _emailService.SendEmailAsync(userEmail, subject, body);
            }
            catch
            {
              
            }

            Notification notification = new Notification
            {
                UserId = userId,
                Title = "Appointment Confirmed",
                Message = $"Your appointment with Dr. {doctorName} on {appointmentDate:dd.MM.yyyy} at {time:hh\\:mm} has been confirmed.",
                Type = "Appointment"
            };

            await _notificationRepository.AddAsync(notification);
        }

        public async Task SendAppointmentCancelledAsync(string userEmail, string userId, string patientName, string doctorName, DateTime appointmentDate, string reason)
        {
            string subject = "Appointment Cancelled";
            string body = $@"
                <h2>Appointment Cancelled</h2>
                <p>Dear {patientName},</p>
                <p>Your appointment has been cancelled.</p>
                <p><strong>Doctor:</strong> {doctorName}</p>
                <p><strong>Date:</strong> {appointmentDate:dd.MM.yyyy}</p>
                <p><strong>Reason:</strong> {reason}</p>
                <p>Please book a new appointment if needed.</p>
            ";

            try
            {
                await _emailService.SendEmailAsync(userEmail, subject, body);
            }
            catch
            {
            }

            Notification notification = new Notification
            {
                UserId = userId,
                Title = "Appointment Cancelled",
                Message = $"Your appointment with Dr. {doctorName} on {appointmentDate:dd.MM.yyyy} has been cancelled. Reason: {reason}",
                Type = "Appointment"
            };

            await _notificationRepository.AddAsync(notification);
        }

        public async Task SendLabResultReadyAsync(string userEmail, string userId, string patientName, string testName)
        {
            string subject = "Lab Result Ready";
            string body = $@"
                <h2>Lab Result Ready</h2>
                <p>Dear {patientName},</p>
                <p>Your lab result is ready.</p>
                <p><strong>Test:</strong> {testName}</p>
                <p>You can download it from your patient panel.</p>
            ";

            try
            {
                await _emailService.SendEmailAsync(userEmail, subject, body);
            }
            catch
            {
             
            }

            Notification notification = new Notification
            {
                UserId = userId,
                Title = "Lab Result Ready",
                Message = $"Your {testName} result is ready. You can download it from your panel.",
                Type = "Lab"
            };

            await _notificationRepository.AddAsync(notification);
        }

        public async Task SendPrescriptionReadyAsync(string userEmail, string userId, string patientName, string doctorName)
        {
            string subject = "New Prescription";
            string body = $@"
                <h2>New Prescription</h2>
                <p>Dear {patientName},</p>
                <p>Dr. {doctorName} has written a prescription for you.</p>
                <p>You can view it from your patient panel.</p>
            ";

            try
            {
                await _emailService.SendEmailAsync(userEmail, subject, body);
            }
            catch
            {
                
            }

            Notification notification = new Notification
            {
                UserId = userId,
                Title = "New Prescription",
                Message = $"Dr. {doctorName} has written a prescription for you.",
                Type = "Prescription"
            };

            await _notificationRepository.AddAsync(notification);
        }
    }
}
