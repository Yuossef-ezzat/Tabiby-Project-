
import { apiRequest } from '../client'

export const appointmentsApi = {
  
  book: ({ doctorId, scheduleId, appointmentDate, notes }) =>
    apiRequest('/Appointment/Book', {
      method: 'POST',
      body: { doctorId, scheduleId, appointmentDate, notes },
    }),
  myAppointments: () => apiRequest('/Appointment/MyAppointments'),
  update: (appointmentId, { scheduleId, appointmentDate, notes }) =>
    apiRequest(`/Appointment/Update/${appointmentId}`, {
      method: 'PUT',
      body: { scheduleId, appointmentDate, notes },
    }),

  
  getOne: (appointmentId) => apiRequest(`/Appointment/GetAppointment/${appointmentId}`),
  cancel: (appointmentId) => apiRequest(`/Appointment/Cancel/${appointmentId}`, { method: 'POST' }),

  
  doctorAppointments: () => apiRequest('/Appointment/DoctorAppointments'),
  confirm: (appointmentId) => apiRequest(`/Appointment/Confirm/${appointmentId}`, { method: 'POST' }),
  complete: (appointmentId) => apiRequest(`/Appointment/Complete/${appointmentId}`, { method: 'POST' }),

  
  
  availableSlots: (doctorId, date) =>
    apiRequest('/Appointment/AvailableSlots', { params: { doctorId, date } }),
}
