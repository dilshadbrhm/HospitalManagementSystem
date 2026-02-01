using HospitalManagement.Application.Dtos.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Interfaces
{
    public interface IAccountService
    {
        Task<(bool Success, string Message, string? Token, string? UserId)> RegisterAsync(RegisterDto model);
        Task<(bool Success, string Message, string Role)> LoginAsync(LoginDto model);
        Task LogoutAsync();
        Task<(bool Success, string Message)> ConfirmEmailAsync(string userId, string token);
        Task<(bool Success, string Message, string? Token)> ForgotPasswordAsync(string email);
        Task<(bool Success, string Message)> ResetPasswordAsync(ResetPasswordDto model);
    }
}
