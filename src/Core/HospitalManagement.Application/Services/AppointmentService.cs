using HospitalManagement.Application.Dtos.Appointment;
using HospitalManagement.Application.Dtos.Department;
using HospitalManagement.Application.Dtos.Doctor;
using HospitalManagement.Application.Dtos.Timeslot;
using HospitalManagement.Application.Interfaces;
using HospitalManagement.Domain;
using HospitalManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AppointmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DetailsAppointmentDto> GetByIdAsync(int id)
        {
            Appointment appointment = await _unitOfWork.Appointments.GetByIdAsync(id);

            if (appointment == null)
            {
                throw new Exception("Appointment not found");
            }

            DetailsAppointmentDto dto = new DetailsAppointmentDto
            {
                Id = appointment.Id,
                PatientId = appointment.PatientId,
                DoctorId = appointment.DoctorId,
                AppointmentDate = appointment.AppointmentDate,
                StartTime = appointment.StartTime,
                EndTime = appointment.EndTime,
                Status = appointment.Status.ToString(),
                Symptoms = appointment.Symptoms,
                Notes = appointment.Notes,
                Fee = appointment.Fee,
                IsPaid = appointment.IsPaid,
                CancellationReason = appointment.CancellationReason,
                CancelledAt = appointment.CancelledAt
            };


            if (appointment.Doctor != null)
            {
                dto.DoctorName = appointment.Doctor.FirstName + " " + appointment.Doctor.LastName;
                dto.DoctorSpecialization = appointment.Doctor.Specialization;

                if (appointment.Doctor.Department != null)
                {
                    dto.DepartmentName = appointment.Doctor.Department.Name;
                }
            }

            if (appointment.Patient != null)
            {
                dto.PatientName = appointment.Patient.FirstName + " " + appointment.Patient.LastName;
                dto.PatientEmail = appointment.Patient.Email;
                dto.PatientPhone = appointment.Patient.Phone;
            }

            return dto;
        }

        public async Task<List<AppointmentItemDto>> GetAllAsync()
        {
            IEnumerable<Appointment> appointments = await _unitOfWork.Appointments.GetAllAsync();

            List<AppointmentItemDto> result = new List<AppointmentItemDto>();

            foreach (Appointment appointment in appointments)
            {
                AppointmentItemDto dto = MapToListDto(appointment);
                result.Add(dto);
            }

            return result;
        }

        public async Task<List<AppointmentItemDto>> GetByPatientIdAsync(int patientId)
        {
            IEnumerable<Appointment> appointments = await _unitOfWork.Appointments
                .FindAsync(a => a.PatientId == patientId && !a.IsDeleted);

            List<AppointmentItemDto> result = new List<AppointmentItemDto>();

            foreach (Appointment appointment in appointments)
            {
                AppointmentItemDto dto = MapToListDto(appointment);
                result.Add(dto);
            }

            return result;
        }

        public async Task<List<AppointmentItemDto>> GetByDoctorIdAsync(int doctorId)
        {
            IEnumerable<Appointment> appointments = await _unitOfWork.Appointments
                .FindAsync(a => a.DoctorId == doctorId && !a.IsDeleted);

            List<AppointmentItemDto> result = new List<AppointmentItemDto>();

            foreach (Appointment appointment in appointments)
            {
                AppointmentItemDto dto = MapToListDto(appointment);
                result.Add(dto);
            }

            return result;
        }

        public async Task<AppointmentResultDto> CreateAsync(CreateAppointmentDto dto, int patientId)
        {
            Doctor doctor = await _unitOfWork.Doctors.GetByIdAsync(dto.DoctorId);

            if (doctor == null)
            {
                AppointmentResultDto result = new AppointmentResultDto();
                result.Success = false;
                result.Message = "The doctor was not found";
                return result;
            }

            if (dto.AppointmentDate.Date < DateTime.Now.Date)
            {
                AppointmentResultDto result = new AppointmentResultDto();
                result.Success = false;
                result.Message = "You can not create an opinion on past history";
                return result;
            }

            bool isAvailable = await IsSlotAvailableAsync(dto.DoctorId, dto.AppointmentDate, dto.StartTime);

            if (!isAvailable)
            {
                List<AlternativeSlotDto> alternatives = await GetAlternativeSlotsAsync(dto.DoctorId, dto.AppointmentDate, dto.StartTime);

                AppointmentResultDto result = new AppointmentResultDto();
                result.Success = false;
                result.Message = "This hour is already full";
                result.AlternativeSlots = alternatives;
                return result;
            }

            Appointment appointment = new Appointment
            {
                PatientId = patientId,
                DoctorId = dto.DoctorId,
                AppointmentDate = dto.AppointmentDate,
                StartTime = dto.StartTime,
                EndTime = dto.StartTime.Add(TimeSpan.FromMinutes(30)),
                Status = AppointmentStatus.Pending,
                Symptoms = dto.Symptoms,
                Notes = dto.Notes,
                Fee = doctor.ConsultationFee,
                IsPaid = false,
                IsDeleted = false
            };


            await _unitOfWork.Appointments.AddAsync(appointment);
            await _unitOfWork.SaveChangesAsync();

            AppointmentResultDto successResult = new AppointmentResultDto
            {
                Success = true,
                Message = "The view was created successfully",
                AppointmentId = appointment.Id

            };
            
            return successResult;
        }

        public async Task<bool> CancelAsync(CancelAppointmentDto dto)
        {
            Appointment appointment = await _unitOfWork.Appointments.GetByIdAsync(dto.AppointmentId);

            if (appointment == null)
            {
                return false;
            }

            appointment.Status = AppointmentStatus.Cancelled;
            appointment.CancellationReason = dto.CancellationReason;
            appointment.CancelledAt = DateTime.Now;

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateStatusAsync(int id, string status)
        {
            Appointment appointment = await _unitOfWork.Appointments.GetByIdAsync(id);

            if (appointment == null)
            {
                return false;
            }

            appointment.Status = Enum.Parse<AppointmentStatus>(status);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsSlotAvailableAsync(int doctorId, DateTime date, TimeSpan startTime)
        {
            bool exists = await _unitOfWork.Appointments.AnyAsync(a =>
                a.DoctorId == doctorId &&
                a.AppointmentDate.Date == date.Date &&
                a.StartTime == startTime &&
                a.Status != AppointmentStatus.Cancelled &&
                !a.IsDeleted);

            if (exists)
            {
                return false;
            }

            return true;
        }

        public async Task<List<AlternativeSlotDto>> GetAlternativeSlotsAsync(int doctorId, DateTime date, TimeSpan requestedTime)
        {
            List<AlternativeSlotDto> alternatives = new List<AlternativeSlotDto>();

            List<TimeSpan> times = new List<TimeSpan>();
            times.Add(requestedTime.Add(TimeSpan.FromMinutes(-60)));
            times.Add(requestedTime.Add(TimeSpan.FromMinutes(-30)));
            times.Add(requestedTime.Add(TimeSpan.FromMinutes(30)));
            times.Add(requestedTime.Add(TimeSpan.FromMinutes(60)));

            foreach (TimeSpan time in times)
            {
                if (time.Hours >= 9 && time.Hours < 18)
                {
                    bool available = await IsSlotAvailableAsync(doctorId, date, time);

                    if (available)
                    {
                        AlternativeSlotDto slot = new AlternativeSlotDto();
                        slot.Date = date;
                        slot.StartTime = time;
                        slot.EndTime = time.Add(TimeSpan.FromMinutes(30));
                        alternatives.Add(slot);
                    }
                }
            }

            return alternatives;
        }

        public async Task<List<TimeSlotSelectDto>> GetAvailableSlotsAsync(int doctorId, DateTime date)
        {
            List<TimeSlotSelectDto> slots = new List<TimeSlotSelectDto>();

            TimeSpan time = new TimeSpan(9, 0, 0);
            TimeSpan endTime = new TimeSpan(18, 0, 0);

            while (time < endTime)
            {
                bool available = await IsSlotAvailableAsync(doctorId, date, time);

                if (available)
                {
                    TimeSlotSelectDto slot = new TimeSlotSelectDto();
                    slot.StartTime = time;
                    slot.EndTime = time.Add(TimeSpan.FromMinutes(30));
                    slots.Add(slot);
                }

                time = time.Add(TimeSpan.FromMinutes(30));
            }

            return slots;
        }

        public async Task<List<DepartmentSelectDto>> GetDepartmentsAsync()
        {
            IEnumerable<Department> departments = await _unitOfWork.Departments.GetAllAsync();

            List<DepartmentSelectDto> result = new List<DepartmentSelectDto>();

            foreach (Department department in departments)
            {
                DepartmentSelectDto dto = new DepartmentSelectDto();
                dto.Id = department.Id;
                dto.Name = department.Name;
                result.Add(dto);
            }

            return result;
        }

        public async Task<List<DoctorSelectDto>> GetDoctorsByDepartmentAsync(int departmentId)
        {
            IEnumerable<Doctor> doctors = await _unitOfWork.Doctors
                .FindAsync(d => d.DepartmentId == departmentId && !d.IsDeleted);

            List<DoctorSelectDto> result = new List<DoctorSelectDto>();

            foreach (Doctor doctor in doctors)
            {
                DoctorSelectDto dto = new DoctorSelectDto
                {
                    Id = doctor.Id,
                    FullName = doctor.FirstName + " " + doctor.LastName,
                    Specialization = doctor.Specialization,
                    ConsultationFee = doctor.ConsultationFee
                };

                result.Add(dto);
            }

            return result;
        }

        private AppointmentItemDto MapToListDto(Appointment appointment)
        {
            AppointmentItemDto dto = new AppointmentItemDto();
            dto.Id = appointment.Id;
            dto.AppointmentDate = appointment.AppointmentDate;
            dto.StartTime = appointment.StartTime;
            dto.EndTime = appointment.EndTime;
            dto.Status = appointment.Status.ToString();
            dto.Fee = appointment.Fee;
            dto.IsPaid = appointment.IsPaid;
            dto.Symptoms = appointment.Symptoms;
            appointment.CancellationReason = string.Empty;
            appointment.CancelledAt = null;

            if (appointment.Doctor != null)
            {
                dto.DoctorName = appointment.Doctor.FirstName + " " + appointment.Doctor.LastName;

                if (appointment.Doctor.Department != null)
                {
                    dto.DepartmentName = appointment.Doctor.Department.Name;
                }
            }

            if (appointment.Patient != null)
            {
                dto.PatientName = appointment.Patient.FirstName + " " + appointment.Patient.LastName;
            }

            return dto;
        }
        public async Task<List<TimeSlotSelectDto>> GetAllSlotsWithStatusAsync(int doctorId, DateTime date)
        {
            List<TimeSlotSelectDto> slots = new List<TimeSlotSelectDto>();

            TimeSpan time = new TimeSpan(9, 0, 0);
            TimeSpan endTime = new TimeSpan(18, 0, 0);

            while (time < endTime)
            {
                bool available = await IsSlotAvailableAsync(doctorId, date, time);

                TimeSlotSelectDto slot = new TimeSlotSelectDto();
                slot.StartTime = time;
                slot.EndTime = time.Add(TimeSpan.FromMinutes(30));
                slot.IsAvailable = available;
                slots.Add(slot);

                time = time.Add(TimeSpan.FromMinutes(30));
            }

            return slots;
        }
        public async Task<Dictionary<DateTime, int>> GetAvailableSlotsCountByDatesAsync(int doctorId, List<DateTime> dates)
        {
            Dictionary<DateTime, int> result = new Dictionary<DateTime, int>();

            foreach (DateTime date in dates)
            {
                List<TimeSlotSelectDto> slots = await GetAvailableSlotsAsync(doctorId, date);
                result.Add(date, slots.Count);
            }

            return result;
        }

        public async Task<CancelResultDto> CancelByPatientAsync(int appointmentId, int patientId, string reason)
        {
            Appointment appointment = await _unitOfWork.Appointments.GetByIdAsync(appointmentId);

            CancelResultDto result = new CancelResultDto();

            if (appointment == null)
            {
                result.Success = false;
                result.Message = "Appointment not found";
                return result;
            }

            if (appointment.PatientId != patientId)
            {
                result.Success = false;
                result.Message = "You can only cancel your own appointments";
                return result;
            }

            if (appointment.Status == AppointmentStatus.Cancelled)
            {
                result.Success = false;
                result.Message = "This appointment is already cancelled";
                return result;
            }

            if (appointment.Status == AppointmentStatus.Completed)
            {
                result.Success = false;
                result.Message = "Completed appointments cannot be cancelled";
                return result;
            }

            DateTime appointmentDateTime = appointment.AppointmentDate.Date.Add(appointment.StartTime);
            TimeSpan timeDifference = appointmentDateTime - DateTime.Now;

            if (timeDifference.TotalHours < 24)
            {
                result.Success = false;
                result.Message = "Appointments can only be cancelled at least 24 hours in advance";
                return result;
            }

            appointment.Status = AppointmentStatus.Cancelled;
            appointment.CancellationReason = reason;
            appointment.CancelledAt = DateTime.Now;

            await _unitOfWork.SaveChangesAsync();

            result.Success = true;
            result.Message = "Appointment cancelled successfully";
            result.AppointmentDate = appointment.AppointmentDate;
            result.StartTime = appointment.StartTime;

            if (appointment.Patient != null)
            {
                result.PatientName = appointment.Patient.FirstName + " " + appointment.Patient.LastName;
                result.PatientEmail = appointment.Patient.Email;
            }

            if (appointment.Doctor != null)
            {
                result.DoctorName = appointment.Doctor.FirstName + " " + appointment.Doctor.LastName;
                result.DoctorEmail = appointment.Doctor.Email;
            }

            return result;
        }

        public async Task<CancelResultDto> CancelByDoctorAsync(int appointmentId, int doctorId, string reason)
        {
            Appointment appointment = await _unitOfWork.Appointments.GetByIdAsync(appointmentId);

            CancelResultDto result = new CancelResultDto();

            if (appointment == null)
            {
                result.Success = false;
                result.Message = "Appointment not found";
                return result;
            }

            if (appointment.DoctorId != doctorId)
            {
                result.Success = false;
                result.Message = "You can only cancel your own appointments";
                return result;
            }

            if (appointment.Status == AppointmentStatus.Cancelled)
            {
                result.Success = false;
                result.Message = "This appointment is already cancelled";
                return result;
            }

            if (appointment.Status == AppointmentStatus.Completed)
            {
                result.Success = false;
                result.Message = "Completed appointments cannot be cancelled";
                return result;
            }

            appointment.Status = AppointmentStatus.Cancelled;
            appointment.CancellationReason = "Cancelled by doctor: " + reason;
            appointment.CancelledAt = DateTime.Now;

            await _unitOfWork.SaveChangesAsync();

            result.Success = true;
            result.Message = "Appointment cancelled successfully";
            result.AppointmentDate = appointment.AppointmentDate;
            result.StartTime = appointment.StartTime;

            if (appointment.Patient != null)
            {
                result.PatientName = appointment.Patient.FirstName + " " + appointment.Patient.LastName;
                result.PatientEmail = appointment.Patient.Email;
            }

            if (appointment.Doctor != null)
            {
                result.DoctorName = appointment.Doctor.FirstName + " " + appointment.Doctor.LastName;
                result.DoctorEmail = appointment.Doctor.Email;
            }

            return result;
        }

        public async Task<AppointmentResultDto> RescheduleAsync(RescheduleAppointmentDto dto, int patientId)
        {
            Appointment appointment = await _unitOfWork.Appointments.GetByIdAsync(dto.AppointmentId);

            AppointmentResultDto result = new AppointmentResultDto();

            if (appointment == null)
            {
                result.Success = false;
                result.Message = "Appointment not found";
                return result;
            }

            if (appointment.PatientId != patientId)
            {
                result.Success = false;
                result.Message = "You can only reschedule your own appointments";
                return result;
            }

            if (appointment.Status == AppointmentStatus.Cancelled || appointment.Status == AppointmentStatus.Completed)
            {
                result.Success = false;
                result.Message = "This appointment cannot be rescheduled";
                return result;
            }

            DateTime appointmentDateTime = appointment.AppointmentDate.Date.Add(appointment.StartTime);
            TimeSpan timeDifference = appointmentDateTime - DateTime.Now;

            if (timeDifference.TotalHours < 24)
            {
                result.Success = false;
                result.Message = "Appointments can only be rescheduled at least 24 hours in advance";
                return result;
            }

            if (dto.NewDate.Date < DateTime.Now.Date)
            {
                result.Success = false;
                result.Message = "Cannot reschedule to a past date";
                return result;
            }

            bool isAvailable = await IsSlotAvailableAsync(appointment.DoctorId, dto.NewDate, dto.NewStartTime);

            if (!isAvailable)
            {
                List<AlternativeSlotDto> alternatives = await GetAlternativeSlotsAsync(appointment.DoctorId, dto.NewDate, dto.NewStartTime);

                result.Success = false;
                result.Message = "The selected time slot is not available";
                result.AlternativeSlots = alternatives;
                return result;
            }

            appointment.AppointmentDate = dto.NewDate;
            appointment.StartTime = dto.NewStartTime;
            appointment.EndTime = dto.NewStartTime.Add(TimeSpan.FromMinutes(30));
            appointment.Notes = appointment.Notes + " | Rescheduled: " + dto.Reason;

            await _unitOfWork.SaveChangesAsync();

            result.Success = true;
            result.Message = "Appointment rescheduled successfully";
            result.AppointmentId = appointment.Id;

            return result;
        }

        public async Task<AppointmentResultDto> RescheduleByDoctorAsync(RescheduleAppointmentDto dto, int doctorId)
        {
            Appointment appointment = await _unitOfWork.Appointments.GetByIdAsync(dto.AppointmentId);

            AppointmentResultDto result = new AppointmentResultDto();

            if (appointment == null)
            {
                result.Success = false;
                result.Message = "Appointment not found";
                return result;
            }

            if (appointment.DoctorId != doctorId)
            {
                result.Success = false;
                result.Message = "You can only reschedule your own appointments";
                return result;
            }

            if (appointment.Status == AppointmentStatus.Cancelled || appointment.Status == AppointmentStatus.Completed)
            {
                result.Success = false;
                result.Message = "This appointment cannot be rescheduled";
                return result;
            }

            if (dto.NewDate.Date < DateTime.Now.Date)
            {
                result.Success = false;
                result.Message = "Cannot reschedule to a past date";
                return result;
            }

            bool isAvailable = await IsSlotAvailableAsync(appointment.DoctorId, dto.NewDate, dto.NewStartTime);

            if (!isAvailable)
            {
                List<AlternativeSlotDto> alternatives = await GetAlternativeSlotsAsync(appointment.DoctorId, dto.NewDate, dto.NewStartTime);

                result.Success = false;
                result.Message = "The selected time slot is not available";
                result.AlternativeSlots = alternatives;
                return result;
            }

            appointment.AppointmentDate = dto.NewDate;
            appointment.StartTime = dto.NewStartTime;
            appointment.EndTime = dto.NewStartTime.Add(TimeSpan.FromMinutes(30));
            appointment.Notes = appointment.Notes + " | Rescheduled by doctor: " + dto.Reason;

            await _unitOfWork.SaveChangesAsync();

            result.Success = true;
            result.Message = "Appointment rescheduled successfully";
            result.AppointmentId = appointment.Id;

            return result;
        }

        public async Task<bool> CanPatientCancelAsync(int appointmentId, int patientId)
        {
            Appointment appointment = await _unitOfWork.Appointments.GetByIdAsync(appointmentId);

            if (appointment == null || appointment.PatientId != patientId)
            {
                return false;
            }

            if (appointment.Status == AppointmentStatus.Cancelled || appointment.Status == AppointmentStatus.Completed)
            {
                return false;
            }

            DateTime appointmentDateTime = appointment.AppointmentDate.Date.Add(appointment.StartTime);
            TimeSpan timeDifference = appointmentDateTime - DateTime.Now;

            return timeDifference.TotalHours >= 24;
        }

    }
}
