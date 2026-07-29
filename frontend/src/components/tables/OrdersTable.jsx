import StatusBadge from '../common/StatusBadge'
import { useToast } from '../../context/ToastContext'
import { ordersApi, ORDER_STATUS_NAMES } from '../../api/endpoints/orders'
import { formatDate, formatEGP } from '../../utils/format'



export default function OrdersTable({ orders, onChanged }) {
  const toast = useToast()

  const updateStatus = async (id, status) => {
    try {
      await ordersApi.updateStatus(id, status)
      toast.success('Order status updated!')
      onChanged?.()
    } catch (err) {
      toast.error(err.message || 'Could not update order status')
    }
  }

  const removeOrder = async (id) => {
    try {
      await ordersApi.removeByPharmacist(id)
      toast.success('Order deleted')
      onChanged?.()
    } catch (err) {
      toast.error(err.message || 'Could not delete order')
    }
  }

  return (
    <div className="card overflow-x-auto">
      <table className="w-full min-w-[640px] text-left text-sm">
        <thead>
          <tr className="border-b border-mist-200 text-xs uppercase tracking-wide text-slate-400">
            <th className="px-5 py-3.5 font-semibold">Patient Name</th>
            <th className="px-5 py-3.5 font-semibold">Items</th>
            <th className="px-5 py-3.5 font-semibold">Total</th>
            <th className="px-5 py-3.5 font-semibold">Date</th>
            <th className="px-5 py-3.5 font-semibold">Status</th>
          </tr>
        </thead>
        <tbody className="divide-y divide-mist-200">
          {orders.map((o) => (
            <tr key={o.id}>
              <td className="px-5 py-4 font-display font-semibold text-ink-900">{o.patientName || `Patient #${o.patientId}`} (Order #{o.id})</td>
              <td className="px-5 py-4 text-slate-500">{o.items.length} items</td>
              <td className="px-5 py-4 font-semibold text-ink-900">{formatEGP(o.total)}</td>
              <td className="px-5 py-4 text-slate-500">{formatDate(o.orderDate)}</td>
              <td className="px-5 py-4">
                <div className="flex items-center gap-2">
                  <StatusBadge status={o.status} />
                  <select
                    value={o.status}
                    onChange={(e) => updateStatus(o.id, e.target.value)}
                    className="rounded-lg border border-mist-200 bg-white px-2 py-1 text-xs text-slate-600 focus:border-vital-500 focus:outline-none"
                  >
                    {ORDER_STATUS_NAMES.map((s) => (
                      <option key={s}>{s}</option>
                    ))}
                  </select>
                  <button
                    onClick={() => removeOrder(o.id)}
                    className="rounded-lg bg-red-50 px-2 py-1 text-xs font-semibold text-red-600 transition-colors hover:bg-red-100"
                  >
                    Delete
                  </button>
                </div>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}
