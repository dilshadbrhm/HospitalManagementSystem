using HospitalManagement.Application.Interfaces;
using HospitalManagement.Application.Services;
using HospitalManagement.Infrastructure.Persistence;
using HospitalManagement.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace HospitalManagement.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("Default")));

            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IDoctorRepository, DoctorRepository>();
            services.AddScoped<IPatientRepository, PatientRepository>();
            services.AddScoped<ITimeSlotRepository, TimeSlotRepository>();
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IPatientRepository, PatientRepository>();
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            services.AddScoped<IPrescriptionRepository, PrescriptionRepository>();
            services.AddScoped<IPrescriptionService, PrescriptionService>();
            services.AddScoped<IBlogRepository, BlogRepository>();
            services.AddScoped<IBlogCategoryRepository, BlogCategoryRepository>();
            services.AddScoped<ILabResultRepository, LabResultRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            return services;
        }
    }
}
