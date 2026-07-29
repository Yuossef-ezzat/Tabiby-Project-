

import { apiRequest } from '../client'

export const doctorsApi = {
  
  search: ({ name, specialization, location, pageNumber = 1, pageSize = 20 } = {}) =>
    apiRequest('/Consultation/GetAllDoctors', {
      params: { Name: name, Specialization: specialization, Location: location, PageNumber: pageNumber, PageSize: pageSize },
    }),
}
