import { apiClient } from './client'
import type { Habit, HabitLog } from './types'

export function listHabits() {
  return apiClient.get<Habit[]>('/api/habits')
}

export function createHabit(name: string, description?: string) {
  return apiClient.post<Habit>('/api/habits', { name, description })
}

export function getHabitLogs(habitId: number, from?: string, to?: string) {
  const params = new URLSearchParams()
  if (from) params.set('from', from)
  if (to) params.set('to', to)
  const query = params.toString()
  return apiClient.get<HabitLog[]>(`/api/habits/${habitId}/logs${query ? `?${query}` : ''}`)
}

export function upsertHabitLog(habitId: number, date: string, completed: boolean, notes?: string) {
  return apiClient.put<HabitLog>(`/api/habits/${habitId}/logs/${date}`, { completed, notes })
}
