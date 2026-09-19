export interface AuthResponse {
  token: string
  expiresAtUtc: string
}

export interface Habit {
  id: number
  name: string
  description: string | null
  isArchived: boolean
  createdAtUtc: string
}

export interface HabitLog {
  id: number
  date: string
  completed: boolean
  notes: string | null
}
