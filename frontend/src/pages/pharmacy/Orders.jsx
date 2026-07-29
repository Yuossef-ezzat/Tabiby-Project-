import { useEffect, useState } from 'react'
import { PackageSearch } from 'lucide-react'
import StatusBadge from '../../components/common/StatusBadge'
import EmptyState from '../../components/common/EmptyState'
import { useToast } from '../../context/ToastContext'
import { ordersApi } from '../../api/endpoints/orders'
import { formatDate, formatEGP } from '../../utils/format'

export default function Orders() {
  const toast = useToast()
  const [orders, setOrders] = useState([])
  const [isLoading, setIsLoading] = useState(true)

  const load = () => {
    ordersApi
      .myOrders()
      .then(setOrders)
      .catch((err) => toast.error(err.message || 'Could not load your orders'))
      .finally(() => setIsLoading(false))
  }
  useEffect(load, [])

  const cancel = async (id) => {
    try {
      await ordersApi.cancel(id)
      toast.success('Order cancelled')
      load()
    } catch (err) {
      toast.error(err.message || 'Could not cancel this order')
    }
  }

  const removeOrder = async (id) => {
    if (!window.confirm('Are you sure you want to delete this order?')) return;
    try {
      await ordersApi.remove(id)
      toast.success('Order deleted')
      load()
    } catch (err) {
      toast.error(err.message || 'Could not delete this order')
    }
  }

  return (
    <div className="space-y-6">
      <div>
        <h2 className="font-display text-2xl font-bold text-ink-900">My Orders</h2>
        <p className="mt-1 text-sm text-slate-500">Track your medication orders.</p>
      </div>

      {isLoading ? (
        <p className="text-sm text-slate-500">Loading…</p>
      ) : orders.length === 0 ? (
        <EmptyState icon={PackageSearch} title="No orders yet" message="Your medication orders will appear here." />
      ) : (
        <div className="space-y-3">
          {orders.map((o) => (
            <div key={o.id} className="card flex flex-col gap-3 p-5 sm:flex-row sm:items-center sm:justify-between">
              <div>
                <p className="font-display text-sm font-semibold text-ink-900">Order #{o.id}</p>
                <p className="text-sm text-slate-500">
                  {formatDate(o.orderDate)} · {o.items.length} item{o.items.length > 1 ? 's' : ''}
                </p>
              </div>
              <div className="flex items-center gap-4">
                <StatusBadge status={o.status} />
                <button
                  onClick={() => removeOrder(o.id)}
                  className="rounded-lg bg-pulse-50 px-3 py-1 text-xs font-semibold text-pulse-600 transition-colors hover:bg-pulse-100"
                >
                  Delete
                </button>
                <span className="font-display text-base font-bold text-ink-900">{formatEGP(o.total)}</span>
                {o.status === 'Pending' && (
                  <button onClick={() => cancel(o.id)} className="btn-ghost text-sm text-pulse-600">
                    Cancel
                  </button>
                )}
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  )
}
