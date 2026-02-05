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

        public async Task<string> GenerateHtmlAsync(int prescriptionId)
        {
            Prescription prescription = await _prescriptionRepository.GetByIdAsync(prescriptionId);

            if (prescription == null)
            {
                return null;
            }

            string patientName = "";
            if (prescription.Patient != null)
            {
                patientName = prescription.Patient.FirstName + " " + prescription.Patient.LastName;
            }

            string doctorName = "";
            string specialization = "";
            string license = "";
            if (prescription.Doctor != null)
            {
                doctorName = prescription.Doctor.FirstName + " " + prescription.Doctor.LastName;
                specialization = prescription.Doctor.Specialization;
                license = prescription.Doctor.LicenseNumber;
            }

            string html = "<!DOCTYPE html><html><head><meta charset='UTF-8'>";
            html += "<title>Prescription</title>";
            html += "<style>";
            html += "body { font-family: Arial, sans-serif; padding: 40px; max-width: 800px; margin: 0 auto; }";
            html += "h1 { color: #2c3e50; text-align: center; border-bottom: 3px solid #3498db; padding-bottom: 15px; }";
            html += ".info { margin: 10px 0; font-size: 16px; }";
            html += ".info strong { display: inline-block; width: 150px; }";
            html += "table { width: 100%; border-collapse: collapse; margin: 25px 0; }";
            html += "th { background: #3498db; color: white; padding: 12px; text-align: left; }";
            html += "td { border: 1px solid #ddd; padding: 10px; }";
            html += "tr:nth-child(even) { background: #f9f9f9; }";
            html += ".footer { margin-top: 50px; text-align: right; border-top: 1px solid #ddd; padding-top: 20px; }";
            html += ".print-btn { background: #3498db; color: white; padding: 10px 30px; border: none; cursor: pointer; font-size: 16px; border-radius: 5px; }";
            html += ".print-btn:hover { background: #2980b9; }";
            html += "@media print { .no-print { display: none; } }";
            html += "</style></head><body>";

            html += "<h1>Medical Prescription</h1>";

            html += "<div class='info'><strong>Date:</strong> " + prescription.PrescriptionDate.ToString("dd.MM.yyyy") + "</div>";
            html += "<div class='info'><strong>Patient:</strong> " + patientName + "</div>";
            html += "<div class='info'><strong>Doctor:</strong> Dr. " + doctorName + "</div>";
            html += "<div class='info'><strong>Specialization:</strong> " + specialization + "</div>";
            html += "<div class='info'><strong>Diagnosis:</strong> " + prescription.Diagnosis + "</div>";

            html += "<table><thead><tr>";
            html += "<th>Medicine</th><th>Dosage</th><th>Frequency</th><th>Duration</th><th>Instructions</th>";
            html += "</tr></thead><tbody>";

            if (prescription.Items != null)
            {
                foreach (PrescriptionItem item in prescription.Items)
                {
                    html += "<tr>";
                    html += "<td>" + item.MedicineName + "</td>";
                    html += "<td>" + item.Dosage + "</td>";
                    html += "<td>" + item.Frequency + "</td>";
                    html += "<td>" + item.Duration + " days</td>";
                    html += "<td>" + (item.Instructions ?? "-") + "</td>";
                    html += "</tr>";
                }
            }

            html += "</tbody></table>";

            string notes = prescription.Notes ?? "-";
            string validUntil = prescription.ValidUntil.HasValue ? prescription.ValidUntil.Value.ToString("dd.MM.yyyy") : "-";

            html += "<div class='info'><strong>Notes:</strong> " + notes + "</div>";
            html += "<div class='info'><strong>Valid Until:</strong> " + validUntil + "</div>";

            html += "<div class='footer'>";
            html += "<p><strong>Dr. " + doctorName + "</strong></p>";
            html += "<p>License: " + license + "</p>";
            html += "</div>";

            html += "<div class='no-print' style='text-align: center; margin-top: 30px;'>";
            html += "<button class='print-btn' onclick='window.print()'>Print / Save as PDF</button>";
            html += "</div>";

            html += "</body></html>";

            return html;
        }

        private byte[] GeneratePrescriptionPdf(Prescription prescription)
        {
            string html = "<html><head><style>";
            html += "body { font-family: Arial; padding: 20px; }";
            html += "h1 { color: #2c3e50; text-align: center; }";
            html += "</style></head><body>";
            html += "<h1>Medical Prescription</h1>";
            html += "</body></html>";

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
