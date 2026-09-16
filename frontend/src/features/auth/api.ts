import { apiClient } from '../../lib/api-client'
import type { AuthResponse, LoginPayload } from './types'

export function login(payload: LoginPayload) {
  return apiClient.post<AuthResponse>('/auth/login', payload)
}