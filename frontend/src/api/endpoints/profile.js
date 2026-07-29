
import { apiRequest } from '../client'

export const profileApi = {
  getMyProfile: () => apiRequest('/ProfileUser'),
  updateMyProfile: (profile) => apiRequest('/ProfileUser', { method: 'PUT', body: profile }),
}
