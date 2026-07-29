

import { apiRequest, emptyOn404 } from '../client'

export const consultationsApi = {
  request: (doctorId) =>
    apiRequest('/Consultation/RequestConsultation', { method: 'POST', body: { doctorId } }),
  myConsultations: () => emptyOn404(apiRequest('/Consultation/MyConsultations')),
  getOne: (id) => apiRequest(`/Consultation/GetMyConsultationById/${id}`),
  remove: (id) => apiRequest(`/Consultation/DeleteConsultation/${id}`, { method: 'DELETE' }),
  
  
  
  updateStatus: (id, status) =>
    apiRequest(`/Consultation/UpdateConsultationStatus/${id}`, {
      method: 'PUT',
      body: { status },
    }),

  
  getMessages: (consultationId) => apiRequest(`/ConsultationChat/${consultationId}/messages`),
  sendMessage: (consultationId, content) =>
    apiRequest(`/ConsultationChat/${consultationId}/messages`, {
      method: 'POST',
      body: { content },
    }),
  markMessagesRead: (consultationId) =>
    apiRequest(`/ConsultationChat/${consultationId}/read`, { method: 'POST' }),
  unreadCount: (consultationId) => apiRequest(`/ConsultationChat/${consultationId}/unread-count`),

  
  addReview: (consultationId, { rating, comment }) =>
    apiRequest(`/ConsultationReview/${consultationId}`, {
      method: 'POST',
      body: { rating, comment },
    }),
  getReview: (consultationId) => apiRequest(`/ConsultationReview/${consultationId}`),
}
