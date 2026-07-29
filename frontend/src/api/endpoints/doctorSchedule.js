

import { apiRequest } from '../client'

export const doctorScheduleApi = {
  create: ({ dayOfWeek, startTime, endTime, isAvailable = true }) =>
    apiRequest('/DoctorSchedule', {
      method: 'POST',
      body: { dayOfWeek, startTime, endTime, isAvailable },
    }),

  getMine: () => apiRequest('/DoctorSchedule/Doctor'),

  update: (scheduleId, { id, dayOfWeek, startTime, endTime, isAvailable }) =>
    apiRequest(`/DoctorSchedule/${scheduleId}`, {
      method: 'PUT',
      body: { id, dayOfWeek, startTime, endTime, isAvailable },
    }),

  remove: (scheduleId) => apiRequest(`/DoctorSchedule/${scheduleId}`, { method: 'DELETE' }),
}
