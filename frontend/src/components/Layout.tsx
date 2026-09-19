import type { ReactNode } from 'react'
import { useAuth } from '../auth/AuthContext'

export function Layout({ children }: { children: ReactNode }) {
  const { isAuthenticated, logout } = useAuth()

  return (
    <div className="layout">
      <header className="app-header">
        <span className="brand">Calendar Scheduler</span>
        {isAuthenticated && (
          <button type="button" onClick={logout}>
            Log out
          </button>
        )}
      </header>
      <main>{children}</main>
    </div>
  )
}
