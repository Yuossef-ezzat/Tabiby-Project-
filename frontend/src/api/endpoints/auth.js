
import { apiRequest } from '../client'

export const authApi = {
  
  login: (email, password) =>
    apiRequest('/Auth/Login', { method: 'POST', body: { email, password }, skipAuth: true }),

  
  
  register: ({ fullName, email, phone, password }) =>
    apiRequest('/Auth/Register', {
      method: 'POST',
      skipAuth: true,
      body: { email, password, displayName: fullName, phoneNumber: phone },
    }),

  forgetPassword: (email) =>
    apiRequest('/Auth/forget-password', { method: 'POST', skipAuth: true, body: { email } }),

  resetPassword: ({ email, token, newPassword, confirmPassword }) =>
    apiRequest('/Auth/reset-password', {
      method: 'POST',
      skipAuth: true,
      body: { email, token, newPassword, confirmPassword },
    }),
}
