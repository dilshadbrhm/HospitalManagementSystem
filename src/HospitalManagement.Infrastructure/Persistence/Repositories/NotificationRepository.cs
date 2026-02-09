using HospitalManagement.Application.Interfaces;
using HospitalManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Infrastructure.Persistence.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly AppDbContext _context;

        public NotificationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Notification>> GetByUserIdAsync(string userId)
        {
            List<Notification> notifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsDeleted)
                .OrderByDescending(n => n.SentAt)
                .ToListAsync();

            return notifications;
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            int count = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead && !n.IsDeleted)
                .CountAsync();

            return count;
        }

        public async Task<Notification> GetByIdAsync(int id)
        {
            Notification notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == id && !n.IsDeleted);

            return notification;
        }

        public async Task AddAsync(Notification notification)
        {
            notification.CreatedAt = DateTime.Now;
            notification.SentAt = DateTime.Now;
            notification.IsRead = false;
            notification.IsDeleted = false;

            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
        }

        public async Task MarkAsReadAsync(int id)
        {
            Notification notification = await _context.Notifications.FindAsync(id);

            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkAllAsReadAsync(string userId)
        {
            List<Notification> notifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            Notification notification = await _context.Notifications.FindAsync(id);

            if (notification != null)
            {
                notification.IsDeleted = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}
