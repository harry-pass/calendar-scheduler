import { useEffect, useState, type FormEvent } from 'react'
import * as habitsApi from '../api/habits'
import type { Habit } from '../api/types'

function todayIso() {
  return new Date().toLocaleDateString('en-CA') // yyyy-MM-dd, local time
}

interface HabitWithToday extends Habit {
  completedToday: boolean
}

export function HabitsPage() {
  const [habits, setHabits] = useState<HabitWithToday[] | null>(null)
  const [newHabitName, setNewHabitName] = useState('')
  const [error, setError] = useState<string | null>(null)
  const today = todayIso()

  useEffect(() => {
    void loadHabits()
  }, [])

  async function loadHabits() {
    try {
      const list = await habitsApi.listHabits()
      const withToday = await Promise.all(
        list.map(async (habit) => {
          const logs = await habitsApi.getHabitLogs(habit.id, today, today)
          return { ...habit, completedToday: logs[0]?.completed ?? false }
        }),
      )
      setHabits(withToday)
    } catch {
      setError('Could not load habits.')
    }
  }

  async function handleAddHabit(event: FormEvent) {
    event.preventDefault()
    if (!newHabitName.trim()) return
    try {
      await habitsApi.createHabit(newHabitName.trim())
      setNewHabitName('')
      await loadHabits()
    } catch {
      setError('Could not create habit.')
    }
  }

  async function toggleToday(habit: HabitWithToday) {
    const nextCompleted = !habit.completedToday
    setHabits((prev) =>
      prev?.map((h) => (h.id === habit.id ? { ...h, completedToday: nextCompleted } : h)) ?? null,
    )
    try {
      await habitsApi.upsertHabitLog(habit.id, today, nextCompleted)
    } catch {
      setError('Could not save that update.')
      await loadHabits()
    }
  }

  return (
    <div className="habits-page">
      <h1>Today's habits</h1>
      {error && <p className="error">{error}</p>}

      <form className="add-habit" onSubmit={handleAddHabit}>
        <input
          type="text"
          placeholder="New habit, e.g. Drink water"
          value={newHabitName}
          onChange={(e) => setNewHabitName(e.target.value)}
        />
        <button type="submit">Add</button>
      </form>

      {habits === null && <p>Loading…</p>}
      {habits?.length === 0 && <p>No habits yet — add your first one above.</p>}

      <ul className="habit-list">
        {habits?.map((habit) => (
          <li key={habit.id} className="habit-row">
            <label>
              <input
                type="checkbox"
                checked={habit.completedToday}
                onChange={() => void toggleToday(habit)}
              />
              {habit.name}
            </label>
          </li>
        ))}
      </ul>
    </div>
  )
}
