using HospitalManagement.Application.Dtos.Prescription;
using HospitalManagement.Application.Interfaces;
using HospitalManagement.Domain;
using HospitalManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Services
{
    public class PrescriptionService : IPrescriptionService
    {
        private readonly IPrescriptionRepository _prescriptionRepository;
        private readonly IAppointmentRepository _appointmentRepository;

        public PrescriptionService(
            IPrescriptionRepository prescriptionRepository,
            IAppointmentRepository appointmentRepository)
        {
            _prescriptionRepository = prescriptionRepository;
            _appointmentRepository = appointmentRepository;
        }

        public async Task<PrescriptionDto> GetByIdAsync(int id)
        {
            Prescription prescription = await _prescriptionRepository.GetByIdAsync(id);

            if (prescription == null)
            {
                return null;
            }

            return MapToDto(prescription);
        }

        public async Task<PrescriptionDto> GetByAppointmentIdAsync(int appointmentId)
        {
            Prescription prescription = await _prescriptionRepository.GetByAppointmentIdAsync(appointmentId);

            if (prescription == null)
            {
                return null;
            }

            return MapToDto(prescription);
        }

        public async Task<List<PrescriptionDto>> GetByPatientIdAsync(int patientId)
        {
            IEnumerable<Prescription> prescriptions = await _prescriptionRepository.GetByPatientIdAsync(patientId);

            List<PrescriptionDto> result = new List<PrescriptionDto>();

            foreach (Prescription prescription in prescriptions)
            {
                result.Add(MapToDto(prescription));
            }

            return result;
        }

        public async Task<List<PrescriptionDto>> GetByDoctorIdAsync(int doctorId)
        {
            IEnumerable<Prescription> prescriptions = await _prescriptionRepository.GetByDoctorIdAsync(doctorId);

            List<PrescriptionDto> result = new List<PrescriptionDto>();

            foreach (Prescription prescription in prescriptions)
            {
                result.Add(MapToDto(prescription));
            }

            return result;
        }

        public async Task<bool> CreateAsync(CreatePrescriptionDto dto, int doctorId)
        {
            Appointment appointment = await _appointmentRepository.GetByIdAsync(dto.AppointmentId);

            if (appointment == null)
            {
                return false;
            }

            Prescription existingPrescription = await _prescriptionRepository.GetByAppointmentIdAsync(dto.AppointmentId);

            if (existingPrescription != null)
            {
                return false;
            }

            Prescription prescription = new Prescription
            {
                AppointmentId = dto.AppointmentId,
                DoctorId = doctorId,
                PatientId = appointment.PatientId,
                PrescriptionDate = DateTime.Now,
                Diagnosis = dto.Diagnosis,
                Notes = dto.Notes,
                ValidUntil = DateTime.Now.AddDays(dto.ValidDays),
                Items = new List<PrescriptionItem>()
            };

            foreach (CreatePrescriptionItemDto itemDto in dto.Items)
            {
                PrescriptionItem item = new PrescriptionItem
                {
                    MedicineName = itemDto.MedicineName,
                    Dosage = itemDto.Dosage,
                    Frequency = itemDto.Frequency,
                    Duration = itemDto.Duration,
                    Instructions = itemDto.Instructions
                };

                prescription.Items.Add(item);
            }

            await _prescriptionRepository.AddAsync(prescription);

            return true;
        }

        public async Task<byte[]> GeneratePdfAsync(int prescriptionId)
        {
            Prescription prescription = await _prescriptionRepository.GetByIdAsync(prescriptionId);

            if (prescription == null)
            {
                return null;
            }

            return GeneratePrescriptionPdf(prescription);
        }

        private byte[] GeneratePrescriptionPdf(Prescription prescription)
        {
            string html = $@"
            <html>
            <head>
                <style>
                    body {{ font-family: Arial; padding: 20px; }}
                    h1 {{ color: #2c3e50; text-align: center; }}
                    .header {{ border-bottom: 2px solid #3498db; padding-bottom: 10px; margin-bottom: 20px; }}
                    .info {{ margin-bottom: 15px; }}
                    .info label {{ font-weight: bold; }}
                    table {{ width: 100%; border-collapse: collapse; margin-top: 20px; }}
                    th, td {{ border: 1px solid #ddd; padding: 10px; text-align: left; }}
                    th {{ background-color: #3498db; color: white; }}
                    .footer {{ margin-top: 30px; text-align: right; }}
                </style>
            </head>
            <body>
                <div class='header'>
                    <h1>Medical Prescription</h1>
                </div>
                <div class='info'>
                    <label>Date:</label> {prescription.PrescriptionDate:dd.MM.yyyy}
                </div>
                <div class='info'>
                    <label>Patient:</label> {prescription.Patient?.FirstName} {prescription.Patient?.LastName}
                </div>
                <div class='info'>
                    <label>Doctor:</label> Dr. {prescription.Doctor?.FirstName} {prescription.Doctor?.LastName}
                </div>
                <div class='info'>
                    <label>Specialization:</label> {prescription.Doctor?.Specialization}
                </div>
                <div class='info'>
                    <label>Diagnosis:</label> {prescription.Diagnosis}
                </div>
                <table>
                    <tr>
                        <th>Medicine</th>
                        <th>Dosage</th>
                        <th>Frequency</th>
                        <th>Duration</th>
                        <th>Instructions</th>
                    </tr>";

            foreach (PrescriptionItem item in prescription.Items)
            {
                html += $@"
                    <tr>
                        <td>{item.MedicineName}</td>
                        <td>{item.Dosage}</td>
                        <td>{item.Frequency}</td>
                        <td>{item.Duration} days</td>
                        <td>{item.Instructions ?? "-"}</td>
                    </tr>";
            }

            html += $@"
                </table>
                <div class='info' style='margin-top: 20px;'>
                    <label>Notes:</label> {prescription.Notes ?? "-"}
                </div>
                <div class='info'>
                    <label>Valid Until:</label> {prescription.ValidUntil:dd.MM.yyyy}
                </div>
                <div class='footer'>
                    <p>Dr. {prescription.Doctor?.FirstName} {prescription.Doctor?.LastName}</p>
                    <p>License: {prescription.Doctor?.LicenseNumber}</p>
                </div>
            </body>
            </html>";

            return System.Text.Encoding.UTF8.GetBytes(html);
        }

        private PrescriptionDto MapToDto(Prescription prescription)
        {
            PrescriptionDto dto = new PrescriptionDto
            {
                Id = prescription.Id,
                AppointmentId = prescription.AppointmentId,
                PrescriptionDate = prescription.PrescriptionDate,
                Diagnosis = prescription.Diagnosis,
                Notes = prescription.Notes,
                ValidUntil = prescription.ValidUntil,
                Items = new List<PrescriptionItemDto>()
            };

            if (prescription.Patient != null)
            {
                dto.PatientName = prescription.Patient.FirstName + " " + prescription.Patient.LastName;
            }

            if (prescription.Doctor != null)
            {
                dto.DoctorName = prescription.Doctor.FirstName + " " + prescription.Doctor.LastName;
                dto.DoctorSpecialization = prescription.Doctor.Specialization;
            }

            if (prescription.Items != null)
            {
                foreach (PrescriptionItem item in prescription.Items)
                {
                    PrescriptionItemDto itemDto = new PrescriptionItemDto
                    {
                        Id = item.Id,
                        MedicineName = item.MedicineName,
                        Dosage = item.Dosage,
                        Frequency = item.Frequency,
                        Duration = item.Duration,
                        Instructions = item.Instructions
                    };

                    dto.Items.Add(itemDto);
                }
            }

            return dto;
        }
    }
}
