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
                return new AppointmentResultDto { Success = false, Message = "Doctor not found" };
            }

            bool isAvailable = await IsSlotAvailableAsync(dto.DoctorId, dto.AppointmentDate, dto.StartTime);
            if (!isAvailable)
            {
                return new AppointmentResultDto { Success = false, Message = "This time slot is already booked" };
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
                Fee = doctor.ConsultationFee,
                IsPaid = false,
                IsDeleted = false
            };

            await _unitOfWork.Appointments.AddAsync(appointment);
            await _unitOfWork.SaveChangesAsync();

            return new AppointmentResultDto
            {
                Success = true,
                Message = "Appointment created successfully",
                AppointmentId = appointment.Id
            };
        }

        public async Task<bool> CancelAsync(CancelAppointmentDto dto)
        {
            Appointment appointment = await _unitOfWork.Appointments.GetByIdAsync(dto.AppointmentId);
            if (appointment == null) return false;

            appointment.Status = AppointmentStatus.Cancelled;
            appointment.CancellationReason = dto.CancellationReason;
            appointment.CancelledAt = DateTime.Now;

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateStatusAsync(int id, string status)
        {
            Appointment appointment = await _unitOfWork.Appointments.GetByIdAsync(id);
            if (appointment == null) return false;

            appointment.Status = Enum.Parse<AppointmentStatus>(status);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsSlotAvailableAsync(int doctorId, DateTime date, TimeSpan startTime)
        {
            IEnumerable<Appointment> appointments = await _unitOfWork.Appointments
                .FindAsync(a => a.DoctorId == doctorId
                             && a.AppointmentDate.Date == date.Date
                             && a.StartTime == startTime
                             && !a.IsDeleted);

            Appointment existing = appointments.FirstOrDefault();
            return existing == null;
        }

        public async Task<List<AlternativeSlotDto>> GetAlternativeSlotsAsync(int doctorId, DateTime date, TimeSpan requestedTime)
        {
            List<AlternativeSlotDto> alternatives = new List<AlternativeSlotDto>();

            TimeSpan[] possibleTimes = new TimeSpan[]
            {
                requestedTime.Add(TimeSpan.FromMinutes(-60)),
                requestedTime.Add(TimeSpan.FromMinutes(-30)),
                requestedTime.Add(TimeSpan.FromMinutes(30)),
                requestedTime.Add(TimeSpan.FromMinutes(60))
            };

            foreach (TimeSpan time in possibleTimes)
            {
                if (time.Hours >= 9 && time.Hours < 18)
                {
                    bool available = await IsSlotAvailableAsync(doctorId, date, time);
                    if (available)
                    {
                        alternatives.Add(new AlternativeSlotDto
                        {
                            Date = date,
                            StartTime = time,
                            EndTime = time.Add(TimeSpan.FromMinutes(30))
                        });
                    }
                }
            }

            return alternatives;
        }

        public async Task<List<TimeSlotSelectDto>> GetAvailableSlotsAsync(int doctorId, DateTime date)
        {
            List<TimeSlotSelectDto> slots = new List<TimeSlotSelectDto>();

            TimeSpan startHour = new TimeSpan(9, 0, 0);
            TimeSpan endHour = new TimeSpan(18, 0, 0);

            for (TimeSpan time = startHour; time < endHour; time = time.Add(TimeSpan.FromMinutes(30)))
            {
                bool available = await IsSlotAvailableAsync(doctorId, date, time);
                if (available)
                {
                    slots.Add(new TimeSlotSelectDto
                    {
                        StartTime = time,
                        EndTime = time.Add(TimeSpan.FromMinutes(30))
                    });
                }
            }

            return slots;
        }

        public async Task<List<DepartmentSelectDto>> GetDepartmentsAsync()
        {
            IEnumerable<Department> departments = await _unitOfWork.Departments.GetAllAsync();

            List<DepartmentSelectDto> result = new List<DepartmentSelectDto>();

            foreach (Department department in departments)
            {
                result.Add(new DepartmentSelectDto
                {
                    Id = department.Id,
                    Name = department.Name
                });
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
                result.Add(new DoctorSelectDto
                {
                    Id = doctor.Id,
                    FullName = doctor.FirstName + " " + doctor.LastName,
                    Specialization = doctor.Specialization,
                    ConsultationFee = doctor.ConsultationFee
                });
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
    