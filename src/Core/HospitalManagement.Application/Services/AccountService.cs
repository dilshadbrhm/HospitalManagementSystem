using HospitalManagement.Application.Dtos.Account;
using HospitalManagement.Application.Interfaces;
using HospitalManagement.Domain;
using HospitalManagement.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Services
{

    public class AccountService : IAccountService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IPatientRepository _patientRepository;

        public AccountService(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IDoctorRepository doctorRepository,
            IPatientRepository patientRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _doctorRepository = doctorRepository;
            _patientRepository = patientRepository;
        }

        public async Task<(bool Success, string Message)> RegisterAsync(RegisterDto model)
        {
            AppUser? existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
                return (false, "This email is already in use");

            var user = new AppUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.Phone,
                CreatedAt = DateTime.Now
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return (false, string.Join(", ", result.Errors.Select(e => e.Description)));

   
            if (!await _roleManager.RoleExistsAsync(model.UserType))
                await _roleManager.CreateAsync(new IdentityRole(model.UserType));

            await _userManager.AddToRoleAsync(user, model.UserType);


            if (model.UserType == "Doctor")
            {
                var existingDoctor = await _doctorRepository.GetByUserIdAsync(user.Id);

                if (existingDoctor == null)
                {
                    var doctor = new Doctor
                    {
                        UserId = user.Id,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        Phone = model.Phone
                    };

                    if (model.DepartmentId != null)
                        doctor.DepartmentId = model.DepartmentId.Value;
                    else
                        doctor.DepartmentId = 0;

                    if (model.Specialization != null)
                        doctor.Specialization = model.Specialization;
                    else
                        doctor.Specialization = "";

                    if (model.LicenseNumber != null)
                        doctor.LicenseNumber = model.LicenseNumber;
                    else
                        doctor.LicenseNumber = "";

                    await _doctorRepository.AddAsync(doctor);
                }
                else
                {
                    return (false, "This user is already registered as a doctor");
                }
            }
            else if (model.UserType == "Patient")
            {
                var existingPatient = await _patientRepository.GetByUserIdAsync(user.Id);

                if (existingPatient == null)
                {
                    var patient = new Patient
                    {
                        UserId = user.Id,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        Phone = model.Phone
                    };
                    await _patientRepository.AddAsync(patient);
                }
                else
                {
                    return (false, "This user is already registered as a patient");
                }
            }
            else
            {
                return (false, "User type not selected");
            }

            return (true, "Registration completed successfully");
        }

        public async Task<(bool Success, string Message)> LoginAsync(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return (false, "The email or password is incorrect");

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
            if (!result.Succeeded)
                return (false, "The email or password is incorrect");

            return (true, "You have successfully logged in");
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }


}
