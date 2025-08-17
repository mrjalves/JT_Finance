using JT_Finance.Shared.Models;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JT_Finance.Shared.Services.Interfaces
{
    public interface INotificationService
    {
        event Action OnChange;
        Task<List<Notification>> GetNotificationsAsync();
        Task AddNotificationAsync(Notification notification);
        Task MarkAsReadAsync(int notificationId);
        int GetUnreadCount();
    }
}
