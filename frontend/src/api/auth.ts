import { apiClient } from './client'
import type { AuthResponse } from './types'

export function register(email: string, password: string) {
  return apiClient.post<AuthResponse>('/api/auth/register', { email, password })
}

export function login(email: string, password: string) {
  return apiClient.post<AuthResponse>('/api/auth/login', { email, password })
}
