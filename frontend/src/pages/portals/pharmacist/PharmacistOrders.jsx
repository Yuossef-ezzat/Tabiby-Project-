import { useEffect, useState } from 'react'
import OrdersTable from '../../../components/tables/OrdersTable'
import { useToast } from '../../../context/ToastContext'
import { ordersApi } from '../../../api/endpoints/orders'

export default function PharmacistOrders() {
  const toast = useToast()
  const [orders, setOrders] = useState([])
  const [isLoading, setIsLoading] = useState(true)

  const load = () => {
    ordersApi
      .getAll()
      .then(setOrders)
      .catch((err) => toast.error(err.message || 'Could not load orders'))
      .finally(() => setIsLoading(false))
  }
  useEffect(load, [])

  return (
    <div className="space-y-6">
      <div>
        <h2 className="font-display text-2xl font-bold text-ink-900">Orders</h2>
        <p className="mt-1 text-sm text-slate-500">Track and update the status of patient medication orders.</p>
      </div>
      {isLoading ? <p className="text-sm text-slate-500">Loading…</p> : <OrdersTable orders={orders} onChanged={load} />}
    </div>
  )
}
