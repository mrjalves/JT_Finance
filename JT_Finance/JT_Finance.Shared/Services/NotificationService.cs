using JT_Finance.Shared.Models;
using JT_Finance.Shared.Services.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JT_Finance.Shared.Services
{
    public class NotificationService : INotificationService
    {
        private readonly List<Notification> _notifications = new();
        public event Action OnChange;

        public Task AddNotificationAsync(Notification notification)
        {
            _notifications.Add(notification);
            NotifyStateChanged();
            return Task.CompletedTask;
        }

        public Task<List<Notification>> GetNotificationsAsync()
        {
            return Task.FromResult(_notifications.OrderByDescending(n => n.Date).ToList());
        }

        public int GetUnreadCount()
        {
            return _notifications.Count(n => !n.IsRead);
        }

        public Task MarkAsReadAsync(int notificationId)
        {
            var notification = _notifications.FirstOrDefault(n => n.Id == notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                NotifyStateChanged();
            }
            return Task.CompletedTask;
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
