import { useEffect, useState } from 'react'
import { Menu, Bell } from 'lucide-react'
import { Link } from 'react-router-dom'
import { useAuth } from '../../context/AuthContext'
import { notificationsApi } from '../../api/endpoints/notifications'
import { createNotificationConnection } from '../../api/signalr'



const NOTIFICATIONS_ROUTE_BY_ROLE = {
  Patient: '/app/notifications',
  Doctor: '/app/notifications',
  Pharmacist: '/app/notifications',
  Nurse: '/app/notifications',
}

export default function Topbar({ onMenuClick, title }) {
  const { user } = useAuth()
  const [unread, setUnread] = useState(0)
  const initials = (user?.fullName || 'S A')
    .split(' ')
    .map((p) => p[0])
    .slice(0, 2)
    .join('')
  const notificationsRoute = NOTIFICATIONS_ROUTE_BY_ROLE[user?.role]

  useEffect(() => {
    notificationsApi.getUnreadCount().then(setUnread).catch(() => {})
    const connection = createNotificationConnection()
    connection.on('NewNotification', () => setUnread((n) => n + 1))
    connection.start().catch(() => {})

    const handleRead = (e) => {
      const count = e.detail?.count || 1
      setUnread((prev) => Math.max(0, prev - count))
    }
    window.addEventListener('NotificationRead', handleRead)

    return () => {
      connection.stop()
      window.removeEventListener('NotificationRead', handleRead)
    }
  }, [])

  return (
    <header className="sticky top-0 z-20 flex items-center gap-4 border-b border-mist-200 bg-mist-50/80 px-5 py-4 backdrop-blur-md sm:px-8">
      <button onClick={onMenuClick} className="text-slate-600 lg:hidden" aria-label="Open menu">
        <Menu size={22} />
      </button>

      <h1 className="font-display text-lg font-bold text-ink-900 sm:text-xl">{title}</h1>

      <div className="ml-auto flex items-center gap-3 sm:gap-4">


        {user?.role !== 'Admin' && (
          <Link
            to={notificationsRoute || '/app/notifications'}
            className="relative flex h-10 w-10 items-center justify-center rounded-full border border-mist-200 bg-white text-slate-600 hover:text-vital-600"
            aria-label="Notifications"
          >
            <Bell size={18} />
            {unread > 0 && (
              <span className="absolute -right-0.5 -top-0.5 flex h-4 w-4 items-center justify-center rounded-full bg-pulse-500 text-[10px] font-bold text-white">
                {unread}
              </span>
            )}
          </Link>
        )}

        {user?.role !== 'Admin' ? (
          <Link to="/app/profile" className="flex items-center gap-2.5">
            <div className="flex h-10 w-10 items-center justify-center rounded-full bg-ink-900 font-display text-sm font-bold text-vital-400">
              {initials}
            </div>
            <div className="hidden text-left md:block">
              <p className="text-sm font-semibold text-ink-900">{user?.fullName || 'Salma Ahmed'}</p>
              <p className="text-xs text-slate-400">{user?.role || 'Patient'}</p>
            </div>
          </Link>
        ) : (
          <div className="flex items-center gap-2.5">
            <div className="flex h-10 w-10 items-center justify-center rounded-full bg-ink-900 font-display text-sm font-bold text-vital-400">
              {initials}
            </div>
            <div className="hidden text-left md:block">
              <p className="text-sm font-semibold text-ink-900">{user?.fullName || 'Admin'}</p>
              <p className="text-xs text-slate-400">Admin</p>
            </div>
          </div>
        )}
      </div>
    </header>
  )
}
