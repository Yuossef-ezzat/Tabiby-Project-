using BLL.Abstractions;
using BLL.Dtos;
using DAL.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.AbstractServices
{
    public interface INotificationService
    {
        public Task<Result> SendNotificationAsync(string message, NotificationType type , int UserId);

        public Task<Result<List<NotificationDto>>> GetNotificationsAsync(int UserId);
        Task<Result> MarkAsReadAsync(int notificationId);
        Task<Result> DeleteNotificationAsync(int notificationId);
        Task<Result<int>> GetUnreadCountAsync(int userId);

    }
}
