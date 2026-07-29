import { createContext, useContext, useState, useCallback, useEffect, useRef } from 'react'
import { authApi } from '../api/endpoints/auth'
import { getToken, setToken, registerUnauthorizedHandler } from '../api/client'
import { decodeJwt, isTokenExpired } from '../utils/jwt'

const AuthContext = createContext(null)


export const ROLE_HOME_ROUTE = {
  Patient: '/app/dashboard',
  Doctor: '/app/doctor/dashboard',
  Pharmacist: '/app/pharmacist/inventory',
  Nurse: '/app/nurse/requests',
  Admin: '/app/admin/accounts',
}

function userFromToken(token, fallbackFullName) {
  const decoded = decodeJwt(token)
  if (!decoded || !decoded.role) return null
  return {
    id: decoded.id,
    email: decoded.email,
    fullName: fallbackFullName || decoded.userName || decoded.email,
    role: decoded.role,
  }
}

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null)
  const [isLoading, setIsLoading] = useState(true)
  const logoutRef = useRef()

  const logout = useCallback(() => {
    setToken(null)
    setUser(null)
  }, [])
  logoutRef.current = logout

  
  useEffect(() => {
    registerUnauthorizedHandler(() => logoutRef.current())

    const token = getToken()
    if (token) {
      const decoded = decodeJwt(token)
      if (decoded && !isTokenExpired(decoded)) {
        setUser(userFromToken(token))
      } else {
        setToken(null)
      }
    }
    setIsLoading(false)
  }, [])

  const login = useCallback(async ({ email, password }) => {
    setIsLoading(true)
    try {
      const result = await authApi.login(email, password)
      setToken(result.token)
      const loggedInUser = userFromToken(result.token, result.fullName)
      if (!loggedInUser) throw new Error('Could not read your account role from the server.')
      setUser(loggedInUser)
      return loggedInUser
    } finally {
      setIsLoading(false)
    }
  }, [])

  const register = useCallback(async (payload) => {
    setIsLoading(true)
    try {
      const result = await authApi.register(payload)
      setToken(result.token)
      const registeredUser = userFromToken(result.token, result.fullName)
      setUser(registeredUser)
      return registeredUser
    } finally {
      setIsLoading(false)
    }
  }, [])

  return (
    <AuthContext.Provider value={{ user, isLoading, login, register, logout }}>
      {children}
    </AuthContext.Provider>
  )
}

export function useAuth() {
  const ctx = useContext(AuthContext)
  if (!ctx) throw new Error('useAuth must be used within AuthProvider')
  return ctx
}
