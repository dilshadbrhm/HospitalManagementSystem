using HospitalManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Interfaces
{
    public interface INotificationRepository
    {
        Task<List<Notification>> GetByUserIdAsync(string userId);
        Task<int> GetUnreadCountAsync(string userId);
        Task<Notification> GetByIdAsync(int id);
        Task AddAsync(Notification notification);
        Task MarkAsReadAsync(int id);
        Task MarkAllAsReadAsync(string userId);
        Task DeleteAsync(int id);
    }
}
