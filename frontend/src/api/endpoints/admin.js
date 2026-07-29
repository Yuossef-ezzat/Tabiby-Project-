






import { apiRequest } from '../client'

export const adminApi = {
  getUsers: ({ name, email, role, isActive, pageNumber = 1, pageSize = 20 } = {}) =>
    apiRequest('/Admin/Users', {
      params: {
        Name: name,
        Email: email,
        Role: role,
        IsActive: isActive,
        PageNumber: pageNumber,
        PageSize: pageSize,
      },
    }),

  createDoctor: ({ email, password, fullName, phone, specialty, location }) =>
    apiRequest('/Admin/Doctors', {
      method: 'POST',
      body: { email, password, displayName: fullName, phoneNumber: phone, specialty, location },
    }),

  createNurse: ({ email, password, fullName, phone, specialization }) =>
    apiRequest('/Admin/Nurses', {
      method: 'POST',
      body: { email, password, displayName: fullName, phoneNumber: phone, specialization },
    }),

  createPharmacist: ({ email, password, fullName, phone, pharmacyName }) =>
    apiRequest('/Admin/Pharmacists', {
      method: 'POST',
      body: { email, password, displayName: fullName, phoneNumber: phone, pharmacyName },
    }),

  deleteUser: (userId) => apiRequest(`/Admin/Users/${userId}`, { method: 'DELETE' }),

  addRole: (userId, roleData) =>
    apiRequest(`/Admin/Users/${userId}/Roles/Add`, { method: 'POST', body: roleData }),
  removeRole: (userId, roleData) =>
    apiRequest(`/Admin/Users/${userId}/Roles/Remove`, { method: 'DELETE', body: roleData }),
  toggleUserActive: (userId) => apiRequest(`/Admin/Users/${userId}/ToggleActive`, { method: 'PUT' }),
  getSpecialties: () => apiRequest('/Admin/Specialties'),
}
