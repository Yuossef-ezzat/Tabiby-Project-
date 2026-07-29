





import { apiRequest, emptyOn404 } from '../client'

export const nursingApi = {
  searchNurses: ({ name, specialization, pageNumber = 1, pageSize = 20 } = {}) =>
    apiRequest('/Nursing/Search', {
      params: { Name: name, Specialization: specialization, PageNumber: pageNumber, PageSize: pageSize },
    }),

  request: ({ nurseId, careType }) =>
    apiRequest('/Nursing/Request', { method: 'POST', body: { nurseId, careType } }),

  myRequests: () => emptyOn404(apiRequest('/Nursing/MyRequests')),

  
  updateStatus: (requestId, status) =>
    apiRequest(`/Nursing/UpdateStatus/${requestId}`, { method: 'PUT', body: { status } }),

  cancel: (requestId) => apiRequest(`/Nursing/Cancel/${requestId}`, { method: 'POST' }),

  addReview: (requestId, { rating, comment }) =>
    apiRequest(`/Nursing/Review/${requestId}`, { method: 'POST', body: { rating, comment } }),
  getReview: (requestId) => apiRequest(`/Nursing/Review/${requestId}`),
}
