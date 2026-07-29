





import { apiRequest } from '../client'

export const medicationsApi = {
  getAll: (searchName) => apiRequest('/Medication/GetAll', { params: { SearchName: searchName } }),
  getById: (id) => apiRequest(`/Medication/GetById/${id}`),

  
  create: ({ name, price, stock, isAvailable, image }) => {
    const form = new FormData()
    form.append('Name', name)
    form.append('Price', price)
    form.append('Stock', stock)
    form.append('IsAvailable', isAvailable)
    if (image) form.append('Image', image)
    return apiRequest('/Medication/CreateMedication', { method: 'POST', form })
  },

  
  
  
  update: (id, { name, price, stock, isAvailable }) =>
    apiRequest(`/Medication/UpdateMedication/${id}`, {
      method: 'POST',
      body: { name, price, stock, isAvailable },
    }),

  remove: (id) => apiRequest(`/Medication/DeleteMedication/${id}`, { method: 'DELETE' }),
}
