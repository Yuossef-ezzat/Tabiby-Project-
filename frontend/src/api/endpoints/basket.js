
import { apiRequest } from '../client'

export const basketApi = {
  get: () => apiRequest('/Basket'),
  addItem: (medicationId, quantity) =>
    apiRequest('/Basket/AddItem', { method: 'POST', body: { medicationId, quantity } }),
  
  updateItem: (medicationId, quantity) =>
    apiRequest(`/Basket/UpdateItem/${medicationId}`, { method: 'PUT', body: quantity }),
  removeItem: (medicationId) =>
    apiRequest(`/Basket/RemoveItem/${medicationId}`, { method: 'DELETE' }),
  clear: () => apiRequest('/Basket/Clear', { method: 'DELETE' }),
}
