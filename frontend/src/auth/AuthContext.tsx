import { createContext, useContext, useState, type ReactNode } from 'react'
import * as authApi from '../api/auth'
import { setAuthToken } from '../api/client'

const TOKEN_STORAGE_KEY = 'calendar-scheduler.token'

interface AuthContextValue {
  isAuthenticated: boolean
  login: (email: string, password: string) => Promise<void>
  register: (email: string, password: string) => Promise<void>
  logout: () => void
}

const AuthContext = createContext<AuthContextValue | null>(null)

export function AuthProvider({ children }: { children: ReactNode }) {
  const [token, setToken] = useState<string | null>(() => {
    const stored = localStorage.getItem(TOKEN_STORAGE_KEY)
    setAuthToken(stored)
    return stored
  })

  function applyToken(newToken: string) {
    localStorage.setItem(TOKEN_STORAGE_KEY, newToken)
    setAuthToken(newToken)
    setToken(newToken)
  }

  async function login(email: string, password: string) {
    const response = await authApi.login(email, password)
    applyToken(response.token)
  }

  async function register(email: string, password: string) {
    const response = await authApi.register(email, password)
    applyToken(response.token)
  }

  function logout() {
    localStorage.removeItem(TOKEN_STORAGE_KEY)
    setAuthToken(null)
    setToken(null)
  }

  return (
    <AuthContext.Provider value={{ isAuthenticated: token !== null, login, register, logout }}>
      {children}
    </AuthContext.Provider>
  )
}

export function useAuth() {
  const context = useContext(AuthContext)
  if (!context) throw new Error('useAuth must be used within an AuthProvider')
  return context
}
