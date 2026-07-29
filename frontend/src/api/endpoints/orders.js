
import { apiRequest, emptyOn404 } from '../client'



export const ORDER_STATUS = {
  Pending: 0,
  Processing: 1,
  Shipped: 2,
  Delivered: 3,
  Cancelled: 4,
  Rejected: 5,
}
export const ORDER_STATUS_NAMES = Object.keys(ORDER_STATUS)

export const ordersApi = {
  
  myOrders: () => emptyOn404(apiRequest('/Order/MyOrders')),
  getMine: (orderId) => apiRequest(`/Order/GetMyOrder/${orderId}`),
  cancel: (orderId) => apiRequest(`/Order/Cancel/${orderId}`, { method: 'POST' }),
  
  
  remove: (orderId) => apiRequest('/Order/Delete', { method: 'DELETE', params: { OrderId: orderId } }),
  create: ({ firstName, lastName, city, country, street }) =>
    apiRequest('/Order/Create', {
      method: 'POST',
      body: { address: { firstName, lastName, city, country, street } },
    }),

  
  getAll: () => apiRequest('/Order/Orders'),
  getForMerchant: (id) => apiRequest(`/Order/GetOrder/${id}`),
  
  updateStatus: (orderId, statusName) =>
    apiRequest(`/Order/Orders/${orderId}/Status`, {
      method: 'PUT',
      body: { status: ORDER_STATUS[statusName] },
    }),
  removeByPharmacist: (orderId) => apiRequest('/Order/Pharmacist/Delete', { method: 'DELETE', params: { OrderId: orderId } }),
}
