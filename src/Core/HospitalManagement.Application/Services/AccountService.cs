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
        public async Task<(bool Success, string Message, string? Token, string? UserId)> RegisterAsync(RegisterDto model)
        {
            AppUser? existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
                return (false, "This email is already in use", null, null);

            AppUser user = new AppUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.Phone,
                CreatedAt = DateTime.Now
            };

            IdentityResult result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return (false, string.Join(", ", result.Errors.Select(e => e.Description)), null, null);

            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);


            if (!await _roleManager.RoleExistsAsync(model.UserType))
                await _roleManager.CreateAsync(new IdentityRole(model.UserType));

            await _userManager.AddToRoleAsync(user, model.UserType);

            if (model.UserType == "Doctor")
            {
                Doctor existingDoctor = await _doctorRepository.GetByUserIdAsync(user.Id);

                if (existingDoctor == null)
                {
                    Doctor doctor = new Doctor
                    {
                        UserId = user.Id,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        Phone = model.Phone,
                        DepartmentId = model.DepartmentId ?? 0,
                        Specialization = model.Specialization ?? "",
                        LicenseNumber = model.LicenseNumber ?? ""
                    };
                    await _doctorRepository.AddAsync(doctor);
                }
                else
                {
                    return (false, "This user is already registered as a doctor", null, null);
                }
            }

            else if (model.UserType == "Patient")
            {
                Patient existingPatient = await _patientRepository.GetByUserIdAsync(user.Id);

                if (existingPatient == null)
                {
                    Patient patient = new Patient
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
                    return (false, "This user is already registered as a patient", null, null);
                }
            }
            else
            {
                return (false, "User type not selected", null, null);
            }

            return (true, "Registration completed. Please check your email to confirm.", token, user.Id);
        }

        public async Task<(bool Success, string Message)> ConfirmEmailAsync(string userId, string token)
        {
            AppUser user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return (false, "User not found");

            IdentityResult result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
                return (false, "Email confirmation failed");

            return (true, "Email confirmed successfully. You can now login.");
        }


        public async Task<(bool Success, string Message)> LoginAsync(LoginDto model)
        {
            AppUser user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return (false, "The email or password is incorrect");


            if (!await _userManager.IsEmailConfirmedAsync(user))
                return (false, "Please confirm your email first");

            SignInResult result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
            if (!result.Succeeded)
                return (false, "The email or password is incorrect");

            return (true, "You have successfully logged in");
        }

        public async Task<(bool Success, string Message, string? Token)> ForgotPasswordAsync(string email)
        {
            AppUser user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return (true, "If this email exists, a reset link will be sent", null);

            string token = await _userManager.GeneratePasswordResetTokenAsync(user);

            return (true, "Password reset link sent to your email", token);
        }

        public async Task<(bool Success, string Message)> ResetPasswordAsync(ResetPasswordDto model)
        {
            AppUser? user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return (false, "Invalid request");

            IdentityResult result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (!result.Succeeded)
                return (false, string.Join(", ", result.Errors.Select(e => e.Description)));

            return (true, "Password reset successfully. You can now login.");
        }

  
        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }


}
