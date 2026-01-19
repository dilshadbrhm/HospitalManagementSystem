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
        Task<(bool Success, string Message)> RegisterAsync(RegisterDto model);
        Task<(bool Success, string Message)> LoginAsync(LoginDto model);
        Task LogoutAsync();
    }
}
