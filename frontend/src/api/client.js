
export const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'https://localhost:7292/api'

const TOKEN_KEY = 'tabiby_token'

export function getToken() {
  return localStorage.getItem(TOKEN_KEY)
}

export function setToken(token) {
  if (token) localStorage.setItem(TOKEN_KEY, token)
  else localStorage.removeItem(TOKEN_KEY)
}



let onUnauthorized = null
export function registerUnauthorizedHandler(fn) {
  onUnauthorized = fn
}

function extractErrorMessage(body, fallback) {
  if (!body) return fallback
  if (typeof body === 'string') return body
  
  
  
  
  if (body.errors && typeof body.errors === 'object') {
    return Object.values(body.errors).flat().join(', ')
  }
  if (body.errorMsg) return body.errorMsg
  if (body.message) return body.message
  if (body.title) return body.title
  if (Array.isArray(body)) return body.join(', ')
  if (typeof body === 'object') {
    const values = Object.values(body)
    const strings = values.filter((v) => typeof v === 'string')
    if (strings.length) return strings.join(', ')
  }
  return fallback
}

export async function apiRequest(
  path,
  { method = 'GET', body, form, skipAuth = false, params } = {},
) {
  const token = skipAuth ? null : getToken()

  let url = `${API_BASE_URL}${path}`
  if (params) {
    const usp = new URLSearchParams()
    Object.entries(params).forEach(([k, v]) => {
      if (v !== undefined && v !== null && v !== '') usp.append(k, v)
    })
    const qs = usp.toString()
    if (qs) url += `?${qs}`
  }

  const headers = {}
  if (token) headers['Authorization'] = `Bearer ${token}`
  if (!form && body !== undefined) headers['Content-Type'] = 'application/json'

  let res
  try {
    res = await fetch(url, {
      method,
      headers,
      body: form ? form : body !== undefined ? JSON.stringify(body) : undefined,
    })
  } catch {
    throw new Error('Could not reach the server. Please check your connection and try again.')
  }

  if (res.status === 401 && !skipAuth) {
    onUnauthorized?.()
    throw new Error('Your session has expired. Please log in again.')
  }

  if (!res.ok) {
    let parsed = null
    try {
      parsed = await res.json()
    } catch {
      
    }
    const error = new Error(extractErrorMessage(parsed, `Request failed (${res.status})`))
    error.status = res.status
    throw error
  }

  if (res.status === 204) return null

  const contentType = res.headers.get('content-type') || ''
  if (contentType.includes('application/json')) return res.json()
  return null
}

export async function emptyOn404(promise) {
  try {
    return await promise
  } catch (err) {
    if (err.status === 404) return []
    throw err
  }
}
