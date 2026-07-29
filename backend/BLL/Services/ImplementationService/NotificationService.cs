using BLL.Abstractions;
using BLL.Abstractions.Errrors;
using BLL.Dtos;
using BLL.Hubs;
using BLL.Services.AbstractServices;
using DAL.Models;
using DAL.Repository;
using DAL.Shared.Enums;
using DAL.Specifications.NotificationSpecs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;


namespace BLL.Services.ImplementationService
{
    public class NotificationService(IUnitOfWork _unitOfWork , IHubContext<NotificationHub> hubContext) : INotificationService
    {
        public async Task<Result> DeleteNotificationAsync(int notificationId)
        {
            var notification = await _unitOfWork.GetRepository<Notification>().GetByIdAsync(notificationId);
            if(notification == null) 
                return Result.Failure(NotificationError.NotificationNotFound(notificationId));

            _unitOfWork.GetRepository<Notification>().Delete(notification);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result<List<NotificationDto>>> GetNotificationsAsync(int UserId)
        {
            var notifications = await _unitOfWork.GetRepository<Notification>().GetAllAsync(new NotificationsByUserIdSpecs(UserId));

            return Result<List<NotificationDto>>.Success(notifications
                .Select(n => new NotificationDto
                {
                    Id = n.Id,
                    Message = n.Message,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt
                }).ToList());
        }

        public async Task<Result<int>> GetUnreadCountAsync(int userId)
        {
            var spec = new UnreadNotificationsForUserSpecification(userId);

            var unreadNotifications = await _unitOfWork.GetRepository<Notification>().GetAllAsync(spec);
            return Result<int>.Success( unreadNotifications.Count());
        }

        public async Task<Result> MarkAsReadAsync(int notificationId)
        {
            var notification = await _unitOfWork.GetRepository<Notification>().GetByIdAsync(notificationId);
            if (notification == null)
                return Result.Failure(NotificationError.NotificationNotFound(notificationId));
            if (notification.IsRead)
                return Result.Success();
            notification.IsRead = true;
            _unitOfWork.GetRepository<Notification>().Update(notification);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result> SendNotificationAsync(string message, NotificationType Type, int UserId)
        {
            var notification = new Notification
            {
                UserId = UserId,
                Message = message,
                Type = Type,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.GetRepository<Notification>().AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();

            try { 
                await hubContext.Clients.User(UserId.ToString()).SendAsync("NewNotification", new NotificationDto
                {
                    Id = notification.Id,
                    Message = message,
                    IsRead = false,
                    CreatedAt = notification.CreatedAt
                });
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine($"Error sending notification via SignalR: {ex.Message}");
            }
            return Result.Success();
        }
    }
}
