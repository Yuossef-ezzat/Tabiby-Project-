


import { apiRequest } from '../client'

export const notificationsApi = {
  getAll: () => apiRequest('/Notification/GetNotifications'),
  getUnreadCount: () => apiRequest('/Notification/GetUnreadCount'),
  markAsRead: (notificationId) =>
    apiRequest('/Notification/MarkAsRead', { method: 'POST', params: { notificationId } }),
  remove: (notificationId) =>
    apiRequest('/Notification/DeleteNotification', { method: 'DELETE', params: { notificationId } }),
}
