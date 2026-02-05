using HospitalManagement.Application.Dtos.Prescription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Interfaces
{
    public interface IPrescriptionService
    {
        Task<PrescriptionDto> GetByIdAsync(int id);
        Task<PrescriptionDto> GetByAppointmentIdAsync(int appointmentId);
        Task<List<PrescriptionDto>> GetByPatientIdAsync(int patientId);
        Task<List<PrescriptionDto>> GetByDoctorIdAsync(int doctorId);
        Task<bool> CreateAsync(CreatePrescriptionDto dto, int doctorId);
        Task<byte[]> GeneratePdfAsync(int prescriptionId);
        Task<string> GenerateHtmlAsync(int prescriptionId);
    }
}
