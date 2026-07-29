import { useEffect, useRef, useState } from 'react'
import { Bell, Check } from 'lucide-react'
import EmptyState from '../../components/common/EmptyState'
import { useToast } from '../../context/ToastContext'
import { notificationsApi } from '../../api/endpoints/notifications'
import { createNotificationConnection } from '../../api/signalr'
import { formatDate } from '../../utils/format'






export default function Notifications() {
  const toast = useToast()
  const [items, setItems] = useState([])
  const [isLoading, setIsLoading] = useState(true)
  const connectionRef = useRef(null)

  useEffect(() => {
    notificationsApi
      .getAll()
      .then(setItems)
      .catch((err) => toast.error(err.message || 'Could not load notifications'))
      .finally(() => setIsLoading(false))

    const connection = createNotificationConnection()
    connectionRef.current = connection
    connection.on('NewNotification', (notification) => {
      setItems((prev) => [notification, ...prev])
    })
    connection.start().catch(() => {})
    return () => connection.stop()
    
  }, [])

  const markAllRead = async () => {
    const unread = items.filter((n) => !n.isRead)
    if (unread.length === 0) return
    try {
      await Promise.all(unread.map((n) => notificationsApi.markAsRead(n.id)))
      setItems((list) => list.map((n) => ({ ...n, isRead: true })))
      window.dispatchEvent(new CustomEvent('NotificationRead', { detail: { count: unread.length } }))
      toast.success('All marked as read')
    } catch (err) {
      toast.error(err.message || 'Could not mark all as read')
    }
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h2 className="font-display text-2xl font-bold text-ink-900">Notifications</h2>
          <p className="mt-1 text-sm text-slate-500">Stay up to date on your care activity.</p>
        </div>
        <button onClick={markAllRead} className="btn-ghost text-sm">
          <Check size={16} />
          Mark all as read
        </button>
      </div>

      {isLoading ? (
        <p className="text-sm text-slate-500">Loading…</p>
      ) : items.length === 0 ? (
        <EmptyState icon={Bell} title="You're all caught up" message="New notifications will appear here." />
      ) : (
        <div className="card divide-y divide-mist-200">
          {items.map((n) => (
            <button
              key={n.id}
              onClick={() =>
                !n.isRead &&
                notificationsApi
                  .markAsRead(n.id)
                  .then(() => {
                    setItems((list) => list.map((x) => (x.id === n.id ? { ...x, isRead: true } : x)))
                    window.dispatchEvent(new CustomEvent('NotificationRead', { detail: { count: 1 } }))
                  })
                  .catch(() => {})
              }
              className={`flex w-full gap-4 p-5 text-left ${!n.isRead ? 'bg-vital-500/5' : ''}`}
            >
              <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-full bg-ink-900/5 text-ink-900">
                <Bell size={18} />
              </div>
              <div className="min-w-0 flex-1">
                <div className="flex items-center justify-between gap-2">
                  <p className="text-sm text-slate-700">{n.message}</p>
                  <span className="shrink-0 text-xs text-slate-400">{formatDate(n.createdAt)}</span>
                </div>
              </div>
              {!n.isRead && <span className="mt-1.5 h-2 w-2 shrink-0 rounded-full bg-vital-500" />}
            </button>
          ))}
        </div>
      )}
    </div>
  )
}
