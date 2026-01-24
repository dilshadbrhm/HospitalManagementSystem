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

            DetailsAppointmentDto dto = new DetailsAppointmentDto();
            dto.Id = appointment.Id;
            dto.PatientId = appointment.PatientId;
            dto.DoctorId = appointment.DoctorId;
            dto.AppointmentDate = appointment.AppointmentDate;
            dto.StartTime = appointment.StartTime;
            dto.EndTime = appointment.EndTime;
            dto.Status = appointment.Status.ToString();
            dto.Symptoms = appointment.Symptoms;
            dto.Notes = appointment.Notes;
            dto.Fee = appointment.Fee;
            dto.IsPaid = appointment.IsPaid;
            dto.CancellationReason = appointment.CancellationReason;
            dto.CancelledAt = appointment.CancelledAt;

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

            Appointment appointment = new Appointment();
            appointment.PatientId = patientId;
            appointment.DoctorId = dto.DoctorId;
            appointment.AppointmentDate = dto.AppointmentDate;
            appointment.StartTime = dto.StartTime;
            appointment.EndTime = dto.StartTime.Add(TimeSpan.FromMinutes(30));
            appointment.Status = AppointmentStatus.Pending;
            appointment.Symptoms = dto.Symptoms;
            appointment.Notes = dto.Notes;
            appointment.Fee = doctor.ConsultationFee;
            appointment.IsPaid = false;
            appointment.IsDeleted = false;

            await _unitOfWork.Appointments.AddAsync(appointment);
            await _unitOfWork.SaveChangesAsync();
                                                                                                               
            AppointmentResultDto successResult = new AppointmentResultDto();
            successResult.Success = true;
            successResult.Message = "The view was created successfully";
            successResult.AppointmentId = appointment.Id;
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
                DoctorSelectDto dto = new DoctorSelectDto();
                dto.Id = doctor.Id;
                dto.FullName = doctor.FirstName + " " + doctor.LastName;
                dto.Specialization = doctor.Specialization;
                dto.ConsultationFee = doctor.ConsultationFee;
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
    }
}
    